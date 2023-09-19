using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using cfg;
using cfg.Config;
/// <summary>
/// 新手引导  目前只有强制引导 弱引导 之后写
/// GUIGuide_Panel 目前直接用了 没有走FireEvent 
/// </summary>
public class GuideCore : Singleton<GuideCore>
{
    private int m_InitProgress = 0;

    public bool SkipGuide = false;
    private bool m_BInitGuideState;
    private bool m_BIsActive;
    public bool m_IsRunning; // 外部关闭新手引导
    private bool m_BUploadPaused = false;

    private Dictionary<string, GuideConfig> m_GuideConfig = new Dictionary<string, GuideConfig>(); //guide表数据
    public Dictionary<string, GuideConfig> m_PendingGuide = new Dictionary<string, GuideConfig>(); // 待办的
    public Dictionary<string, GuideSequence> m_GuideToSequence = new Dictionary<string, GuideSequence>(); // 下一顺序
    public Dictionary<string, GuideSequence> m_GuideSequence = new Dictionary<string, GuideSequence>(); //顺序

    public GuideSequence ActiveSeq;
    public GuideStatusMgr GuideStatus;
    public GUIGuide_Panel Guide_panel;

    public void OnInit()
    {
        GuideStatus = GuideStatusMgr.Instance;
        if (SkipGuide) return;

        if (m_BInitGuideState) return;
        m_BInitGuideState = true;
        m_InitProgress = 0;
        m_BIsActive = true;
        m_GuideConfig = DataLoader.Instance.GetTbGuideConfig().DataMap;
        SetupGuideSelection();
        if (CheckRemaining()) //是否有待办的新手引导
        {
            // 在引导过程中保持引导UI GUIGuide_panel 开启，只是控制内容显示
            UIManager.Instance.OpenUI(EnumUIType.GUIDE, EPanelLayerType.Guide, () =>
             {
                 m_InitProgress = 1;
                 Guide_panel = UIManager.Instance.GetUI<GUIGuide_Panel>(EnumUIType.GUIDE);
             });
            MainLoopScript.AddUpdateHandler(EUpdatePriority.Realtime, FrameUpdate);
        }
        else
        {
            m_InitProgress = 1;
            ClientLog.Instance.Log("所有引导都已完成，关闭引导系统");
        }
    }

    public void SetRunning(bool bVal)
    {
        m_IsRunning = bVal;
    }

    /// <summary>
    /// 去请求获得数据
    /// </summary>
    public void ReqStatusData()
    {

    }

    public GuideStatusMgr GetGuideStatus()
    {
        return GuideStatus;
    }

    public void StoreData()
    {
        if (GuideStatus == null) return;
        if (m_BUploadPaused) return;

        if (GuideStatus.Dirty && GuideStatus.GetSwap_PendingGuideDataCount() == 0)
        {
            GuideStatus.StoreData();
        }
    }

    public void PauseUploadGuideStatus()
    {
        m_BUploadPaused = true;
    }

    public void TryResumeUploadGuideStatus(Action<bool> callback)
    {
        if (m_BUploadPaused)
        {
            if (GuideStatus.GetPendingGuideDataCount() > 0)
            {
                GuideStatus.StoreData();
                return;
            }
            else
            {
                m_BUploadPaused = false;
            }
        }

        callback?.Invoke(true);
    }

    public bool CheckRemaining()
    {
        return m_PendingGuide.Count > 0;
    }

    public void FrameUpdate(float time)
    {
        if (!m_BIsActive) return;
        if (!m_IsRunning) return;
        if (m_InitProgress < 1) return;
        if (ActiveSeq != null)
        {
            ActiveSeq.FrameUpdate();
            EventMgr.Instance.FireEvent(EEventType.GUIDE_RUNNING_NOW);
            if (ActiveSeq.IsComplete())
            {
                GuideStatus.SetActiveSeq(null);
                GuideStatus.SetGuideSeqFinished(ActiveSeq, m_GuideToSequence);
                Guide_panel.SetSeq(null);
                ActiveSeq.OnRelease();
                ActiveSeq = null;

                if (CheckRemaining())
                {
                    TryExecute();
                }
                else
                {
                    //所有引导都已完成，关闭引导系统
                    MainLoopScript.DelUpdateHandler(FrameUpdate);
                    m_BIsActive = false;
                }
            }
        }
        else
        {
            TryExecute();
        }

        StoreData();
        if (Guide_panel != null)
            Guide_panel.FrameUpdate();
    }

    public void TryExecute()
    {
        var nextSeq = SelectSeq();
        if (nextSeq != null)
            ExecuteSeq(nextSeq);
    }

    public bool IsForceGuideState()
    {
        return ActiveSeq != null;
    }

    public void ExecuteSeq(GuideSequence seq)
    {
        ActiveSeq = seq;
        GuideStatus.SetActiveSeq(seq);
        ActiveSeq.Execute();
        Guide_panel.SetSeq(ActiveSeq);
    }

    /// <summary>
    /// 
    /// </summary>
    public void SetupGuideSelection()
    {
        m_PendingGuide = new Dictionary<string, GuideConfig>(); // 保存等待执行的guide tag
        m_GuideToSequence = new Dictionary<string, GuideSequence>(); // 每个guide tag对应的seq
        m_GuideSequence = new Dictionary<string, GuideSequence>();
        List<string> tmpNoNextStepGuide = new List<string>();
        foreach (var key in m_GuideConfig.Keys)
        {
            var value = m_GuideConfig[key];
            if (!string.IsNullOrEmpty(value.NextStep))
                BuildSeq(value);
            else
                tmpNoNextStepGuide.Add(key);
        }

        foreach (var value in tmpNoNextStepGuide)
        {
            if (!m_GuideToSequence.ContainsKey(value))
            {
                var seq = new GuideSequence();
                seq.Head = value;
                m_GuideToSequence[value] = seq;
                m_GuideSequence[value] = seq;
            }
        }

        // 通过所有的seq来标记哪些seq已经完成
        GuideStatus.UnpackCompleteSeq(m_GuideToSequence);
        // 只有unpack之后，isGuideGroupFinished才能正确工作
        foreach (var key in m_GuideConfig.Keys)
        {
            if (!GuideStatus.IsGuideGroupFinished(key))
            {
                m_PendingGuide[key] = m_GuideConfig[key];
            }
        }

        // 定位合法起点
        foreach (var seq in m_GuideSequence.Values)
        {
            seq.SetStartPoint(FindValidSeqStart(seq.Head));
        }

    }

    public void BuildSeq(GuideConfig guideConfig)
    {
        GuideSequence seq;
        var guideTag = guideConfig.Tag;
        if (m_GuideToSequence.ContainsKey(guideTag))
        {
            seq = m_GuideToSequence[guideTag];
            if (seq != null) return;
        }

        seq = FindSeqForGuide(guideConfig.NextStep);
        if (seq != null)
        {
            MarkGuideToSeq(guideTag, seq.Head, seq);
            // 更新序列信息
            m_GuideSequence[seq.Head] = null;
            m_GuideSequence[guideTag] = seq;
            seq.Head = guideTag;
        }
        else
        {
            seq = new GuideSequence();
            seq.Head = guideTag;
            m_GuideSequence[guideTag] = seq;
            MarkGuideToSeq(guideTag, "", seq);
        }
    }

    public void MarkGuideToSeq(string startTag, string endTag, GuideSequence seq)
    {
        if (startTag == endTag) return;

        m_GuideToSequence[startTag] = seq;
        MarkGuideToSeq(m_GuideConfig[startTag].NextStep, endTag, seq);
    }

    public GuideSequence FindSeqForGuide(string guideTag)
    {
        if (string.IsNullOrEmpty(guideTag)) return null;
        GuideSequence seq = null;
        if (m_GuideToSequence.ContainsKey(guideTag))
            seq = m_GuideToSequence[guideTag];

        if (seq != null)
            return seq;
        else
        {
            var config = m_GuideConfig[guideTag];
            if (config == null) ClientLog.Instance.LogError($"Guide配置缺少:{guideTag}");
            if (config != null && config.NextStep == guideTag) ClientLog.Instance.LogError($"{guideTag}的nextStep是自己");
            seq = FindSeqForGuide(config.NextStep);
            return seq;
        }
    }

    public string FindValidSeqStart(string seqHeadTag)
    {
        if (string.IsNullOrEmpty(seqHeadTag)) return null;

        var config = m_GuideConfig[seqHeadTag];
        if (GuideStatus.IsGuideGroupFinished(seqHeadTag))
        {
            return FindValidSeqStart(config.NextStep);
        }
        return seqHeadTag;
    }

    public GuideSequence SelectSeq()
    {
        var ActiveSeq = GuideStatus.GetActiveSeq();
        if (!string.IsNullOrEmpty(ActiveSeq))
        {
            var cachedSeq = m_GuideSequence[ActiveSeq];
            if (cachedSeq != null && !cachedSeq.IsComplete())
            {
                cachedSeq.IsRestarted = true;
                return cachedSeq;
            }
            else
            {
                // "记录的引导序列不是可执行序列"
                GuideStatus.SetActiveSeq(null);
                GuideStatus.SetActiveSeqStep(null);
                GuideStatus.SetActiveDetailStep(null);
            }
        }

        GuideSequence resultSeq = null;
        foreach (var value in m_GuideSequence.Values)
        {
            if (value.IsValidForTrigger())
            {
                if (resultSeq == null)
                    resultSeq = value;
                else
                {
                    if (value.Head != value.CurGroupTag)
                    {
                        resultSeq = value;
                        break;
                    }
                }
            }
        }

        return resultSeq;
    }

    public void OnGroupExecute(GuideGroup group)
    {
        Guide_panel.SetGuideGroup(group);
        GuideStatus.SetActiveSeqStep(group);
    }

    public void OnGroupComplete(GuideGroup group)
    {
        m_PendingGuide.Remove(group.GetTag());
        Guide_panel.SetGuideGroup(null);
        GuideStatus.SetGuideGroupFinished(group.GetTag(), group.Sequence.Head);
        GuideStatus.SetActiveSeqStep(null);
        GuideStatus.SetActiveDetailStep(null);
    }

    /// <summary>
    /// 得到 表数据
    /// </summary>
    public GuideConfig GetGuideGroupConfig(string tag)
    {
        return m_GuideConfig[tag];
    }

    public EGuideConditionType GetGuideGroupStartCondition(GuideConfig guideGroupConfig)
    {
        return (EGuideConditionType)guideGroupConfig.StartCondition;
    }

    public void SetPendingGuide(string groupTag)
    {
        m_PendingGuide[groupTag] = null;
    }


    public override void OnRelease()
    {
        MainLoopScript.DelUpdateHandler(FrameUpdate);
        if (ActiveSeq != null)
            ActiveSeq.OnRelease();
        m_BInitGuideState = false;
        m_InitProgress = 0;
        GuideStatus.OnRelease();
        // 关闭 新手引导界面
        UIManager.Instance.CloseUI(EnumUIType.GUIDE);
        m_BIsActive = false;
        m_IsRunning = false;
        m_PendingGuide.Clear();
        m_GuideToSequence.Clear();
        m_GuideSequence.Clear();
    }
}
