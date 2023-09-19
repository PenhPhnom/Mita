using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerListenCondition : GuideEventListenConditionBase
{
    public override EEventType GetEventType()
    {
        return EEventType.SYSTEM_OPTION;
    }

    /// <summary>
    /// 检测条件是否满足
    /// </summary>
    /// <returns></returns>
    public override bool InternalMeet()
    {
        return false;
    }

    /// <summary>
    /// 检测条件
    /// </summary>
    public override void Monitor(object param)
    {

    }

    public override void Dispose()
    {

    }
}
