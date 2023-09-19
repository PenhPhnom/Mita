using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GuideEventListenConditionBase : GuideConditionBase
{
    public bool m_IsFullfilled;
    public GuideEventListenConditionBase()
    {
        EventMgr.Instance.RegisterEvent(GetEventType(), Monitor);
    }

    public abstract void Monitor(object param);
    public abstract EEventType GetEventType();
    public abstract bool InternalMeet();
    public virtual void InternalSetup() { }
    public override void SetUp()
    {
        InternalSetup();
        m_IsFullfilled = InternalMeet();
    }
    public override bool Meet() { return m_IsFullfilled; }
    public void SetFullfilled()
    {
        m_IsFullfilled = true;
    }

    public override bool Check(string param, string param2)
    {
        return false;
    }

    public abstract void Dispose();

    public override void OnRelease()
    {
        EventMgr.Instance.UnRegisterEvent(GetEventType(), Monitor);
        Dispose();
    }
}
