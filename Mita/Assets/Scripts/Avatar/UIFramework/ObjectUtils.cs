using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public static class ObjectUtils
{
    public static void SetObjectLayer(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform trans in obj.GetComponentsInChildren<Transform>())
        {
            trans.gameObject.layer = layer;
        }
    }

    public static void SetObjectLayerByName(GameObject obj, string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        ObjectUtils.SetObjectLayer(obj, layer);
    }
    public static void SetObjectLayerByName(Component obj, string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        ObjectUtils.SetObjectLayer(obj.gameObject, layer);
    }
    public static void SetObjectWorldPositionByVector3(GameObject obj, Vector3 position)
    {
        obj.transform.position = position;
    }
    public static void SetObjectWorldPosition(GameObject obj, float x, float y, float z)
    {
        obj.transform.position = new Vector3(x, y, z);
    }


    public static void Normalized(GameObject obj)
    {

        obj.transform.localPosition = new Vector3(0, 0, 0);
        obj.transform.localScale = new Vector3(1, 1, 1);
        var rt = obj.GetComponent<RectTransform>();
        rt.anchorMax = Vector2.one;
        rt.anchorMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.offsetMin = Vector2.zero;

    }

    public static void SetObjectLocalPosition(GameObject obj, float x, float y, float z)
    {
        if (float.IsNaN(x) || float.IsNaN(y) || float.IsNaN(z))
        {
            return;
        }
        obj.transform.localPosition = new Vector3(x, y, z);
    }
    public static void SyncRotation(Transform source, GameObject target)
    {
        target.transform.localRotation = source.localRotation;
    }
    public static void SetObjectLocalPosition(Transform objTr, float x, float y, float z)
    {
        if (float.IsNaN(x) || float.IsNaN(y) || float.IsNaN(z))
        {
            return;
        }
        objTr.localPosition = new Vector3(x, y, z);
    }
    public static void SetObjectEulerangles(GameObject obj, float x, float y, float z)
    {
        obj.transform.eulerAngles = new Vector3(x, y, z);
    }
    public static void SetLookAt(GameObject obj, Vector3 lookTarget)
    {
        obj.transform.LookAt(lookTarget);
    }

    public static void SetObjectLookAt(GameObject obj, float x, float y, float z)
    {
        obj.transform.LookAt(new Vector3(x, y, z));
    }

    public static void SetObjectLocalEulerangles(GameObject obj, float x, float y, float z)
    {
        obj.transform.localEulerAngles = new Vector3(x, y, z);
    }

    public static void SetObjectWorldRotation(GameObject obj, Quaternion rotation)
    {
        obj.transform.rotation = rotation;
    }

    public static void SetObjectLocalScale(GameObject obj, float x, float y, float z)
    {
        obj.transform.localScale = new Vector3(x, y, z);
    }

    public static void SetObjectLocalScale(Transform obj, float x, float y, float z)
    {
        obj.localScale = new Vector3(x, y, z);
    }
    public static void SetObjectLocalScaleUniform(GameObject obj, float scale)
    {
        obj.transform.localScale = new Vector3(scale, scale, scale);
    }
    public static void SetObjectLocalScaleUniform(Transform obj, float scale)
    {
        obj.localScale = new Vector3(scale, scale, scale);
    }
    public static void SetObjectSize(RectTransform rectTransform, float width, float height)
    {
        rectTransform.sizeDelta = new Vector2(width, height);
    }

    public static void SetObjectWidth(RectTransform rectTran, float width)
    {
        rectTran.sizeDelta = new Vector2(width, rectTran.sizeDelta.y);
    }

    public static void SetObjectHeight(RectTransform rectTran, float height)
    {
        rectTran.sizeDelta = new Vector2(rectTran.sizeDelta.x, height);
    }

    public static Vector2 WorldPosToScreenLocalPos(UnityEngine.Camera camera, UnityEngine.Camera uiCamera, RectTransform rectangle, Vector3 target)
    {
        Vector3 tarPos = target;
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(camera, tarPos);

        Vector2 imgPos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectangle, screenPos, uiCamera, out imgPos);

        return new Vector2(imgPos.x, imgPos.y);
    }

    public static Vector2 WorldPosToScreenLocalPos(UnityEngine.Camera camera, UnityEngine.Camera uiCamera, RectTransform rectangle, Transform targetTR)
    {
        Vector3 tarPos = targetTR.position;
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(camera, tarPos);

        Vector2 imgPos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectangle, screenPos, uiCamera, out imgPos);

        return new Vector2(imgPos.x, imgPos.y);
    }
    public static Vector2 WorldPosToScreenLocalPos(UnityEngine.Camera camera, UnityEngine.Camera uiCamera, RectTransform rectangle, Transform targetTR, float offsetX, float offsetY, float offsetZ)
    {
        Vector3 tarPos = targetTR.position + new Vector3(offsetX, offsetY, offsetZ);
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(camera, tarPos);

        Vector2 imgPos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectangle, screenPos, uiCamera, out imgPos);

        return new Vector2(imgPos.x, imgPos.y);
    }
    public static Vector2 BattleWorldPosToScreenLocalPos(UnityEngine.Camera camera, UnityEngine.Camera uiCamera, RectTransform rectangle, float x, float y, float z)
    {
        Vector3 tarPos = new Vector3(x, y, z);
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(camera, tarPos); // / QualityManagement.RawTextureCameraDisplay.Instance.screenScaleRatio;

        Vector2 imgPos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectangle, screenPos, uiCamera, out imgPos);

        return new Vector2(imgPos.x, imgPos.y);
    }
    public static void BattleWorldPosToScreenPos(UnityEngine.Camera camera, UnityEngine.Camera uiCamera, RectTransform rectangle, float x, float y, float z, out float screenX, out float screenY)
    {
        Vector3 tarPos = new Vector3(x, y, z);
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(camera, tarPos);// / QualityManagement.RawTextureCameraDisplay.Instance.screenScaleRatio;

        Vector2 imgPos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectangle, screenPos, uiCamera, out imgPos);

        screenX = imgPos.x;
        screenY = imgPos.y;
    }
    public static Vector2 BattleWorldPosToScreenLocalPos(UnityEngine.Camera camera, UnityEngine.Camera uiCamera, RectTransform rectangle, Vector3 tarPos)
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(camera, tarPos);// / QualityManagement.RawTextureCameraDisplay.Instance.screenScaleRatio;

        Vector2 imgPos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectangle, screenPos, uiCamera, out imgPos);

        return new Vector2(imgPos.x, imgPos.y);
    }
    public static Vector2 ScreenPosToRectLocalPos(UnityEngine.Camera uiCamera, RectTransform rectangle, Vector3 screenPos)
    {
        Vector2 imgPos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectangle as RectTransform, screenPos, uiCamera, out imgPos);

        return new Vector2(imgPos.x, imgPos.y);
    }

    public static Vector3 ScreenPosToWorldPos(UnityEngine.Camera camera, float x, float y, float z)
    {
        Vector3 tarPos = new Vector3(x, y, z);
        return camera.ScreenToWorldPoint(tarPos);
    }

    public static bool RectContainsScreenPos(RectTransform rectangle, Vector2 screenPos, Camera cam)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rectangle, screenPos, cam);
    }

    public static bool PhysicsCast(Ray ray, out RaycastHit hit, int layer)
    {
        int layerMask = 1 << layer;
        return Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore);
    }

    public static bool PhysicsCastAny(Ray ray, out RaycastHit hit)
    {
        return Physics.Raycast(ray, out hit);
    }

    public static RaycastHit[] PhysicsCastAll(Ray ray, float distance)
    {
        return Physics.RaycastAll(ray, distance);
    }

    public static List<Vector3> TrackPoint(Vector3[] track)
    {
        List<Vector3> lsPoint = new List<Vector3>();
        Vector3[] vector3s = PathControlPointGenerator(track);
        int SmoothAmount = track.Length * 50;
        for (int i = 1; i < SmoothAmount; i++)
        {
            float pm = (float)i / SmoothAmount;
            Vector3 currPt = Interp(vector3s, pm);
            lsPoint.Add(currPt);
        }
        return lsPoint;
    }

    private static Vector3[] PathControlPointGenerator(Vector3[] path)
    {
        Vector3[] suppliedPath;
        Vector3[] vector3s;

        suppliedPath = path;
        int offset = 2;
        vector3s = new Vector3[suppliedPath.Length + offset];
        Array.Copy(suppliedPath, 0, vector3s, 1, suppliedPath.Length);
        vector3s[0] = vector3s[1] + (vector3s[1] - vector3s[2]);
        vector3s[vector3s.Length - 1] = vector3s[vector3s.Length - 2] + (vector3s[vector3s.Length - 2] - vector3s[vector3s.Length - 3]);
        if (vector3s[1] == vector3s[vector3s.Length - 2])
        {
            Vector3[] tmpLoopSpline = new Vector3[vector3s.Length];
            Array.Copy(vector3s, tmpLoopSpline, vector3s.Length);
            tmpLoopSpline[0] = tmpLoopSpline[tmpLoopSpline.Length - 3];
            tmpLoopSpline[tmpLoopSpline.Length - 1] = tmpLoopSpline[2];
            vector3s = new Vector3[tmpLoopSpline.Length];
            Array.Copy(tmpLoopSpline, vector3s, tmpLoopSpline.Length);
        }
        return (vector3s);
    }

    public static Vector3 Interp(Vector3[] pos, float t)
    {
        int length = pos.Length - 3;
        int currPt = Mathf.Min(Mathf.FloorToInt(t * length), length - 1);
        float u = t * (float)length - (float)currPt;
        Vector3 a = pos[currPt];
        Vector3 b = pos[currPt + 1];
        Vector3 c = pos[currPt + 2];
        Vector3 d = pos[currPt + 3];

        return .5f * (
            (-a + 3f * b - 3f * c + d) * (u * u * u) +
            (2f * a - 5f * b + 4f * c - d) * (u * u) +
            (-a + c) * u +
            2f * b
        );
    }

    public static void SetComponentGameObjectActive(Component comp, bool isActive)
    {
        comp.gameObject.SetActive(isActive);
    }
    public static Bounds SetBoundsCenter(Bounds bounds, float centerX, float centerY, float centerZ)
    {
        var center = bounds.size;
        center.x = centerX;
        center.y = centerY;
        center.z = centerZ;
        bounds.center = center;
        // bounds.size = new Vector3(sizeX, sizeY, sizeZ);
        return bounds;
    }
    public static Bounds SetBoundsSize(Bounds bounds, float sizeX, float sizeY, float sizeZ)
    {
        var size = bounds.size;
        size.x = sizeX;
        size.y = sizeY;
        size.z = sizeZ;
        bounds.size = size;
        // bounds.size = new Vector3(sizeX, sizeY, sizeZ);
        return bounds;
    }

    public static void SetRectTransformAnchoredPosition(GameObject rectangle, float anchoredPositionX, float anchoredPositionY)
    {
        if (rectangle != null)
        {
            rectangle.transform.localPosition = Vector3.zero;
            (rectangle.transform as RectTransform).anchoredPosition = new Vector2(anchoredPositionX, anchoredPositionY);
        }
    }

    // 设置材质颜色  先放这吧
    static MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
    public static void SetMaterialColor(GameObject go, string property, float r, float g, float b, float a)
    {
        Renderer renderer = go.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            int pID = Shader.PropertyToID(property);
            renderer.GetPropertyBlock(materialPropertyBlock);
            materialPropertyBlock.SetColor(pID, new Color(r, g, b, a));
            renderer.SetPropertyBlock(materialPropertyBlock);
            materialPropertyBlock.Clear();
        }
    }

    // 设置LineRenderer 两点
    public static void SetLineRendererPosition(GameObject go, Vector3 start, Vector3 end)
    {
        LineRenderer line = go.GetComponentInChildren<LineRenderer>();
        if (line != null)
        {
            line.SetPosition(0, start);
            line.SetPosition(1, end);
        }
    }

    // 设置LineRenderer 两点，为了槽位做的
    public static void SetMultiLineRendererPos(GameObject go, Vector3 start,
        float endx, float endy, float endz,
        float offsetx, float offsety, float offsetz,
        int vertextCount)
    {
        LineRenderer[] lineList = go.GetComponentsInChildren<LineRenderer>();
        if (lineList == null)
        {
            return;
        }
        Vector3 end = new Vector3(endx, endy, endz);
        float dis = Vector3.Distance(start, end);
        int pID = Shader.PropertyToID("_MainTex_ST");
        lineList[1].GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetVector(pID, new Vector4(1, 1, 0.9f - dis, 0));
        lineList[1].SetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.Clear();
        Vector3 offset = new Vector3(offsetx, offsety, offsetz);
        Vector3 center = (start + end) / 2 + offset;
        List<Vector3> pointList = new List<Vector3>();
        List<Vector3> pointListArrow = new List<Vector3>();
        for (float ratio = 0; ratio <= 1; ratio += 1.0f / vertextCount)
        {
            Vector3 tangentLineVertex1 = Vector3.Lerp(start, center, ratio);
            Vector3 tangentLineVectex2 = Vector3.Lerp(center, end, ratio);
            Vector3 bezierPoint = Vector3.Lerp(tangentLineVertex1, tangentLineVectex2, ratio);
            if (Vector3.Distance(bezierPoint, end) > 1)
            {
                pointList.Add(bezierPoint);
            }
            pointListArrow.Add(bezierPoint);
        }
        pointListArrow.Add(end);
        lineList[0].positionCount = pointList.Count;
        lineList[0].SetPositions(pointList.ToArray());
        lineList[1].positionCount = pointListArrow.Count;
        lineList[1].SetPositions(pointListArrow.ToArray());
    }

    public static float GetClipLength(this Animator animator, string clip)
    {
        if (null == animator || string.IsNullOrEmpty(clip) || null == animator.runtimeAnimatorController)
            return 0;
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        AnimationClip[] tAnimationClips = ac.animationClips;
        if (null == tAnimationClips || tAnimationClips.Length <= 0) return 0;
        AnimationClip tAnimationClip;
        for (int tCounter = 0, tLen = tAnimationClips.Length; tCounter < tLen; tCounter++)
        {
            tAnimationClip = ac.animationClips[tCounter];
            if (null != tAnimationClip && tAnimationClip.name == clip)
                return tAnimationClip.length;
        }
        return 0;
    }


    // 设置Canvas的状态
    public static void SetCanvasEnable(GameObject go, bool enable)
    {
        if (enable)
        {
            go.SetLayerRecursive(LayerMask.NameToLayer("UI"));
        }
        else
        {
            go.SetLayerRecursive(LayerMask.NameToLayer("Ignore Raycast"));
        }

        GraphicRaycaster[] raycaster = go.GetComponentsInChildren<GraphicRaycaster>(true);
        if (raycaster != null)
        {
            for (int i = 0; i < raycaster.Length; i++)
            {
                raycaster[i].enabled = enable;
            }
        }
    }

    public static Bounds CalculateRelativeRectTransformBounds(Transform canvas, RectTransform target)
    {
        return RectTransformUtility.CalculateRelativeRectTransformBounds(canvas, target);
    }

    public static void CloseLightShadow(GameObject obj)
    {
        var light = obj.GetComponentInChildren<Light>();
        light.shadows = LightShadows.None;
    }

    public static void SetParent(GameObject go, Transform transParent, bool worldPositionStays = true)
    {
        go.transform.SetParent(transParent, worldPositionStays);
    }

    public static bool IsCSObjectNull(object obj)
    {
        return obj == null;
    }

    public static bool IsUnityObjectNull(UnityEngine.Object obj)
    {
        return obj == null;
    }

    public static void ControlEffectAnimatorTrigger(Transform trans, string triggerName)
    {

    }


    public static void SetChildrenTransformScale(GameObject go, float uniformScale)
    {
        var trans = go.transform;
        var scale = Vector3.one * uniformScale;
        int count = trans.childCount;
        for (int i = 0; i < count; ++i)
        {
            trans.GetChild(i).localScale = scale;
        }
    }

    public static void SetGameObjectCompEnabledByName(GameObject gameObject, string componentTypeName, bool enabled)
    {
        if (gameObject == null)
        {
            Debug.LogError("SetGameObjectCompEnabled 没有目标gameObject");
            return;
        }

        if (string.IsNullOrEmpty(componentTypeName))
        {
            Debug.LogError("SetGameObjectCompEnabled 没有目标指定类型");
            return;
        }

        var comp = gameObject.GetComponent(componentTypeName) as Behaviour;
        if (comp == null)
        {

            Debug.LogError($"SetGameObjectCompEnabled 没有目标组件 {gameObject}/{componentTypeName}", gameObject);
            return;
        }

        comp.enabled = enabled;
    }

    public static void SetGameObjectCompEnabled(GameObject gameObject, System.Type componentType, bool enabled)
    {
        if (gameObject == null)
        {
            Debug.LogError("SetGameObjectCompEnabled 没有目标gameObject");
            return;
        }

        if (componentType == null)
        {
            Debug.LogError("SetGameObjectCompEnabled 没有目标指定类型");
            return;
        }

        var comp = gameObject.GetComponent(componentType) as Behaviour;
        if (comp == null)
        {

            Debug.LogError($"SetGameObjectCompEnabled 没有目标组件 {gameObject}/{componentType}", gameObject);
            return;
        }

        comp.enabled = enabled;
    }

    public static void IterChildTransform(Transform rootTr, System.Action<Transform> callback)
    {
        int childCount = rootTr.childCount;
        for (int i = 0; i < childCount; ++i)
        {
            callback(rootTr.GetChild(i));
        }
    }

    public static void PlayWwise(GameObject effectTimeLine, float speed = 1, bool bSoldier = false)
    {
        if (effectTimeLine.IsNull())
        {
            return;
        }
    }

    public static void StopWwise(GameObject effectTimeLine)
    {
        if (effectTimeLine.IsNull())
        {
            return;
        }
    }

    public static List<RaycastResult> IsClickUIBtn(int x, int y)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(x, y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        for (int i = results.Count - 1; i >= 0; i--)
        {
            if (!results[i].gameObject.GetComponent<UnityEngine.UI.Button>())
                results.RemoveAt(i);

        }
        return results;
    }
}
