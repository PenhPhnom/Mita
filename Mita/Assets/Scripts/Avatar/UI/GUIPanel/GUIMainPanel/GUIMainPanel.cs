using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data
{
    public int hour;
}

public class GUIMainPanel : BaseUI
{
    private guimain_panelData m_UIData = new guimain_panelData();
    private ScrollRectParam<List<Data>> m_ScrollRectParam;
    public string resourcePath = "Multiple Cell Type Demo";
    public override bool HasOpenTween()
    {
        return true;
    }
    public override EnumUIType GetUIType() { return EnumUIType.Login; }

    public GUIMainPanel()
    {
        //注册按钮事件
    }

    protected override void OnAwake(GameObject obj)
    {
        if (m_UIData == null) return;
        m_UIData.InitUI(obj);
        GlobalFunction.AddEnevntTrigger(m_UIData.BtnShowmessage.gameObject, EnumTouchEventType.OnClick, OnClickBtnMessage);
        //GlobalFunction.AddEnevntTrigger(m_UIData.BtnShowmessageOther.gameObject, EnumTouchEventType.OnDoubleClick, OnDoubleClickBtnMessage);
        //GlobalFunction.AddEnevntTrigger(m_UIData.BtnShowmessage.gameObject, EnumTouchEventType.OnLongPress, OnClickBtnMessageOnPress);
        GlobalFunction.AddEnevntTrigger(m_UIData.BtnShowmessage.gameObject, EnumTouchEventType.OnClick, OnClickBtnMessage10);

        LoadData2();
    }

    private void LoadData2()
    {

        var _data = new List<Data>();

        //_data.Add(new HeaderData() { category = "Platinum Players" });
        //_data.Add(new RowData() { userName = "John Smith", userAvatarSpritePath = resourcePath + "/avatar_male", userHighScore = 21323199 });
        //_data.Add(new RowData() { userName = "Jane Doe", userAvatarSpritePath = resourcePath + "/avatar_female", userHighScore = 20793219 });
        //_data.Add(new RowData() { userName = "Julie Prost", userAvatarSpritePath = resourcePath + "/avatar_female", userHighScore = 19932132 });
        //_data.Add(new FooterData());

        //_data.Add(new HeaderData() { category = "Gold Players" });
        //_data.Add(new RowData() { userName = "Jim Bob", userAvatarSpritePath = resourcePath + "/avatar_male", userHighScore = 1002132 });
        //_data.Add(new RowData() { userName = "Susan Anthony", userAvatarSpritePath = resourcePath + "/avatar_female", userHighScore = 991234 });
        //_data.Add(new FooterData());

        //_data.Add(new HeaderData() { category = "Silver Players" });
        //_data.Add(new RowData() { userName = "Gary Richards", userAvatarSpritePath = resourcePath + "/avatar_male", userHighScore = 905723 });
        //_data.Add(new RowData() { userName = "John Doe", userAvatarSpritePath = resourcePath + "/avatar_male", userHighScore = 702318 });
        //_data.Add(new RowData() { userName = "Lisa Ford", userAvatarSpritePath = resourcePath + "/avatar_female", userHighScore = 697767 });
        //_data.Add(new RowData() { userName = "Jacob Morris", userAvatarSpritePath = resourcePath + "/avatar_male", userHighScore = 409393 });
        //_data.Add(new RowData() { userName = "Carolyn Shephard", userAvatarSpritePath = resourcePath + "/avatar_female", userHighScore = 104352 });
        //_data.Add(new RowData() { userName = "Guy Wilson", userAvatarSpritePath = resourcePath + "/avatar_male", userHighScore = 88321 });
        //_data.Add(new RowData() { userName = "Jackie Jones", userAvatarSpritePath = resourcePath + "/avatar_female", userHighScore = 20826 });
        //_data.Add(new RowData() { userName = "Sally Brewer", userAvatarSpritePath = resourcePath + "/avatar_female", userHighScore = 17389 });
        //_data.Add(new RowData() { userName = "Joe West", userAvatarSpritePath = resourcePath + "/avatar_male", userHighScore = 2918 });
        //_data.Add(new FooterData());

        for (int i = 0; i < 50; i++)
            _data.Add(new Data() { hour = i });

        m_ScrollRectParam = new ScrollRectParam<List<Data>>(_data, m_UIData.EnsScroller, _data.Count, 100
        //(dataIndex) =>
        //{
        //    if (_data[dataIndex] is HeaderData)
        //    {
        //        return 0;
        //    }
        //    else if (_data[dataIndex] is RowData)
        //    {
        //        return 1;
        //    }
        //    else
        //    {
        //        return 2;
        //    };
        //},
        //(dataIndex) =>
        //{
        //    if (_data[dataIndex] is HeaderData)
        //    {
        //        return 70f;
        //    }
        //    else if (_data[dataIndex] is RowData)
        //    {
        //        return 100f;
        //    }
        //    else
        //    {
        //        return 90f;
        //    }
        //},
        //(_view, dataIndex, cellIndex) =>
        //{

        //}
        );

        m_UIData.EnsScroller.ReloadData();
    }

    protected override void OnStart()
    {
        // ClientLog.Instance.Log(m_data.XXXXImg.name);
        //GlobalFunction.SetRawImage(m_UIData.RawImgIytjk, "eyebrowShape02", "avararlcon");
        GlobalFunction.SetImage(m_UIData.ImgXxx, "TestAtlas", "change_01_icon");
        GlobalFunction.SetColorByTag(m_UIData.TxtOk, "TestColor");
        //DataLoader.Instance.GetTargetGroup(11);

        //tree = new RedPointTree();
        //tree.RegistRedEvent(NodeNames.Root, "Root", (redpointCnt) =>
        //{
        //    //redpointText.text = tostring(redpointCnt);
        //    GlobalFunction.SetText(m_UIData.TxtOk, redpointCnt.ToString());
        //});
        RedDotMgr.Instance.AddListener(RedDot.Main, MainRedDotCallback);

        GlobalFunction.SetText(m_UIData.TxtOk, "tid#B1", 78);
        //GlobalFunction.SetText(m_UIData.TxtOk, "tid#C1", 40, 33);

    }

    private void MainRedDotCallback(int value)
    {
        ClientLog.Instance.Log("红点刷新，路径:" + RedDot.Main + ",当前帧数:" + Time.frameCount + ",值:" + value);
    }

    private void OnClickBtnMessage10(GameObject _listener, object _args, params object[] _params)
    {
        ClientLog.Instance.Log("你是我的小丫小苹果111 OnClickBtnMessage10");
    }

    private void OnClickBtnMessage(GameObject _listener, object _args, params object[] _params)
    {
        //if (_args != null && (bool)_args)
        //{
        //    ClientLog.Instance.Log("OnClickBtnMessage");
        //    MessageBox.Instance.ShowMessageBox("你是我的小丫小苹果111", (result, param) =>
        //    {
        //        ClientLog.Instance.Log("你是我的小丫小苹果111 ClickOK");
        //    });
        //}
        //else
        //{
        ClientLog.Instance.Log("你是我的小丫小苹果333333");
        //}

        //MessageTips.Instance.ShowMessageTips("你是我的小丫小苹果");
        GlobalFunction.SetImage(m_UIData.ImgXxx, "TestAtlas", "change_02_icon");
        //通过服务器或者其他 给 
        //RedDotMgr.Instance.ChangeValue();
    }

    private void OnClickBtnMessageOnPress(GameObject _listener, object _args, params object[] _params)
    {
        GlobalFunction.SetImage(m_UIData.ImgXxx, "TestAtlas", "change_03_icon");
    }


    private void OnDoubleClickBtnMessage(GameObject _listener, object _args, params object[] _params)
    {
        ClientLog.Instance.Log("OnDoubleClickBtnMessage");
        MessageBox.Instance.ShowMessageBox("你是我的小丫小苹果222", (result, param) =>
        {
            ClientLog.Instance.Log("你是我的小丫小苹果222");
        });
    }

    public override GameObject GetComponentByString(string name)
    {
        return m_UIData.GetComponentByString(name);
    }

    public override void OnRelease()
    {
        if (m_ScrollRectParam != null)
            m_ScrollRectParam.OnRelease();

        //tree.UnRegistRedEvent(NodeNames.Root, "Root");
        RedDotMgr.Instance.RemoveListener(RedDot.Main, MainRedDotCallback);
    }
}
