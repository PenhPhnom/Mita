/// <summary>
/// ScreenAdapter.cs
/// Desc:   
/// </summary>

using UnityEngine;
public class StayInSafeArea : MonoBehaviour
{
    public RectTransform refRectTransform;
    public Rect safeArea;
    // public Rect safeArea2;
    //public rest
    RectTransform m_RectTr;
    ScreenAdapterManager sam;
    Vector3[] localCorners = new Vector3[4];
    private void Awake()
    {
        m_RectTr = transform as RectTransform;

    }

    [ContextMenu("UpdateForScreen")]
    public void UpdateForScreen()
    {
        if (sam == null)
        {
            sam = ScreenAdapterManager.Instance;
        }

        m_RectTr.GetLocalCorners(localCorners);

        //safeArea.xMin = sam.LeftEdge - m_RectTr.anchorMin.x * sam.ScreenWidth;
        //safeArea.xMax = m_RectTr.anchorMax.x * sam.ScreenWidth - sam.RightEdge;
        //safeArea.yMin = sam.BottomEdge - m_RectTr.anchorMin.y * sam.ScreenHeight;
        //safeArea.yMax = m_RectTr.anchorMax.y * sam.ScreenHeight - sam.TopEdge;

        safeArea.xMin = sam.LeftEdge - sam.HalfUIRefWidth;
        safeArea.xMax = sam.HalfUIRefWidth - sam.RightEdge;
        safeArea.yMin = sam.BottomEdge - sam.HalfUIRefHeight;
        safeArea.yMax = sam.HalfUIRefHeight - sam.TopEdge;

        safeArea.xMin = safeArea.xMin - localCorners[0].x;
        safeArea.xMax = safeArea.xMax - localCorners[2].x;
        safeArea.yMin = safeArea.yMin - localCorners[0].y;
        safeArea.yMax = safeArea.yMax - localCorners[2].y;

        float w = safeArea.xMax - safeArea.xMin;
        float h = safeArea.yMax - safeArea.yMin;

        var matrix = refRectTransform.worldToLocalMatrix * m_RectTr.parent.localToWorldMatrix;
        var inversMatrix = m_RectTr.parent.worldToLocalMatrix * refRectTransform.localToWorldMatrix;

        Vector3 position = matrix.MultiplyPoint(new Vector3(m_RectTr.anchoredPosition.x, m_RectTr.anchoredPosition.y));
        position = Clamp(position, safeArea);
        position = inversMatrix.MultiplyPoint(position);
        m_RectTr.anchoredPosition = position;

    }

    private void LateUpdate()
    {
        UpdateForScreen();
    }

    private void OnEnable()
    {
        sam = ScreenAdapterManager.Instance;
        ScreenAdapterManager.Instance.onOrientationChanged += UpdateForScreen;
        UpdateForScreen();
    }

    private void OnDisable()
    {
        sam = null;
        ScreenAdapterManager.Instance.onOrientationChanged -= UpdateForScreen;
    }

    private Vector2 Clamp(Vector2 position, Rect safeArea)
    {
        position.x = Mathf.Clamp(position.x, safeArea.xMin, safeArea.xMax);
        position.y = Mathf.Clamp(position.y, safeArea.yMin, safeArea.yMax);
        return position;
    }
}
