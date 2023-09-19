using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;

//此脚本为自动生成 切勿手动更改!!!

public partial class maintestItemData
{
    private Injection m_Injection;
    public Button BtnClick;	
    public Text TxtShow;	


    public void InitUI(GameObject obj)
    {
        if (obj == null)
        {
            ClientLog.Instance.LogError("当前Item传入的Obj为空");
            return;
        }
        m_Injection = obj.GetComponent<Injection>();
        BtnClick = m_Injection.UIObjects[0].Component.GetComponent<Button>();		
        TxtShow = m_Injection.UIObjects[1].Component.GetComponent<Text>();		

    }
}