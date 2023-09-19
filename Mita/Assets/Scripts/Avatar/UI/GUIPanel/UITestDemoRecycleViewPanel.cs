using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class UITestDemoRecycleViewPanel : MonoBehaviour
{
    private UITestDemoRecycleViewPanelData uiData = new UITestDemoRecycleViewPanelData();
    //模拟服务器发送的表数据
    public List<string> myServerList = new List<string> {"一", "二", "三", "四", "五",};
    //模拟服务器发送了五百条数据 客户端经过处理后展示五百条数据
    List<int> myServerDataList = Enumerable.Range(1, 500).ToList();

    public int GotoIndex = 369;
    private void Awake()
    {
        uiData.InitUI(this.gameObject);
        StartScrollView();
        OnBtnRegister();
    }
    private void StartScrollView()
    {
        UITools.AddRvPanelList
        (
            uiData.RvScrollView,
            myServerDataList.Count,
            (item, index) =>
        {
            if (item?.GetComponent<UITestCellItem>() is UITestCellItem cellItem)
                cellItem.SetData(myServerDataList[index], item, index);
            else
                return;
            //cellItem.OnDataChanged += StartScrollView;
        });
    }

    private void OnBtnRegister()
    {
        uiData.BtnButton1.onClick.AddListener(() => { uiData.RvScrollView.GoToOneLine(); });
        uiData.BtnButton2.onClick.AddListener(() => { uiData.RvScrollView.GoToCellPos(GotoIndex); });

        uiData.BtnButton4.onClick.AddListener(TestUpdateData);
    }

    private void TestUpdateData()
    {
        for (int i = 0; i < myServerDataList.Count; i++)
            myServerDataList[i] = Random.Range(0, 500);

        StartScrollView();
    }
}
