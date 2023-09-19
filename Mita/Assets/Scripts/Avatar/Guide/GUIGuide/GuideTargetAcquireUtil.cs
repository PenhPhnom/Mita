using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 新手引导 获取目标物的封装脚本
/// </summary>
public class GuideTargetAcquireUtil
{
    /// <summary>
    /// 获得到UI控件
    /// </summary>
    public static GameObject GetFixUIObject(string param1, string param2)
    {
        var panel = UIManager.Instance.GetUIObject((EnumUIType)(int.Parse(param1)));
        if (panel != null)
        {
            return panel.GetComponentByString(param2);
        }


        return null;
    }


}
