using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TimeMgr : Singleton<TimeMgr>
{
    private long m_lCurrTime = 0;

    public long CurrTime
    {
        get
        {
            return m_lCurrTime;
        }
        set
        {
            m_lCurrTime = value;
        }
    }

    private static int ID = 0;
    private List<TimeTickData> mTickDataList = new List<TimeTickData>();
    private TimeTickData m_TimeTickData = null;
    public void Init()
    {
        MainLoopScript.AddUpdateHandler(EUpdatePriority.Realtime, Update);
    }

    public void Update(float _delta)
    {
        for (int i = 0; i < mTickDataList.Count; i++)
        {
            m_TimeTickData = mTickDataList[i];
            m_TimeTickData.mTime += _delta;
            if (m_TimeTickData.mTime > m_TimeTickData.mPerTime)
            {
                // Remove
                if (m_TimeTickData.mTickNum <= 1 && mTickDataList.Count > i)
                {
                    mTickDataList.RemoveAt(i);
                    i--;
                }

                m_TimeTickData.mTime -= m_TimeTickData.mPerTime;
                m_TimeTickData.mTickNum--;
                m_TimeTickData.mCallHandler(m_TimeTickData.mTickNum);
            }
        }
    }
    /// <summary>
    /// 注册 (tickNum：总共次数、perTime：多少秒一次 _immediatelyCallKey">是否立即回调一次（默认不回调））
    /// </summary>
    /// <param name="</param>
    public int Register(long _tickNum, float _perTime, HandleEvent _callHandler, bool _immediatelyCallKey = false)
    {
        if (_tickNum <= 0)
            _tickNum = 1;

        ID++;
        TimeTickData tTimeTickData = new TimeTickData();
        tTimeTickData.mTickNum = _tickNum;
        tTimeTickData.mPerTime = _perTime;
        tTimeTickData.mCallHandler += _callHandler;
        tTimeTickData.mTime = 0f;
        tTimeTickData.mID = ID;
        if (_immediatelyCallKey)
        {
            tTimeTickData.mCallHandler(_tickNum);
        }
        mTickDataList.Add(tTimeTickData);
        return ID;
    }

    public void UnRegister(int _id)
    {
        if (_id <= 0)
            return;

        for (int i = 0; i < mTickDataList.Count; i++)
        {
            if (mTickDataList[i].mID == _id)
            {
                mTickDataList.RemoveAt(i);
                if (mTickDataList.Count == 0)
                {
                    mTickDataList.Clear();
                }
                return;
            }
        }
    }

    public override void OnRelease()
    {

    }
}

public class TimeTickData
{
    public HandleEvent mCallHandler;
    public long mTickNum = 0;
    public float mTime = 0.0f;
    public float mPerTime = 0.0f;
    public float mID = 0;
}
