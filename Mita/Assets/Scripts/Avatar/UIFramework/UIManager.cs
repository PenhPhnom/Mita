//
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// User interface manager.
/// </summary>
public partial class UIManager : Singleton<UIManager>
{
    #region UIInfoData class
    /// <summary>
    ///
    /// </summary>
    class UIInfoData
    {
        public EnumUIType UIType { get; private set; }
        public BaseUI ScriptType { get; private set; }
        public string Name { get; private set; }
        public object[] UIParams { get; private set; }

        public EPanelLayerType LayerType { get; private set; }

        public Action callback;
        public UIInfoData(EnumUIType _uiType, string _name, EPanelLayerType _layerType, Action _callback, params object[] _uiParams)
        {
            UIType = _uiType;
            Name = _name;
            UIParams = _uiParams;
            ScriptType = UIPathDefines.GetUIScriptByType(UIType);
            LayerType = _layerType;
            callback = _callback;
        }
    }
    #endregion

    private Dictionary<EnumUIType, BaseUI> m_DicOpenUIs = null;
    private Dictionary<EnumUIType, bool> dicLoadingUIs = null;

    private Stack<UIInfoData> stackOpenUIs = null;

    private float m_LoadingStartTime = 0f;

    private GameObject m_UIRoot;
    private GameObject m_BlackTrans;
    private bool m_BlackVisible = false;
    private MaskLayer m_MaskLayer;
    private List<EMaskNameType> m_MasklayerList = new List<EMaskNameType>();
    /// <summary>
    /// Init this Singleton.
    /// </summary>
    public override void Init()
    {
        EventMgr.Instance.RegisterEvent(EEventType.UI_ADD_MASK, OnAddMask);
        EventMgr.Instance.RegisterEvent(EEventType.UI_REMOVE_MASK, OnRemoveMask);
        m_DicOpenUIs = new Dictionary<EnumUIType, BaseUI>();
        dicLoadingUIs = new Dictionary<EnumUIType, bool>();
        stackOpenUIs = new Stack<UIInfoData>();
        m_UIRoot = GameObject.Find("UISystem/Canvas/uiRoot");
        m_BlackTrans = GameObject.Find("UISystem/Canvas/uiRoot/Pop/BlackMask");
        GameObject mask = GameObject.Find("UISystem/Canvas/uiRoot/Mask");
        if (mask != null)
            m_MaskLayer = mask.GetComponent<MaskLayer>();
    }

    #region Get UI & UIObject By EnunUIType 

    public GameObject GetUIRoot()
    {
        return m_UIRoot;
    }

    /// <summary>
    /// </summary>
    public T GetUI<T>(EnumUIType _uiType) where T : BaseUI
    {
        return GetUIObject(_uiType) as T;
    }

    /// <summary>
    /// 
    /// </summary>
    public BaseUI GetUIObject(EnumUIType _uiType)
    {
        BaseUI pBaseUI = null;
        m_DicOpenUIs.TryGetValue(_uiType, out pBaseUI);
        return pBaseUI;
    }
    #endregion


    #region Preload UI Prefab By EnumUIType
    /// <summary>
    /// 
    /// </summary>
    public void PreloadUI(EnumUIType[] _uiTypes)
    {
        for (int i = 0; i < _uiTypes.Length; i++)
        {
            PreloadUI(_uiTypes[i]);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void PreloadUI(EnumUIType _uiType)
    {
        string path = UIPathDefines.GetPrefabNameByType(_uiType);
        Resources.Load(path);
        //ResManager.Instance.ResourcesLoad(path);
    }

    #endregion

    #region Open UI By EnumUIType
    /// <summary>
    /// 
    /// </summary>
    public void OpenUI(EnumUIType uiType, Action callback = null)
    {
        OpenUI(false, uiType, EPanelLayerType.Panel, callback);
    }

    /// <summary>
    /// 
    /// </summary>
    public void OpenUI(EnumUIType uiType, EPanelLayerType _layerType = EPanelLayerType.Panel, Action callback = null, params object[] uiObjParams)
    {
        OpenUI(false, uiType, _layerType, callback, uiObjParams);
    }

    /// <summary>
    /// 
    /// </summary>
    public void OpenUICloseOthers(EnumUIType uiType, EPanelLayerType _layerType = EPanelLayerType.Panel)
    {
        OpenUI(true, uiType, _layerType, null);
    }

    /// <summary>
    ///
    /// </summary>
    public void OpenUICloseOthers(EnumUIType uiType, EPanelLayerType _layerType = EPanelLayerType.Panel, Action callback = null, params object[] uiObjParams)
    {
        OpenUI(true, uiType, _layerType, callback, uiObjParams);
    }

    /// <summary>
    /// 
    /// </summary>
    private void OpenUI(bool _isCloseOthers, EnumUIType _uiType, EPanelLayerType _layerType = EPanelLayerType.Panel, Action callback = null, params object[] _uiParams)
    {
        m_LoadingStartTime = Time.realtimeSinceStartup;
        if (dicLoadingUIs.ContainsKey(_uiType))
        {
            return;
        }
        if (_isCloseOthers)
        {
            CloseUIAll();
        }
        if (!m_DicOpenUIs.ContainsKey(_uiType))
        {
            string _name = UIPathDefines.GetPrefabNameByType(_uiType);
            //
            stackOpenUIs.Push(new UIInfoData(_uiType, _name, _layerType, callback, _uiParams));
        }
        if (stackOpenUIs.Count > 0)
        {
            CoroutineController.Instance.StartCoroutine(AsyncLoadData());
        }
    }

    private IEnumerator<int> AsyncLoadData()
    {
        UIInfoData _uiInfoData = null;

        if (stackOpenUIs != null && stackOpenUIs.Count > 0)
        {
            do
            {
                _uiInfoData = stackOpenUIs.Pop();
                #region BlackMask
                var isUseBlackMask = false;
                isUseBlackMask = _uiInfoData.LayerType == EPanelLayerType.Pop;
                //只有Pop可以设置黑遮罩
                if (isUseBlackMask)
                {
                    var sidx = m_BlackTrans.transform.GetSiblingIndex();
                    sidx -= 1;
                    if (sidx < 0)
                        sidx = 0;
                    m_BlackTrans.transform.SetSiblingIndex(sidx);
                    if (!m_BlackVisible)
                    {
                        m_BlackVisible = true;
                        m_BlackTrans.transform.localScale = Vector3.one;
                    }
                }
                else
                {
                    var exist = false;
                    //找到打开的UI
                    foreach (var item in m_DicOpenUIs.Values)
                    {
                        if (item != null && item.PanelLayerType == EPanelLayerType.Pop)
                        {
                            exist = true;
                            break;
                        }
                    }

                    if (m_BlackVisible && !exist)
                    {
                        m_BlackVisible = false;
                        m_BlackTrans.transform.localScale = Vector3.zero;
                    }
                }
                #endregion

                dicLoadingUIs.Add(_uiInfoData.UIType, true);
                ParamData pParamData = new ParamData();
                pParamData.objectParam = _uiInfoData;
                ResourceMgr.Instance.LoadResourceAsync(_uiInfoData.Name, TypeInts.GameObject, (resObject, data) =>
                {
                    Transform panelParent = null;
                    UIInfoData pUIInfoData = data.objectParam as UIInfoData;
                    GameObject pUIObject = resObject as GameObject;
                    BaseUI pBaseUI = pUIInfoData.ScriptType;
                    if (pBaseUI == null)
                        ClientLog.Instance.LogError($"没有实例化代码 {pUIInfoData.UIType} 要修改");
                    pBaseUI.UI_Object = pUIObject;
                    pBaseUI.PanelLayerType = pUIInfoData.LayerType;
                    pBaseUI.SetUIWhenOpening(pUIInfoData.UIParams);
                    switch (pUIInfoData.LayerType)
                    {
                        case EPanelLayerType.Bottom:
                            panelParent = GameObject.Find("UISystem/Canvas/uiRoot/Bottom").transform;
                            break;
                        case EPanelLayerType.Pop:
                            panelParent = GameObject.Find("UISystem/Canvas/uiRoot/Pop").transform;
                            break;
                        case EPanelLayerType.Panel:
                            panelParent = GameObject.Find("UISystem/Canvas/uiRoot/Panel").transform;
                            break;
                        case EPanelLayerType.Top:
                            panelParent = GameObject.Find("UISystem/Canvas/uiRoot/Top").transform;
                            break;
                        case EPanelLayerType.Guide:
                            panelParent = GameObject.Find("UISystem/Canvas/uiRoot/Guide").transform;
                            break;
                        case EPanelLayerType.Loading:
                            panelParent = GameObject.Find("UISystem/Canvas/uiRoot/Loading").transform;
                            break;
                        case EPanelLayerType.NetError:
                            panelParent = GameObject.Find("UISystem/Canvas/uiRoot/NetError").transform;
                            break;

                        default:
                            ClientLog.Instance.LogError("预设不存在,需要查看是否写上");
                            break;
                    }
                    pUIObject.transform.SetParent(panelParent, false);

                    m_DicOpenUIs.Add(pUIInfoData.UIType, pBaseUI);
                    dicLoadingUIs.Remove(pUIInfoData.UIType);

                    pBaseUI.SetUIWhenShowing();

                    if (pBaseUI.HasOpenTween() && pBaseUI.ViewHelper != null)
                    {
                        // 播完动画
                        pBaseUI.ViewHelper.Play(() =>
                        {
                            if (pBaseUI.State != EnumObjectState.Showing) return;
                            pBaseUI.SetUIWhenNormal(pUIInfoData.callback);
                            m_LoadingStartTime = Time.realtimeSinceStartup - m_LoadingStartTime;
                            ClientLog.Instance.Log($"OpenPanel 时间：{m_LoadingStartTime}");
                        });
                    }
                    else
                    {
                        pBaseUI.SetUIWhenNormal(pUIInfoData.callback);
                        m_LoadingStartTime = Time.realtimeSinceStartup - m_LoadingStartTime;
                        ClientLog.Instance.Log($"OpenPanel 时间：{m_LoadingStartTime}");
                    }
                }, null, null, pParamData);
            } while (stackOpenUIs.Count > 0);
        }
        yield return 0;
    }

    #endregion


    #region Close UI By EnumUIType
    /// <summary>
    /// �رս��档
    /// </summary>
    /// <param name="uiType">User interface type.</param>
    public void CloseUI(EnumUIType _uiType)
    {
        //GameObject _uiObj = null;
        BaseUI _uiObj = null;
        if (!m_DicOpenUIs.TryGetValue(_uiType, out _uiObj))
        {
            ClientLog.Instance.Log("m_DicOpenUIs TryGetValue Failure! _uiType :", _uiType.ToString());
            return;
        }
        CloseUI(_uiType, _uiObj);
    }

    /// <summary>
    /// Closes the U.
    /// </summary>
    /// <param name="_uiTypes">_ui types.</param>
    public void CloseUI(EnumUIType[] _uiTypes)
    {
        for (int i = 0; i < _uiTypes.Length; i++)
        {
            CloseUI(_uiTypes[i]);
        }
    }

    /// <summary>
    /// �ر�����UI����
    /// </summary>
    public void CloseUIAll()
    {
        if (m_DicOpenUIs != null)
        {
            List<EnumUIType> _keyList = new List<EnumUIType>(m_DicOpenUIs.Keys);
            foreach (EnumUIType _uiType in _keyList)
            {
                //GameObject _uiObj = m_DicOpenUIs[_uiType];
                BaseUI _uiObj = m_DicOpenUIs[_uiType];
                CloseUI(_uiType, _uiObj);
            }
            m_DicOpenUIs.Clear();
        }
    }

    private void CloseUI(EnumUIType _uiType, BaseUI _uiObj)
    {
        if (_uiObj == null)
        {
            m_DicOpenUIs.Remove(_uiType);
        }
        else
        {
            if (_uiObj != null)
            {
                _uiObj.SetUIWhenDisappearing();
            }

            if (_uiObj != null && _uiObj.ViewHelper != null && _uiObj.HasCloseTween())
            {
                _uiObj.ViewHelper.Rewind(() =>
                {
                    OnCloseUI(_uiObj, _uiType, _uiObj.UI_Object);
                });
            }
            else
            {
                OnCloseUI(_uiObj, _uiType, _uiObj.UI_Object);
            }
        }
    }

    private void OnCloseUI(BaseUI baseUI, EnumUIType _uiType, GameObject _uiObj)
    {
        DOTween.Kill(_uiObj, true);
        if (baseUI != null)
        {
            baseUI.StateChanged += CloseUIHandler;
            baseUI.Release();
        }
        else
        {
            ResourceMgr.Instance.UnLoadResource(_uiObj, TypeInts.GameObject);
            m_DicOpenUIs.Remove(_uiType);
        }

        var exist = false;
        int topSidx = 0;
        int tempSidx = -1;
        //找到打开的UI
        foreach (var item in m_DicOpenUIs.Values)
        {
            if (item != null && item.PanelLayerType == EPanelLayerType.Pop)
            {
                exist = true;
                tempSidx = m_BlackTrans.transform.GetSiblingIndex();
                if (tempSidx > topSidx)
                    topSidx = tempSidx;
            }
        }

        //只有Pop可以设置黑遮罩
        if (exist)
        {
            var bSidx = m_BlackTrans.transform.GetSiblingIndex();
            var sidx = bSidx > topSidx ? topSidx : topSidx - 1;
            if (sidx < 0)
                sidx = 0;
            m_BlackTrans.transform.SetSiblingIndex(sidx);
            if (!m_BlackVisible)
            {
                m_BlackVisible = true;
                m_BlackTrans.transform.localScale = Vector3.one;
            }
        }
        else
        {
            if (m_BlackVisible && !exist)
            {
                m_BlackVisible = false;
                m_BlackTrans.transform.localScale = Vector3.zero;
            }
        }

    }


    /// <summary>
    /// 
    /// </summary>
    private void CloseUIHandler(object _sender, EnumObjectState _newState, EnumObjectState _oldState)
    {
        if (_newState == EnumObjectState.Closing)
        {
            BaseUI pBaseUI = _sender as BaseUI;
            m_DicOpenUIs.Remove(pBaseUI.GetUIType());
            pBaseUI.StateChanged -= CloseUIHandler;
        }
    }
    #endregion

    public void Update()
    {

    }

    public void LateUpdate()
    {

    }

    public override void OnRelease()
    {
        if (m_DicOpenUIs != null && m_DicOpenUIs.Values != null)
        {
            foreach (var item in m_DicOpenUIs.Values)
            {
                if (item != null)
                    item.Release();
            }
            m_DicOpenUIs.Clear();
        }
        ClearMask();
        EventMgr.Instance.UnRegisterEvent(EEventType.UI_ADD_MASK, OnAddMask);
        EventMgr.Instance.UnRegisterEvent(EEventType.UI_REMOVE_MASK, OnRemoveMask);
    }

    /// <summary>
    /// 清除Mask
    /// </summary>
    public void ClearMask()
    {
        if (m_MaskLayer != null)
            m_MaskLayer.raycastTarget = false;
    }

    /// <summary>
    /// 遮罩显示
    /// </summary>
    public void OnAddMask(object param)
    {
        EMaskNameType maskKey = (EMaskNameType)param;
        if (m_MasklayerList != null)
        {
            if (m_MasklayerList.Contains(maskKey)) return;

            m_MasklayerList.Add(maskKey);

            if (m_MaskLayer != null)
                m_MaskLayer.raycastTarget = true;
        }
    }

    /// <summary>
    /// 遮罩取消
    /// </summary>
    /// <param name="str"></param>
    public void OnRemoveMask(object param)
    {
        var maskKey = (EMaskNameType)param;
        if (m_MasklayerList != null)
        {
            if (m_MasklayerList.Contains(maskKey))
                m_MasklayerList.Remove(maskKey);

            if (m_MaskLayer != null && m_MasklayerList.Count <= 0)
                m_MaskLayer.raycastTarget = false;
        }
    }
}

