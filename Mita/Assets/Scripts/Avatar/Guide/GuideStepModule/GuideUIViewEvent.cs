using cfg.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI 新手引导 事件处理
/// </summary>
public class GuideUIViewEvent : GuideSeqModuleBase
{
    /// <summary>
    /// 处理 新手引导 事件的
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="param"></param>
    public override void OnTriggerEvent(EGuideTriggerEventType eventType, params object[] param)
    {
        if (eventType == EGuideTriggerEventType.UI_OPEN_PANEL)
        {

        }
        else if (eventType == EGuideTriggerEventType.UI_MAIN_BTNXXXX)
        {

        }
    }

    public override void SetUp()
    {
        //有需要的话 可以在这里添加 事件监听
        EventMgr.Instance.RegisterEvent(EEventType.SYSTEM_OPTION, OnPanelShow);

    }

    public override void HandleViewConfig(GuideShowConfig guideShow)
    {

    }

    public override void OnStepComplete(GuideShowConfig guideShow)
    {

    }

    public void OnPanelShow(object param)
    {

    }

    public override void OnRelease()
    {
        EventMgr.Instance.UnRegisterEvent(EEventType.SYSTEM_OPTION, OnPanelShow);
    }
}
