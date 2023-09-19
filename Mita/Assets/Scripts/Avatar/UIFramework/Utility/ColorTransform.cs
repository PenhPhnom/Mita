using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class CPair
{
    public string tag;
    public Color col;
}

[CreateAssetMenu(menuName = "AssetTools/ColorTransform")]
public class ColorTransform : ScriptableObject
{
    public List<CPair> ColorList;

    public Color GetColorByTag(string tag)
    {
        Color retCol = new Color();
        bool isHave = false;
        foreach (var color in ColorList)
        {
            if (0 == string.Compare(color.tag, tag))
            {
                retCol = color.col;
                isHave = true;
            }
        }

        if (!isHave)
        {
            ClientLog.Instance.LogError($"{tag}不存在");
        }
        return retCol;
    }

    public void SetTextColorByTag(string tag, Graphic text)
    {
        var needColor = new Color();
        bool state = false;  //是否存在
        foreach (var color in ColorList)
        {
            if (0 == string.Compare(color.tag, tag))
            {
                state = true;
                needColor = color.col;
            }
        }

        if (state)
        {
            text.color = needColor;
        }
        else
        {
            ClientLog.Instance.LogError($"{text.name}颜色设置失败 {tag} 不存在");
        }
    }
}