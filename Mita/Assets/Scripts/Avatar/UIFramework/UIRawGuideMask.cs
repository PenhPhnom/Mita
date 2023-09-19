using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum MaskShape : int
{
    Circle = 0,
    Rect = 1,
    CornerRect = 2,
}

public class UIRawGuideMask : MonoBehaviour
{

    [SerializeField] RawImage _rawImage; //遮罩图片
    [SerializeField] RectTransform _rectTrans;
    Material _materia;
    [SerializeField] Canvas _canvas;

    //[Tooltip("Type of tween easing to apply")]
    //[SerializeField] protected LeanTweenType tweenType = LeanTweenType.easeOutQuad;

    [SerializeField] protected Ease tweenType = Ease.OutQuart;


    void Awake()
    {
        //材质float 0 为圆形
        //材质float 1 为矩形
        //材质float大于 1 为边缘模糊矩形
        _materia = _rawImage.material;
    }

    public void FadeAlpha(float alpha, float time)
    {
        //tweenType.alpha(_rawImage.rectTransform, alpha, time).setEase(tweenType).setEase(tweenType);

        //DG.Tweening.DOTween.To(x => _rawImage.rectTransform.alpha = x, 1.0f, 0.0f, 5.0f).SetId("Tween");
        DOTween.ToAlpha
        (
            () => _rawImage.color,
            (c) => _rawImage.color = c,
            alpha,
            time
        ).OnComplete(() => { }).SetEase(tweenType);
    }

    public void SetAlpha(float alpha, float time)
    {
        _rawImage.color = new Color(_rawImage.color.r, _rawImage.color.g, _rawImage.color.b, alpha);
    }

    /// <summary>
    /// 获取对象RectTransform的中心点
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    public Vector2 GetTargetCenter(RectTransform rect)
    {
        Vector3[] _corners = new Vector3[4];
        rect.GetWorldCorners(_corners);

        float x = _corners[0].x + ((_corners[3].x - _corners[0].x) / 2f);
        float y = _corners[0].y + ((_corners[1].y - _corners[0].y) / 2f);
        Vector3 centerWorld = new Vector3(x, y, 0);
        Vector2 center = WorldToCanvasPos(centerWorld);
        return center;
    }

    /// <summary>
    /// 获取对象RectTransform的世界中心点
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    public Vector3 GetTargetWorldCenter(RectTransform rect)
    {
        Vector3[] _corners = new Vector3[4];
        rect.GetWorldCorners(_corners);

        float x = _corners[0].x + ((_corners[3].x - _corners[0].x) / 2f);
        float y = _corners[0].y + ((_corners[1].y - _corners[0].y) / 2f);
        float z = _corners[0].z;

        return new Vector3(x, y, z);
    }

    /// <summary>
    /// 获取对象RectTransform的半径
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    public float GetTargetRad(RectTransform rect)
    {
        Vector3[] _corners = new Vector3[4];
        rect.GetWorldCorners(_corners);
        //计算最终高亮显示区域的半径       
        float _radius = Vector2.Distance(WorldToCanvasPos(_corners[0]),
            WorldToCanvasPos(_corners[2])) / 2f;
        return _radius;
    }
    /// <summary>
    /// 世界坐标向画布坐标转换
    /// </summary>
    /// <param name="world">世界坐标</param>
    /// <returns>返回画布上的二维坐标</returns>
    public Vector2 WorldToCanvasPos(Vector3 world)
    {
        if (null == _canvas) _canvas = transform.GetComponentInParent<Canvas>();
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform,
            world, _canvas.GetComponent<Camera>(), out position);
        return position;
    }

    private void CalcTargetCorner(GameObject target, out Vector2 pos1, out Vector2 pos2)
    {
        RectTransform rec = target.GetComponent<RectTransform>();
        Vector3[] _corners = new Vector3[4];
        rec.GetWorldCorners(_corners);
        pos1 = WorldToCanvasPos(_corners[0]);
        pos2 = WorldToCanvasPos(_corners[2]);
    }

    public System.Action<bool> onSetEnableGuideViewUpdate;
    public void SetEnableGuideViewUpdate(bool enable)
    {
        if (onSetEnableGuideViewUpdate != null)
        {
            onSetEnableGuideViewUpdate(enable);
        }
    }

    #region input raycast

    #endregion

    #region circle

    public void CalcCircleParam(GameObject target, out float p1, out float p2, out float p3, out float p4)
    {
        var rect = target.transform as RectTransform;
        var pos = GetTargetWorldCenter(rect);
        var rad = GetTargetRad(rect);

        p1 = pos.x;
        p2 = pos.y;
        p3 = p4 = rad;
    }
    public void CreateUICircleMask(GameObject target)
    {
        var rect = target.transform as RectTransform;
        var pos = GetTargetWorldCenter(rect);
        var rad = GetTargetRad(rect);

        CreateCircleMask(pos.x, pos.y, rad);

    }
    public void CreateUICircleMask(GameObject target, float rad)
    {
        var rect = target.transform as RectTransform;
        var pos = GetTargetWorldCenter(rect);

        CreateCircleMask(pos.x, pos.y, rad);
    }

    /// <summary>
    /// 创建圆形点击区域(最新使用)
    /// </summary>
    /// <param name="x">圆心的屏幕位置X</param>
    /// <param name="y">圆心的屏幕位置Y</param>
    /// <param name="r">圆的半径</param>
    public void CreateCircleMask(float x = 0, float y = 0, float r = 0)
    {
        _rectTrans.sizeDelta = Vector2.zero;
        _materia.SetFloat("_MaskType", (float)MaskShape.Circle);
        _materia.SetVector("_Origin", (new Vector4(x, y, r, r)));
    }
    #endregion

    #region base rect

    /// <summary>
    /// 创建矩形点击区域
    /// </summary>
    /// <param name="obj">目标位置</param>
    /// <param name="CallBack">回调</param>
    public void CreateRectangleMask(GameObject target)
    {
        if (target == null)
            return;

        Vector2 pos1, pos2;
        CalcTargetCorner(target, out pos1, out pos2);
        CreateRectangleMask(pos1, pos2);
    }

    /// <summary>
    /// 创建矩形点击区域
    /// </summary>
    /// <param name="pos1">左下角位置</param>
    /// <param name="pos2">右上角位置</param>
    public void CreateRectangleMask(Vector3 pos1, Vector3 pos2)
    {
        CreateRectangleMask(pos1.x, pos1.y, pos2.x, pos2.y);
    }

    /// <summary>
    /// 创建矩形点击区域
    /// </summary>
    /// <param name="pos1X">左下角的X坐标</param>
    /// <param name="pos1Y">左下角的Y坐标</param>
    /// <param name="pos2X">右上角的X坐标</param>
    /// <param name="pos2X">右上角的Y坐标</param>
    public void CreateRectangleMask(float pos1X, float pos1Y, float pos2X, float pos2Y)
    {
        _rectTrans.sizeDelta = Vector2.zero;
        _materia.SetFloat("_MaskType", (float)MaskShape.Rect);
        _materia.SetVector("_Origin", new Vector4(pos1X, pos1Y, pos2X, pos2Y));
    }

    #endregion
    #region  corner rect
    /// <summary>
    /// 创建矩形点击区域
    /// </summary>
    /// <param name="obj">目标位置</param>
    /// <param name="CallBack">回调</param>
    public void CreateCornerRectangleMask(GameObject target, float corner)
    {
        if (target == null)
            return;

        Vector2 pos1, pos2;
        CalcTargetCorner(target, out pos1, out pos2);
        CreateCornerRectangleMask(pos1, pos2, corner);
    }

    /// <summary>
    /// 创建圆角矩形点击区域
    /// </summary>
    /// <param name="pos1">左下角位置</param>
    /// <param name="pos2">右上角位置</param>
    public void CreateCornerRectangleMask(Vector3 pos1, Vector3 pos2, float corner)
    {
        CreateCornerRectangleMask(pos1.x, pos1.y, pos2.x, pos2.y, corner);
    }

    /// <summary>
    /// 创建圆角矩形点击区域
    /// </summary>
    /// <param name="pos1X">左下角的X坐标</param>
    /// <param name="pos1Y">左下角的Y坐标</param>
    /// <param name="pos2X">右上角的X坐标</param>
    /// <param name="pos2X">右上角的Y坐标</param>
    public void CreateCornerRectangleMask(float pos1X, float pos1Y, float pos2X, float pos2Y, float corner)
    {
        _rectTrans.sizeDelta = Vector2.zero;
        _materia.SetFloat("_MaskType", (float)MaskShape.CornerRect);
        _materia.SetVector("_Origin", new Vector4(pos1X, pos1Y, pos2X, pos2Y));
        _materia.SetFloat("_Corner", corner);
    }

    #endregion

}