using UnityEngine;
using UnityEngine.UI;

//此脚本为自动生成 切勿手动更改!!!

partial class GUIMessageBoxData
{
    private Injection m_Injection;
    [HideInInspector] public Image ImgXxx;	
    [HideInInspector] public Button BtnOk;	
    [HideInInspector] public Button BtnCacel;	


    public void InitUI(GameObject obj)
    {
        if (obj == null)
        {
            ClientLog.Instance.LogError("当前Panel传入的Obj为空");
            return;
        }
        m_Injection = obj.GetComponent<Injection>();
        ImgXxx = m_Injection.UIObjects[0].Component.GetComponent<Image>();		
        BtnOk = m_Injection.UIObjects[1].Component.GetComponent<Button>();		
        BtnCacel = m_Injection.UIObjects[2].Component.GetComponent<Button>();		

    }
}