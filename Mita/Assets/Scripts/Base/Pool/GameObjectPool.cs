using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unity中GameObject的池子
/// </summary>
public class GameObjectPool : Singleton<GameObjectPool>
{
    public bool shouldShowLog;
    public Transform root;

    // 预制体-对象池
    private Dictionary<GameObject, ObjectPool<GameObject>> prefabLookup;
    // 实例-对象池（隶属于同一预制体的实例对应的对象池是一样的）
    private Dictionary<GameObject, ObjectPool<GameObject>> instanceLookup;

    private bool hasRefreshedLog = false;

    private void Update()
    {
        if (shouldShowLog && hasRefreshedLog)
        {
            ShowLog();
            hasRefreshedLog = false;
        }
    }

    public override void Init()
    {
        prefabLookup = new Dictionary<GameObject, ObjectPool<GameObject>>();
        instanceLookup = new Dictionary<GameObject, ObjectPool<GameObject>>();
    }

    public void WarmPoolNonStatic(GameObject prefab, int size, bool isActive)
    {
        var pool = new ObjectPool<GameObject>(() => { return InstantiatePrefab(prefab, isActive); }, size);
        prefabLookup[prefab] = pool;

        hasRefreshedLog = true;
    }

    public GameObject CreateObjectNonStatic(GameObject prefab)
    {
        return CreateObjectNonStatic(prefab, Vector3.zero, Quaternion.identity);
    }

    public GameObject CreateObjectNonStatic(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!prefabLookup.ContainsKey(prefab))
            WarmPool(prefab, 1, true);

        var pool = prefabLookup[prefab];

        var go = pool.GetItem();
        go.transform.position = position;
        go.transform.rotation = rotation;
        go.SetActive(true);

        instanceLookup.Add(go, pool);
        hasRefreshedLog = true;
        return go;
    }

    public void ReleaseObjectNonStatic(GameObject go)
    {
        go.SetActive(false);

        if (instanceLookup.ContainsKey(go))
        {
            instanceLookup[go].ReleaseItem(go);
            instanceLookup.Remove(go);
            hasRefreshedLog = true;
        }
    }

    public void ReleasePoolNonStatic(GameObject prefab)
    {
        if (!prefabLookup.ContainsKey(prefab)) return;
        prefabLookup[prefab].Destroy();
        prefabLookup.Remove(prefab);
        hasRefreshedLog = true;
    }

    private GameObject InstantiatePrefab(GameObject prefab, bool isActive)
    {
        var go = Object.Instantiate(prefab) as GameObject;
        if (isActive)
            go.SetActive(true);
        else
            go.SetActive(false);

        if (root != null)
            go.transform.parent = root;
        return go;
    }

    public void ShowLog()
    {
        foreach (var item in prefabLookup)
        {
            ClientLog.Instance.Log(string.Format("“游戏对象池”  预制体名称：{0}  {1}个在被使用，共有{2}个", item.Key.name, item.Value.CountUsedItems, item.Value.Count));
        }
    }

    public void WarmPool(GameObject prefab, int size, bool isActive = false)
    {
        WarmPoolNonStatic(prefab, size, isActive);
    }

    public GameObject CreateObject(GameObject prefab)
    {
        return CreateObjectNonStatic(prefab);
    }

    public GameObject CreateObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        return CreateObjectNonStatic(prefab, position, rotation);
    }

    public void ReleaseObject(GameObject go)
    {
        ReleaseObjectNonStatic(go);
    }

    public void ReleasePool(GameObject prefab)
    {
        ReleasePoolNonStatic(prefab);
    }

    public override void OnRelease()
    {

    }
}