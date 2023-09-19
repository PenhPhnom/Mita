using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ProfileMgr : Singleton<ProfileMgr>
{
    private Shader shader;
    private Shader sceneShader;
    private Bloom m_Bloom;
    private DepthOfField m_DepthOfField;
    private LiftGammaGain m_LiftGammaGain;
    private Tonemapping m_Tonemapping;
    private int m_level = 5;

    public int GetLevel()
    {
        return m_level;
    }

    public void InitProfile()
    {
        bool bBloom = false, bDepth = false, bShadow = false, bMAX = false;

        if (m_level > 1) { bBloom = true; bDepth = true; bShadow = true; bMAX = true; }


        ParamData data = new ParamData();
        data.bParam = bBloom;
        data.bParam2 = bDepth;
        data.bParam3 = bShadow;
        data.bParam4 = bMAX;
        /// 加载 Shader 根据 手机机型  来加载 shader的 Lod 以及 后处理
        ResourceMgr.Instance.LoadResourceAsync("SceneLitLux", TypeInts.Shader, (a, b) =>
        {
            sceneShader = (Shader)a;
            GameObject go = GameObject.Find("volume");
            if (go != null)
            {
                Volume volum = go.GetComponent<Volume>();
                if (volum != null)
                {
                    foreach (var item in volum.profile.components)
                    {
                        if (item is Bloom)
                        {
                            m_Bloom = item as Bloom;
                        }
                        else if (item is DepthOfField)
                        {
                            m_DepthOfField = item as DepthOfField;
                        }
                    }
                }
            }
            else
            {
                ClientLog.Instance.Log("场景找不到 volume ");
            }
            SetProfile(b.bParam, b.bParam2, b.bParam3, b.bParam4);
        }, "scenelitlux", null, data);
    }

    public void SetProfile(bool bBloom, bool bDepth, bool bShadow, bool bMax)
    {
        SetBloom(bBloom);
        SetDepthOfField(bDepth);
        //SetShadow(bShadow);
        SetSceneShaderLod(bMax);
    }

    /// <summary>
    /// 设置Bloom
    /// </summary>
    /// <param name="bVal"></param>
    public void SetBloom(bool bVal)
    {
        if (m_Bloom)
        {
            m_Bloom.active = bVal;
            ClientLog.Instance.Log("Profile m_Bloom:  ", m_Bloom.active);
        }
    }

    /// <summary>
    /// 设置井深 DepthOfField
    /// </summary>
    /// <param name="bVal"></param>
    public void SetDepthOfField(bool bVal)
    {
        if (m_DepthOfField)
        {
            m_DepthOfField.active = bVal;
            ClientLog.Instance.Log("Profile m_DepthOfField:  ", m_DepthOfField.active);
        }
    }

    /// <summary>
    /// 设置阴影
    /// </summary>
    /// <param name="bVal">是否需要阴影</param>
    public void SetShadow(bool bVal)
    {
        GameObject go = GameObject.Find("LightPlayer");
        if (go != null)
        {
            Light light = go.GetComponent<Light>();

            if (!bVal)
                light.shadows = LightShadows.None;
        }
    }

    public void SetSceneShaderLod(bool bMax)
    {
        if (sceneShader != null)
        {
            sceneShader.maximumLOD = bMax ? 300 : 100;
            ClientLog.Instance.Log("Profile SceneShader maximumLOD： ", sceneShader.maximumLOD);
        }
    }

    //设置 Shader 的 Lod
    public void SetShaderLod(Shader _shader)
    {
        shader = _shader;
        SetShaderLod();
    }

    private void SetShaderLod()
    {
        bool bMax = false;
        if (m_level > 1) bMax = true;

        if (shader != null)
        {
            shader.maximumLOD = bMax ? 300 : 100;
            ClientLog.Instance.Log("Profile Shader maximumLOD： ", shader.maximumLOD);
        }
    }


    /// <summary>
    /// App会多次启动，Screen.height 每次改变就会修改 只能APP传进来
    /// </summary>
    /// <param name="with"></param>
    /// <param name="high"></param>
    public void SetResolution(int with, int high, int level)
    {
        m_level = level;
        if (shader != null)
            SetShaderLod();
        int scWidth = 0;
        int designWidth = Screen.width, designHeight = Screen.height;

#if UNITY_IOS
        scWidth = GlobalValue.IOS_Width;
        designWidth = (int)(with * 0.8);
        designHeight = (int)(high * 0.8);

        if (designWidth <= scWidth)
        {
            designWidth = scWidth;
            designHeight = scWidth * high / with;
        }
#elif UNITY_ANDROID && !UNITY_EDITOR
        // 高端
        if (level == 2)
        {
            designWidth = GlobalValue.Andro_Hight_Width;
            designHeight = designWidth * high / with;
        }
        else  // 低端
        {
            designWidth = GlobalValue.Andro_Low_Width;
            designHeight = designWidth * high / with;
        }
#endif
        Screen.SetResolution(designWidth, designHeight, true);
        ClientLog.Instance.Log("SetResolution: ", " designWidth: ", designWidth, " designHeight: ", designHeight, " level ", level);
    }

    /// <summary>
    /// 固定高度的优化
    /// </summary>
    /// <param name="with"></param>
    /// <param name="high"></param>
    public void SetResolutionFixed(int with, int high, int level)
    {
        m_level = level;
        if (shader != null)
            SetShaderLod();
        int scWidth = 0;
        int designWidth = Screen.width, designHeight = Screen.height;

#if !UNITY_EDITOR
        // 高端
        if (level == 2)
        {
            designWidth = 1080;
            designHeight = designWidth * high / with;
        }
        else  // 低端
        {
            designWidth = 720;
            designHeight = designWidth * high / with;
        }
#endif
        Screen.SetResolution(designWidth, designHeight, true);
        ClientLog.Instance.Log("SetResolution: ", " designWidth: ", designWidth, " designHeight: ", designHeight, " level ", level);
    }



    public override void OnRelease()
    {
        if (shader != null)
            shader = null;
        if (sceneShader != null)
            sceneShader = null;
        if (m_Bloom != null)
            m_Bloom = null;
        if (m_DepthOfField != null)
            m_DepthOfField = null;
        if (m_LiftGammaGain != null)
            m_LiftGammaGain = null;
        if (m_Tonemapping != null)
            m_Tonemapping = null;
        m_level = 5;
    }
}
