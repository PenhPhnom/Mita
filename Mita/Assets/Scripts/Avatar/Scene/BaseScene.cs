using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 
/// </summary>
public class BaseScene
{
    private string m_sceneName;

    private bool m_IsActive;

    public string SceneName => m_sceneName;
    public bool IsActive => m_IsActive;

    protected float _dataProgress;
    private float _totalProgress;
    private BaseSceneLoad _sceneLoad;
    private Dictionary<ESceneProcess, float> _progressConfig = new Dictionary<ESceneProcess, float>();
    private Dictionary<ESceneProcess, float> _progressValue = new Dictionary<ESceneProcess, float>();
    public BaseUI LoadingView;
    protected object _data;

    public string PathURL;
    public BaseScene(string name, object obj, string url = "")
    {
        m_sceneName = name;
        _data = obj;
        PathURL = url;
        _progressConfig.Clear();
        SetProgressConfig(ESceneProcess.LOADLOADINGSCENE, GetInitDataProgress());
        SetProgressConfig(ESceneProcess.LOADNEWSCENE, GetScenePercent());
        SetProgressConfig(ESceneProcess.LOADINGNEWDATA, GetExtraDataTotalPercent());
    }

    public void Dispose()
    {
        if (_sceneLoad != null)
        {
            _sceneLoad.Dispose();
            _sceneLoad = null;
        }

    }

    public void SetActive(bool bVal)
    {
        m_IsActive = bVal;
    }


    public BaseSceneLoad CreateSceneLoad()
    {
        _sceneLoad = new NormalSceneLoad();
        return _sceneLoad;
    }

    public BaseSceneLoad GetSceneLoad()
    {
        return _sceneLoad;
    }

    public virtual EnumUIType GetLoadingType()
    {
        return EnumUIType.LOADING;
    }

    public void OnProgress(float process)
    {
        ClientLog.Instance.Log("process: ", process.ToString());
        //if (process <= 0)
        //{
        //    if (LoadingView == null)
        //    {
        //        LoadingView = UIManager.Instance.GetUI<BaseUI>(GetLoadingType());
        //        if (LoadingView == null)
        //            UIManager.Instance.OpenUICloseOthers(GetLoadingType());
        //    }
        //}

        //if (LoadingView != null)
        //{
        //    EventMgr.Instance.FireEvent(EEventType.LOADINGPROCESS, process);
        //}
    }

    public void SetProgressConfig(ESceneProcess key, float percent)
    {
        _progressConfig[key] = percent;
    }

    public void OnSceneSwitchStart()
    {

    }

    public void OnSceneExit()
    {
        SetActive(false);
    }

    public void InitData()
    {

    }

    public void SetProgressValue(ESceneProcess key, float value)
    {
        _progressValue[key] = value;
        float totalProgress = 0;
        foreach (var cKey in _progressConfig.Keys)
        {
            float pWeight = _progressConfig[cKey];
            float pValue = 0;
            if (_progressValue.ContainsKey(cKey))
                pValue = _progressValue[cKey];
            totalProgress += pWeight * pValue;
            ClientLog.Instance.Log("setProgressValue {0} ===>  {1} ===>  {2}", cKey, pValue, pWeight);
        }
        OnProgress(totalProgress);
    }

    public float GetInitDataProgress()
    {
        return 1f;
    }

    public float GetInterimLoadPercent()
    {
        return 0.02f;
    }

    public float GetScenePercent()
    {
        return 0.4f;
    }

    public float GetExtraDataTotalPercent()
    {
        return (1 - GetInterimLoadPercent() - GetScenePercent());
    }


    public virtual bool IsInitDataComplete()
    {
        return false;
    }

    public virtual void OnPreSceneExit(Action callBack)
    {
        ClientLog.Instance.LogError("需要重写");
    }


    public void OnNewSceneStartLoaded()
    {

    }

    //此处为切换场景时，由场景管理器通知释放场景，根据各自场景自身需求做处理
    public virtual void OnDestroyScene()
    {
        ClientLog.Instance.LogError("需要重写");
    }

    public virtual string GetSceneName()
    {
        ClientLog.Instance.LogError("需要重写");
        return "";
    }

    public AsyncOperation LoadUnitySceneAsync()
    {
        //这里加载调用场景的方法  TODO
        AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(GetSceneName());
        return async;
    }

    public virtual void OnSceneResStartLoad()
    {

    }

    /// <summary>
    /// 场景已经加载完 处于准备状态
    /// </summary>
    public virtual void OnSceneResLoaded()
    {

    }

    public virtual void OnNewSceneLoaded()
    {
        ClientLog.Instance.LogError("需要重写");
    }

    public virtual float OnExtraDataProgress()
    {
        return _dataProgress;
    }

    public void OnNewSceneFinish(Action callBack)
    {
        callBack?.Invoke();
    }

    public void OnExtraDataCompleted(Action callback)
    {
        _dataProgress = 1;
        CloseLoadingPanel(callback);
    }

    public void CloseLoadingPanel(Action callback)
    {
        if (LoadingView != null)
        {
            UIManager.Instance.CloseUI(GetLoadingType());
            LoadingView = null;
            callback?.Invoke();
        }
        else
        {
            callback?.Invoke();
        }
    }

    public void OnLoadingCompleted()
    {

    }

    public virtual void OnFinish()
    {

    }
}

