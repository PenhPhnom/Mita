using cfg.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideStep
{
    private static bool m_isFin;
    public GuideConditionBase FinCondition;
    public GuideDetailConfig DetailConfig;
    public GuideShowConfig ShowConfig;
    private int _meetCheckTimes = 0;

    public string GetTag()
    {
        return DetailConfig.Tag;
    }

    public void OnRelease()
    {
        m_isFin = false;
        if (FinCondition != null)
            FinCondition.OnRelease();
        FinCondition = null;
    }

    public bool IsComplete()
    {
        return m_isFin;
    }

    /// <summary>
    /// 执行事件
    /// </summary>
    public void Excute()
    {
        // 遮挡界面等处理 TODO
        //EventMgr.Instance.FireEvent(EEventType.UI_REMOVE_MASK, EMaskNameType.GUIDEMASK);

        if (!string.IsNullOrEmpty(DetailConfig.GuideLog))
        {
            //有需要这里做开始打点处理 TODO
        }

        //处理事件
        if (DetailConfig.TriggerStartEvent != 0)
        {
            GuideCore.Instance.ActiveSeq.OnEventTrigger((EGuideTriggerEventType)DetailConfig.TriggerStartEvent, DetailConfig.EventStartParams);
        }

        m_isFin = FinCondition.Meet();
        if (m_isFin)
        {
            OnComplete();
        }
    }

    public void FrameUpdate()
    {
        if (!m_isFin)
        {
            m_isFin = FinCondition.Meet();
            if (m_isFin)
            {
                OnComplete();
            }
        }
    }

    public void OnComplete()
    {
        //处理 关闭遮罩  TODO
        // EventMgr.Instance.FireEvent(EEventType.UI_ADD_MASK, EMaskNameType.GUIDEMASK);

        //处理 引导触发完的事件
        if (DetailConfig.TriggerOverEvent != 0)
        {
            GuideCore.Instance.ActiveSeq.OnEventTrigger((EGuideTriggerEventType)DetailConfig.TriggerOverEvent, DetailConfig.EventOverParams);
        }

        if (!string.IsNullOrEmpty(DetailConfig.FinishGuideLog))
        {
            //这里做打点完成处理 TODO
        }


        EventMgr.Instance.FireEvent(EEventType.CONFORMITY_GUIDE_FINFISH);
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
        step.FinCondition = GuideConditionFactory.CreateConditon(
        (EGuideConditionType)detailData.FinConditionType,
        detailData.FinConditionParam,
        detailData.FinConditionParam2);
        return step;
    }
}
