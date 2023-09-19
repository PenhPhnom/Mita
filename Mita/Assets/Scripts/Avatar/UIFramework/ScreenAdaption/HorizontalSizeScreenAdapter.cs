/// <summary>
/// ScreenAdapter.cs
/// Desc:   
/// </summary>

using UnityEngine;
public class HorizontalSizeScreenAdapter : ScreenAdapter
{

    [ContextMenu("UpdateForScreen")]
    protected override void UpdateForScreen()
    {
        var sam = ScreenAdapterManager.Instance;
        var sizeDeltaX = -(sam.LeftEdge + sam.RightEdge);
        var offsetX = sam.LeftEdge - sam.RightEdge;
        offsetX = offsetX * 0.5f;

        m_RectTr.sizeDelta = new Vector2(sizeDeltaX, m_RectTr.sizeDelta.y);
        m_RectTr.anchoredPosition = new Vector2(offsetX, m_RectTr.anchoredPosition.y);

    }

}
