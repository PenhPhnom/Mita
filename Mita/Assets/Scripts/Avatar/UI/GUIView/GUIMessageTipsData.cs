using UnityEngine;
using UnityEngine.UI;

//此脚本为自动生成 切勿手动更改!!!

partial class GUIMessageTipsData
{
    private Injection m_Injection;
    public Image ImgBottom;	
    public Text TxtInfo;	


    public void InitUI(GameObject obj)
    {
        if (obj == null)
        {
            ClientLog.Instance.LogError("当前Panel传入的Obj为空");
            return;
        }
        m_Injection = obj.GetComponent<Injection>();
        ImgBottom = m_Injection.UIObjects[0].Component.GetComponent<Image>();		
        TxtInfo = m_Injection.UIObjects[1].Component.GetComponent<Text>();		

    }
}