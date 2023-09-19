using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class UITestCellItem : MonoBehaviour
{
    private UITestCellItemData uiData = new UITestCellItemData();

    private void Awake()
    {
        uiData.InitUI(this.gameObject);
        uiData.BtnTest.onClick.AddListener(OnTestBtnClick);
    }

    private void OnTestBtnClick()
    {
        uiData.ImgBgTest.color = new Color(0.5f, 1, 1, 1);
        UITools.SetText(uiData.TxtTest,"OnClick");
    }

    public void SetData(int info, GameObject go, int index = 0)
    {

        UITools.SetText(uiData.TxtTest, info);
        uiData.ImgBgTest.color = new Color(1f, 1, 1, 1);
        // if (index <= 4)
        //     UITools.SetText(uiData.TxtTest, info);
        // else
        //     UITools.SetText(uiData.TxtTest, index % 2 == 0 ? "test？" : "测试？");

        UITools.SetActive(uiData.ImgBgTest, index % 2 == 0);
    }

    public void OnRelease()
    {

    }
    private void OnDestroy()
    {
        OnRelease();
    }
}
