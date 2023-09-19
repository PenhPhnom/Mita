using UnityEngine;
using UnityEngine.UI;

//此脚本为自动生成 切勿手动更改!!!

partial class UITestDemoRecycleViewPanelData
{
    private Injection m_Injection;
    public RecycleView RvScrollView;	
    public Button BtnButton1;	
    public Button BtnButton2;	
    public Button BtnButton3;	
    public Button BtnButton4;	
    public Button BtnButton5;	


    public void InitUI(GameObject obj)
    {
        if (obj == null)
        {
            ClientLog.Instance.LogError("当前Panel传入的Obj为空");
            return;
        }
        m_Injection = obj.GetComponent<Injection>();
        RvScrollView = m_Injection.UIObjects[0].Component.GetComponent<RecycleView>();		
        BtnButton1 = m_Injection.UIObjects[1].Component.GetComponent<Button>();		
        BtnButton2 = m_Injection.UIObjects[2].Component.GetComponent<Button>();		
        BtnButton3 = m_Injection.UIObjects[3].Component.GetComponent<Button>();		
        BtnButton4 = m_Injection.UIObjects[4].Component.GetComponent<Button>();		
        BtnButton5 = m_Injection.UIObjects[5].Component.GetComponent<Button>();		

    }
}