using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIMessageBox : BaseUI
{
    GUIMessageBoxData m_UIData = new GUIMessageBoxData();
    public GUIMessageBox()
    {

    }
    public override EnumUIType GetUIType()
    {
        return EnumUIType.MESSAGEBOX;
    }

    public override void OnRelease()
    {
        EventMgr.Instance.UnRegisterEvent(EEventType.MESSAGEBOX_CONTENT, OnShowMessageBox);
    }

    protected override void OnAwake(GameObject obj)
    {
        if (obj == null) return;
        m_UIData.InitUI(obj);
        //ShowMessageBoxUI(false);
        //EventMgr.Instance.RegisterEvent(EEventType.MESSAGEBOX_CONTENT, OnShowMessageBox);
        GlobalFunction.AddEnevntTrigger(m_UIData.BtnOk.gameObject, EnumTouchEventType.OnClick, OnClickBtnOK);
        GlobalFunction.AddEnevntTrigger(m_UIData.BtnCacel.gameObject, EnumTouchEventType.OnClick, OnClickBtnCancel);
    }

    private void ShowMessageBoxUI(bool bVisble)
    {
        GlobalFunction.SetGameObjectVisibleState(UI_Object, bVisble);
    }

    protected override void OnSetUIparam(params object[] param)
    {
        OnShowMessageBox(param[0]);
    }

    private void OnShowMessageBox(object param)
    {
        MessageBoxParam boxparam = (MessageBoxParam)param;
        ShowMessageBoxUI(boxparam.BVisible, boxparam.Element);
    }

    private void ShowMessageBoxUI(bool bVisible, MessageBoxElement element)
    {
        if (!bVisible)
        {
            ShowMessageBoxUI(false);
            return;
        }

        ShowMessageBoxUI(true);
        //GlobalFunction.SetGameObjectVisibleState(m_uiData.OKBtn, element.BTipBox);
        //GlobalFunction.SetGameObjectVisibleState(m_uiData.cancelBtn, element.BTipBox);
        string content = element.StrInfo;

        ClientLog.Instance.Log(content);
    }

    private void OnClickBtnOK(GameObject _listener, object _args, params object[] _params)
    {
        MessageBox.Instance.OnOK();
    }

    private void OnClickBtnCancel(GameObject _listener, object _args, params object[] _params)
    {
        MessageBox.Instance.OnCancel();
    }
}
