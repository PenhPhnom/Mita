//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;


///// <summary>
///// 一些工具方法
///// 有一些方法会被移动到更通用的区域
///// </summary>
//public class Utils
//{
//    private static Dictionary<string, Color> colorDict = new Dictionary<string, Color>();
//    private static string colorTranPath = "UI/Utils/ColorTransform.asset";
//    static void ApplyColorToGraphic(UnityEngine.UI.Graphic uiObj, string tag)

//    {
//        if (uiObj != null)
//        {
//            Color color;
//            if (colorDict.TryGetValue(tag, out color))
//            {
//                uiObj.color = color;
//            }
//            else
//            {
//                Debug.LogError($"颜色设置失败 color tag:[{tag}] Graphic:[{uiObj}]", uiObj);
//            }
//        }
//    }
//    //设置TextColor
//    public static void SetTextColorByTag(UnityEngine.UI.Graphic uiObj, string tag)
//    {
//        if (string.IsNullOrEmpty(tag))
//        {
//            //用UnityDebug支持点击跳转到GameObject，用Exception触发Lua的报错方便定位lua代码
//            Debug.LogError($"无效的color tag,Graphic:[{uiObj}]", uiObj);
//            throw new System.ArgumentException("$无效的color tag,Graphic:[{uiObj}]");
//        }
//        void finish(OperationHandle colTran)
//        {
//            ColorTransform colorTransform = ScriptableObject.Instantiate((Object)colTran.Result) as ColorTransform;
//            for (int i = 0; i < colorTransform.ColorList.Count; i++)
//            {
//                CPair cpair = colorTransform.ColorList[i];
//                if (!colorDict.ContainsKey(cpair.tag))
//                {
//                    colorDict.Add(cpair.tag, cpair.col);
//                }
//            }
//            Object.Destroy(colorTransform);
//            colorTransform = null;
//            ApplyColorToGraphic(uiObj, tag);
//        }

//        if (colorDict.Count == 0)
//        {
//            ResourceManager.Instance.LoadAssetAsync(ResourcesUtil.TypeInts.Object, colorTranPath, finish);
//        }
//        else
//        {
//            ApplyColorToGraphic(uiObj, tag);
//        }
//    }
//    public static void SetOutlineColorByTag(UnityEngine.UI.Graphic uiObj, string tag)
//    {
//        void finish(OperationHandle colTran)
//        {
//            ColorTransform colorTransform = ScriptableObject.Instantiate((Object)colTran.Result) as ColorTransform;
//            for (int i = 0; i < colorTransform.ColorList.Count; i++)
//            {
//                CPair cpair = colorTransform.ColorList[i];
//                if (!colorDict.ContainsKey(cpair.tag))
//                {
//                    colorDict.Add(cpair.tag, cpair.col);
//                }
//            }
//            Object.Destroy(colorTransform);
//            colorTransform = null;
//            if (uiObj != null)
//            {
//                var outline = uiObj.transform.GetComponent<Outline>();
//                if (outline != null)
//                {
//                    outline.effectColor = colorDict[tag];
//                }
//            }
//        }

//        if (colorDict.Count == 0)
//        {
//            ResourceManager.Instance.LoadAssetAsync(ResourcesUtil.TypeInts.Object, colorTranPath, finish);
//        }
//        else
//        {
//            if (uiObj != null)
//            {
//                var outline = uiObj.transform.GetComponent<Outline>();
//                if (outline != null)
//                {
//                    outline.effectColor = colorDict[tag];
//                }
//            }
//        }
//    }

//    public static void SetOutlineEnable(UnityEngine.UI.Graphic uiObj, bool isEnable)
//    {
//        if (uiObj != null)
//        {
//            var outline = uiObj.transform.GetComponent<Outline>();
//            if (outline != null)
//            {
//                outline.enabled = isEnable;
//            }
//        }
//    }

//    public static void SetShadowEnable(UnityEngine.UI.Graphic uiObj, bool isEnable)
//    {
//        if (uiObj != null)
//        {
//            var shadow = uiObj.transform.GetComponent<Shadow>();
//            if (shadow != null)
//            {
//                shadow.enabled = isEnable;
//            }
//        }
//    }

//    // float ObjectHeightGetter(RectTransform rectTran)
//    // {
//    //     return rectTran.sizeDelta.y;
//    // }
//    // void ObjectHeightSetter(RectTransform rectTran, float height)
//    // {
//    //     rectTran.sizeDelta = new Vector2(rectTran.sizeDelta.x, height);
//    // }

//    // public void SetObjectDoTweenHeight(RectTransform rectTran, float height, float time)
//    // {
//    //             void ObjectHeightSetter(RectTransform rectTran, float height)
//    // {
//    //     rectTran.sizeDelta = new Vector2(rectTran.sizeDelta.x, height);
//    // }

//    //     // DoTween
//    //     DOTween.To(ObjectHeightGetter(rectTran), ObjectHeightSetter(rectTran), height, time).Play();

//    // }

//    public static bool HasColorByTag(string tag)
//    {
//        if (colorDict.Count == 0)
//        {
//            OperationHandle colTran = ResourceManager.Instance.LoadAsset(ResourcesUtil.TypeInts.Object, colorTranPath);
//            ColorTransform colorTransform = ScriptableObject.Instantiate((Object)colTran.Result) as ColorTransform;
//            for (int i = 0; i < colorTransform.ColorList.Count; i++)
//            {
//                CPair cpair = colorTransform.ColorList[i];
//                if (!colorDict.ContainsKey(cpair.tag))
//                {
//                    colorDict.Add(cpair.tag, cpair.col);
//                }
//            }
//            Object.DestroyImmediate(colorTransform);
//        }

//        return colorDict.ContainsKey(tag);
//    }
//    public static bool IsUIGraphicExist(Graphic graphic)
//    {
//        return graphic != null;
//    }
//    public static bool IsRawImageTextureExist(RawImage rawImage)
//    {
//        return rawImage.texture != null;
//    }
//    public static bool IsImageSpriteExist(Image image)
//    {
//        return image.sprite != null;
//    }

//    public static bool IsSpriteExist(Sprite sprite)
//    {
//        return sprite != null;
//    }

//    public static void SetGraphicColor(Graphic graphic, Color color)
//    {
//        if (graphic == null)
//            return;
//        graphic.color = color;
//    }
//    public static void SetGraphicColorRaw(Graphic graphic, float r, float g, float b, float a)
//    {
//        if (graphic == null)
//            return;
//        graphic.color = new Color(r, g, b, a);
//    }
//    public static void SetGraphicAlpha(Graphic graphic, float alpha)
//    {
//        var color = graphic.color;
//        color.a = alpha;
//        graphic.color = color;
//    }

//    public static void OpenURL(string url)
//    {
//        Application.OpenURL(url);
//    }

//    // 按钮图片变色
//    public static void SetRaycastTarget(UnityEngine.GameObject go, bool isRaycastTarget)
//    {
//        var graphic = go.GetComponent<UnityEngine.UI.Graphic>();
//        if (graphic == null)
//            return;
//        graphic.raycastTarget = isRaycastTarget;
//    }

//    public static Color SetColorTryParseHtmlString(string colorStr)
//    {
//        Color nowColor;
//        ColorUtility.TryParseHtmlString(colorStr, out nowColor);
//        return nowColor;
//    }

//    public static void SetImageColorTryParseHtmlString(Image image, string colorStr)
//    {
//        Color nowColor;
//        ColorUtility.TryParseHtmlString(colorStr, out nowColor);
//        image.color = nowColor;
//    }
//}
