/// <summary>
/// ScreenAdapter.cs
/// Desc:   
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ScreenAdapter : MonoBehaviour
{
    public ScreenAdapterManager.EAnchorType anchorType;

    protected RectTransform m_RectTr;
    private void Awake()
    {
        m_RectTr = transform as RectTransform;
    }

    [ContextMenu("UpdateForScreen")]
    protected virtual void UpdateForScreen()
    {
        Vector2 anchoredPosition = CalcAnchoredPosition();
        m_RectTr.anchoredPosition = anchoredPosition;
    }

    public Vector2 CalcFreePosition(Vector2 freePos)
    {
        Vector2 anchoredPosition = freePos;
        var sam = ScreenAdapterManager.Instance;
        if (sam != null)
        {
            switch (anchorType)
            {
                case ScreenAdapterManager.EAnchorType.Left:
                    anchoredPosition = new Vector2(sam.LeftEdge, freePos.y);
                    break;
                case ScreenAdapterManager.EAnchorType.Right:
                    anchoredPosition = new Vector2(-sam.RightEdge, freePos.y);
                    break;
                case ScreenAdapterManager.EAnchorType.Top:
                    anchoredPosition = new Vector2(freePos.x, -sam.TopEdge);
                    break;
                case ScreenAdapterManager.EAnchorType.Bottom:
                    anchoredPosition = new Vector2(freePos.x, sam.BottomEdge);
                    break;
            }
        }
        return anchoredPosition;
    }

    public Vector2 CalcAnchoredPosition()
    {
        Vector2 anchoredPosition = m_RectTr.anchoredPosition;
        var sam = ScreenAdapterManager.Instance;
        if (sam != null)
        {
            switch (anchorType)
            {
                case ScreenAdapterManager.EAnchorType.Left:
                    anchoredPosition = new Vector2(sam.LeftEdge, m_RectTr.anchoredPosition.y);
                    break;
                case ScreenAdapterManager.EAnchorType.Right:
                    anchoredPosition = new Vector2(-sam.RightEdge, m_RectTr.anchoredPosition.y);
                    break;
                case ScreenAdapterManager.EAnchorType.Top:
                    anchoredPosition = new Vector2(m_RectTr.anchoredPosition.x, -sam.TopEdge);
                    break;
                case ScreenAdapterManager.EAnchorType.Bottom:
                    anchoredPosition = new Vector2(m_RectTr.anchoredPosition.x, sam.BottomEdge);
                    break;
            }
        }
        return anchoredPosition;
    }

    private void OnEnable()
    {
        ScreenAdapterManager.Instance.onOrientationChanged += UpdateForScreen;
        UpdateForScreen();
    }

    private void OnDisable()
    {
        ScreenAdapterManager.Instance.onOrientationChanged -= UpdateForScreen;
    }
}
