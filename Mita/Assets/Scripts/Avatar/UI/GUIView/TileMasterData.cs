using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//此脚本为自动生成 切勿手动更改!!!

public partial class TileMasterData
{
    private Injection m_Injection;
    public RectTransform TransImgroot;	
    public Image ImgThing1;	
    public Image ImgThing2;	
    public Image ImgThing3;	
    public RectTransform TransImgtxt;	
    public Text TxtThing1;	
    public Text TxtThing2;	
    public Text TxtThing3;	


    public void InitUI(GameObject obj)
    {
        if (obj == null)
        {
            ClientLog.Instance.LogError("当前Panel传入的Obj为空");
            return;
        }
        m_Injection = obj.GetComponent<Injection>();
        TransImgroot = m_Injection.UIObjects[0].Component.GetComponent<RectTransform>();		
        ImgThing1 = m_Injection.UIObjects[1].Component.GetComponent<Image>();		
        ImgThing2 = m_Injection.UIObjects[2].Component.GetComponent<Image>();		
        ImgThing3 = m_Injection.UIObjects[3].Component.GetComponent<Image>();		
        TransImgtxt = m_Injection.UIObjects[4].Component.GetComponent<RectTransform>();		
        TxtThing1 = m_Injection.UIObjects[5].Component.GetComponent<Text>();		
        TxtThing2 = m_Injection.UIObjects[6].Component.GetComponent<Text>();		
        TxtThing3 = m_Injection.UIObjects[7].Component.GetComponent<Text>();		

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
