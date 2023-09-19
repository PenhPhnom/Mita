using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClickCondition : GuideConditionBase
{
    // 现在是测试的 Param 都是 数字
    private bool m_IsFullfilled;
    public override bool Check(string param, string param2)
    {
        m_IsFullfilled = param == "1";
        return m_IsFullfilled;
    }

    public override bool Meet()
    {
        return m_IsFullfilled;
    }

    public override void SetUp()
    {
        //var panel = UIManager.Instance.GetUIObject((EnumUIType)(int.Parse(Param)));
        //var name = Param2
        //1.找到相应的Panel 找到相应的按钮 是否走了点击

        GameObject objBtn = GuideTargetAcquireUtil.GetFixUIObject(Param, Param2);
        GlobalFunction.AddEnevntTrigger(objBtn, EnumTouchEventType.OnClick, OnClickGameObject);
    }

    public void OnClickGameObject(GameObject _listener, object _args, params object[] _params)
    {
        m_IsFullfilled = true;
    }

    public override void OnRelease()
    {
        m_IsFullfilled = false;
    }
}
