using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 按照设计 是不应该继承 MonoBehaviour 暂且先走依赖
/// </summary>
public abstract class BaseUI
{
    public delegate void StateChangedEvent(object sender, EnumObjectState newState, EnumObjectState oldState);

    #region Cache gameObject & transfrom

    private Transform m_UITransform;
    /// <summary>
    /// Gets the cached transform.
    /// </summary>
    /// <value>The cached transform.</value>
    public Transform UI_Transform
    {
        get
        {
            if (!m_UITransform)
            {
                if (UI_Object != null)
                    m_UITransform = UI_Object.transform;
            }
            return m_UITransform;
        }
    }

    protected GameObject m_CachedGameObject;
    /// <summary>
    /// Gets the cached game object.
    /// </summary>
    /// <value>The cached game object.</value>
    public GameObject UI_Object;
    public UIViewHelper ViewHelper;
    public EPanelLayerType PanelLayerType;
    #endregion

    #region UIType & EnumObjectState
    /// <summary>
    /// The state.
    /// </summary>
    protected EnumObjectState state = EnumObjectState.None;

    /// <summary>
    /// Occurs when state changed.
    /// </summary>
    public event StateChangedEvent StateChanged;

    /// <summary>
    /// Gets or sets the state.
    /// </summary>
    /// <value>The state.</value>
    public EnumObjectState State
    {
        protected set
        {
            if (value != state)
            {
                EnumObjectState oldState = state;
                state = value;
                if (null != StateChanged)
                {
                    StateChanged(this, state, oldState);
                }
                EventMgr.Instance.FireEvent(EEventType.UI_STATECHANGED);
                ClientLog.Instance.Log($"UI state 状态：{oldState} ====> {state} ");
            }
        }
        get { return state; }
    }

    /// <summary>
    /// Gets the type of the user interface.
    /// </summary>
    /// <returns>The user interface type.</returns>
    public abstract EnumUIType GetUIType();

    #endregion

    /// <summary>
    /// UI层级置顶
    /// </summary>
    protected virtual void SetDepthToTop() { }

    // Update is called once per frame
    public void Update()
    {
        if (EnumObjectState.Ready == state)
        {
            OnUpdate(Time.deltaTime);
        }
    }

    public void LateUpdate()
    {
        if (EnumObjectState.Ready == state)
        {
            OnLateUpdate();
        }
    }

    /// <summary>
    /// Release this instance.
    /// </summary>
    public void Release()
    {
        State = EnumObjectState.Closing;
        ResourceMgr.Instance.UnLoadResource(UI_Object, TypeInts.GameObject);
        OnRelease();
    }

    private void SetUI(params object[] uiParams)
    {
        State = EnumObjectState.Loading;
        OnSetUIparam(uiParams);
    }

    public void SetUIWhenOpening(params object[] uiParams)
    {
        State = EnumObjectState.Initial;
        if (UI_Object != null)
            ViewHelper = UI_Object.transform.GetComponent<UIViewHelper>();
        OnAwake(UI_Object);
        SetUI(uiParams);
        OnStart();
        CoroutineController.Instance.StartCoroutine(AsyncOnLoadData());
        OnPlayOpenUIAudio();
    }

    private IEnumerator AsyncOnLoadData()
    {
        yield return new WaitForSeconds(0);
        if (State == EnumObjectState.Loading)
        {
            OnResourcesUI();
            State = EnumObjectState.Ready;
        }
    }

    public void SetUIWhenShowing()
    {
        State = EnumObjectState.Showing;
        OnUIWhenShowing();
    }

    public void SetUIWhenNormal(Action callback)
    {
        State = EnumObjectState.Normal;
        OnUIWhenNormal();
        callback?.Invoke();
        EventMgr.Instance.FireEvent(EEventType.UI_SHOW_PANEL, GetUIType());
    }

    public void SetUIWhenDisappearing()
    {
        State = EnumObjectState.Disappearing;
        OnUIWhenDisappearing();
        OnPlayCloseUIAudio();
    }

    /// <summary>
    /// TODO 还没想好怎么搞
    /// </summary>
    /// <param name="uiObj"></param>
    public void DoDestroyDelayUI(GameObject uiObj)
    {
        //OnDestroyUIObject();
        //if (uiObj != null)
        //    DestroyImmediate(uiObj);

    }

    /// <summary>
    /// 实例化Gameobject出来
    /// </summary>
    protected abstract void OnAwake(GameObject obj);
    protected virtual void OnSetUIparam(params object[] uiParams) { }
    protected virtual void OnStart() { }
    protected virtual void OnResourcesUI() { }
    /// <summary>
    /// 目前没有调用 想法是用 MainLoopScript.AddUpdateHandler(EUpdatePriority.Realtime, XXXXX)注册的方法
    /// </summary>
    /// <param name="deltaTime"></param>
    protected virtual void OnUpdate(float deltaTime) { }
    protected virtual void OnLateUpdate() { }
    public virtual void OnUIWhenShowing() { }
    public virtual void OnUIWhenNormal() { }
    public virtual void OnUIWhenDisappearing() { }
    public virtual bool HasOpenTween() { return false; }
    public virtual bool HasCloseTween() { return false; }
    public virtual bool IsFullScreen() { return false; }
    /// <summary>
    /// UI淡出动画结束或者无淡入动画，将要执行删除UIObject时调用,用在一些特殊场合。
    /// </summary>
    protected virtual void OnDestroyUIObject() { }
    /// <summary>
    /// 播放打开界面音乐
    /// </summary>
    protected virtual void OnPlayOpenUIAudio() { }

    /// <summary>
    /// 播放关闭界面音乐
    /// </summary>
    protected virtual void OnPlayCloseUIAudio() { }

    public abstract void OnRelease();

    public virtual GameObject GetComponentByString(string name) { return null; }
}