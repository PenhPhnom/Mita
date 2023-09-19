using cfg.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideInternalEventHandler : Singleton<GuideInternalEventHandler>
{
    private List<GuideSeqModuleBase> m_Modules = new List<GuideSeqModuleBase>();
    private Dictionary<EGuideTriggerEventType, GuideSeqModuleBase> m_TriggerEventToModuleDic = new Dictionary<EGuideTriggerEventType, GuideSeqModuleBase>();

    public override void Init()
    {
        base.Init();

        GuideUIViewEvent uiViewEvent = new GuideUIViewEvent();
        uiViewEvent.SetUp();
        m_Modules.Add(uiViewEvent);
        m_TriggerEventToModuleDic.Add(EGuideTriggerEventType.UI_OPEN_PANEL, uiViewEvent);
        m_TriggerEventToModuleDic.Add(EGuideTriggerEventType.UI_MAIN_BTNXXXX, uiViewEvent);

    }

    public void OnEventTrigger(EGuideTriggerEventType eventType, params object[] args)
    {
        var moduleFunc = m_TriggerEventToModuleDic[eventType];
        if (moduleFunc != null)
        {
            moduleFunc.OnTriggerEvent(eventType, args);
        }
        else
            ClientLog.Instance.LogError($"未处理的GuideEvent{eventType}");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="showConfig">引导表 数据</param>
    public void HandleViewConfig(GuideShowConfig showConfig)
    {
        foreach (var item in m_Modules)
        {
            item.HandleViewConfig(showConfig);
        }
    }

    public override void OnRelease()
    {
        if (m_Modules != null)
        {
            foreach (var item in m_Modules)
            {
                item.OnRelease();
            }
        }
    }
}
