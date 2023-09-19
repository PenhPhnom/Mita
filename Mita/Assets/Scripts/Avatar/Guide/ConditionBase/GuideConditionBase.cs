using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GuideConditionBase
{
    public string Param;
    public string Param2;

    public void Init(string param, string param2)
    {
        Param = param;
        Param2 = param2;
        SetUp();
    }
    public abstract void SetUp();
    //检测
    public abstract bool Check(string param, string param2);
    /// <summary>
    /// 检测条件是否满足
    /// </summary>
    public abstract bool Meet();
    public virtual bool Valid() { return true; }
    public abstract void OnRelease();
}
