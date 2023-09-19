using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//此脚本为自动生成 切勿手动更改!!!

public partial class CanvasData
{
    private Injection m_Injection;
    public Image ImgTop;	
    public Image ImgLight;	
    public Image ImgTimebg;	
    public Text TxtTime;	
    public Image ImgBottom;	
    public Button BtnReturn;	
    public Text TxtSocre;	
    public Image ImgScorebg;	
    public Text TxtScorevalue;	
    public Image ImgGameover;	
    public RectTransform TransBubbles;	
    public Button BtnReplay;	
    public Button BtnReturntomain;	
    public Text TxtFinalscore;	


    public void InitUI(GameObject obj)
    {
        if (obj == null)
        {
            ClientLog.Instance.LogError("当前Panel传入的Obj为空");
            return;
        }
        m_Injection = obj.GetComponent<Injection>();
        ImgTop = m_Injection.UIObjects[0].Component.GetComponent<Image>();		
        ImgLight = m_Injection.UIObjects[1].Component.GetComponent<Image>();		
        ImgTimebg = m_Injection.UIObjects[2].Component.GetComponent<Image>();		
        TxtTime = m_Injection.UIObjects[3].Component.GetComponent<Text>();		
        ImgBottom = m_Injection.UIObjects[4].Component.GetComponent<Image>();		
        BtnReturn = m_Injection.UIObjects[5].Component.GetComponent<Button>();		
        TxtSocre = m_Injection.UIObjects[6].Component.GetComponent<Text>();		
        ImgScorebg = m_Injection.UIObjects[7].Component.GetComponent<Image>();		
        TxtScorevalue = m_Injection.UIObjects[8].Component.GetComponent<Text>();		
        ImgGameover = m_Injection.UIObjects[9].Component.GetComponent<Image>();		
        TransBubbles = m_Injection.UIObjects[10].Component.GetComponent<RectTransform>();		
        BtnReplay = m_Injection.UIObjects[11].Component.GetComponent<Button>();		
        BtnReturntomain = m_Injection.UIObjects[12].Component.GetComponent<Button>();		
        TxtFinalscore = m_Injection.UIObjects[13].Component.GetComponent<Text>();		

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
