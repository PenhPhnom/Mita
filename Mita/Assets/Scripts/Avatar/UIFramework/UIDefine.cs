using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ELayer
{
    Bottom = 1, //�ײ���פ
    Panel = 2, //��� ȫ������
    Pop = 3, //�����Ի��� �����ڵ�����
    Front = 4, //����ϲ� �ӽ���� Pop��Frontͬ�㼶
    Top = 5, //������פ
    FlyTip = 6, //����ơ��㲥
    Plot = 7, //����
    Guide = 8, //��ѧ
    Loading = 9, //Loading����
    NetError = 10//������󵯿�
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

