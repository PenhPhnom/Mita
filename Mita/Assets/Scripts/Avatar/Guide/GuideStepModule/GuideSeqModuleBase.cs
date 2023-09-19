using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using cfg.Config;

public abstract class GuideSeqModuleBase
{
    public abstract void SetUp();
    /// <summary>
    /// 处理 新手引导 事件的
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="param"></param>
    public abstract void OnTriggerEvent(EGuideTriggerEventType guideEventType, params object[] args);

    //guideShow" 表数据
    public abstract void HandleViewConfig(GuideShowConfig guideShow);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="guideShow">表数据 </param>
    public abstract void OnStepComplete(GuideShowConfig guideShow);

    public bool CheckEventParam(object param)
    {
        return param != null;
    }

    public abstract void OnRelease();
}
