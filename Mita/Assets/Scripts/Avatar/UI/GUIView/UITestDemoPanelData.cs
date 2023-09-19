using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//此脚本为自动生成 切勿手动更改!!!

public partial class UITestDemoPanelData
{
    private Injection m_Injection;
    public GameObject GoTest;	
    public RectTransform TransTest;	
    public Canvas CvsTest;	
    public Image ImgTest;	
    public Text TxtTest;	
    public Button BtnTest;	
    public Toggle TogTest;	
    public ToggleGroup TogGroupTest;	
    public InputField InptTest;	
    public Scrollbar ScrlTest;	
    public Slider SldrTest;	
    public Dropdown DrpdnTest;	
    public Grid GrdTest;	
    public ScrollRect ScrollRectTest;	
    public RawImage RawImgTest;	
    public Mask MaskTest;	
    public GridLayoutGroup GrdLayoutGroupTest;	
    public HorizontalLayoutGroup HLayoutGroupTest;	
    public VerticalLayoutGroup VLayoutGroupTest;	
    public ContentSizeFitter ContentSizeFitterTest;	
    public Selectable SelectableTest;	
    public RecycleView RvTestList;	


    public void InitUI(GameObject obj)
    {
        if (obj == null)
        {
            ClientLog.Instance.LogError("当前Panel传入的Obj为空");
            return;
        }
        m_Injection = obj.GetComponent<Injection>();
        GoTest = m_Injection.UIObjects[0].Target as GameObject;		
        TransTest = m_Injection.UIObjects[1].Component.GetComponent<RectTransform>();		
        CvsTest = m_Injection.UIObjects[2].Component.GetComponent<Canvas>();		
        ImgTest = m_Injection.UIObjects[3].Component.GetComponent<Image>();		
        TxtTest = m_Injection.UIObjects[4].Component.GetComponent<Text>();		
        BtnTest = m_Injection.UIObjects[5].Component.GetComponent<Button>();		
        TogTest = m_Injection.UIObjects[6].Component.GetComponent<Toggle>();		
        TogGroupTest = m_Injection.UIObjects[7].Component.GetComponent<ToggleGroup>();		
        InptTest = m_Injection.UIObjects[8].Component.GetComponent<InputField>();		
        ScrlTest = m_Injection.UIObjects[9].Component.GetComponent<Scrollbar>();		
        SldrTest = m_Injection.UIObjects[10].Component.GetComponent<Slider>();		
        DrpdnTest = m_Injection.UIObjects[11].Component.GetComponent<Dropdown>();		
        GrdTest = m_Injection.UIObjects[12].Component.GetComponent<Grid>();		
        ScrollRectTest = m_Injection.UIObjects[13].Component.GetComponent<ScrollRect>();		
        RawImgTest = m_Injection.UIObjects[14].Component.GetComponent<RawImage>();		
        MaskTest = m_Injection.UIObjects[15].Component.GetComponent<Mask>();		
        GrdLayoutGroupTest = m_Injection.UIObjects[16].Component.GetComponent<GridLayoutGroup>();		
        HLayoutGroupTest = m_Injection.UIObjects[17].Component.GetComponent<HorizontalLayoutGroup>();		
        VLayoutGroupTest = m_Injection.UIObjects[18].Component.GetComponent<VerticalLayoutGroup>();		
        ContentSizeFitterTest = m_Injection.UIObjects[19].Component.GetComponent<ContentSizeFitter>();		
        SelectableTest = m_Injection.UIObjects[20].Component.GetComponent<Selectable>();		
        RvTestList = m_Injection.UIObjects[21].Component.GetComponent<RecycleView>();		

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
}
