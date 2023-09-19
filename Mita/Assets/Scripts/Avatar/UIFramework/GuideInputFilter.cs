using Sirenix.OdinInspector;
using UnityEngine;
public enum MaskType
{
    None = 0,
    UILayer = 1,
    WorldLayer = 2,
    All = 99
}

public class GuideInputFilter : MonoBehaviour, ICanvasRaycastFilter
{
    public bool IsRaycast = true;

    [ShowInInspector]
    private GameObject target;

    [ShowInInspector]
    private MaskType targetType = MaskType.None;

    [ShowInInspector]
    private Camera worldCamera;

    private int layerMask = 0;

    private Canvas _canvas;
    private bool _inited;
    public void Init()
    {
        if (_inited)
            return;
        s_active = this;
    }

    void Awake()
    {
        Init();
    }

    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        switch (targetType)
        {
            case MaskType.None:
                return false;
            case MaskType.UILayer:
                return UIObjectFilter(sp, eventCamera);
            case MaskType.WorldLayer:
                return WorldObjectFilter(sp, worldCamera);
            case MaskType.All:
                return true;
            default:
                return true;
        }

    }

    private bool WorldObjectFilter(Vector2 sp, Camera eventCamera)
    {
        if (target == null)
            return true;

        Ray CustomRay = eventCamera.ScreenPointToRay(sp);
        RaycastHit ObjHit;
        hitObject = null;
        if (Physics.Raycast(CustomRay, out ObjHit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Collide))
        {
            if (ObjHit.collider.gameObject != null)
            {
                hitObject = ObjHit.collider.gameObject;
                if (hitObject == target)
                    return false;
                else if (hitObject.transform.IsChildOf(target.transform))
                    return false;
                else
                    return true;
            }
        }

        return true;
    }

    public GameObject hitObject;

    private bool UIObjectFilter(Vector2 sp, Camera eventCamera)
    {
        if (target == null)
            return true;

        return !RectTransformUtility.RectangleContainsScreenPoint(target.GetComponent<RectTransform>(),
            sp, eventCamera);
    }

    /// <summary>
    /// 
    /// </summary>
    public void SetRaycastParamInternal(GameObject target, MaskType maskType, Camera worldCamera)
    {
        this.target = target;
        this.targetType = maskType;
        this.worldCamera = worldCamera;

        switch (maskType)
        {
            case MaskType.None:
            case MaskType.All:
                if (target != null)
                    ClientLog.Instance.LogError($"不需要target   [{target.ToString()}]");
                //do nothing
                break;
            case MaskType.UILayer:
                break;
            case MaskType.WorldLayer:
                //对World模式，layermask使用target的layer
                this.layerMask = 1 << target.layer;
                break;
            default:
                ClientLog.Instance.LogError($"未处理的mask形式:{maskType}");
                break;
        }
    }
    public Canvas GetRootCanvas()
    {

        if (null == _canvas)
        {
            _canvas = transform.GetComponentInParent<Canvas>();
        }

        return _canvas;
    }
    #region static api

    static GuideInputFilter s_active;
    static Vector3[] s_corners = new Vector3[4];

    static GuideInputFilter active
    {
        get
        {
            if (s_active == null)
            {
                s_active = Object.FindObjectOfType<GuideInputFilter>();
            }
            return s_active;
        }
    }
    static Canvas GetUIRootCanvas()
    {
        return UISystem.Instance.UIRootCanvas;
    }

    static public void CalcUIRectCenter(GameObject target, out float x, out float y)
    {
        if (target == null)
        {
            x = 0;
            y = 0;
            return;
        }
        var canvas = GetUIRootCanvas();
        var cam = canvas.worldCamera;
        var rect = target.transform as RectTransform;

        rect.GetWorldCorners(s_corners);
        Vector2 worldPos = new Vector2();
        worldPos.x = s_corners[0].x + ((s_corners[3].x - s_corners[0].x) / 2f);
        worldPos.y = s_corners[0].y + ((s_corners[1].y - s_corners[0].y) / 2f);

        var screenPos = cam.WorldToScreenPoint(worldPos);

        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
            screenPos, cam, out position);

        x = position.x;
        y = position.y;
    }

    static public void CalcWorldTargetUICenter(GameObject target, Camera worldCamera, float offsetY, out float x, out float y)
    {
        Vector3 offset = new Vector3(0, offsetY, 0);
        var canvas = GetUIRootCanvas();

        var screenPos = worldCamera.WorldToScreenPoint(target.transform.position + offset);

        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
            screenPos, canvas.worldCamera, out position);

        x = position.x;
        y = position.y;
    }

    static public void SetRectSizeByUIObject(RectTransform rectTransform, GameObject target, float xOffset, float yOffset)
    {
        if (target == null)
            return;
        var rect = target.transform as RectTransform;
        if (rect == null)
            return;

        rectTransform.sizeDelta = rect.rect.size * rectTransform.lossyScale / rect.lossyScale + new Vector2(xOffset, yOffset);
    }

    static public void SetRectScaleByUIObject(RectTransform rectTransform, GameObject target, float refSize)
    {
        if (target == null)
            return;
        var rect = target.transform as RectTransform;
        if (rect == null)
            return;

        float scale = rect.sizeDelta.y / refSize;

        rectTransform.localScale = Vector3.one * scale;

    }

    static public void SetRaycastParam(GameObject target, MaskType maskType, Camera worldCamera = null)
    {
        if (active == null)
        {
            return;
        }
        active.SetRaycastParamInternal(target, maskType, worldCamera);
    }

    #endregion

}