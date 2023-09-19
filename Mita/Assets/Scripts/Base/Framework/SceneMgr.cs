using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public enum ESceneName
{
    EMPTYSCENE,
    CREATEPLAYERSCENE,
    CHARROOMSCENE,
    CHATPLAYER,
    AVATARPLAYER,
}

/// <summary>
/// 目前跟场景加载步骤有关系 就先不放到 公用的Enum里面了
/// </summary>
public enum ESceneProcess
{
    UNKNOWN = -999,
    WAITLOADINGPANEL = -1, // Loading Panel
    STARTPRESCENEEXIT = 0, // 开始加载新场景
    ONCURSCENEINIT = 1, // 有些场景开始就需要进行分帧处理
    ONCURSCENEINITING = 2,
    ONCURSCENEDOPRESCENEEXIT = 3,
    STARTLOADINGSCENE = 4,
    LOADLOADINGSCENE = 5,
    RELEASEPRESCENE = 6,
    PRELOADNEWSCENE = 7,
    LOADNEWSCENE = 8,
    LOADINGNEWDATA = 9,
    FINALSTAGE = 10
}

/// <summary>
/// 加载场景的类型  目前闲置加载Unity的
/// </summary>
public enum ESceneType
{
    UNITY = 1,
    //PREFAB = 2,
    //ADDITIVEPREFAB = 3,
    //DONTDESTORYUNIT = 4,
    //UNLOADADDITIVEPREFAB = 5
}

/// <summary>
/// 场景管理 现在是未完成状态 需要等着搞完了再来写 或者其他人来接手接着写
/// 未完成还有： ESceneType
/// </summary>
public class SceneMgr : Singleton<SceneMgr>
{
    private ESceneProcess m_SceneProcess = ESceneProcess.UNKNOWN;
    private BaseScene _curScene; // 正在加载的
    private BaseScene _preScene;// 已经加载的
    private BaseSceneLoad _sceneLoad;
    private float _loadingStartTime;

    private void Update(float time)
    {
        switch (m_SceneProcess)
        {
            case ESceneProcess.UNKNOWN:
                MainLoopScript.DelUpdateHandler(Update);
                break;
            case ESceneProcess.WAITLOADINGPANEL:
                _curScene.OnProgress(0);
                //要走U3D的LoadingUI的话就打开，走App的就不动
                //if (_curScene.LoadingView != null)
                //{
                //    if (_curScene.LoadingView.State == EnumObjectState.Ready)
                //    {
                //        m_SceneProcess = ESceneProcess.STARTPRESCENEEXIT;
                //    }
                //}
                m_SceneProcess = ESceneProcess.STARTPRESCENEEXIT;
                break;
            case ESceneProcess.STARTPRESCENEEXIT:
                _curScene.OnSceneSwitchStart();
                if (_preScene != null && _preScene.IsActive)
                    _preScene.OnSceneExit();
                m_SceneProcess = ESceneProcess.ONCURSCENEINIT;
                break;
            case ESceneProcess.ONCURSCENEINIT:
                m_SceneProcess = ESceneProcess.ONCURSCENEINITING;
                _curScene.InitData();
                break;
            case ESceneProcess.ONCURSCENEINITING:
                _curScene.SetProgressValue(m_SceneProcess, _curScene.GetInitDataProgress());
                if (_curScene.IsInitDataComplete())
                {
                    _curScene.SetProgressValue(m_SceneProcess, 1);
                    m_SceneProcess = ESceneProcess.ONCURSCENEDOPRESCENEEXIT;
                }
                break;
            case ESceneProcess.ONCURSCENEDOPRESCENEEXIT:
                _curScene.OnPreSceneExit(
                    () =>
                    {
                        m_SceneProcess = ESceneProcess.STARTLOADINGSCENE;
                    });
                break;
            case ESceneProcess.STARTLOADINGSCENE:
                if (_preScene != null)
                    _preScene.OnNewSceneStartLoaded();

                _sceneLoad.StartLoadingScene(_curScene);
                m_SceneProcess = ESceneProcess.LOADLOADINGSCENE;
                break;
            case ESceneProcess.LOADLOADINGSCENE:
                _curScene.SetProgressValue(m_SceneProcess, _sceneLoad.GetLoadingSceneProgress());
                if (_sceneLoad.IsLoadingSceneReady())
                    m_SceneProcess = ESceneProcess.RELEASEPRESCENE;
                break;
            case ESceneProcess.RELEASEPRESCENE:
                if (_preScene != null)
                    _preScene.OnDestroyScene();
                GC.Collect();
                m_SceneProcess = ESceneProcess.PRELOADNEWSCENE;
                break;
            case ESceneProcess.PRELOADNEWSCENE:
                _curScene.OnSceneResStartLoad();
                _sceneLoad.StartPreLoadNewScene(_curScene);
                //if (_stackSceneLoad != null)
                //    _stackSceneLoad: onSceneEnterForeground();
                m_SceneProcess = ESceneProcess.LOADNEWSCENE;
                break;
            case ESceneProcess.LOADNEWSCENE:
                _curScene.SetProgressValue(m_SceneProcess, _sceneLoad.GetNewSceneLoadProgress());
                if (_sceneLoad.IsNewSceneLoadReady())
                {
                    _curScene.SetActive(true);
                    _curScene.OnSceneResLoaded();
                    m_SceneProcess = ESceneProcess.LOADINGNEWDATA;

                    if (_preScene != null)
                        _preScene.OnNewSceneLoaded();

                    _sceneLoad.DisposeLoadingScene();
                }
                break;
            case ESceneProcess.LOADINGNEWDATA:
                //加载场景所需要的数据
                float progress = _curScene.OnExtraDataProgress();
                _curScene.SetProgressValue(m_SceneProcess, progress);
                if (progress >= 0.99999)
                {
                    m_SceneProcess = ESceneProcess.FINALSTAGE;
                    if (_preScene != null)
                    {
                        _preScene.OnNewSceneFinish(FinishCallBack);
                        _preScene = null;
                    }
                    else
                    {
                        FinishCallBack();
                    }
                    MainLoopScript.DelUpdateHandler(Update);
                }
                break;
        }
        PrintSceneLoading(m_SceneProcess);
    }

    private void FinishCallBack()
    {
        _curScene.OnExtraDataCompleted(() =>
        {
            _curScene.OnLoadingCompleted();
            _curScene.OnProgress(1);
            m_SceneProcess = ESceneProcess.UNKNOWN;
            _curScene.OnFinish();
        });

        if (_sceneLoad != null)
        {
            _sceneLoad.DisposeLoadingScene();
            _sceneLoad = null;
        }

    }

    /// <summary>
    /// 加载相应的场景
    /// </summary>
    public void SwitchScene()
    {
        if (m_SceneProcess != ESceneProcess.UNKNOWN)
        {
            ClientLog.Instance.LogError("当前有未完成的场景切换");
            return;
        }

        NormalClass normaClass = new NormalClass();

        BaseScene baseScene = new BaseScene("", normaClass);
        //TODO  这里是 根据加载的场景的类型 来加载场景
        //TODO  根据加载的场景来做优化处理

        if (_curScene != null && _curScene.SceneName == baseScene.SceneName)
        {
            ClientLog.Instance.LogError("在当前场景，不允许再次切入");
            return;
        }

        _preScene = _curScene;

        _curScene = baseScene;

        _sceneLoad = baseScene.CreateSceneLoad();

        m_SceneProcess = ESceneProcess.WAITLOADINGPANEL;

        _loadingStartTime = Time.realtimeSinceStartup;
        MainLoopScript.AddUpdateHandler(EUpdatePriority.Realtime, Update);
    }


    public override void OnRelease()
    {
        MainLoopScript.DelUpdateHandler(Update);
        if (_curScene != null)
        {
            _curScene.Dispose();
            _curScene = null;
        }
        if (_preScene != null)
        {
            _preScene.Dispose();
            _preScene = null;
        }

        if (_sceneLoad != null)
        {
            _sceneLoad.Dispose();
            _sceneLoad = null;
        }

        m_SceneProcess = ESceneProcess.UNKNOWN;
    }

    private void PrintSceneLoading(ESceneProcess step)
    {
        ClientLog.Instance.Log("Scene Loading: {0}  {1}  {2}", (int)step - 1, Time.realtimeSinceStartup - _loadingStartTime, Time.frameCount);
    }
}
