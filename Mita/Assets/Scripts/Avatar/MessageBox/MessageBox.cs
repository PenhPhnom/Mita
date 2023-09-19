using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 按钮类型
/// </summary>
public enum EMessageBoxStyle
{
    NONE,
    ONEBUTTON,
    TOWBUTTON,
    THREEBUTTON,
}

/// <summary>
/// 当MessageBox多于1个，为队列时，需要对队列做一些操作，比如响应第一个之后，按类型取消后面的
/// </summary>   
public enum EMessageBoxType
{
    NONE,
}

public class MessageBoxParam
{
    public MessageBoxElement Element;
    public bool BVisible;
    public bool BCheck;
}

/// <summary>
/// message的信息与参数等
/// </summary>
public class MessageBoxElement
{
    public int iMsgBoxId = 0;
    public MsgBoxCallback Callback = null;
    public MsgBoxCheckCallback CheckCallback = null;
    public MsgBoxInfoCallback InfoCallback = null;
    public object UserData = null;
    public string StrInfo = "";
    public bool BSpriteBox = false;
    public bool BTipBox = false;
    public bool BOnlyOkBtn = false;
    public float BFadeoutTime = 0f;
    public EMessageBoxType MsgBoxType = EMessageBoxType.NONE;
}

public delegate void MsgBoxCallback(bool bOK, object param);
public delegate void MsgBoxCheckCallback(bool bOK, bool bCheck, object param);
public delegate void MsgBoxInfoCallback(out string szInfo, int iMsgBoxId);

public class MessageBox : Singleton<MessageBox>
{
    private int m_IMsgBoxCountId = 0;
    private MessageBoxElement m_CurMessageBox = null;
    private List<MessageBoxElement> m_ListMessageBox = new List<MessageBoxElement>();

    public void Update()
    {
        if (null == m_CurMessageBox)
            return;

        // 每一帧都在更新内容
        if (null != m_CurMessageBox.InfoCallback)
        {
            m_CurMessageBox.InfoCallback(out m_CurMessageBox.StrInfo, m_CurMessageBox.iMsgBoxId);
            EventMgr.Instance.FireEvent(EEventType.MESSAGEBOX_TEXT, m_CurMessageBox.StrInfo);
        }

        if (m_CurMessageBox.BFadeoutTime > 0f)
        {
            m_CurMessageBox.BFadeoutTime -= Time.deltaTime;
            if (m_CurMessageBox.BFadeoutTime <= 0f)
            {
                OnCancel();
            }
        }
    }

    /// <summary>
    /// Message方法
    /// </summary>
    /// <param name="StrInfo">内容</param>
    /// <param name="Callback">回调</param>
    /// <param name="InfoCallback">内容 ID</param>
    /// <param name="CheckCallback"></param>
    /// <param name="bOkOnly">仅有OK</param>
    /// <param name="BTipBox"> 只是提示的BOX </param>
    /// <param name="fFadeOutTime"> 隐藏时间 </param>
    /// <param name="param"> 给UserData赋值 </param>
    /// <param name="Type"></param>
    /// <returns></returns>
    public int ShowMessageBox(string StrInfo = "", MsgBoxCallback Callback = null, MsgBoxInfoCallback InfoCallback = null,
      MsgBoxCheckCallback CheckCallback = null, bool bOkOnly = false, bool BTipBox = false, float fFadeOutTime = 0f, object param = null,
      EMessageBoxType Type = EMessageBoxType.NONE)
    {
        //判断此时是否允许弹窗
        if (!IsEnableMsgBox())
        {
            ClientLog.Instance.Log("现在不允许弹窗");
            return 0;
        }

        MessageBoxElement Info = new MessageBoxElement();
        Info.iMsgBoxId = MakeUniqueMessageBoxId();
        Info.StrInfo = StrInfo;
        Info.Callback = Callback;
        Info.InfoCallback = InfoCallback;
        Info.CheckCallback = CheckCallback;
        Info.BSpriteBox = false;
        Info.BFadeoutTime = fFadeOutTime;
        Info.BTipBox = BTipBox;
        Info.BOnlyOkBtn = bOkOnly;
        Info.UserData = param;
        Info.MsgBoxType = Type;

        if (null == m_CurMessageBox)
        {
            m_CurMessageBox = Info;
            //
            ShowMessageBoxUI(true);
        }
        else
            m_ListMessageBox.Add(Info);

        return Info.iMsgBoxId;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bVisible"></param>
    void ShowMessageBoxUI(bool bVisible)
    {
        if (bVisible)
        {
            MessageBoxParam param = new MessageBoxParam();
            param.BCheck = m_CurMessageBox != null && m_CurMessageBox.CheckCallback != null;
            param.BVisible = bVisible;
            param.Element = m_CurMessageBox;
            //EventMgr.Instance.FireEvent(EEventType.MESSAGEBOX_CONTENT, param);
            UIManager.Instance.OpenUI(EnumUIType.MESSAGEBOX, EPanelLayerType.Pop, null, param);
        }
        else
            UIManager.Instance.CloseUI(EnumUIType.MESSAGEBOX);
    }

    /// <summary>
    /// 
    /// </summary>
    private void ShowNextMessageBox()
    {
        if (m_ListMessageBox.Count > 0)
        {
            m_CurMessageBox = m_ListMessageBox[0];
            m_ListMessageBox.RemoveAt(0);
            ShowMessageBoxUI(true);
        }
        else
        {
            m_CurMessageBox = null;
            ShowMessageBoxUI(false);
        }
    }

    /// <summary>
    /// OK
    /// </summary>
    public void OnOK()
    {
        if (null != m_CurMessageBox)
        {
            m_CurMessageBox.CheckCallback?.Invoke(true, false, m_CurMessageBox.UserData);
            m_CurMessageBox.Callback?.Invoke(true, m_CurMessageBox.UserData);
        }
        ShowNextMessageBox();
    }

    /// <summary>
    /// 取消
    /// </summary>
    public void OnHide()
    {
        if (null == m_CurMessageBox || !m_CurMessageBox.BTipBox)
            return;

        ShowNextMessageBox();
    }

    /// <summary>
    /// 取消所有
    /// </summary>
    public void OnForceHideAll(bool cancelAll)
    {
        if (null == m_CurMessageBox)
            return;

        ShowMessageBoxUI(false);
        if (cancelAll)
        {
            for (int i = m_ListMessageBox.Count - 1; i >= 0; --i)
            {
                if (m_ListMessageBox[i].BTipBox) continue;
                m_ListMessageBox.RemoveAt(i);
            }

            m_CurMessageBox = null;
        }
    }

    /// <summary>
    /// 取消
    /// </summary>
    public void OnCancel()
    {
        if (null != m_CurMessageBox)
        {
            m_CurMessageBox.CheckCallback?.Invoke(false, false, m_CurMessageBox.UserData);
            m_CurMessageBox.Callback?.Invoke(false, m_CurMessageBox.UserData);
        }
        ShowNextMessageBox();
    }

    /// <summary>
    /// 移除
    /// </summary>
    /// <param name="Type"></param>
    /// <param name="bDoCancelCallback"></param>
    public void RemoveMessageBoxByType(EMessageBoxType Type, bool bDoCancelCallback)
    {
        if (m_ListMessageBox.Count > 0)
        {
            for (int iLoop = m_ListMessageBox.Count - 1; iLoop >= 0; --iLoop)
            {
                MessageBoxElement BoxElement = m_ListMessageBox[iLoop];
                if (BoxElement.MsgBoxType == Type)
                {
                    if (bDoCancelCallback)
                    {
                        if (null != BoxElement.CheckCallback)
                        {
                            BoxElement.CheckCallback(false, false, BoxElement.UserData);
                        }

                        if (null != BoxElement.Callback)
                        {
                            BoxElement.Callback(false, BoxElement.UserData);
                        }
                    }

                    //
                    m_ListMessageBox.RemoveAt(iLoop);
                }
            }
        }
    }

    /// <summary>
    /// 判断是否允许弹窗 某些条件是不允许弹窗的
    /// </summary>
    private bool IsEnableMsgBox()
    {
        //条件或者有变量修改
        return true;
    }

    private int MakeUniqueMessageBoxId()
    {
        ++m_IMsgBoxCountId;
        return m_IMsgBoxCountId;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="iMsgBoxId"></param>
    public void CloseMessageBox_NoCallback(int iMsgBoxId)
    {
        if (null != m_CurMessageBox && m_CurMessageBox.iMsgBoxId == iMsgBoxId)
            ShowNextMessageBox();
        else
        {
            foreach (MessageBoxElement Info in m_ListMessageBox)
            {
                if (Info.iMsgBoxId == iMsgBoxId)
                {
                    m_ListMessageBox.Remove(Info);
                    break;
                }
            }
        }
    }

    public override void OnRelease()
    {
        //ShowMessageBoxUI(false);
        m_IMsgBoxCountId = 0;
        m_CurMessageBox = null;
        m_ListMessageBox = new List<MessageBoxElement>();
    }
}