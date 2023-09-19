using UnityEngine;
using UnityEngine.UI;

//此脚本为自动生成 切勿手动更改!!!

partial class UITestCellItemData
{
    private Injection m_Injection;
    public Image ImgBgTest;	
    public Text TxtTest;	
    public Button BtnTest;	


    public void InitUI(GameObject obj)
    {
        if (obj == null)
        {
            ClientLog.Instance.LogError("当前Item传入的Obj为空");
            return;
        }
        m_Injection = obj.GetComponent<Injection>();
        ImgBgTest = m_Injection.UIObjects[0].Component.GetComponent<Image>();		
        TxtTest = m_Injection.UIObjects[1].Component.GetComponent<Text>();		
        BtnTest = m_Injection.UIObjects[2].Component.GetComponent<Button>();		

    }
}