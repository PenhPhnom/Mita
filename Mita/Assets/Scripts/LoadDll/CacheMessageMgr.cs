using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CacheMessageMgr
{
    Queue m_Queue = new Queue();
    private static CacheMessageMgr _Instance = null;

    public static CacheMessageMgr Instance
    {
        get
        {
            if (null == _Instance)
            {
                _Instance = new CacheMessageMgr();
            }
            return _Instance;
        }
    }

    public void SetMessageQueue(string msg)
    {
        m_Queue.Enqueue(msg);
    }

    /// <summary>
    /// »ñµÃ
    /// </summary>
    /// <returns></returns>
    public Queue GetMessageQueue()
    {
        return m_Queue;
    }

    public void SetMessageQueueClear()
    {
        if (m_Queue != null) m_Queue.Clear();
    }

}
