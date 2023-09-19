using cfg.Config;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 新手引导 处理文字 以及对话的 可能有多个这种对话
// 0、没有引导板子
// 1、短句，箭头朝上
// 2、短句，箭头朝下
// 3、小头像，箭头朝左,头像在右
// 4、小头像，箭头朝右，头像在左
// 5、半身像，贴底部
/// 
/// </summary>
public class GuideDialog
{
    private GUIGuide_PanelData m_UIData;
    private GuideDetailConfig m_DetailConfig;
    private GuideShowConfig m_ShowConfig;
    private GameObject m_DialogObj;
    private GuideTargetHelper m_GuideTargetHelper;

    public GuideDialog(GUIGuide_PanelData uiData, GuideTargetHelper guideTargetHelper)
    {
        m_UIData = uiData;
        m_GuideTargetHelper = guideTargetHelper;
        GlobalFunction.SetGameObjectVisibleState(m_DialogObj, false);
    }

    public void OnStepStart(GuideDetailConfig detailConfig, GuideShowConfig showConfig)
    {
        m_DetailConfig = detailConfig;
        m_ShowConfig = showConfig;

        if (m_ShowConfig == null) return;
        if (m_ShowConfig.DialogMode == (int)EPlotType.None) return;
        m_DialogObj = m_UIData.GoDialog;
        if (m_DialogObj == null) ClientLog.Instance.LogError($"未处理的dialog {m_ShowConfig.DialogMode}");
        //如果有对话 m_DialogObj = 对话的Obj板子 里面也会有对话框 给相应的 文字组件赋值
        GlobalFunction.SetGameObjectVisibleState(m_DialogObj, true);
        GlobalFunction.SetText(m_UIData.TxtDialog, m_ShowConfig.DialogContent);
        UpdateDialogPosition();
    }

    public void OnStepComplete()
    {
        GlobalFunction.SetGameObjectVisibleState(m_DialogObj, false);
        m_DialogObj = null;
    }

    public void FrameUpdate()
    {
        if (m_DialogObj == null) return;
        UpdateDialogPosition();
    }

    private void UpdateDialogPosition()
    {
        if (m_ShowConfig == null) return;
        if (m_ShowConfig.DialogMode == (int)EPlotType.HalfIcon) return;
        m_GuideTargetHelper.CalcTargetUICenter();
        ObjectUtils.SetRectTransformAnchoredPosition(
            m_DialogObj,
             m_GuideTargetHelper.TargetCenterOnScreenX + m_ShowConfig.DialogPosX,
            m_GuideTargetHelper.TargetCenterOnScreenY + m_ShowConfig.DialogPosY);
    }

    public void OnRelease()
    {
        m_DetailConfig = null;
        m_ShowConfig = null;
    }

}
