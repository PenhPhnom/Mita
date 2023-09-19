using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MessageTipsParam
{
    public string ContentText;
    public float DelayTime;
    public Action OnFinishCallBack;
}

public class MessageTips : Singleton<MessageTips>
{
    public void ShowMessageTips(string contentText, Action onFinish = null)
    {
        MessageTipsParam param = new MessageTipsParam();
        param.ContentText = contentText;
        param.OnFinishCallBack = onFinish;
        EventMgr.Instance.FireEvent(EEventType.MESSAGETIPS_CONTENT, param);
        UIManager.Instance.OpenUI(EnumUIType.MESSAGETIPS, EPanelLayerType.Top, null, param);
    }

    public override void OnRelease()
    {

    }
}