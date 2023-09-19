using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �� Image ��RawImage ��ֵ  //TODO  ��Ҫ��������֤ �Ƿ�������ڴ�
/// </summary>
public class UISpriteMgr : Singleton<UISpriteMgr>
{
    private Dictionary<string, TPAtlas> m_AtlasDic = new Dictionary<string, TPAtlas>();
    // 
    public void SetImage(Image img, string atlasName, string spriteName, bool isSetNative, Action callBack, float alpha, string abName = null, string abFolderPath = null)
    {
        if (img != null)
        {
            if (string.IsNullOrEmpty(atlasName) || string.IsNullOrEmpty(spriteName))
            {
                callBack?.Invoke();
                return;
            };

            if (m_AtlasDic != null && m_AtlasDic.ContainsKey(atlasName))
            {
                //�ҳ�Ҫ�ҵ�Image Ȼ��ֵ
                foreach (var item in m_AtlasDic[atlasName].sprites)
                {
                    if (item && item.name == spriteName)
                    {
                        img.sprite = item;
                        break;
                    }
                }
                SetGraphicColorRaw(img, img.color.r, img.color.g, img.color.b, alpha);
                if (isSetNative) img.SetNativeSize();
                callBack?.Invoke();
                return;
            }

            ParamData pParamData = new ParamData();
            pParamData.bParam = isSetNative;
            pParamData.fParam = alpha;
            pParamData.objectParam = img;
            pParamData.sParam = spriteName;
            pParamData.sParam2 = atlasName;
            pParamData.callBack = callBack;

            ResourceMgr.Instance.LoadResourceAsync(atlasName, TypeInts.Atlas, (obj, param) =>
            {
                if (obj == null) return;
                TPAtlas atlas = (obj) as TPAtlas;//ScriptableObject.Instantiate
                Image pImg = (Image)param.objectParam;
                foreach (var item in atlas.sprites)
                {
                    if (item && item.name == param.sParam)
                    {
                        pImg.sprite = item;
                        break;
                    }
                }

                SetGraphicColorRaw(pImg, pImg.color.r, pImg.color.g, pImg.color.b, param.fParam);
                if (param.bParam2) pImg.SetNativeSize();
                param.callBack?.Invoke();
                m_AtlasDic[param.sParam2] = atlas;
                //ResourceMgr.Instance.UnLoadResource(atlas, TypeInts.Atlas);
                //atlas = null;
            }, abName, abFolderPath, pParamData);
        }
    }

    /// <summary>
    /// ���� RawImage
    /// </summary>
    /// <param name="raw">RawImage</param>
    /// <param name="name">texture������</param>
    /// <param name="abName">�����ڵ�AB��</param>
    /// <param name="isSetNative"></param>
    /// <param name="isLoadingAlpha"></param>
    /// <param name="callback"></param>
    /// <param name="abFolderPath">AB����·��</param>
    /// <param name="paramData"></param>
    public void SetRawImage(RawImage raw, string name, string abName, bool isSetNative = false, bool isLoadingAlpha = true, Action callback = null, string abFolderPath = null)
    {
        if (raw != null)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(abName))
            {
                callback?.Invoke();
                return;
            };

            //Ϊ�˱����������
            if (isLoadingAlpha) SetGraphicColorRaw(raw, raw.color.r, raw.color.g, raw.color.b, 0);

            ParamData pParamData = new ParamData();
            pParamData.bParam = isLoadingAlpha;
            pParamData.bParam2 = isSetNative;
            pParamData.objectParam = raw;
            pParamData.callBack = callback;

            ResourceMgr.Instance.LoadResourceAsync(name, TypeInts.Texture, (obj, param) =>
            {
                if (obj == null) return;
                RawImage pImg = (RawImage)param.objectParam;
                pImg.texture = (Texture)obj;
                if (param.bParam) SetGraphicColorRaw(raw, raw.color.r, raw.color.g, raw.color.b, 1);
                if (param.bParam2)
                    pImg.SetNativeSize();
                param.callBack?.Invoke();
            }, abName, abFolderPath, pParamData);
        }
    }

    public void SetGraphicColorRaw(Graphic graphic, float r, float g, float b, float a)
    {
        if (graphic == null)
            return;
        graphic.color = new Color(r, g, b, a);
    }

    // ͼƬ��ɫ
    public void SetImageSaturation(Graphic img, float saturation)
    {
        //var UIFX = img.GetComponent<CustomUIFX>();
        //if (UIFX == null)
        //{
        //    Debug.LogError($"image [{img}] has no CustomUIFX", img);
        //    return;
        //}
        //UIFX.SetSaturation(saturation);
    }


    public override void OnRelease()
    {
        if (m_AtlasDic != null)
            m_AtlasDic.Clear();
    }
}
