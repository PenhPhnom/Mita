using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//此脚本为自动生成 切勿手动更改!!!

public partial class StartGameUIData
{
    private Injection m_Injection;
    public Image ImgBG;	
    public RectTransform TransChocolate;	
    public Image ImgChocolateStick;	
    public Image ImgChocolate;	
    public Image ImgCream;	
    public Image ImgDoll;	
    public Text TxtTitle;	
    public Button BtnExit;	
    public Button BtnStart;	
    public RectTransform TransStars;	
    public Text TxtLevelLab;	
    public InputField InptLevel;	
    public Button BtnShuoming;	
    public Image ImgShuoming;	
    public Button BtnExtSM;	


    public void InitUI(GameObject obj)
    {
        if (obj == null)
        {
            ClientLog.Instance.LogError("当前Panel传入的Obj为空");
            return;
        }
        m_Injection = obj.GetComponent<Injection>();
        ImgBG = m_Injection.UIObjects[0].Component.GetComponent<Image>();		
        TransChocolate = m_Injection.UIObjects[1].Component.GetComponent<RectTransform>();		
        ImgChocolateStick = m_Injection.UIObjects[2].Component.GetComponent<Image>();		
        ImgChocolate = m_Injection.UIObjects[3].Component.GetComponent<Image>();		
        ImgCream = m_Injection.UIObjects[4].Component.GetComponent<Image>();		
        ImgDoll = m_Injection.UIObjects[5].Component.GetComponent<Image>();		
        TxtTitle = m_Injection.UIObjects[6].Component.GetComponent<Text>();		
        BtnExit = m_Injection.UIObjects[7].Component.GetComponent<Button>();		
        BtnStart = m_Injection.UIObjects[8].Component.GetComponent<Button>();		
        TransStars = m_Injection.UIObjects[9].Component.GetComponent<RectTransform>();		
        TxtLevelLab = m_Injection.UIObjects[10].Component.GetComponent<Text>();		
        InptLevel = m_Injection.UIObjects[11].Component.GetComponent<InputField>();		
        BtnShuoming = m_Injection.UIObjects[12].Component.GetComponent<Button>();		
        ImgShuoming = m_Injection.UIObjects[13].Component.GetComponent<Image>();		
        BtnExtSM = m_Injection.UIObjects[14].Component.GetComponent<Button>();		

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
