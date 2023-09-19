using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalScene : BaseScene
{
    public NormalScene(string name, object obj, string url = "") : base(name, obj, url)
    {
        SetProgressConfig(ESceneProcess.ONCURSCENEINITING, 0.2F);
        SetProgressConfig(ESceneProcess.LOADLOADINGSCENE, 0.2F);
        SetProgressConfig(ESceneProcess.LOADNEWSCENE, 0.5F);
        SetProgressConfig(ESceneProcess.LOADINGNEWDATA, 0.1F);
    }

    public override EnumUIType GetLoadingType()
    {
        return EnumUIType.LOADING;
    }

    public override void OnPreSceneExit(Action callBack)
    {
        callBack?.Invoke();
    }

    public override void OnDestroyScene()
    {

    }

    public override string GetSceneName()
    {
        return SceneName;
    }


    public override void OnNewSceneLoaded()
    {

    }

    public override void OnSceneResStartLoad()
    {
        NormalClass data = (NormalClass)_data;
        data.SceneResStartCallBack?.Invoke();
    }

    public override void OnSceneResLoaded()
    {
        if (_data != null)
        {
            NormalClass data = (NormalClass)_data;
            data.ResLoaded?.Invoke();
            ProfileMgr.Instance.InitProfile();
            _dataProgress = 1;
        }
    }

    public override bool IsInitDataComplete()
    {
        return true;
    }

    public override void OnFinish()
    {
        if (_data != null)
        {
            NormalClass data = (NormalClass)_data;
            data.FinishCallBack?.Invoke();
            AppBridge.SendMessageToAvatarApp("{\"messageType\":2003,\"content\":\"加载完成\"}");
        }
    }
}


public class NormalClass
{
    /// <summary>
    /// 场景scene加载完成
    /// </summary>
    public Action ResLoaded;
    /// <summary>
    /// 全部加载完成的回调
    /// </summary>
    public Action FinishCallBack;
    /// <summary>
    /// 
    /// </summary>
    public Action SceneResStartCallBack;

}