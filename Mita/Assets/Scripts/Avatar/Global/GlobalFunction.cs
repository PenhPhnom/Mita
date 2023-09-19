using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using UnityEngine;
using UnityEngine.UI;

public class GlobalFunction
{
    /// <summary>
    /// 加载Atlas Image
    /// </summary>
    /// <param name="img"></param>
    /// <param name="atlasName">图集名字</param>
    /// <param name="spriteName">spriteName</param>
    /// <param name="isSetNative">serNative</param>
    /// <param name="callBack">回调</param>
    /// <param name="alpha">alpha</param>
    public static void SetImage(Image img, string atlasName, string spriteName, bool isSetNative = false, Action callBack = null, float alpha = 1, string abName = null, string abFolderPath = null)
    {
        UISpriteMgr.Instance.SetImage(img, atlasName, spriteName, isSetNative, callBack, alpha, abName, abFolderPath);
    }

    /// <summary>
    /// 加载 RawImage
    /// </summary>
    /// <param name="raw">RawImage</param>
    /// <param name="name">texture的名字</param>
    /// <param name="abName">所属于的AB包</param>
    /// <param name="isSetNative">serNative</param>
    /// <param name="isLoadingAlpha">加载的时候alpha默认0</param>
    /// <param name="callback"></param>
    /// <param name="abFolderPath">AB包的路径</param>
    public static void SetRawImage(RawImage raw, string name, string abName, bool isSetNative = false, bool isLoadingAlpha = true, Action callback = null, string abFolderPath = null)
    {
        UISpriteMgr.Instance.SetRawImage(raw, name, abName, isSetNative, isLoadingAlpha, callback, abFolderPath);
    }

    public static void SetText(Text txt, string content, params object[] args)
    {
        if (txt != null)
        {
            string text = content.Contains("tid#") ? DataLoader.Instance.GetTranslateByID(content) : content;
            for (int i = 0; i < args.Length; i++)
            {
                text = text.Replace($"$[num{i}]", args[i].ToString()); ;
            }

            txt.text = text;
        }
    }

    public static void SetSlider(Slider slider, float progress)
    {
        if (slider != null)
        {
            slider.value = progress;
        }
    }

    /// <summary>
    /// TouchEvent 事件(点击 双击 拖拽)
    /// </summary>
    public static void AddEnevntTrigger(GameObject obj, EnumTouchEventType _type, OnTouchEventHandle _handle, params object[] _params)
    {
        if (obj != null)
        {
            UIEventListener RegisterListener = UIEventListener.Get(obj);
            RegisterListener.SetEventHandle(_type, _handle, _params);
        }
        else
            ClientLog.Instance.LogError("AddEnevntTrigger error： obj is null");
    }

    /// <summary>
    /// 按钮的点击
    /// </summary>
    /// <param name="btn"></param>
    /// <param name="call"></param>
    public static void AddOnClick(Button btn, UnityEngine.Events.UnityAction call)
    {
        if (btn != null)
        {
            btn.onClick.AddListener(call);
        }
        else
            ClientLog.Instance.LogError("AddOnClick error： btn is null");
    }

    /// <summary>
    /// 显示隐藏
    /// </summary>
    public static void SetGameObjectVisibleState(GameObject obj, bool visible = false)
    {
        if (obj != null)
        {
            obj.SetActive(visible);
        }
    }

    /// <summary>
    /// 显示隐藏
    /// </summary>
    static public void SetGameObjectVisibleState(Component go, bool visible)
    {
        if (go != null && go.gameObject != null)
            go.gameObject.SetActive(visible);
    }

    /// <summary>
    /// 根据URL获取到路径
    /// </summary>
    /// <param name="url">url</param>
    public static string UrlDecode(string url)
    {
        return Uri.UnescapeDataString(url);
    }

    /// <summary>
    /// 根据URL获取到路径
    /// </summary>
    public static string UrlDecodeGetPath(string url)
    {
        string str = UrlDecode(url);
        string path = str.Replace("file://", "");
        return path;
    }

    public static int ParseInt(string szVal, int iDefaultVal = 0)
    {
        int iVal = iDefaultVal;
        if (!string.IsNullOrEmpty(szVal))
            int.TryParse(szVal, out iVal);

        return iVal;
    }

    /// <summary>
    /// 根据 ColorTransform 配置的获取值
    /// </summary>
    /// <param name="tag">tag</param>
    /// <param name="isOutLine">描边</param>
    public static void SetColorByTag(Graphic uiObj, string tag, bool isOutLine = false)
    {
        UIColorMgr.Instance.SetColorByTag(uiObj, tag, isOutLine);
    }

    /// <summary>
    /// 按钮图片变色
    /// </summary>
    public static void SetBtnGray(Button btn, bool isGray)
    {
        if (btn != null)
        {
            SetBtnSaturation(btn, isGray ? 0 : 1);
        }
    }

    /// <summary>
    /// 按钮图片变色
    /// </summary>
    public static void SetBtnSaturation(Button btn, float saturation)
    {
        var img = btn.image;
        if (img != null)
        {
            UISpriteMgr.Instance.SetImageSaturation(img, saturation);
        }
    }



}