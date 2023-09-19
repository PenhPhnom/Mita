using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GuideConstStatsConditionBase : GuideConditionBase
{
    public override bool Meet()
    {
        return Check(Param, Param2);
    }
    public abstract void Dispose();

    public override void OnRelease()
    {
        Dispose();
    }

    public override void SetUp()
    {

    }
}
