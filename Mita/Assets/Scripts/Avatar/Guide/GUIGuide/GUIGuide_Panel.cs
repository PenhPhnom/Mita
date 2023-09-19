using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 引导界面  写一块太乱，每一个脚本处理不同引导 
/// </summary>
public class GUIGuide_Panel : BaseUI
{
    private GUIGuide_PanelData m_UIData = new GUIGuide_PanelData();
    public GuideStep ActiveStep;
    public GuideSequence ActiveSeq;
    private GuideDialog m_GuideDialog;
    private GuideMask m_GuideMask;
    private GuideTargetHelper m_GuideTargetHelper;
    private GuideTargetMarker m_GuideTargetMarker;
    private GuideDragMarker m_GuideDragMarker;

    protected override void OnAwake(GameObject obj)
    {
        if (m_UIData == null) return;
        m_UIData.InitUI(obj);
        m_GuideTargetHelper = new GuideTargetHelper(m_UIData);
        m_GuideDialog = new GuideDialog(m_UIData, m_GuideTargetHelper);
        m_GuideMask = new GuideMask(m_UIData, m_GuideTargetHelper);
        m_GuideTargetMarker = new GuideTargetMarker(m_UIData, m_GuideTargetHelper);
        m_GuideDragMarker = new GuideDragMarker(m_UIData, m_GuideTargetHelper);

        GlobalFunction.AddEnevntTrigger(m_UIData.BtnInput.gameObject, EnumTouchEventType.OnClick, OnBtnInputClick);
        GlobalFunction.AddEnevntTrigger(m_UIData.BtnKillCurGuide.gameObject, EnumTouchEventType.OnClick, OnBtnKillCurGuideClick);
    }

    public override EnumUIType GetUIType()
    {
        return EnumUIType.GUIDE;
    }

    public void OnStepStart(GuideStep step)
    {
        //    public GuideDetailConfig _config;
        //   public GuideShowConfig _showConfig;
        if (null != ActiveStep)
            ClientLog.Instance.LogError("当前step没有清空");

        ActiveStep = step;
        m_GuideTargetHelper.OnStepStart(step.DetailConfig, step.ShowConfig);
        m_GuideDialog.OnStepStart(step.DetailConfig, step.ShowConfig);
        m_GuideMask.OnStepStart(step.DetailConfig, step.ShowConfig);
        m_GuideTargetMarker.OnStepStart(step.DetailConfig, step.ShowConfig);
        m_GuideDragMarker.OnStepStart(step.DetailConfig, step.ShowConfig);
    }


    public void OnStepComplete(GuideStep step)
    {
        if (step != ActiveStep)
            ClientLog.Instance.LogError("停止的step不是当前step");

        if (step == null)
            ClientLog.Instance.LogError("不能停止null的step");

        ActiveStep = null;
        m_GuideTargetHelper.OnStepComplete();
        m_GuideDialog.OnStepComplete();
        m_GuideMask.OnStepComplete();
        m_GuideTargetMarker.OnStepComplete();
        m_GuideDragMarker.OnStepComplete();
    }


    public override void OnRelease()
    {
        m_GuideTargetHelper.OnRelease();
        m_GuideDialog.OnRelease();
        m_GuideMask.OnRelease();
        m_GuideTargetMarker.OnRelease();
        m_GuideDragMarker.OnRelease();
    }

    public void SetSeq(GuideSequence seq)
    {
        ActiveSeq = seq;
    }

    public void SetGuideGroup(GuideGroup group)
    {

    }

    private void OnBtnInputClick(GameObject _listener, object _args, params object[] _params)
    {
        EventMgr.Instance.FireEvent(EEventType.GUIDE_CLICK_ON_INPUT);
    }

    private void OnBtnKillCurGuideClick(GameObject _listener, object _args, params object[] _params)
    {
        if (GuideCore.Instance.ActiveSeq != null)
            GuideCore.Instance.ActiveSeq.GuideGroupKill();

    }

    public void FrameUpdate()
    {
        if (ActiveStep == null) return;
        m_GuideTargetHelper.FrameUpdate();
        m_GuideDialog.FrameUpdate();
        m_GuideMask.FrameUpdate();
        m_GuideTargetMarker.FrameUpdate();
        m_GuideDragMarker.FrameUpdate();
    }
}
