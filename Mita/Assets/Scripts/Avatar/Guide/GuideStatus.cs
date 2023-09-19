using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideData
{
    public string groupTag;
    public int groupFinish;
    public string seqTag;
    public int seqFinish;
}

public class GuideStatusData
{
    public Dictionary<string, string> finishedGuideSeq = new Dictionary<string, string>();
    public Dictionary<string, string> finishedGuideGroup = new Dictionary<string, string>();
    public string activeSeq;
    public string activeSeqStep;
    public string activeDetailStep;
}

/// <summary>
/// 引导的状态   跟服务器的数据发送 数据处理
/// </summary>
public class GuideStatusMgr : Singleton<GuideStatusMgr>
{
    public bool Dirty = false;
    public bool WeakGuideDirty = false;
    private List<GuideData> m_Swap_PendingGuideData = new List<GuideData>();
    private List<GuideData> m_PendingGuideData = new List<GuideData>();
    public GuideStatusData m_GuideStatusData;
    private Dictionary<string, string> m_CompleteSeqGroups = new Dictionary<string, string>();

    /// <summary>
    ///  这里根据服务器的返回数据来赋值 TODO
    /// </summary>
    public void OnRequestGuideData()
    {
        m_GuideStatusData = new GuideStatusData();
    }

    public void UnpackCompleteSeq(Dictionary<string, GuideSequence> guideToSequence)
    {
        if (m_GuideStatusData == null)
        {
            m_GuideStatusData = new GuideStatusData();
            return;
        }

        if (m_GuideStatusData.finishedGuideSeq == null)
            return;

        foreach (var guideTag in guideToSequence.Keys)
        {
            var value = guideToSequence[guideTag];
            if (value != null)
            {
                if (m_GuideStatusData.finishedGuideSeq.ContainsKey(value.Head))
                {
                    if (!string.IsNullOrEmpty(m_GuideStatusData.finishedGuideSeq[value.Head]))
                    {
                        m_CompleteSeqGroups[guideTag] = "1";
                    }
                }
            }
        }

    }



    public bool GetActiveDetailSetp()
    {
        return string.IsNullOrEmpty(m_GuideStatusData.activeDetailStep);
    }

    public string GetActiveSeqSetp()
    {
        return m_GuideStatusData.activeSeqStep;
    }

    public void SetActiveSeqStep(GuideGroup group)
    {
        Dirty = true;
        m_GuideStatusData.activeSeqStep = group != null ? group.GetTag() : null;
    }

    public void SetActiveDetailStep(GuideStep step)
    {
        Dirty = true;
        m_GuideStatusData.activeSeqStep = step != null ? step.DetailConfig.Tag : null;
    }


    public void SetGuideGroupFinished(string guideTag, string seqTag)
    {
        Dirty = true;
        if (m_GuideStatusData.finishedGuideGroup == null)
            m_GuideStatusData.finishedGuideGroup = new Dictionary<string, string>();
        if (!m_GuideStatusData.finishedGuideGroup.ContainsKey(guideTag))
            m_GuideStatusData.finishedGuideGroup.Add(guideTag, "1");

        m_GuideStatusData.finishedGuideGroup[guideTag] = "1";
        GuideData guideData = new GuideData();
        guideData.groupTag = guideTag;
        guideData.groupFinish = 1;
        guideData.seqTag = seqTag;
        guideData.seqFinish = 0;
        m_PendingGuideData.Add(guideData);
    }

    /// <summary>
    ///  给  m_Swap_PendingGuideData 赋值
    /// </summary>
    public void StoreData()
    {
        Dirty = false;
        var temp = new List<GuideData>(m_Swap_PendingGuideData);
        m_Swap_PendingGuideData = new List<GuideData>(m_PendingGuideData);
        m_PendingGuideData = new List<GuideData>(temp);

        ClientLog.Instance.Log($"GuideStatus 当前引导状态 activeSeq {m_GuideStatusData.activeSeq}    activeSeqStep:  {m_GuideStatusData.activeSeqStep}       activeDetailStep:  {m_GuideStatusData.activeDetailStep}");

        // 给服务器发消息 来表明哪些已经完成
        //TODO  

    }

    public void SetActiveSeq(GuideSequence seq)
    {
        Dirty = true;
        m_GuideStatusData.activeSeq = seq != null ? seq.Head : null;
    }

    public void SetGuideSeqFinished(GuideSequence seq, Dictionary<string, GuideSequence> guideToSequence)
    {
        Dirty = true;
        if (m_GuideStatusData.finishedGuideSeq == null)
            m_GuideStatusData.finishedGuideSeq = new Dictionary<string, string>();

        m_GuideStatusData.finishedGuideSeq[seq.Head] = "1";
        List<string> tempList = new List<string>();

        foreach (var key in m_GuideStatusData.finishedGuideGroup.Keys)
        {
            if (guideToSequence[key] == seq)
            {
                tempList.Add(key);
            }
        }

        foreach (var key in tempList)
        {
            m_GuideStatusData.finishedGuideGroup[key] = null;
        }

        GuideData guideData = new GuideData();
        guideData.groupTag = null;
        guideData.groupFinish = 0;
        guideData.seqTag = seq.Head;
        guideData.seqFinish = 1;
        m_PendingGuideData.Add(guideData);
    }

    public bool IsGuideGroupFinished(string guideTag)
    {
        if (m_CompleteSeqGroups != null && m_CompleteSeqGroups.ContainsKey(guideTag))
        {
            return !string.IsNullOrEmpty(m_CompleteSeqGroups[guideTag]);
        }


        if (m_GuideStatusData.finishedGuideGroup == null)
            return false;


        if (m_GuideStatusData.finishedGuideGroup.ContainsKey(guideTag))
        {
            return !string.IsNullOrEmpty(m_GuideStatusData.finishedGuideGroup[guideTag]);
        }

        return false;
    }

    public string GetActiveSeq()
    {
        return m_GuideStatusData.activeSeq;
    }

    public int GetPendingGuideDataCount()
    {
        return m_PendingGuideData.Count;
    }

    public int GetSwap_PendingGuideDataCount()
    {
        return m_Swap_PendingGuideData.Count;
    }

    public override void OnRelease()
    {

    }
}
