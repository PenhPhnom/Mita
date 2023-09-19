using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 获取颜色  //TODO  需要做性能验证 是否会增加内存
/// </summary>
public class UIColorMgr : Singleton<UIColorMgr>
{
    private Dictionary<string, Color> m_ColorDic = new Dictionary<string, Color>();

    /// <summary>
    /// 根据 Tag 设置 Color
    /// </summary>
    public void SetColorByTag(Graphic uiObj, string tag, bool isOutLine = false)
    {
        if (string.IsNullOrEmpty(tag))
        {
            ClientLog.Instance.LogError($"无效的color tag,Graphic:[{uiObj}]");
            return;
        }

        if (m_ColorDic.Count == 0)
        {
            ParamData pParamData = new ParamData();
            pParamData.objectParam = uiObj;
            pParamData.sParam = tag;
            pParamData.bParam = isOutLine;
            ResourceMgr.Instance.LoadResourceAsync("ColorTransform", TypeInts.ColorTransform, (obj, param) =>
            {
                //ColorTransform colorTransform = ScriptableObject.Instantiate(obj) as ColorTransform;
                ColorTransform colorTransform = obj as ColorTransform;
                foreach (var cpair in colorTransform.ColorList)
                {
                    if (!m_ColorDic.ContainsKey(cpair.tag))
                        m_ColorDic.Add(cpair.tag, cpair.col);
                }

                //ResourceMgr.Instance.UnLoadResource(colorTransform, TypeInts.ColorTransform);
                //colorTransform = null;
                ApplyColorToGraphic((Graphic)param.objectParam, param.sParam, param.bParam);
            }, null, null, pParamData);
        }
        else
        {
            ApplyColorToGraphic(uiObj, tag, isOutLine);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="uiObj"></param>
    /// <param name="tag"></param>
    private void ApplyColorToGraphic(Graphic uiObj, string tag, bool isOutLine)
    {
        if (uiObj != null)
        {
            if (m_ColorDic.ContainsKey(tag))
                uiObj.color = m_ColorDic[tag];

            if (isOutLine)
            {
                var outline = uiObj.transform.GetComponent<Outline>();
                if (outline != null && m_ColorDic.ContainsKey(tag))
                    outline.effectColor = m_ColorDic[tag];
            }
        }
    }


    public override void OnRelease()
    {

    }

}
