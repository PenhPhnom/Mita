using System;
using UnityEngine;

public class GUIMessageTips : BaseUI
{
    private GUIMessageTipsData m_UIData = new GUIMessageTipsData();
    private int m_Time = -1;
    protected override void OnAwake(GameObject obj)
    {
        if (obj != null)
            m_UIData.InitUI(obj);
        EventMgr.Instance.RegisterEvent(EEventType.MESSAGETIPS_CONTENT, ShowMessageTips);
    }
    public override EnumUIType GetUIType()
    {
        return EnumUIType.MESSAGETIPS;
    }

    public override bool HasOpenTween()
    {
        return true;
    }
    public override bool HasCloseTween()
    {
        return false;
    }

    public void OnMessageTimeCallBack(object param)
    {
        GlobalFunction.SetGameObjectVisibleState(m_UIData.ImgBottom, false);
    }

    protected override void OnSetUIparam(params object[] uiParams)
    {
        if (uiParams == null) return;
        ShowMessageTips(uiParams[0]);
    }

    /// <summary>
    /// 接收 Tips 事件
    /// </summary>
    /// <param name="param"></param>
    public void ShowMessageTips(object param)
    {
        if (param == null) return;
        MessageTipsParam tipsParam = param as MessageTipsParam;
        if (m_Time != 0)
        {
            TimeMgr.Instance.UnRegister(m_Time);
        }
        m_Time = TimeMgr.Instance.Register(1, 1.5f, OnMessageTimeCallBack);
        GlobalFunction.SetGameObjectVisibleState(m_UIData.ImgBottom, true);
        GlobalFunction.SetText(m_UIData.TxtInfo, tipsParam.ContentText);
        ClientLog.Instance.Log($" 我是MessageTips");
    }

    public override void OnRelease()
    {
        TimeMgr.Instance.UnRegister(m_Time);
        EventMgr.Instance.UnRegisterEvent(EEventType.MESSAGETIPS_CONTENT, ShowMessageTips);
    }
}
