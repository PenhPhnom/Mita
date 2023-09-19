using cfg.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideGroup
{
    private GuideConfig m_DetailConfig;
    public GuideSequence Sequence;
    private int stepIndex;
    private GuideStep m_ActiveStep;
    private GuideConditionBase m_FailCondition;
    private int m_RecorverSetpEndIndex = 0;
    private List<string> m_Steps = new List<string>();
    public GuideGroup(GuideConfig config, GuideSequence seq)
    {
        m_DetailConfig = config;
        Sequence = seq;
        stepIndex = 0;
    }

    public string GetTag()
    {
        return m_DetailConfig.Tag;
    }

    public bool NeedRecorver()
    {
        if (m_DetailConfig.Recover != null && m_DetailConfig.Recover.Length > 0)
        {
            if (Sequence.IsRestarted)
            {
                Sequence.IsRestarted = false;
                return true;
            }

            // 记录的执行中group不是当前group
            if (GuideCore.Instance.GuideStatus.GetActiveSeqSetp() != m_DetailConfig.Tag)
                return false;

            return GuideCore.Instance.GuideStatus.GetActiveDetailSetp();
        }
        else
        {
            // 如果恢复的group没有恢复步骤，也要标记seq完成了restart
            Sequence.IsRestarted = false;
        }
        return false;
    }

    public void Execute()
    {
        if (m_DetailConfig.FailCondition != 0)
        {
            m_FailCondition = GuideConditionFactory.CreateConditon(
                (EGuideConditionType)m_DetailConfig.FailCondition,
                m_DetailConfig.FailConditionParam,
                m_DetailConfig.FailConditionParam2
            );
        }
        m_Steps.Clear();
        m_RecorverSetpEndIndex = 0;
        //if (m_Steps == null)
        if (NeedRecorver())
        {
            foreach (var item in m_DetailConfig.Recover)
            {
                m_Steps.Add(item);
                m_RecorverSetpEndIndex++;
            }
        }

        foreach (var item in m_DetailConfig.Content)
        {
            m_Steps.Add(item);
        }

        stepIndex = 0;
        SafeAdvanceStep();
    }

    public void SafeAdvanceStep()
    {

        try
        {
            AdvanceStep();
        }
        catch (System.Exception)
        {
            ClientLog.Instance.LogError("执行错误 但是也得干掉所有 不能卡流程");
            KillSelfAndSeq();
        }
    }

    public void AdvanceStep()
    {
        if (m_ActiveStep != null)
        {
            m_ActiveStep.OnRelease();
            m_ActiveStep = null;
        }
        stepIndex++;
        if (IsComplete())
        {
            OnLastStepComplete();
            return;
        }

        m_ActiveStep = CreateStep(m_Steps[stepIndex - 1]);
        m_ActiveStep.Excute();
        Sequence.HandleViewConfig(m_ActiveStep.ShowConfig);
        if (stepIndex > m_RecorverSetpEndIndex)
        {
            // 恢复步骤不记录进度
            GuideCore.Instance.GuideStatus.SetActiveDetailStep(m_ActiveStep);
        }

        GuideCore.Instance.Guide_panel.OnStepStart(m_ActiveStep);
        //EventMgr.Instance.FireEvent(EEventType.CONFORMITY_CONTROL_GUIDE);
        // 第一次update失败就是完全失败
        try
        {
            FrameUpdate();
        }
        catch (System.Exception)
        {

            KillSelfAndSeq();
        }
    }

    public bool IsComplete()
    {
        return stepIndex > m_Steps.Count;
    }

    public void FrameUpdate()
    {
        if (m_FailCondition != null && m_FailCondition.Meet())
        {
            KillSelfAndSeq();
            ClientLog.Instance.Log($"引导触发失败条件 {m_DetailConfig.Tag}");
            return;
        }

        if (m_ActiveStep != null)
        {
            try
            {
                m_ActiveStep.FrameUpdate();
            }
            catch (System.Exception)
            {

            }

            if (m_ActiveStep.IsComplete())
            {
                GuideCore.Instance.Guide_panel.OnStepComplete(m_ActiveStep);
                if (stepIndex > m_RecorverSetpEndIndex)
                {
                    // 恢复步骤不记录进度
                    GuideCore.Instance.GuideStatus.SetActiveDetailStep(null);
                }

                if (m_ActiveStep.DetailConfig.Tag == m_DetailConfig.Keystep)
                {
                    OnKeyStepComplete();
                }
                SafeAdvanceStep();
            }
        }
    }

    public void OnKeyStepComplete()
    {
        //GuideCore.Instance.GuideStatus.SetGuideGroupFinished();
    }

    public void OnLastStepComplete()
    {

    }

    public void KillSelfAndSeq()
    {
        KillSelf();
        Sequence.KillSeq();
    }
    public void KillSelf()
    {
        if (m_ActiveStep != null)
        {
            m_ActiveStep.OnComplete();
            GuideCore.Instance.Guide_panel.ActiveStep = m_ActiveStep;
            GuideCore.Instance.Guide_panel.OnStepComplete(m_ActiveStep);
            m_ActiveStep.OnRelease();
            m_ActiveStep = null;
        }

        // 标记关键步结束
        GuideCore.Instance.GuideStatus.SetGuideGroupFinished(GetTag(), Sequence.Head);// 

        foreach (var stepTag in m_Steps)
        {
            //得到数据
            GuideDetailConfig detailData = DataLoader.Instance.GetGuideDetailConfigByTag(stepTag);
            if (detailData != null && detailData.TriggerStartEvent != 0)
            {
                GuideCore.Instance.ActiveSeq.OnEventTrigger((EGuideTriggerEventType)detailData.TriggerStartEvent, detailData.EventStartParams);
            }

            if (detailData != null && detailData.TriggerOverEvent != 0)
            {
                GuideCore.Instance.ActiveSeq.OnEventTrigger((EGuideTriggerEventType)detailData.TriggerOverEvent, detailData.EventStartParams);
            }
        }
        stepIndex = m_Steps.Count + 1;
    }


    public GuideStep CreateStep(string stepTag)
    {
        //得到表数据  GuideDetailConfig
        GuideDetailConfig detailData = DataLoader.Instance.GetGuideDetailConfigByTag(stepTag);
        if (detailData == null)
        {
            return null;
        }

        GuideStep step = new GuideStep();
        step.DetailConfig = detailData;
        // 得到表 GuideShow 数据
        if (!string.IsNullOrEmpty(detailData.GuideShow))
            step.ShowConfig = DataLoader.Instance.GetGuideShowConfigByTag(detailData.GuideShow);

        step.FinCondition = GuideConditionFactory.CreateConditon(
        (EGuideConditionType)detailData.FinConditionType,
        detailData.FinConditionParam,
        detailData.FinConditionParam2);
        return step;
    }


    public void OnRelease()
    {
        EventMgr.Instance.FireEvent(EEventType.UI_REMOVE_MASK, EMaskNameType.GUIDEMASK);
    }
}
