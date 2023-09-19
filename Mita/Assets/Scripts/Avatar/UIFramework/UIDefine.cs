using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ELayer
{
    Bottom = 1, //底部常驻
    Panel = 2, //面板 全屏界面
    Pop = 3, //弹窗对话框 附带黑底遮罩
    Front = 4, //面板上层 子界面等 Pop和Front同层级
    Top = 5, //顶部常驻
    FlyTip = 6, //走马灯、广播
    Plot = 7, //剧情
    Guide = 8, //教学
    Loading = 9, //Loading界面
    NetError = 10//网络错误弹框
}

public enum EOpen
{
    Normal = 1,
    Toggle = 2,
    Exclu = 3,
    None = 4,
}


public class UIDefine
{

}

public class UIClass
{
    public string path = "MOMO.COS.View.Tips.confirm_panel";
    public ELayer layer = ELayer.Pop;
    public EOpen open = EOpen.Normal;
    public bool affected = true;
    public bool destroy = false;
    public bool only = true;
    //depend = null,
    public string group = "confirm";

}

public class UIClassTest
{
    private Dictionary<ELayer, GameObject> _layerMap = new Dictionary<ELayer, GameObject>();
    public void Init(GameObject uiRoot)
    {
        var rootTrans = uiRoot.transform;
        _layerMap[ELayer.Bottom] = rootTrans.GetChild(0).gameObject;
        _layerMap[ELayer.Panel] = rootTrans.GetChild(1).gameObject;
        _layerMap[ELayer.Pop] = rootTrans.GetChild(2).gameObject;
        _layerMap[ELayer.Front] = _layerMap[ELayer.Pop];
        _layerMap[ELayer.Top] = rootTrans.GetChild(3).gameObject;
        _layerMap[ELayer.FlyTip] = rootTrans.GetChild(4).gameObject;
        _layerMap[ELayer.Plot] = rootTrans.GetChild(5).gameObject;
        _layerMap[ELayer.Guide] = rootTrans.GetChild(6).gameObject;
        _layerMap[ELayer.Loading] = rootTrans.GetChild(7).gameObject;
        _layerMap[ELayer.NetError] = rootTrans.GetChild(8).gameObject;
    }

    public void showPanel()
    {



    }


}

