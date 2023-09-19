/// <summary>
/// ScreenAdapterManager.cs
/// Desc:   
/// </summary>

// #define ENABLE_DEBUG

using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class ScreenAdapterManager : MonoBehaviour
{
    public enum EAnchorType
    {
        Left,
        Right,
        Top,
        Bottom
    }

    public event System.Action onOrientationChanged;
    public event System.Action onOrientationChangedP2;
    public Action<ScreenAdapterManager> checkEdgeHandler;
    [NonSerialized] public ScreenOrientation mOrientation;

    public static ScreenAdapterManager Instance { get; private set; }

    [NonSerialized, ShowInInspector] public float LeftEdge;
    [NonSerialized, ShowInInspector] public float RightEdge;
    [NonSerialized, ShowInInspector] public float TopEdge;
    [NonSerialized, ShowInInspector] public float BottomEdge;

    public RectTransform refRect;
    [NonSerialized] public Rect m_SafeArea;

    public int delayUpdateParamsCount = 1;

    [ShowInInspector]
    int uiRefWidth;
    [ShowInInspector]
    int uiRefHeight;

    float halfUIRefWidth;
    float halfUIRefHegiht;

    float screenRatio;

    [ShowInInspector]
    float screenRefRatio;

    bool m_isOverride = false;

    public static ScreenOrientation screenOrientation
    {
        get
        {
#if UNITY_EDITOR
            // Screen.orientation EDITOR下一直是ScreenOrientation.Portrait
            return ((float)Screen.width / Screen.height) > 1f ? ScreenOrientation.LandscapeLeft : ScreenOrientation.Portrait;
#else
                return Screen.orientation;
#endif
        }
    }

    public int UIRefWidth { get => uiRefWidth; }
    public int UIRefHeight { get => uiRefHeight; }
    public float HalfUIRefWidth { get => halfUIRefWidth; }
    public float HalfUIRefHeight { get => halfUIRefHegiht; }
    public float ScreenRatio { get => screenRatio; }

    public float ScreenRefRatio { get => screenRefRatio; }

    private void Awake()
    {
        Instance = this;
        UpdateScreenParams();
        UpdateOrientation(screenOrientation);
    }

    void Update()
    {
#if UNITY_EDITOR
        UpdateScreenParams();
#endif

        if (delayUpdateParamsCount == 0)
        {
            UpdateScreenParams();
        }
        else
        {
            delayUpdateParamsCount--;
        }

        if (m_isOverride)
            return;

        var curOrient = screenOrientation;
        if (mOrientation != curOrient)
        {
            UpdateOrientation(curOrient);

            if (onOrientationChanged != null)
                onOrientationChanged();

            if (onOrientationChangedP2 != null)
                onOrientationChangedP2();
        }
    }
    void UpdateScreenParams()
    {
        m_SafeArea = Screen.safeArea;

        uiRefWidth = (int)refRect.sizeDelta.x;
        uiRefHeight = (int)refRect.sizeDelta.y;

        halfUIRefWidth = 0.5f * uiRefWidth;
        halfUIRefHegiht = 0.5f * uiRefHeight;

        screenRatio = halfUIRefWidth / halfUIRefHegiht;
        screenRefRatio = (float)Screen.height / (float)uiRefHeight;

    }
    private void UpdateOrientation(ScreenOrientation curOrient)
    {
        mOrientation = screenOrientation;
        if (m_SafeArea != Screen.safeArea ||
            m_SafeArea.size.x * m_SafeArea.size.y != Screen.width * Screen.height)
        {
            m_SafeArea = Screen.safeArea;
            ChageEdge();
        }
    }

    private void ChageEdge()
    {
        if (checkEdgeHandler != null)
        {
            checkEdgeHandler(this);
        }
        else
        {
            LeftEdge = (mOrientation == ScreenOrientation.LandscapeLeft || mOrientation == ScreenOrientation.LandscapeRight) ? 50 : 0;
            RightEdge = (mOrientation == ScreenOrientation.LandscapeRight || mOrientation == ScreenOrientation.LandscapeLeft) ? 50 : 0;
            TopEdge = (mOrientation == ScreenOrientation.Portrait || mOrientation == ScreenOrientation.PortraitUpsideDown) ? 50 : 0;
            BottomEdge = (mOrientation == ScreenOrientation.PortraitUpsideDown || mOrientation == ScreenOrientation.Portrait) ? 50 : 0;
        }
    }
#if ENABLE_DEBUG
        public bool showInfo;
        private void OnGUI()
        {
            if (!showInfo)
                return;

            GUILayout.Label("m_SafeArea:" + m_SafeArea);
            GUILayout.Label("mCurOrientation:" + mOrientation);
            GUILayout.Label("LeftEdge:" + LeftEdge);
            GUILayout.Label("RightEdge:" + RightEdge);
            GUILayout.Label("TopEdge:" + TopEdge);
            GUILayout.Label("BottomEdge:" + BottomEdge);
        }
#endif
    [Button]
    public void OverrideOrientation(ScreenOrientation newOrientation)
    {
        this.m_isOverride = newOrientation == Screen.orientation;

        mOrientation = newOrientation;
        ChageEdge();

        LeftEdge = (mOrientation == ScreenOrientation.LandscapeLeft || mOrientation == ScreenOrientation.LandscapeRight) ? 50 : 0;
        RightEdge = (mOrientation == ScreenOrientation.LandscapeRight || mOrientation == ScreenOrientation.LandscapeLeft) ? 50 : 0;
        TopEdge = (mOrientation == ScreenOrientation.Portrait || mOrientation == ScreenOrientation.PortraitUpsideDown) ? 50 : 0;
        BottomEdge = (mOrientation == ScreenOrientation.PortraitUpsideDown || mOrientation == ScreenOrientation.Portrait) ? 50 : 0;

        if (onOrientationChanged != null)
            onOrientationChanged();

        if (onOrientationChangedP2 != null)
            onOrientationChangedP2();
    }
}
