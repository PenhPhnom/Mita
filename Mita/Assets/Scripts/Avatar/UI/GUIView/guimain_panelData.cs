using EnhancedUI.EnhancedScroller;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//此脚本为自动生成 切勿手动更改!!!

public partial class guimain_panelData
{
    private Injection m_Injection;
    public Image ImgXxx;
    public Button BtnShowmessage;
    public Text TxtOk;
    public Button BtnShowmessageOther;
    public RawImage RawImgIytjk;
    public EnhancedScroller EnsScroller;


    public void InitUI(GameObject obj)
    {
        if (obj == null)
        {
            ClientLog.Instance.LogError("当前Panel传入的Obj为空");
            return;
        }
        m_Injection = obj.GetComponent<Injection>();
        ImgXxx = m_Injection.UIObjects[0].Component.GetComponent<Image>();
        BtnShowmessage = m_Injection.UIObjects[1].Component.GetComponent<Button>();
        TxtOk = m_Injection.UIObjects[2].Component.GetComponent<Text>();
        BtnShowmessageOther = m_Injection.UIObjects[3].Component.GetComponent<Button>();
        RawImgIytjk = m_Injection.UIObjects[4].Component.GetComponent<RawImage>();
        EnsScroller = m_Injection.UIObjects[5].Component.GetComponent<EnhancedScroller>();
        AutoInitText();
    }

    private void AutoInitText()
    {
        //GlobalFunction.SetText(TxtOk,);

    }

    public T GetComponentByString<T>(string cptName)
    {
        foreach (T component in from t in m_Injection.UIObjects where cptName == t.Name select t.Component.GetComponent<T>())
        {
            if (component is T)
            {
                return component;
            }
            else
            {
                ClientLog.Instance.LogError($"找到的组件 {cptName} 不是类型 {typeof(T)}");
                return default(T);
            }
        }

        ClientLog.Instance.LogError($"当前参数 {cptName} 无法匹配到对应的组件");
        return default(T);
    }

    public GameObject GetComponentByString(string cptName)
    {
        foreach (var item in m_Injection.UIObjects)
            if (cptName == item.Target.name)
                return item.Target as GameObject;

        return null;
    }
}
