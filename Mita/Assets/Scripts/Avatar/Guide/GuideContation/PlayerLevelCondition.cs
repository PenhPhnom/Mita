using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevelCondition : GuideConstStatsConditionBase
{
    public override bool Check(string param, string param2)
    {
        //��ҵĵȼ�Level > ��int��param return true

        return true;
    }

    public override void Dispose()
    {

    }
}
