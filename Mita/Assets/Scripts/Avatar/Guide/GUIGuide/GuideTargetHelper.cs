using cfg.Config;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GuideTargetHelper
{
    public bool TargetPosDirty;
    public EGuideTargetEnvType GuideViewTargetEnv;
    private GuideShowConfig m_ShowConfig;
    private GuideDetailConfig m_DetailConfig;
    // detailConfig,  showConfig
    private Dictionary<EGuideTargetType, EGuideTargetEnvType> m_GuideTargetType2Env = new Dictionary<EGuideTargetType, EGuideTargetEnvType>();
    private Dictionary<EGuideTargetType, Func<string, string, GameObject>> TargetAcquireFunc = new Dictionary<EGuideTargetType, Func<string, string, GameObject>>();
    private GUIGuide_PanelData m_UIData;
    public GuideTargetHelper(GUIGuide_PanelData uiData)
    {
        m_UIData = uiData;

        TargetPosDirty = true;
        m_GuideTargetType2Env.Add(EGuideTargetType.None, EGuideTargetEnvType.UIObject);
        m_GuideTargetType2Env.Add(EGuideTargetType.FixUIWidget, EGuideTargetEnvType.UIObject);

        TargetAcquireFunc.Add(EGuideTargetType.None, null);
        TargetAcquireFunc.Add(EGuideTargetType.FixUIWidget, GuideTargetAcquireUtil.GetFixUIObject);
    }

    public void OnStepStart(GuideDetailConfig detailConfig, GuideShowConfig showConfig)
    {
        m_DetailConfig = detailConfig;
        m_ShowConfig = showConfig;
        TargetPosDirty = true;
        CleanUp();
    }

    public void OnStepComplete()
    {
        CleanUp();
    }

    public void CleanUp()
    {
        guideViewTarget = null;
        guideViewTargetEnv = EGuideTargetEnvType.None;
        guideInputTarget = null;
        guideInputTargetEnv = EGuideTargetEnvType.None;
    }

    public void FrameUpdate()
    {
        TargetPosDirty = true;
    }

    public void CalcTargetUICenter()
    {
        CalcShowConfigCenter();
    }
    public float TargetCenterOnScreenX;
    public float TargetCenterOnScreenY;
    private void CalcShowConfigCenter()
    {
        if (!TargetPosDirty) return;
        var targetEnv = GetGuideViewTargeEnv();
        var target = GetGuideViewTarget();
        float x = 0f, y = 0f;
        if (target != null)
        {
            if (targetEnv == EGuideTargetEnvType.UIObject)
            {
                GuideInputFilter.CalcUIRectCenter(target, out x, out y);
            }
        }

        TargetCenterOnScreenX = x;
        TargetCenterOnScreenY = y;
        TargetPosDirty = false;
    }
    public GameObject guideViewTarget;
    public GameObject GetGuideViewTarget()
    {
        if (m_ShowConfig == null) return null;

        if (guideViewTarget == null)
            guideViewTarget = GetGuideViewTargetRaw((EGuideTargetType)m_ShowConfig.GuideShowTargetType, m_ShowConfig.GuideShowTargetParam, m_ShowConfig.GuideShowTargetParam2);

        return guideViewTarget;
    }

    public GameObject GetGuideViewTargetRaw(EGuideTargetType viewTargetType, string viewTargetParam, string viewTargetParam2)
    {

        if (viewTargetType == EGuideTargetType.None)
            return GetGuideInputTarget();

        if (TargetAcquireFunc.ContainsKey(viewTargetType))
            return TargetAcquireFunc[viewTargetType]?.Invoke(viewTargetParam, viewTargetParam2);

        return null;
    }

    public GameObject guideInputTarget;
    // view使用input target时调用，增加一些检查性质对输出
    public GameObject GetGuideInputTarget()
    {
        var inputMaskTargetType = (EGuideTargetType)m_DetailConfig.InputMaskTargetType;

        if (guideInputTarget == null)
        {
            if (TargetAcquireFunc.ContainsKey(inputMaskTargetType))
            {
                guideInputTarget = TargetAcquireFunc[inputMaskTargetType]?.Invoke(m_DetailConfig.InputMaskTargetParam, m_DetailConfig.InputMaskTargetParam2);
                return guideInputTarget;
            }

            ClientLog.Instance.LogError($"未处理的Input目标类型{inputMaskTargetType}");
        }
        return guideInputTarget;
    }




    EGuideTargetEnvType guideViewTargetEnv;
    EGuideTargetEnvType guideInputTargetEnv;
    public EGuideTargetEnvType GetGuideViewTargeEnv()
    {
        if (guideViewTargetEnv != EGuideTargetEnvType.None) return guideViewTargetEnv;
        guideViewTargetEnv = GetGuideViewTargeEnvRaw(m_ShowConfig.GuideShowTargetType);
        return guideViewTargetEnv;
    }

    public EGuideTargetEnvType GetGuideViewTargeEnvRaw(int target = 0)
    {
        EGuideTargetType showSourceType = EGuideTargetType.None;
        if (target == (int)EGuideTargetType.None)
        {
            return GetGuideInputTargeEnv();
        }
        else
        {
            showSourceType = (EGuideTargetType)target;
            if (m_GuideTargetType2Env.ContainsKey(showSourceType))
                return m_GuideTargetType2Env[showSourceType];
        }

        ClientLog.Instance.LogError($"未处理的Input目标类型{showSourceType}");
        return EGuideTargetEnvType.None;
    }

    public EGuideTargetEnvType GetGuideInputTargeEnv()
    {
        if (guideInputTargetEnv != EGuideTargetEnvType.None) return guideInputTargetEnv;

        var inputMaskTargetType = (EGuideTargetType)m_DetailConfig.InputMaskTargetType;
        if (m_GuideTargetType2Env.ContainsKey(inputMaskTargetType))
            return m_GuideTargetType2Env[inputMaskTargetType];

        guideInputTargetEnv = m_GuideTargetType2Env[inputMaskTargetType];
        ClientLog.Instance.LogError($"未处理的Input目标类型{inputMaskTargetType}");
        return EGuideTargetEnvType.None;
    }

    private Camera m_GuideInputTargetWorldCamera;
    /// <summary>
    /// 如果按使用到3D 就需要找到3D的摄像机
    /// </summary>
    /// <param name="targetEnvType"></param>
    public Camera GetGuideInputTargetWorldCamera(EGuideTargetEnvType targetEnvType)
    {
        if (m_GuideInputTargetWorldCamera != null)
            GetWorldCamera(targetEnvType);

        return m_GuideInputTargetWorldCamera;
    }
    /// <summary>
    /// 如果有多个摄像机 取到其中需要的
    /// </summary>
    /// <returns></returns>
    public Camera GetWorldCamera(EGuideTargetEnvType targetEnvType)
    {
        if (targetEnvType == EGuideTargetEnvType.None) return null;
        else if (targetEnvType == EGuideTargetEnvType.UIObject) return null;

        return null;
    }

    //找到 相应的目标UI
    public GameObject GetFixUIObject()
    {
        return null;
    }

    public void OnRelease()
    {

    }
}
