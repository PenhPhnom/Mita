using cfg.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 新手引导 遮罩
/// </summary>
public class GuideMask
{
    private GUIGuide_PanelData m_UIData;
    private GuideDetailConfig m_DetailConfig;
    private GuideShowConfig m_ShowConfig;
    private GuideTargetHelper m_GuideTargetHelper;
    private UIRawGuideMask m_MaskClass;
    private const float maskBlackAlpha = 0.8f;
    private const float maskTranslateAlpha = 0;
    private const float maskAlphaTime = 0.3f;
    private float m_MaskActiveFrame = 0;
    private int m_FadeScheduler;
    public GuideMask(GUIGuide_PanelData uiData, GuideTargetHelper guideTargetHelper)
    {
        m_UIData = uiData;
        m_GuideTargetHelper = guideTargetHelper;

        m_MaskClass = m_UIData.GoMask.GetComponent<UIRawGuideMask>();
        GlobalFunction.AddEnevntTrigger(m_UIData.GoInputFilter, EnumTouchEventType.OnClick, OnMaskClick);

        GlobalFunction.SetGameObjectVisibleState(m_UIData.BtnKillCurGuide, false);
    }

    public void OnStepStart(GuideDetailConfig detailConfig, GuideShowConfig showConfig)
    {
        m_DetailConfig = detailConfig;
        m_ShowConfig = showConfig;
        SetInputMask();
        InitMaskAlpha();
        SetViewMaskState();
        m_MaskActiveFrame = Time.frameCount;
    }

    public void OnStepComplete()
    {
        GuideInputFilter.SetRaycastParam(null, MaskType.None);
        GlobalFunction.SetGameObjectVisibleState(m_UIData.GoInputFilter, false);
        GlobalFunction.SetGameObjectVisibleState(m_UIData.GoMask, false);
        GlobalFunction.SetGameObjectVisibleState(m_UIData.BtnInput, false);
        GlobalFunction.SetGameObjectVisibleState(m_UIData.BtnKillCurGuide, false);
        TimeMgr.Instance.UnRegister(m_FadeScheduler);
    }

    public void FrameUpdate()
    {
        SetInputMask();
        SetViewMaskState();
    }

    private void OnMaskClick(GameObject _listener, object _args, params object[] _params)
    {
        if (m_ShowConfig == null) return;
        //MessageBox.Instance.ShowMessageBox(m_ShowConfig.MaskClickMessage);
        FadeInGuideMask();
        if (IsMaskClickValid())
            EventMgr.Instance.FireEvent(EEventType.GUIDE_CLICK_ON_INPUT_MASK);

        GlobalFunction.SetGameObjectVisibleState(m_UIData.BtnKillCurGuide, true);
    }


    private void SetInputMask()
    {
        var maskTarget = m_GuideTargetHelper.GetGuideInputTarget();
        var maskTargetType = m_GuideTargetHelper.GetGuideInputTargeEnv();
        if (maskTargetType == EGuideTargetEnvType.None)
        {
            GuideInputFilter.SetRaycastParam(null, MaskType.None);
            GlobalFunction.SetGameObjectVisibleState(m_UIData.GoInputFilter, false);
        }
        else
        {
            if (maskTarget == null)
                maskTargetType = EGuideTargetEnvType.None;

            GlobalFunction.SetGameObjectVisibleState(m_UIData.GoInputFilter, true);
            GuideInputFilter.SetRaycastParam(maskTarget, (MaskType)maskTargetType, m_GuideTargetHelper.GetGuideInputTargetWorldCamera(maskTargetType));
        }
    }

    private void InitMaskAlpha()
    {
        if (m_ShowConfig == null) return;
        m_MaskClass?.SetAlpha(m_ShowConfig.FadeMask == (int)EMaskFade.FadeByClick ? maskTranslateAlpha : maskBlackAlpha, 0);
    }

    private void SetViewMaskState()
    {
        if (m_ShowConfig == null) return;
        var maskTarget = m_GuideTargetHelper.GetGuideViewTarget();
        var maskTargetType = m_GuideTargetHelper.GetGuideViewTargeEnv();
        if (maskTargetType == EGuideTargetEnvType.None)
        {
            GlobalFunction.SetGameObjectVisibleState(m_UIData.GoMask, false);
            return;
        }
        GlobalFunction.SetGameObjectVisibleState(m_UIData.GoMask, true);

        m_GuideTargetHelper.CalcTargetUICenter();
        if (m_ShowConfig.MaskHighLightShape == (int)EMaskHighLightShape.Circle)
        {
            m_MaskClass.CreateCircleMask(m_GuideTargetHelper.TargetCenterOnScreenX, m_GuideTargetHelper.TargetCenterOnScreenY, m_ShowConfig.MaskHighLightParam);
        }
        else if (m_ShowConfig.MaskHighLightShape == (int)EMaskHighLightShape.Rect)
        {
            m_MaskClass.CreateRectangleMask(maskTarget);
        }
        else if (m_ShowConfig.MaskHighLightShape == (int)EMaskHighLightShape.CornerRect)
        {
            m_MaskClass.CreateCornerRectangleMask(maskTarget, m_ShowConfig.MaskHighLightParam);
        }
        else if (m_ShowConfig.MaskHighLightShape == (int)EMaskHighLightShape.FullScreen)
        {
            m_MaskClass.CreateCircleMask();
        }
        else if (m_ShowConfig.MaskHighLightShape == (int)EMaskHighLightShape.NoBlackMask)
        {
            m_MaskClass.CreateCornerRectangleMask(maskTarget, 0);
        }
        else
        {
            // 如果到这里就错了呀
            ClientLog.Instance.LogError($"未处理的遮挡高光类型{m_ShowConfig.Tag}   {m_ShowConfig.MaskHighLightShape}  {maskTargetType}   {maskTarget}");
        }

    }

    private bool IsMaskClickValid()
    {
        return Time.frameCount > m_MaskActiveFrame;
    }

    private void FadeInGuideMask()
    {
        if (m_ShowConfig == null || m_MaskClass == null) return;
        if (m_ShowConfig.FadeMask == (int)EMaskFade.FadeByClick)
        {
            m_MaskClass.FadeAlpha(maskBlackAlpha, maskAlphaTime);
            m_FadeScheduler = TimeMgr.Instance.Register(1, 1f, FadeOutGuideMask);
        }
    }

    private void FadeOutGuideMask(object param)
    {
        if (m_ShowConfig.FadeMask == (int)EMaskFade.FadeByClick)
        {
            m_MaskClass.FadeAlpha(maskTranslateAlpha, maskAlphaTime);
        }

        TimeMgr.Instance.UnRegister(m_FadeScheduler);
    }

    public void OnRelease()
    {
        TimeMgr.Instance.UnRegister(m_FadeScheduler);
    }
}
