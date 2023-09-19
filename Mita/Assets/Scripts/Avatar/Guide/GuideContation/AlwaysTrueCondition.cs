using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysTrueCondition : GuideConstStatsConditionBase
{
    public override bool Check(string param, string param2)
    {
        return true;
    }

    public override void Dispose()
    {

    }
}
