using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 以后补充说明 每一个字段的意思
/// </summary>
public enum EEventType
{
    MESSAGETEXT, // 消息文本
    LOADINGPROCESS, // 加载过程
    SYSTEM_OPTION, // 系统黑白

    MESSAGEBOX_TEXT,// 里面的内容
    MESSAGEBOX_CONTENT,// MessageBox 所有要显示的

    MESSAGETIPS_CONTENT,// MessageTips 所有要显示的

    UI_STATECHANGED, // UI状态
    UI_ADD_MASK, //添加触摸遮罩
    UI_REMOVE_MASK, //移除触摸遮罩

    GUIDE_RUNNING_NOW,      // 新手引导运行中 每帧都发
    GUIDE_CLICK_ON_INPUT_MASK, //点击引导的input mask
    GUIDE_CLICK_ON_INPUT,     // 点击引导界面的btn_input

    CONFORMITY_CONTROL_GUIDE,// 新手引导整合
    CONFORMITY_GUIDE_FINFISH,//新手引导完成

    GUIDE_UI_ONSTEPSTART,

    UI_SHOW_PANEL,   // 显示窗口
    UI_HIDE_PANEL,   // 关闭窗口
    UI_BTN_CLICK,    // 按钮点击

    Max,
}

public delegate void HandleEvent(object objParam);

public class EventItem
{
    public void Add(HandleEvent handle)
    {
        m_Handle += handle;
    }

    public void Remove(HandleEvent handle)
    {
        m_Handle -= handle;
    }

    public void Handle(object objParam)
    {
        if (null == m_Handle)
        {
            return;
        }

        m_Handle(objParam);
    }

    private HandleEvent m_Handle;
}


public class EventPair
{
    private EEventType m_eType;
    private object m_objParam;
    private HandleEvent m_handle;

    public EventPair(EEventType eType, object obj)
    {
        m_eType = eType;
        m_objParam = obj;
    }

    public EventPair(HandleEvent handle, object obj)
    {
        m_handle = handle;
        m_objParam = obj;
    }

    public HandleEvent Handle
    {
        get
        {
            return m_handle;
        }
    }

    public EEventType Type
    {
        get
        {
            return m_eType;
        }
    }

    public object Param
    {
        get
        {
            return m_objParam;
        }
    }
}

public class TimeEventPair
{
    public object m_Param;
    public HandleEvent m_Handle;
    public int m_iId;
    public float m_TimeMS;

    public TimeEventPair(HandleEvent handle, object obj, int time, int iId)
    {
        m_Handle = handle;
        m_Param = obj;
        m_iId = iId;
        m_TimeMS = time;
    }
}

/// <summary>
/// 用于FireEvent多个参数时，公用类 
/// </summary>
public class CommonEventParam
{
    public object objParam1;
    public object objParam2;
    public object objParam3;
    public object objParam4;
}

public class EventMgr
{
    static private EventMgr m_Insance = null;
    static public EventMgr Instance
    {
        get
        {
            if (null == m_Insance)
                m_Insance = new EventMgr();
            //
            return m_Insance;
        }
    }

    public EventMgr()
    {
        for (int i = 0; i < (int)EEventType.Max; i++)
        {
            m_EventItemArray[i] = new EventItem();
        }
    }

    private EventItem[] m_EventItemArray = new EventItem[(int)EEventType.Max];
    private List<EventPair> m_lstEvent = new List<EventPair>();
    private List<TimeEventPair> m_listTimeEvent = new List<TimeEventPair>();
    private int m_iTimeEventId = 0;

    public void RegisterEvent(EEventType eType, HandleEvent handle)
    {
        if (null == m_EventItemArray)
        {
            return;
        }

        EventItem item = GetEventItem(eType);

        if (null == item)
        {
            item = new EventItem();
        }

        item.Add(handle);
    }

    public void UnRegisterEvent(EEventType eType, HandleEvent handle)
    {
        if (null == m_EventItemArray)
        {
            return;
        }

        EventItem item = GetEventItem(eType);

        if (null == item)
        {
            return;
        }

        item.Remove(handle);
    }

    public void FireEvent(EEventType eType, object obj = null)
    {
        EventItem item = GetEventItem(eType);

        if (null == item)
        {
            return;
        }

        item.Handle(obj);
    }

    public void PushEvent(EEventType eType, object obj)
    {
        if (null == m_lstEvent)
        {
            return;
        }

        m_lstEvent.Add(new EventPair(eType, obj));
    }

    public void PushEvent(HandleEvent handle, object obj)
    {
        if (null == m_lstEvent)
        {
            return;
        }

        m_lstEvent.Add(new EventPair(handle, obj));
    }

    /// <summary>
    /// 添加Time回调事件，ClearSameEvent : 为true表示同时只能有一个Event，Push新的时候把之前还没回调的删除 
    /// </summary>
    public int PushTimeEvent(HandleEvent handle, object obj, int timeMS, bool ClearSameEvent)
    {
        if (null == handle)
            return 0;

        if (ClearSameEvent)
        {
            PopTimeEvent(handle);
        }

        ++m_iTimeEventId;
        m_listTimeEvent.Add(new TimeEventPair(handle, obj, timeMS, m_iTimeEventId));
        return m_iTimeEventId;
    }

    /// <summary>
    /// 按回调删除Event，删除列表中的所有对应的Handle，不执行回调 
    /// </summary>
    public void PopTimeEvent(HandleEvent handle)
    {
        if (null == handle)
            return;

        for (int iLoop = m_listTimeEvent.Count - 1; iLoop >= 0; --iLoop)
        {
            TimeEventPair pEvent = m_listTimeEvent[iLoop];
            if (pEvent.m_Handle == handle)
                m_listTimeEvent.RemoveAt(iLoop);
        }
    }

    /// <summary>
    /// 按Id删除Event，不执行回调 
    /// </summary>
    public void PopTimeEvent(int iId)
    {
        for (int iLoop = 0, iCount = m_listTimeEvent.Count; iLoop < iCount; ++iLoop)
        {
            TimeEventPair pEvent = m_listTimeEvent[iLoop];
            if (pEvent.m_iId == iId)
            {
                m_listTimeEvent.RemoveAt(iLoop);
                return;
            }
        }
    }

    private EventItem GetEventItem(EEventType eType)
    {
        if (null == m_EventItemArray)
        {
            return null;
        }

        return m_EventItemArray[(int)eType];
    }

    /// <summary>
    /// 
    /// </summary>
    public void Update()
    {
        if (null != m_lstEvent)
        {
            for (int i = 0; i < m_lstEvent.Count; i++)
            {
                EventPair child = m_lstEvent[i];
                if (null == child)
                {
                    continue;
                }

                if (null == child.Handle)
                {
                    FireEvent(child.Type, child.Param);
                }
                else
                {
                    child.Handle(child.Param);
                }
            }
            m_lstEvent.Clear();
        }

        //
        if (m_listTimeEvent.Count > 0)
        {
            int iDeltaTime = (int)(Time.deltaTime * 1000f);
            for (int iLoop = 0; iLoop < m_listTimeEvent.Count;)
            {
                TimeEventPair pEvent = m_listTimeEvent[iLoop];
                pEvent.m_TimeMS -= iDeltaTime;
                if (pEvent.m_TimeMS <= 0)
                {
                    if (null != pEvent.m_Handle)
                        pEvent.m_Handle(pEvent.m_Param);

                    m_listTimeEvent.RemoveAt(iLoop);
                    continue;
                }

                ++iLoop;
            }
        }
    }
}
