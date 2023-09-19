using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TouchHandle
{
    private event OnTouchEventHandle eventHandle = null;
    private object[] handleParams;

    public object[] GetParams()
    {
        return handleParams;
    }

    public TouchHandle(OnTouchEventHandle _handle, params object[] _params)
    {
        SetHandle(_handle, _params);
    }

    public TouchHandle()
    {

    }

    public void SetHandle(OnTouchEventHandle _handle, params object[] _params)
    {
        DestoryHandle();
        eventHandle += _handle;
        handleParams = _params;
    }

    public void CallEventHandle(GameObject _lsitener, object _args)
    {
        if (null != eventHandle)
        {
            eventHandle(_lsitener, _args, handleParams);
        }
    }

    public void DestoryHandle()
    {
        if (null != eventHandle)
        {
            eventHandle -= eventHandle;
            eventHandle = null;
        }
    }
}
[SerializeField]
public class UIEventListener : UnityEngine.EventSystems.EventTrigger
{
    public delegate void VoidDelegate();
    public delegate void BoolDelegate(bool b);
    public delegate void PointerEventDataDelegate(PointerEventData eventData);

    public TouchHandle onClickHandle;
    public TouchHandle onDoubleClickHandle;
    public TouchHandle onDownHandle;
    public TouchHandle onEnterHandle;
    public TouchHandle onExitHandle;
    public TouchHandle onUpHandle;
    public TouchHandle onSelectHandle;
    public TouchHandle onUpdateSelectHandle;
    public TouchHandle onDeSelectHandle;
    public TouchHandle onDragHandle;
    public TouchHandle onDragEndHandle;
    public TouchHandle onDropHandle;
    public TouchHandle onScrollHandle;
    public TouchHandle onMoveHandle;
    public TouchHandle onLongPressHandle;

    public VoidDelegate onClick;
    public VoidDelegate onDown;
    public VoidDelegate onEnter;
    public VoidDelegate onExit;
    public VoidDelegate onUp;
    public VoidDelegate onSelect;

    public VoidDelegate onStartDrag;
    public VoidDelegate onDrag;
    public VoidDelegate onEndDrag;
    public BoolDelegate onLongPress;

    public PointerEventDataDelegate onDragBegin;
    public PointerEventDataDelegate onDragEnd;
    public PointerEventDataDelegate onDragging;
    public PointerEventDataDelegate onPointUp;
    public PointerEventDataDelegate onPointDown;
    public PointerEventDataDelegate onScroll;

    private bool isLongPress = false;                                                                // 长按标志位
    private float longPressTime = 0;                                                                 // 判定长按计时
    public float LimitLongPressTime = 0.5f;                                                          // 判定长按起作用间隔
    public bool DisableLongPress = false;                                                            // 是否禁用长按（默认开启）
    private static bool singleInput = false;                                                         // 单一控件响应标志
    public bool ignoreSingleInput = false;                                                           // 忽略单一控件响应（默认不忽略）
    private bool selfInput = false;                                                                  // 自身响应输入
    private List<int> downPoints = new List<int>();                                                  // 会有多点触摸 改成记点击Id
    private Dictionary<int, PointerEventData> dragPoints = new Dictionary<int, PointerEventData>();  // 会有多点触摸 改成记点击Id
    public GameObject passObject;                                                                    // 透传事件对象

    private float clickIntervalTimer = 0f;
    private float clickInterval = 0f;

    private float m_LastIsDownTime;
    private float m_Delay = 0.2f;

    // 事件响应顺序  OnPointerDown OnBeginDrag OnPointerUp OnPointerClick OnEndDrag
    static public UIEventListener Get(GameObject go)
    {
        return go.AddMissingComponent<UIEventListener>();
    }

    public void SetClickInteralTime(float interval)
    {
        clickIntervalTimer = interval;
        clickInterval = interval;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (isLongPress && DisableLongPress == false) return;
        if (clickInterval > 0)
            return;
        else
        {
            clickInterval = clickIntervalTimer;
            onClickHandle?.CallEventHandle(gameObject, eventData);
            onClick?.Invoke();
            PassEvent(eventData, ExecuteEvents.pointerClickHandler);
            //EventMgr.Instance.FireEvent(EEventType.UI_BTN_CLICK, onClickHandle.GetParams());
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (!ignoreSingleInput)
        {
            if (singleInput && !selfInput) return;
            else { singleInput = true; }
        }
        if (downPoints.Contains(eventData.pointerId))
            return;
        else
            downPoints.Add(eventData.pointerId);
        selfInput = true;
        if (onDoubleClickHandle != null)
        {
            if (Time.time - m_LastIsDownTime <= m_Delay)
            {
                onDoubleClickHandle.CallEventHandle(gameObject, eventData);
            }
        }
        onDown?.Invoke();
        onPointDown?.Invoke(eventData);
        PassEvent(eventData, ExecuteEvents.pointerDownHandler);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (downPoints.Contains(eventData.pointerId))
            downPoints.Remove(eventData.pointerId);
        else
            return;
        if (downPoints.Count == 0)
        {
            if (!ignoreSingleInput)
            {
                singleInput = false;
            }
            selfInput = false;
        }
        if (onDoubleClickHandle != null)
            m_LastIsDownTime = Time.time;

        onUpHandle?.CallEventHandle(gameObject, eventData);
        onUp?.Invoke();
        onPointUp?.Invoke(eventData);
        PassEvent(eventData, ExecuteEvents.pointerUpHandler);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        onEnterHandle?.CallEventHandle(gameObject, eventData);
        onEnter?.Invoke();
        PassEvent(eventData, ExecuteEvents.pointerEnterHandler);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        onExitHandle?.CallEventHandle(gameObject, eventData);
        onExit?.Invoke();
        PassEvent(eventData, ExecuteEvents.pointerExitHandler);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        onSelectHandle?.CallEventHandle(gameObject, eventData);
        onSelect?.Invoke();
    }

    public override void OnUpdateSelected(BaseEventData eventData)
    {
        onUpdateSelectHandle?.CallEventHandle(gameObject, eventData);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        onDeSelectHandle?.CallEventHandle(gameObject, eventData);
    }

    public override void OnScroll(PointerEventData eventData)
    {
        onScrollHandle?.CallEventHandle(gameObject, eventData);
        onScroll?.Invoke(eventData);
    }

    public override void OnMove(AxisEventData eventData)
    {
        onMoveHandle?.CallEventHandle(gameObject, eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (dragPoints.ContainsKey(eventData.pointerId) == false)
            return;
        onDragHandle?.CallEventHandle(gameObject, eventData);
        onDrag?.Invoke();
        onDragging?.Invoke(eventData);
        PassEvent(eventData, ExecuteEvents.dragHandler);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (dragPoints.ContainsKey(eventData.pointerId))
            dragPoints.Remove(eventData.pointerId);
        else
            return;
        if (dragPoints.Count == 0)
        {
            if (!ignoreSingleInput)
            {
                singleInput = false;
            }
            selfInput = false;
        }
        onDragEndHandle?.CallEventHandle(gameObject, eventData);
        onEndDrag?.Invoke();
        onDragEnd?.Invoke(eventData);
        PassEvent(eventData, ExecuteEvents.endDragHandler);
    }

    public override void OnDrop(PointerEventData eventData)
    {
        onDropHandle?.CallEventHandle(gameObject, eventData);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (downPoints.Contains(eventData.pointerId) == false)
            return;
        if (!ignoreSingleInput)
        {
            if (singleInput && !selfInput) return;
            else { singleInput = true; }
        }
        if (dragPoints.ContainsKey(eventData.pointerId))
            return;
        else
            dragPoints.Add(eventData.pointerId, eventData);
        selfInput = true;
        onStartDrag?.Invoke();
        onDragBegin?.Invoke(eventData);
        PassEvent(eventData, ExecuteEvents.beginDragHandler);
    }

    private List<PointerEventData> eventDatas = new List<PointerEventData>();
    private void CheckPointerEvent(bool check = true)
    {
        eventDatas.Clear();
        foreach (var item in dragPoints)
        {
            if ((item.Value.pointerDrag == null && item.Value.dragging == false) || check == false)
                eventDatas.Add(item.Value);
        }
        for (int i = 0; i < eventDatas.Count; i++)
            OnEndDrag(eventDatas[i]);
        if (eventDatas.Count > 0 || check == false)
        {
            if (!ignoreSingleInput)
                if (singleInput && selfInput)
                    singleInput = false;
            selfInput = false;
            downPoints.Clear();
            dragPoints.Clear();
        }
    }

    private void CheckLongPress()
    {
        if (DisableLongPress)
            return;
        if (downPoints.Count > 0 && dragPoints.Count == 0 && selfInput)
        {
            if (!isLongPress)
            {
                longPressTime += Time.deltaTime;
                if (longPressTime > LimitLongPressTime) // 长按秒数
                {
                    isLongPress = true;
                    onLongPressHandle?.CallEventHandle(gameObject, isLongPress);
                    onLongPress?.Invoke(isLongPress);
                }
            }
        }
        else
        {
            if (isLongPress)
            {
                isLongPress = false;
                onLongPressHandle?.CallEventHandle(gameObject, isLongPress);
                onLongPress?.Invoke(isLongPress);
            }
            longPressTime = 0;
        }
    }

    private void Update()
    {
        if (EventSystem.current.isFocused == false && selfInput)
            CheckPointerEvent(false);
        CheckPointerEvent();
        CheckLongPress();
        if (clickInterval > 0)
            clickInterval -= Time.deltaTime;
    }

    private void OnDisable()
    {
        if (!ignoreSingleInput)
            if (singleInput && selfInput)
                singleInput = false;
        selfInput = false;
        downPoints.Clear();
        dragPoints.Clear();
    }

    // 把事件透下去
    public void PassEvent<T>(PointerEventData data, ExecuteEvents.EventFunction<T> function)
        where T : IEventSystemHandler
    {
        if (passObject != null)
            ExecuteEvents.Execute(passObject, data, function);
    }

    public void SetEventHandle(EnumTouchEventType _type, OnTouchEventHandle _handle, params object[] _params)
    {
        switch (_type)
        {
            case EnumTouchEventType.OnClick:
                if (null == onClickHandle)
                {
                    onClickHandle = new TouchHandle();
                }
                onClickHandle.SetHandle(_handle, _params);
                break;
            case EnumTouchEventType.OnDoubleClick:
                if (null == onDoubleClickHandle)
                {
                    onDoubleClickHandle = new TouchHandle();
                }
                onDoubleClickHandle.SetHandle(_handle, _params);
                break;
            case EnumTouchEventType.OnDown:
                if (onDownHandle == null)
                {
                    onDownHandle = new TouchHandle();
                }
                onDownHandle.SetHandle(_handle, _params);
                break;
            case EnumTouchEventType.OnUp:
                if (onUpHandle == null)
                {
                    onUpHandle = new TouchHandle();
                }
                onUpHandle.SetHandle(_handle, _params);
                break;
            case EnumTouchEventType.OnEnter:
                if (onEnterHandle == null)
                {
                    onEnterHandle = new TouchHandle();
                }
                onEnterHandle.SetHandle(_handle, _params);
                break;
            case EnumTouchEventType.OnExit:
                if (onExitHandle == null)
                {
                    onExitHandle = new TouchHandle();
                }
                onExitHandle.SetHandle(_handle, _params);
                break;
            case EnumTouchEventType.OnDrag:
                if (onDragHandle == null)
                {
                    onDragHandle = new TouchHandle();
                }
                onDragHandle.SetHandle(_handle, _params);
                break;
            case EnumTouchEventType.OnDrop:
                if (onDropHandle == null)
                {
                    onDropHandle = new TouchHandle();
                }
                onDropHandle.SetHandle(_handle, _params);
                break;

            case EnumTouchEventType.OnDragEnd:
                if (onDragEndHandle == null)
                {
                    onDragEndHandle = new TouchHandle();
                }
                onDragEndHandle.SetHandle(_handle, _params);
                break;
            case EnumTouchEventType.OnSelect:
                if (onSelectHandle == null)
                {
                    onSelectHandle = new TouchHandle();
                }
                onSelectHandle.SetHandle(_handle, _params);
                break;
            case EnumTouchEventType.OnUpdateSelect:
                if (onUpdateSelectHandle == null)
                {
                    onUpdateSelectHandle = new TouchHandle();
                }
                onUpdateSelectHandle.SetHandle(_handle, _params);
                break;
            case EnumTouchEventType.OnDeSelect:
                if (onDeSelectHandle == null)
                {
                    onDeSelectHandle = new TouchHandle();
                }
                onDeSelectHandle.SetHandle(_handle, _params);
                break;
            case EnumTouchEventType.OnScroll:
                if (onScrollHandle == null)
                {
                    onScrollHandle = new TouchHandle();
                }
                onScrollHandle.SetHandle(_handle, _params);
                break;
            case EnumTouchEventType.OnMove:
                if (onMoveHandle == null)
                {
                    onMoveHandle = new TouchHandle();
                }
                onMoveHandle.SetHandle(_handle, _params);
                break;
            case EnumTouchEventType.OnLongPress:
                if (onLongPressHandle == null)
                {
                    onLongPressHandle = new TouchHandle();
                }
                onLongPressHandle.SetHandle(_handle, _params);
                break;
        }
    }

    private void RemoveAllHandle()
    {
        RemoveHandle(onClickHandle);
        RemoveHandle(onDoubleClickHandle);
        RemoveHandle(onDownHandle);
        RemoveHandle(onEnterHandle);
        RemoveHandle(onExitHandle);
        RemoveHandle(onUpHandle);
        RemoveHandle(onDropHandle);
        RemoveHandle(onDragHandle);
        RemoveHandle(onDragEndHandle);
        RemoveHandle(onScrollHandle);
        RemoveHandle(onMoveHandle);
        RemoveHandle(onUpdateSelectHandle);
        RemoveHandle(onSelectHandle);
        RemoveHandle(onDeSelectHandle);
        RemoveHandle(onLongPressHandle);
    }

    private void RemoveHandle(TouchHandle _handle)
    {
        if (null != _handle)
        {
            _handle.DestoryHandle();
            _handle = null;
        }
    }

    private void OnDestroy()
    {
        if (onClick != null) onClick = null;
        if (onDown != null) onDown = null;
        if (onEnter != null) onEnter = null;
        if (onExit != null) onExit = null;
        if (onUp != null) onUp = null;
        if (onSelect != null) onSelect = null;
        if (onScroll != null) onScroll = null;

        if (onStartDrag != null) onStartDrag = null;
        if (onDrag != null) onDrag = null;
        if (onEndDrag != null) onEndDrag = null;
        if (onLongPress != null) onLongPress = null;
        if (onDragBegin != null) onDragBegin = null;
        if (onDragEnd != null) onDragEnd = null;
        if (onDragging != null) onDragging = null;
        if (onPointUp != null) onPointUp = null;
        if (onPointDown != null) onPointDown = null;
        RemoveAllHandle();
    }
}