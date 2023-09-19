using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public enum TargetType
{
    UI = 0,
    World = 1,
}

[UnityEngine.ExecuteInEditMode]
public class GuideFromToPointer : MonoBehaviour
{
    public GameObject OriginTarget;
    public TargetType OriginType;
    private System.Action<GameObject, RectTransform> m_OriginCalculator;

    public GameObject destinationTarget;
    public TargetType destinationType;

    private System.Action<GameObject, RectTransform> m_DestCalculator;

    public RectTransform ScreenOrigin;
    public RectTransform ScreenDest;

    private Camera m_UICamera;
    public Camera uiCamera
    {
        get
        {
            if (m_UICamera == null)
            {
                var _canvas = transform.GetComponentInParent<Canvas>();
                m_UICamera = _canvas.worldCamera;
            }

            return m_UICamera;

        }
    }

    public Camera WorldCamera;

    public void UpdateUIAnchor(GameObject src, RectTransform output)
    {
        Vector2 resultPos;
        var srcRect = src.transform as RectTransform;
        var selfRectTr = transform as RectTransform;

        var screenPos = uiCamera.WorldToScreenPoint(srcRect.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(selfRectTr, screenPos, uiCamera, out resultPos);

        output.transform.localPosition = resultPos;
    }

    public void UpdateWorldAnchor(GameObject src, RectTransform output)
    {
        Vector2 resultPos;
        var selfRectTr = transform as RectTransform;

        var screenPos = WorldCamera.WorldToScreenPoint(src.transform.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(selfRectTr, screenPos, uiCamera, out resultPos);

        output.transform.localPosition = resultPos;
    }

    System.Action<GameObject, RectTransform> GetCalculator(TargetType type)
    {
        switch (type)
        {
            case TargetType.UI:
                return UpdateUIAnchor;
            case TargetType.World:
                return UpdateWorldAnchor;
            default:
                Debug.LogError($"未处理的类型[{type}]");
                return null;
        }
    }

    public void SetPointerParam(Camera _worldCamera, GameObject _origin, TargetType _originType, GameObject _dest, TargetType _destType)
    {
        OriginTarget = _origin;
        OriginType = _originType;
        m_OriginCalculator = GetCalculator(_originType);

        destinationTarget = _dest;
        destinationType = _destType;
        m_DestCalculator = GetCalculator(_destType);

        WorldCamera = _worldCamera;

        Update();
    }

    public void CleanUp()
    {
        OriginTarget = null;
        m_OriginCalculator = null;
        destinationTarget = null;
        m_DestCalculator = null;
    }

    void Update()
    {
        if (m_OriginCalculator != null)
        {
            m_OriginCalculator(OriginTarget, ScreenOrigin);
        }

        if (m_DestCalculator != null)
        {
            m_DestCalculator(destinationTarget, ScreenDest);
        }

        SetArrowTransform();
    }

    #region view arrow

    public RectTransform arrow;
    public Image arrowImg;
    public Vector2 arrowSize;
    [Button]
    public void AcquireArrawParam()
    {
        arrowImg.SetNativeSize();

        arrowSize = arrowImg.rectTransform.sizeDelta;

    }

    float GetArrowScale(float length)
    {
        return length / arrowSize.x;
    }

    [Button]
    void SetArrowTransform()
    {
        Vector3 vector = ScreenDest.anchoredPosition - ScreenOrigin.anchoredPosition;

        arrow.anchoredPosition = ScreenOrigin.anchoredPosition;
        arrow.localScale = new Vector3(1, 1, GetArrowScale(vector.magnitude));
        arrow.LookAt(ScreenDest, Vector3.back);
    }

    #endregion

}