using System;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public static class UITools
{
    //用于UI或GameObject显隐
    #region SetActive

    /// <summary>
    /// 通过游戏物体显隐
    /// </summary>
    /// <param name="go">当前游戏物体</param>
    /// <param name="bState">true 显示 or false 隐藏</param>
    public static void SetActive(GameObject go, bool bState) => BaseSetActive(go, bState);

    /// <summary>
    /// 通过组件显隐
    /// </summary>
    /// <param name="ct">当前组件名字</param>
    /// <param name="bState">true 显示 or false 隐藏</param>
    public static void SetActive(Component ct, bool bState) => BaseSetActive(ct, bState);

    private static void BaseSetActive(Component ct, bool bState)
    {
        try
        {
            ct.gameObject.SetActive(bState);
        }
        catch (NullReferenceException e)
        {
            Debug.LogError("当前传入了一个NUll值 设置物体显隐失败请检查堆栈后重新赋值");
        }
    }

    private static void BaseSetActive(GameObject go, bool bState)
    {
        try
        {
            go.SetActive(bState);
        }
        catch (NullReferenceException e)
        {
            Debug.LogError("当前传入了一个NUll值 设置物体显隐失败请检查堆栈后重新赋值");
        }
    }

    #endregion
    //需要显隐局部组件 and 该组件下没有子物体 or 少量子物体的情况下 建议使用该方法（相较于SetActiveGo 会增加额外渲染和计算）
    #region SetActiveLocal

    /// <summary>
    /// 通过游戏物体显隐
    /// </summary>
    /// <param name="go">当前游戏物体</param>
    /// <param name="bState">true 显示 or false 隐藏</param>
    public static void SetActiveLocal(GameObject go, bool bState) => BaseSetActiveLocal(go, bState);

    /// <summary>
    /// 通过组件显隐
    /// </summary>
    /// <param name="ct">当前组件名字</param>
    /// <param name="bState">true 显示 or false 隐藏</param>
    public static void SetActiveLocal(Component ct, bool bState) => BaseSetActiveLocal(ct, bState);

    private static void BaseSetActiveLocal(GameObject go, bool bState)
    {
        try
        {
            go.transform.localScale = bState ? Vector3.one : Vector3.zero;
        }
        catch (NullReferenceException e)
        {
            Debug.LogError("当前传入了一个NUll值 设置物体显隐失败请检查堆栈后重新赋值");
        }
    }

    private static void BaseSetActiveLocal(Component ct, bool bState)
    {
        try
        {
            ct.transform.localScale = bState ? Vector3.one : Vector3.zero;
        }
        catch (NullReferenceException e)
        {
            Debug.LogError("当前传入了一个NUll值 设置物体显隐失败请检查堆栈后重新赋值");
        }
    }

    #endregion
    #region SetText
    public static void SetText(Text txt, string context = "") => txt.text = context;

    /// <summary>
    /// 设置Text文本
    /// </summary>
    /// <param name="trans">要设置的Transform</param>
    /// <param name="num">需要输出的内容(int 类型)，这里是为了策划配ID读表用的 先留个坑位</param>
    public static void SetText(Transform trans, int num = 0) => SetText(trans.GetComponent<Text>(), num.ToString());

    /// <summary>
    ///设置Text文本
    /// </summary>
    /// <param name="trans">要设置的Transform</param>
    /// <param name="context">需要输入的内容</param>
    public static void SetText(Transform trans, string context = "") => SetText(trans.GetComponent<Text>(), context);

    /// <summary>
    /// 设置Text文本
    /// </summary>
    /// <param name="txt">Text组件</param>
    /// <param name="num">传入的内容(int类型)</param>
    public static void SetText(Text txt, int num = 0) => SetText(txt, num.ToString());

    #endregion
    #region SetIcon

    public static void
        SetIcon(Image image, int uiType, int sourceId, System.Action callBack, bool bSetDefault = false) =>
        SetIconBase(image, uiType, sourceId, callBack, bSetDefault);

    public static void SetIconSync(Image image, int uiType, int sourceId, System.Action callBack,
        bool bSetDefault = false) => SetIconBase(image, uiType, sourceId, callBack, bSetDefault, true);

    private static void SetIconBase(Image image, int uiType, int sourceId, System.Action callBack,
        bool bSetDefault = false, bool bSync = false)
    {
    }

    #endregion
    #region  无限列表
    public static void AddRvPanelList(RecycleView rvList, int count, System.Action<GameObject, int> callback = null)
    {
        if (rvList == null)
        {
            ClientLog.Instance.LogError("当前无限列表为空");
            return;
        }
        else
        {
            rvList.Init();
            rvList.ShowList(count, callback);
        }
    }
    #endregion
}