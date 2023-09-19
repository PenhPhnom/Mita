using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideConditionFactory
{
    public static GuideConditionBase CreateConditon(EGuideConditionType type, string param = null, string param2 = null)
    {
        GuideConditionBase conditionBase = null;
        if (type == EGuideConditionType.None)
        {
            conditionBase = new AlwaysTrueCondition();
        }
        else if (type == EGuideConditionType.TestTrue)
        {
            conditionBase = new AlwaysTrueCondition();
        }
        else if (type == EGuideConditionType.Level)
        {
            conditionBase = new PlayerLevelCondition();
        }
        else if (type == EGuideConditionType.OpenUI)
        {
            conditionBase = new PanelShowCondition();
        }
        else if (type == EGuideConditionType.CloseUI)
        {
            conditionBase = new PanelCloseCondition();
        }
        else if (type == EGuideConditionType.ClickBtn)
        {
            conditionBase = new ButtonClickCondition();
        }
        else
        {
            ClientLog.Instance.LogError($"引导条件创建失败{type}");
            return null;
        }
        conditionBase.Init(param, param2);
        return conditionBase;
    }
}
