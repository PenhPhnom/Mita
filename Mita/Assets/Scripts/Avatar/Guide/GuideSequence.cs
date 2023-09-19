using cfg.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideSequence //: Singleton<GuideSequence>
{
    private List<GuideSeqModuleBase> m_Modules = new List<GuideSeqModuleBase>();
    private Dictionary<EGuideTriggerEventType, GuideSeqModuleBase> m_TriggerEventToModuleDic = new Dictionary<EGuideTriggerEventType, GuideSeqModuleBase>();
    private bool m_IsComplete;
    private GuideGroup m_CurGuideGroup;
    public bool IsRestarted;
    private GuideConfig m_CurGroupConfig;
    public string CurGroupTag;
    private GuideConditionBase m_CurGroupStartCondition;
    public string Head;
    public GuideSequence()
    {
        GuideUIViewEvent uiViewEvent = new GuideUIViewEvent();
        uiViewEvent.SetUp();
        m_Modules.Add(uiViewEvent);
        m_TriggerEventToModuleDic.Add(EGuideTriggerEventType.UI_OPEN_PANEL, uiViewEvent);
        m_TriggerEventToModuleDic.Add(EGuideTriggerEventType.UI_MAIN_BTNXXXX, uiViewEvent);
    }

    public void Execute()
    {
        m_CurGuideGroup = new GuideGroup(m_CurGroupConfig, this);
        GuideCore.Instance.OnGroupExecute(m_CurGuideGroup);
        m_CurGuideGroup.Execute();
    }

    /// <summary>
    /// 更新当前序列步骤
    /// </summary>
    /// <param name="startGroupTag"></param>
    public void SetStartPoint(string startGroupTag)
    {
        if (string.IsNullOrEmpty(startGroupTag))
        {
            CurGroupTag = null;
            m_CurGroupConfig = null;
            m_CurGroupStartCondition = null;
            m_IsComplete = true;
            return;
        }

        CurGroupTag = startGroupTag;
        m_CurGroupConfig = GuideCore.Instance.GetGuideGroupConfig(startGroupTag);
        m_CurGroupStartCondition = GuideConditionFactory.CreateConditon((EGuideConditionType)m_CurGroupConfig.StartCondition, m_CurGroupConfig.StartConditionParam, m_CurGroupConfig.StartConditionParam2);
    }

    public bool IsValidForTrigger()
    {
        if (m_CurGroupStartCondition == null)
            return false;

        return m_CurGroupStartCondition.Meet();
    }

    public void FrameUpdate()
    {
        if (IsComplete()) return;
        if (m_CurGuideGroup != null)
        {
            m_CurGuideGroup.FrameUpdate();
            // 在经过错误处理后，当前的group已经被清空
            if (m_CurGuideGroup.IsComplete())
            {
                GuideCore.Instance.OnGroupComplete(m_CurGuideGroup);
                m_CurGuideGroup.OnRelease();
                m_CurGuideGroup = null;
                AdvanceSeqStep();
            }
        }
        else
        {
            if (m_CurGroupStartCondition.Meet())
            {
                Execute();
            }
        }
    }

    public void AdvanceSeqStep()
    {
        var nextStep = m_CurGroupConfig.NextStep;
        if (!string.IsNullOrEmpty(nextStep))
        {
            SetStartPoint(nextStep);
            if (m_CurGroupStartCondition.Meet())
            {
                Execute();
            }
        }
        else
        {
            m_IsComplete = true;
        }
    }

    public void KillSeq()
    {
        if (m_CurGuideGroup != null)
        {
            m_CurGuideGroup.OnLastStepComplete();
            GuideCore.Instance.OnGroupComplete(m_CurGuideGroup);
            m_CurGuideGroup.OnRelease();
            m_CurGuideGroup = null;
        }
        KillPendingGroup(m_CurGroupConfig.NextStep);

    }

    public void KillPendingGroup(string groupTag)
    {
        if (string.IsNullOrEmpty(groupTag))
        {
            GuideCore.Instance.SetPendingGuide(groupTag);
            GuideCore.Instance.GuideStatus.SetGuideGroupFinished(groupTag, null);

            var groupConfig = GuideCore.Instance.GetGuideGroupConfig(groupTag);
            KillPendingGroup(groupConfig.NextStep);
        }
        else
        {
            m_IsComplete = true;
        }
    }

    public bool IsComplete()
    {
        return m_IsComplete;
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

    public void OnStepComplete(GuideShowConfig showConfig)
    {
        foreach (var item in m_Modules)
        {
            item.OnStepComplete(showConfig);
        }
    }

    public void GuideGroupKill()
    {
        if (m_CurGuideGroup != null)
            m_CurGuideGroup.KillSelf();
        KillSeq();
    }

    public void OnRelease()
    {
        EventMgr.Instance.FireEvent(EEventType.UI_REMOVE_MASK, EMaskNameType.GUIDEMASK);

        if (m_Modules != null)
        {
            foreach (var item in m_Modules)
            {
                item.OnRelease();
            }
        }
        m_CurGroupStartCondition = null;
    }
}
