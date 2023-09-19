using UnityEngine;
using UnityEngine.UI;

//此脚本为自动生成 切勿手动更改!!!

public partial class GUIGuide_PanelData
{
    private Injection m_Injection;
    public GameObject GoBg;	
    public GameObject GoMask;	
    public GameObject GoWidget;	
    public GameObject GoWidgetChild;	
    public GameObject GoSelect;	
    public GameObject GoCircle;	
    public Button BtnSpecial;	
    public GameObject GoFinger;	
    public Image ImgFinger;	
    public Button BtnInput;	
    public GameObject GoDialog;	
    public Text TxtDialog;	
    public GameObject GoInputFilter;	
    public Button BtnKillCurGuide;	
    public GameObject GoFromToPointer;	


    public void InitUI(GameObject obj)
    {
        if (obj == null)
        {
            ClientLog.Instance.LogError("当前Panel传入的Obj为空");
            return;
        }
        m_Injection = obj.GetComponent<Injection>();
        GoBg = m_Injection.UIObjects[0].Target as GameObject;		
        GoMask = m_Injection.UIObjects[1].Target as GameObject;		
        GoWidget = m_Injection.UIObjects[2].Target as GameObject;		
        GoWidgetChild = m_Injection.UIObjects[3].Target as GameObject;		
        GoSelect = m_Injection.UIObjects[4].Target as GameObject;		
        GoCircle = m_Injection.UIObjects[5].Target as GameObject;		
        BtnSpecial = m_Injection.UIObjects[6].Component.GetComponent<Button>();		
        GoFinger = m_Injection.UIObjects[7].Target as GameObject;		
        ImgFinger = m_Injection.UIObjects[8].Component.GetComponent<Image>();		
        BtnInput = m_Injection.UIObjects[9].Component.GetComponent<Button>();		
        GoDialog = m_Injection.UIObjects[10].Target as GameObject;		
        TxtDialog = m_Injection.UIObjects[11].Component.GetComponent<Text>();		
        GoInputFilter = m_Injection.UIObjects[12].Target as GameObject;		
        BtnKillCurGuide = m_Injection.UIObjects[13].Component.GetComponent<Button>();		
        GoFromToPointer = m_Injection.UIObjects[14].Target as GameObject;		

    }

    public T GetComponentByString<T>(string cptName)
    {
        foreach (var t in m_Injection.UIObjects)
        {
            if (cptName == t.Name)
            {
                T component = t.Component.GetComponent<T>();
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
        }

        ClientLog.Instance.LogError($"当前参数 {cptName} 无法匹配到对应的组件");
        return default(T);
    }
}