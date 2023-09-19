using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUILoadingPanel : BaseUI
{
    public Slider SliderProgress;
    public override EnumUIType GetUIType()
    {
        return EnumUIType.LOADING;
    }

    public GUILoadingPanel()
    {
        EventMgr.Instance.RegisterEvent(EEventType.LOADINGPROCESS, LoadingProcess);
    }
    protected override void OnAwake(GameObject obj)
    {

    }

    protected override void OnStart()
    {

    }

    private void LoadingProcess(object param)
    {
        SliderProgress.value = (float)param;
    }

    public override void OnRelease()
    {
        EventMgr.Instance.UnRegisterEvent(EEventType.LOADINGPROCESS, LoadingProcess);
    }
}
