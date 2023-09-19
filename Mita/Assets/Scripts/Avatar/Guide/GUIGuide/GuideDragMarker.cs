using cfg.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 新手引导 拖拽
/// </summary>
public class GuideDragMarker
{
    private GUIGuide_PanelData m_UIData;
    private GuideDetailConfig m_DetailConfig;
    private GuideShowConfig m_ShowConfig;
    private GuideFromToPointer m_GuideFromToPointer;
    private GuideTargetHelper m_GuideTargetHelper;
    public GameObject guideViewTarget;
    private EGuideTargetEnvType m_GuideDragDestType;
    private Camera m_WorldCamera;
    public GuideDragMarker(GUIGuide_PanelData uiData, GuideTargetHelper guideTargetHelper)
    {
        m_UIData = uiData;
        m_GuideTargetHelper = guideTargetHelper;

        if (uiData != null && uiData.GoFromToPointer != null)
            m_GuideFromToPointer = uiData.GoFromToPointer.GetComponent<GuideFromToPointer>();
        GlobalFunction.SetGameObjectVisibleState(m_UIData.GoFromToPointer, false);
    }

    public void OnStepStart(GuideDetailConfig detailConfig, GuideShowConfig showConfig)
    {
        m_DetailConfig = detailConfig;
        m_ShowConfig = showConfig;

        if (!IsMarkerTypeValid()) return;

        SetMarker();
    }

    public void OnStepComplete()
    {
        GlobalFunction.SetGameObjectVisibleState(m_UIData.GoFromToPointer, false);
    }

    public void FrameUpdate()
    {

    }

    public void SetMarker()
    {
        var originTarget = m_GuideTargetHelper.GetGuideViewTarget();
        var originType = m_GuideTargetHelper.GetGuideViewTargeEnv();
        if (guideViewTarget == null)
        {
            guideViewTarget = m_GuideTargetHelper.GetGuideViewTargetRaw((EGuideTargetType)m_ShowConfig.GuideShowTargetType, m_ShowConfig.GuideShowTargetParam, m_ShowConfig.GuideShowTargetParam2);

            m_GuideDragDestType = m_GuideTargetHelper.GetGuideViewTargeEnvRaw(m_ShowConfig.DragDestTargetType);
        }

        if (m_WorldCamera == null)
        {
            EGuideTargetEnvType dragEnv;
            dragEnv = m_GuideDragDestType > originType ? m_GuideDragDestType : originType;
            m_WorldCamera = m_GuideTargetHelper.GetWorldCamera(dragEnv);
        }
        if (originTarget == null) ClientLog.Instance.LogError("拖拽起点未找到,检查是否还未创建");
        if (guideViewTarget == null) ClientLog.Instance.LogError("拖拽终点未找到,检查是否还未创建");

        m_GuideFromToPointer.SetPointerParam(m_WorldCamera, originTarget, (TargetType)EnvTypeToDragTargetType(originType), guideViewTarget, (TargetType)EnvTypeToDragTargetType(m_GuideDragDestType));

        GlobalFunction.SetGameObjectVisibleState(m_UIData.GoFromToPointer, true);
    }

    public int EnvTypeToDragTargetType(EGuideTargetEnvType envType)
    {
        if (envType == EGuideTargetEnvType.None) return 0;
        return envType == EGuideTargetEnvType.UIObject ? 0 : 1;
    }


    public bool IsMarkerTypeValid()
    {
        if (m_ShowConfig == null) return false;
        if (m_ShowConfig.TargetMarkerType == (int)EGuideTargetMarkerType.DragArrow) return true;

        return false;
    }

    public void OnRelease()
    {

    }
}
