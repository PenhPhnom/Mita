using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalSceneLoad : BaseSceneLoad
{
    //目前是 走的U3D自带的加载场景测试 换成AB加载 要修改此处的
    public AsyncOperation _sceneAsync;
    public AsyncOperation _loadingScene;
    private BaseScene _curScene;
    private bool m_IsNewSceneLoadReady = false;
    public ESceneType GetLoaderType()
    {
        return ESceneType.UNITY;
    }

    public override void StartLoadingScene(BaseScene curScene)
    {
        _loadingScene = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("EmptyScene");
    }

    public override void DestoryDispose()
    {

    }

    public override void DestoryPreDispose()
    {

    }

    public override void DisposeLoadingScene()
    {
        if (_loadingScene != null)
        {
            _loadingScene = null;
        }
    }

    public override float GetLoadingSceneProgress()
    {
        return _loadingScene.progress;
    }

    public override float GetNewSceneLoadProgress()
    {
        if (_sceneAsync == null) return 0;
        return _sceneAsync.progress;
    }

    public override void GetScenePrefab()
    {

    }

    /// <summary>
    /// 检测场景是否已经加载完成
    /// </summary>
    /// <returns></returns>
    public override bool IsNewSceneLoadReady()
    {
        if (_sceneAsync != null)
        {
            return m_IsNewSceneLoadReady || _sceneAsync.isDone;
        }
        else
        {
            return m_IsNewSceneLoadReady;
        }
    }

    public override bool IsLoadingSceneReady()
    {
        return true;
    }

    public override void StartPreLoadNewScene(BaseScene curscene)
    {
        _curScene = curscene;
        ResourceMgr.Instance.LoadResourceAsync(curscene.SceneName, TypeInts.SceneInstance, (_, _) =>
        {
            if (_curScene != null)
            {
                _sceneAsync = curscene.LoadUnitySceneAsync();
                _sceneAsync.completed += operation =>
                {
                    if (Camera.main.gameObject != null)
                        Camera.main.gameObject.AddMissingComponent<FindUICamera>();
                    m_IsNewSceneLoadReady = true;
                };
            }
        }, null, curscene.PathURL);
    }

    //进行资源的卸载与释放
    public override void Dispose()
    {
        if (_curScene != null)
        {
            ResourceMgr.Instance.UnLoadResource(_curScene.SceneName, TypeInts.SceneInstance);
            _curScene = null;
        }
        if (_sceneAsync != null)
        {
            _sceneAsync = null;
        }
    }
}
