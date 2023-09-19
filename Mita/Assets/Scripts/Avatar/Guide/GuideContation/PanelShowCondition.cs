using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelShowCondition : GuideEventListenConditionBase
{
    public override EEventType GetEventType()
    {
        return EEventType.UI_SHOW_PANEL;
    }

    public override bool InternalMeet()
    {
        var panel = UIManager.Instance.GetUIObject((EnumUIType)(int.Parse(Param)));
        return panel != null ? true : false;
    }

    public override void Monitor(object param)
    {
        int pal = (int)param;
        int result;
        int.TryParse(Param, out result);
        if (pal == result)
            SetFullfilled();
    }

    public override void Dispose()
    {

    }
}
