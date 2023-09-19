using cfg.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  处理 目标物的显示 手指 圆圈等
/// </summary>
public class GuideTargetMarker
{
    private GUIGuide_PanelData m_UIData;
    private GuideDetailConfig m_DetailConfig;
    private GuideShowConfig m_ShowConfig;
    private GuideTargetHelper m_GuideTargetHelper;

    public GuideTargetMarker(GUIGuide_PanelData uiData, GuideTargetHelper guideTargetHelper)
    {
        m_UIData = uiData;
        m_GuideTargetHelper = guideTargetHelper;
    }

    public void OnStepStart(GuideDetailConfig detailConfig, GuideShowConfig showConfig)
    {
        m_DetailConfig = detailConfig;
        m_ShowConfig = showConfig;

        if (!IsMarkerTypeValid()) return;

        GlobalFunction.SetGameObjectVisibleState(m_UIData.GoSelect, false);
        GlobalFunction.SetGameObjectVisibleState(m_UIData.GoCircle, false);
        GlobalFunction.SetGameObjectVisibleState(m_UIData.GoFinger, false);

        if (m_ShowConfig.TargetMarkerType == (int)EGuideTargetMarkerType.Circle)
        {
            GlobalFunction.SetGameObjectVisibleState(m_UIData.GoCircle, true);
        }
        else if (m_ShowConfig.TargetMarkerType == (int)EGuideTargetMarkerType.CircleFinger)
        {
            GlobalFunction.SetGameObjectVisibleState(m_UIData.GoCircle, true);
            GlobalFunction.SetGameObjectVisibleState(m_UIData.GoFinger, true);
        }
        else if (m_ShowConfig.TargetMarkerType == (int)EGuideTargetMarkerType.Rect)
        {
            GlobalFunction.SetGameObjectVisibleState(m_UIData.GoSelect, true);
        }
        else if (m_ShowConfig.TargetMarkerType == (int)EGuideTargetMarkerType.RectFinger)
        {
            GlobalFunction.SetGameObjectVisibleState(m_UIData.GoSelect, true);
            GlobalFunction.SetGameObjectVisibleState(m_UIData.GoFinger, true);
        }
        else if (m_ShowConfig.TargetMarkerType == (int)EGuideTargetMarkerType.Finger)
        {
            GlobalFunction.SetGameObjectVisibleState(m_UIData.GoFinger, true);
        }

        SetMarker();
    }

    public void SetMarker()
    {
        m_GuideTargetHelper.CalcTargetUICenter();
        var viewTarget = m_GuideTargetHelper.GetGuideViewTarget();
        ObjectUtils.SetRectTransformAnchoredPosition(m_UIData.GoSelect, m_GuideTargetHelper.TargetCenterOnScreenX, m_GuideTargetHelper.TargetCenterOnScreenY);

        GuideInputFilter.SetRectSizeByUIObject((RectTransform)m_UIData.GoSelect.transform, viewTarget, 12, 12);

        ObjectUtils.SetRectTransformAnchoredPosition(m_UIData.GoCircle, m_GuideTargetHelper.TargetCenterOnScreenX, m_GuideTargetHelper.TargetCenterOnScreenY);

        float fingerPosX = m_GuideTargetHelper.TargetCenterOnScreenX + m_ShowConfig.FingerPosX;
        float fingerPosY = m_GuideTargetHelper.TargetCenterOnScreenY + m_ShowConfig.FingerPosY;

        ObjectUtils.SetRectTransformAnchoredPosition(m_UIData.GoFinger, m_GuideTargetHelper.TargetCenterOnScreenX, m_GuideTargetHelper.TargetCenterOnScreenY);

        var normalizedFingerX = (fingerPosX / Screen.width);
        var normalizedFingerY = (fingerPosY / Screen.height);
        int dir = 0;
        // 减少错误计算
        if (Mathf.Abs(normalizedFingerX) < 0.01)
            normalizedFingerX = 0.01f;
        if (Mathf.Abs(normalizedFingerY) < 0.01)
            normalizedFingerY = 0.01f;
        if ((normalizedFingerX * normalizedFingerY) > 0)
            dir++; // 1,3

        if (normalizedFingerX > 0)
            dir += 2; //右

        // 0:左上 r :0
        // 1:左下 r :90
        // 2:右下 r :180
        // 3:右上 r :270
        ObjectUtils.SetObjectLocalEulerangles(m_UIData.GoFinger, 0, 0, 90 * dir);
    }

    public void OnStepComplete()
    {
        GlobalFunction.SetGameObjectVisibleState(m_UIData.GoSelect, false);
        GlobalFunction.SetGameObjectVisibleState(m_UIData.GoCircle, false);
        GlobalFunction.SetGameObjectVisibleState(m_UIData.GoFinger, false);
    }

    public void FrameUpdate()
    {
        if (IsMarkerTypeValid())
            SetMarker();
    }

    public bool IsMarkerTypeValid()
    {
        if (m_ShowConfig == null) return false;
        if (m_ShowConfig.TargetMarkerType == (int)EGuideTargetMarkerType.None) return false;
        if (m_ShowConfig.TargetMarkerType == (int)EGuideTargetMarkerType.DragArrow) return false;

        return true;
    }


    public void OnRelease()
    {
        m_DetailConfig = null;
        m_ShowConfig = null;
    }
}
