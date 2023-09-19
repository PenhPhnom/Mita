/// <summary>
/// GameObjectExt.cs
/// Desc:   
/// </summary>

using UnityEngine;

public static class GameObjectExt
{
    static public T AddMissingComponent<T>(this GameObject go) where T : Component
    {
        T cmp = go.GetComponent<T>();
        if (cmp == null)
        {
            cmp = go.AddComponent<T>();
        }
        return cmp;
    }

    /// <summary>
    /// 获取Transform的路径。可以打印出出了问题的GameObject的路径
    /// </summary>
    /// <param name="transform">需要查找的trans</param>
    /// <param name="stopAt">在哪个trans停止</param>
    /// <returns>在场景中的路径</returns>
    public static string GetFullTransformPath(Transform transform, Transform stopAt = null)
    {
        var path = transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            if (transform == stopAt) break;
            path = transform.name + "/" + path;
        }
        return path;
    }

    /// <summary>
    /// Set the layer of a gameobject and all child objects
    /// </summary>
    /// <param name="o"></param>
    /// <param name="layer"></param>
    public static void SetLayerRecursive(this GameObject o, int layer)
    {
        SetLayerInternal(o.transform, layer);
    }

    private static void SetLayerInternal(Transform t, int layer)
    {
        t.gameObject.layer = layer;
        
        int count = t.childCount;
        Transform o = null;
        for (int i = 0; i < count; ++i)
        {
            o = t.GetChild(i);
            SetLayerInternal(o, layer);
        }
    }

    public static Transform InstantXF(this GameObject obj, Transform parent = null)
    {
        if (obj)
        {
            var go = parent != null ? Object.Instantiate(obj, parent) : Object.Instantiate(obj);
            return go.transform;
        }
        return null;
    }

    /// <summary>
    ///  避免Lua直接调用obj.transform. 产生ObjectTranslator内存
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static Transform GetXF(this GameObject obj)
    {
        return obj ? obj.transform : null;
    }
}

public static class UnityEngineObjectExtention
{
    public static bool IsNull(this UnityEngine.Object o) // 或者名字叫IsDestroyed等等
    {
        return o == null;
    }
}

public static class SystemObjectExtention
{
    public static bool IsNull(this System.Object o) // 或者名字叫IsDestroyed等等
    {
        return o == null;
    }

}