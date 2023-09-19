using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParamData
{
    public int iParam = 0;
    public int iParam2 = 0;
    public float fParam = 0f;
    public float fParam2 = 0f;
    public bool bParam = false;
    public bool bParam2 = false;
    public bool bParam3 = false;
    public bool bParam4 = false;
    public string sParam = "";
    public string sParam2 = "";
    public string sParam3 = "";
    public string sParam4 = "";
    public object objectParam;
    public Action callBack;
}

public class TypeInts
{
    public const int Object = 0;
    public const int GameObject = 1;
    public const int Sprite = 2;
    public const int Material = 3;
    public const int TextAsset = 4;
    public const int Texture = 5;
    public const int SceneInstance = 6;
    public const int SceneZombie = 7;
    public const int AssetBundle = 8;
    public const int AudioClip = 9;
    public const int AssetBundleManifest = 10;
    public const int Texture2D = 11;
    public const int AnimClip = 12;
    public const int RuntimeAnimatorController = 13;
    public const int AnimatorOverrideController = 14;
    public const int Shader = 15;
    public const int Atlas = 16;
    public const int ColorTransform = 17;
}

public class ResourceMgr : Singleton<ResourceMgr>
{
    //GameObject资源与AB包映射关系
    private Dictionary<string, string> _resName2AbNameMap = new Dictionary<string, string>
    {
        //{ "Clo07", "charactor01" },
    };

    public delegate void ResourceLoadCallBack(GameObject obj);
    private class ResourceObject
    {
        //资源名
        public string _name;
        //资源类型
        public int _intType;
        //包名
        public string _abName;
        //包路径
        public string _abFolderPath;
        //引用计数
        public int _refCount;
        //回调Action的列表
        public List<System.Action<UnityEngine.Object, ParamData>> _actionList = new List<System.Action<UnityEngine.Object, ParamData>>();
        //Action与ParamData的映射表
        public Dictionary<System.Action<UnityEngine.Object, ParamData>, ParamData> _action2ParamDataMap = new Dictionary<System.Action<UnityEngine.Object, ParamData>, ParamData>();
        //资源
        public UnityEngine.Object _res;
        //延迟2帧解决换装问题
        public int _updateTimes;
    }
    //已经加载的资源
    private Dictionary<string, ResourceObject> _loadedResourceMap;
    //正在加载的资源
    private Dictionary<string, ResourceObject> _loadingResourceMap;
    //等待加载的资源
    private Dictionary<string, ResourceObject> _waitingResourceMap;
    //准备销毁的资源
    private Dictionary<string, ResourceObject> _unLoadResourceMap;
    //创建临时存储变量，用于提升性能
    private List<ResourceObject> tempLoadeds;
    //同时加载的最大资源个数
    private const int MAX_LOADING_COUNT = 10;
    //缓存资源实例与资源的映射关系
    private Dictionary<UnityEngine.Object, string> _objUsedResource;
    //缓存场景名与AB包的映射关系
    private Dictionary<string, string> _sceneName2ABNameMap;

    private delegate void ResourceObjLoadCallBack(ResourceObject resObj);

    private const int GAMEOBJECT_POOL_SIZE = 1; //GameObject预热缓冲池大小

    public override void OnRelease()
    {
        _loadedResourceMap = new Dictionary<string, ResourceObject>();
        _loadingResourceMap = new Dictionary<string, ResourceObject>();
        _waitingResourceMap = new Dictionary<string, ResourceObject>();
        _unLoadResourceMap = new Dictionary<string, ResourceObject>();
        tempLoadeds = new List<ResourceObject>();
        _objUsedResource = new Dictionary<UnityEngine.Object, string>();
        _sceneName2ABNameMap = new Dictionary<string, string>();
    }

    //继承了单例模式提供的初始化函数
    public override void Init()
    {
        base.Init();
        OnRelease();
    }

    //从AB中加载资源
    private AssetBundleRequest LoadResFromAB(AssetBundle ab, string resName)
    {
        if (ab == null)
            return null;
        AssetBundleRequest abr = ab.LoadAssetAsync(resName);
        return abr;
    }

    //卸载资源
    private void UnLoadResource(ResourceObject resObj)
    {
        if (resObj == null)
        {
            ClientLog.Instance.LogError(string.Format("UnLoadResource resObj NULL"));
            return;
        }
        //引用计数-1
        resObj._refCount--;
        //如果是为0，则卸载
        if (resObj._refCount <= 0)
        {
            //从字典中删掉
            _loadedResourceMap.Remove(resObj._name);
            //加入卸载字典
            _unLoadResourceMap.Add(resObj._name, resObj);
        }
    }

    private void DoUnLoad(ResourceObject resObj)
    {
        //如果是GameObject卸载资源的缓冲池
        /*if (resObj._intType == TypeInts.GameObject)
        {
            GameObjectPool.Instance.ReleasePool(resObj._res as GameObject);
        }*/
        //销毁res
        /*        UnityEngine.Object.DestroyImmediate(resObj._res, true);
                resObj._res = null;*/
        //卸载对应的AB包
        AssetBundleManager.Instance.Unload(resObj._abName);
        //ClientLog.Instance.Log("ZLLog:" + "卸载资源" + resObj._name);
    }

    //检测等待队列
    private void UpdateWaiting()
    {
        if (_waitingResourceMap.Count <= 0 || _loadingResourceMap.Count >= MAX_LOADING_COUNT) return;
        tempLoadeds.Clear();
        foreach (var resObj in _waitingResourceMap.Values)
        {
            _loadingResourceMap.Add(resObj._name, resObj);
            tempLoadeds.Add(resObj);
            if (_loadingResourceMap.Count >= MAX_LOADING_COUNT) break;
        }

        foreach (var resObj in tempLoadeds)
        {
            _waitingResourceMap.Remove(resObj._name);
            LoadAssetBundle(resObj);
        }
    }

    private void UpdateUnLoad()
    {
        if (_unLoadResourceMap.Count <= 0) return;
        tempLoadeds.Clear();
        foreach (var resObj in _unLoadResourceMap.Values)
        {
            tempLoadeds.Add(resObj);
            DoUnLoad(resObj);
        }
        foreach (var resObj in tempLoadeds)
        {
            _unLoadResourceMap.Remove(resObj._name);
        }
    }

    private void UpdateLoaded()
    {
        if (_loadedResourceMap.Count <= 0) return;
        tempLoadeds.Clear();
        foreach (var resObj in _loadedResourceMap.Values)
        {
            if (resObj._actionList.Count > 0)
            {
                if (resObj._updateTimes > 0)
                {
                    resObj._updateTimes--;
                }
                else
                {
                    tempLoadeds.Add(resObj);
                }
            }
        }
        foreach (var resObj in tempLoadeds)
        {
            LoadResourceFinish(resObj);
        }
    }

    public void Update()
    {
        UpdateLoaded();
        UpdateWaiting();
        UpdateUnLoad();
    }

    //在对象池中分配Prefab实例
    private void SpawnPrefabInPool(ResourceObject resObj)
    {
        foreach (System.Action<UnityEngine.Object, ParamData> callFun in resObj._actionList)
        {
            //从对象池分配实例
            GameObject retInstanceObj = GameObjectPool.Instance.CreateObject(resObj._res as GameObject);
            //缓存对象与资源关系
            _objUsedResource.Add(retInstanceObj, resObj._name);
            //实际分配完毕后增加引用计数
            resObj._refCount++;
            try
            {
                if (resObj._action2ParamDataMap.ContainsKey(callFun))
                    callFun(retInstanceObj, resObj._action2ParamDataMap[callFun]);
                else
                    callFun(retInstanceObj, null);
            }
            catch (Exception e)
            {
                ClientLog.Instance.LogError("SpawnPrefabInPool error res:" + resObj._name + " " + e);
            }
        }
        resObj._actionList.Clear();
        resObj._action2ParamDataMap.Clear();
    }

    //直接生成对象实例
    private void SpawnObject<T>(ResourceObject resObj) where T : UnityEngine.Object
    {
        foreach (System.Action<UnityEngine.Object, ParamData> callFun in resObj._actionList)
        {
            //根据类型T实例化对象
            var retInstanceObj = UnityEngine.Object.Instantiate(resObj._res) as T;
            //缓存对象与资源关系
            _objUsedResource.Add(retInstanceObj, resObj._name);
            //实际分配完毕后增加引用计数
            resObj._refCount++;
            try
            {
                if (resObj._action2ParamDataMap.ContainsKey(callFun))
                    callFun(retInstanceObj, resObj._action2ParamDataMap[callFun]);
                else
                    callFun(retInstanceObj, null);
            }
            catch (Exception e)
            {
                ClientLog.Instance.LogError("SpawnObject error res:" + resObj._name + " " + e);
            }

        }
        resObj._actionList.Clear();
        resObj._action2ParamDataMap.Clear();
    }

    //直接返回資源
    private void SpawnResource<T>(ResourceObject resObj) where T : UnityEngine.Object
    {
        foreach (System.Action<UnityEngine.Object, ParamData> callFun in resObj._actionList)
        {
            if (!_objUsedResource.ContainsKey(resObj._res))
            {
                //缓存对象与资源关系
                _objUsedResource.Add(resObj._res, resObj._name);
            }
            //实际分配完毕后增加引用计数
            resObj._refCount++;
            try
            {
                if (resObj._action2ParamDataMap.ContainsKey(callFun))
                    callFun(resObj._res, resObj._action2ParamDataMap[callFun]);
                else
                    callFun(resObj._res, null);
            }
            catch (Exception e)
            {
                ClientLog.Instance.LogError("SpawnResource error res:" + resObj._name + " " + e);
            }
        }
        resObj._actionList.Clear();
        resObj._action2ParamDataMap.Clear();
    }

    //不加载资源直接回调
    private void OnlyLoadAssetBundleCallBack(ResourceObject resObj)
    {
        foreach (System.Action<UnityEngine.Object, ParamData> callFun in resObj._actionList)
        {
            //记录场景名与AB包名映射
            _sceneName2ABNameMap.Add(resObj._name, resObj._abName);
            resObj._refCount++;
            try
            {
                if (resObj._action2ParamDataMap.ContainsKey(callFun))
                    callFun(null, resObj._action2ParamDataMap[callFun]);
                else
                    callFun(null, null);
            }
            catch (Exception e)
            {
                ClientLog.Instance.LogError("OnlyLoadAssetBundleCallBack error res:" + resObj._name + " " + e);
            }
        }
        resObj._actionList.Clear();
        resObj._action2ParamDataMap.Clear();
    }

    private void LoadResourceFinish(ResourceObject resObj)
    {
        //Shader/Mesh/Material/Texture类型资源无须再次实例化
        switch (resObj._intType)
        {
            case TypeInts.GameObject:
                SpawnObject<UnityEngine.GameObject>(resObj);
                break;
            case TypeInts.Object:
                SpawnObject<UnityEngine.Object>(resObj);
                break;
            case TypeInts.Sprite:
                SpawnResource<UnityEngine.Sprite>(resObj);
                break;
            case TypeInts.Material:
                SpawnResource<UnityEngine.Material>(resObj);
                break;
            case TypeInts.TextAsset:
                SpawnResource<UnityEngine.TextAsset>(resObj);
                break;
            case TypeInts.Texture:
                SpawnResource<UnityEngine.Texture>(resObj);
                break;
            case TypeInts.Atlas:
                SpawnResource<UnityEngine.ScriptableObject>(resObj);
                break;
            case TypeInts.ColorTransform:
                SpawnResource<UnityEngine.ScriptableObject>(resObj);
                break;
            case TypeInts.AudioClip:
                SpawnObject<UnityEngine.AudioClip>(resObj);
                break;
            case TypeInts.Texture2D:
                SpawnResource<UnityEngine.Texture2D>(resObj);
                break;
            case TypeInts.AnimClip:
                SpawnObject<UnityEngine.AnimationClip>(resObj);
                break;
            case TypeInts.Shader:
                SpawnResource<Shader>(resObj);
                break;
            case TypeInts.AnimatorOverrideController:
                SpawnObject<UnityEngine.AnimatorOverrideController>(resObj);
                break;
            case TypeInts.RuntimeAnimatorController:
                SpawnResource<RuntimeAnimatorController>(resObj);
                break;
            case TypeInts.SceneInstance:
            case TypeInts.SceneZombie:
            case TypeInts.AssetBundle:
                OnlyLoadAssetBundleCallBack(resObj);
                break;
            case TypeInts.AssetBundleManifest://感觉没有这种情况，先不处理
                break;
        }
    }

    private void LoadAssetBundle(ResourceObject resObj)
    {
        AssetBundleManager.Instance.LoadAsync(resObj._abName, resObj._intType, (AssetBundle _ab) =>
        {
            if (_ab == null)
            {
                string errormsg = string.Format("LoadResource resName:" + resObj._name + " ab:" + resObj._abName + " NULL error !");
                ClientLog.Instance.LogError(errormsg);
            }
            else
            {
                //加入已加载列表
                _loadedResourceMap.Add(resObj._name, resObj);
                //从加载中列表取出
                _loadingResourceMap.Remove(resObj._name);
                //需要加载资源的类型先加载资源
                if (resObj._intType != TypeInts.SceneInstance && resObj._intType != TypeInts.SceneZombie && resObj._intType != TypeInts.AssetBundle)
                {
                    //从AB包加载资源
                    AssetBundleRequest abr = _ab.LoadAssetAsync(resObj._name);
                    resObj._res = abr.asset;
                    //分配GameObject
                    LoadResourceFinish(resObj);
                }
                else
                {
                    //分配GameObject
                    LoadResourceFinish(resObj);
                }
            }
        }, resObj._abFolderPath);
    }

    private void PreLoadResource(string resName, int intType, string abName, System.Action<UnityEngine.Object, ParamData> finishLoadObjectHandler, string abFolderPath, ParamData paramData)
    {
        //判断是否已有资源
        ResourceObject resObj = null;
        //如果已经加载过的资源，分配GameObject
        if (_loadedResourceMap.ContainsKey(resName))
        {
            resObj = _loadedResourceMap[resName];
            resObj._actionList.Add(finishLoadObjectHandler);
            resObj._updateTimes = 1;
            if (paramData != null)
                resObj._action2ParamDataMap.Add(finishLoadObjectHandler, paramData);
            return;
        }
        //如果是正在加载的资源，加入回调函数
        if (_loadingResourceMap.ContainsKey(resName))
        {
            resObj = _loadingResourceMap[resName];
            resObj._actionList.Add(finishLoadObjectHandler);
            if (paramData != null)
                resObj._action2ParamDataMap.Add(finishLoadObjectHandler, paramData);
            return;
        }
        //如果是等待加载的资源，加入回调函数
        if (_waitingResourceMap.ContainsKey(resName))
        {
            resObj = _waitingResourceMap[resName];
            resObj._actionList.Add(finishLoadObjectHandler);
            if (paramData != null)
                resObj._action2ParamDataMap.Add(finishLoadObjectHandler, paramData);
            return;
        }
        //如果是准备卸载的资源，加入回来，分配GameObject
        if (_unLoadResourceMap.ContainsKey(resName))
        {
            resObj = _unLoadResourceMap[resName];
            _loadedResourceMap.Add(resName, resObj);
            resObj._actionList.Add(finishLoadObjectHandler);
            resObj._updateTimes = 1;
            if (paramData != null)
                resObj._action2ParamDataMap.Add(finishLoadObjectHandler, paramData);
            _unLoadResourceMap.Remove(resName);
            return;
        }
        //如果是没有的资源，那么创建资源对象，放入正在加载队列，并开始加载
        resObj = new ResourceObject();
        resObj._name = resName;
        resObj._intType = intType;
        resObj._abName = abName;
        resObj._abFolderPath = abFolderPath;
        resObj._refCount = 0;
        resObj._actionList.Add(finishLoadObjectHandler);
        if (paramData != null)
            resObj._action2ParamDataMap.Add(finishLoadObjectHandler, paramData);
        //如果加载队列没满，放在加载队列并加载
        /*if (_loadingResourceMap.Count < MAX_LOADING_COUNT)
        {
            _loadingResourceMap.Add(resName, resObj);
            LoadAssetBundle(resObj);
        }
        else//否则放入等待队列
        {*/
        //都走等待队列强制异步
        _waitingResourceMap.Add(resName, resObj);
        //}
    }

    private void LoadResourceNonStatic(string resName, int intType, System.Action<UnityEngine.Object, ParamData> finishLoadObjectHandler, string abName = null, string abFolderPath = null, ParamData paramData = null)
    {
        if (string.IsNullOrEmpty(resName)) return;

        ClientLog.Instance.Log("加载的资源： ", resName, "  ", " 类型： ", intType.ToString(), "  外部地址： ", abFolderPath);

        //非特定包加载，包名=资源名
        if (abName == null)
        {
            //如果在映射表里，那么使用映射的名字
            if (_resName2AbNameMap.ContainsKey(resName))
            {
                abName = _resName2AbNameMap[resName];
            }
            else//否则使用资源名作为包名
            {
                abName = resName;
            }
        }
        //准备加载资源
        PreLoadResource(resName, intType, abName, finishLoadObjectHandler, abFolderPath, paramData);
    }

    private void DoUnLoadObject(UnityEngine.Object resInstanceObj)
    {
        //对象不是本模块维护的
        if (!_objUsedResource.ContainsKey(resInstanceObj))
        {
            //销毁对象
            UnityEngine.Object.DestroyImmediate(resInstanceObj, true);
            return;
        }

        //卸载对象对应资源
        string resName = _objUsedResource[resInstanceObj];
        ResourceObject resObj = null;
        if (_loadedResourceMap.ContainsKey(resName))
        {
            resObj = _loadedResourceMap[resName];
            UnLoadResource(resObj);
        }
        //不实例化对象的特殊处理
        if (resObj != null && (resObj._intType == TypeInts.RuntimeAnimatorController || resObj._intType == TypeInts.Shader))
        {
            //只有计数为0的才去掉对应关系，因为多次计数都用的同一个对象
            if (resObj._refCount <= 0)
            {
                //去掉对象与资源关系
                _objUsedResource.Remove(resInstanceObj);
                //不销毁对象(销毁了AB包里的资源就没了)
                //UnityEngine.Object.DestroyImmediate(resInstanceObj, true);
            }
        }
        else
        {
            //去掉对象与资源关系
            _objUsedResource.Remove(resInstanceObj);
            //销毁对象
            UnityEngine.Object.DestroyImmediate(resInstanceObj, true);
        }
    }

    //卸载对象池资源
    private void UnLoadPrefabInPool(UnityEngine.Object resInstanceObj)
    {
        //参数异常
        if (resInstanceObj == null) return;
        DoUnLoadObject(resInstanceObj);
        //回收对象池
        GameObjectPool.Instance.ReleaseObject(resInstanceObj as GameObject);
    }

    //卸载Object资源
    private void UnLoadObject(UnityEngine.Object resInstanceObj)
    {
        //参数异常
        if (resInstanceObj == null) return;
        DoUnLoadObject(resInstanceObj);
    }

    //卸载只加载AB包的资源
    private void UnLoadAssetBundle(string resName)
    {
        if (string.IsNullOrEmpty(resName)) return;
        if (_sceneName2ABNameMap.ContainsKey(resName))
        {
            string abName = _sceneName2ABNameMap[resName];
            //卸载对应的AB包
            AssetBundleManager.Instance.Unload(abName);
            _sceneName2ABNameMap.Remove(resName);
        }
    }

    private void UnLoadResourceNonStatic(UnityEngine.Object resObj, int intType, string resName)
    {
        switch (intType)
        {
            case TypeInts.GameObject:
            case TypeInts.Object:
            case TypeInts.Sprite:
            case TypeInts.Material:
            case TypeInts.TextAsset:
            case TypeInts.Texture:
            case TypeInts.AudioClip:
            case TypeInts.Texture2D:
            case TypeInts.AnimClip:
            case TypeInts.Shader:
            case TypeInts.Atlas:
            case TypeInts.ColorTransform:
            case TypeInts.RuntimeAnimatorController:
                UnLoadObject(resObj);
                break;
            case TypeInts.AssetBundleManifest://感觉没有这种情况，先不处理
                break;
            case TypeInts.SceneInstance:
            case TypeInts.SceneZombie:
            case TypeInts.AssetBundle:
                UnLoadAssetBundle(resName);
                break;
        }
    }

    //外部调用
    //param:
    //      resName:资源名
    //      intType:资源类型 class TypeInts
    //      finishLoadObjectHandler:加载回调函数，参数GameObject实例与ParamData参数数据
    //      abName:AB包名，如果是非主包管理下的资源包，需要传该值进行加载，否则不用传，默认使用资源名作为包名(具体映射关系后续根据情况再修改)
    //      abFolderPath:AB包文件夹路径，如果是非主包管理下的资源包，需要传该值进行加载，否则不用传
    //      paramData:参数数据，如果传了会透传到回调函数返回
    public void LoadResourceAsync(string resName, int intType, System.Action<UnityEngine.Object, ParamData> finishLoadObjectHandler, string abName = null, string abFolderPath = null, ParamData paramData = null)
    {
#if UNITY_EDITOR
        LoadResourceAssetDatabase(resName, intType, finishLoadObjectHandler, paramData);
#else
        LoadResourceNonStatic(resName, intType, finishLoadObjectHandler, abName, abFolderPath, paramData);
#endif
    }

    //卸载object资源
    //param:
    //      resObj:资源对象
    //      intType:资源类型 class TypeInts

    public void UnLoadResource(UnityEngine.Object resObj, int intType)
    {
        UnLoadResourceNonStatic(resObj, intType, null);
    }
    //卸载只加载AB包的资源
    //param:
    //      resName:如果是场景或者AB包，没有资源对象，传场景或包名
    //      intType:资源类型 class TypeInts
    public void UnLoadResource(string resName, int intType)
    {
        UnLoadResourceNonStatic(null, intType, resName);
    }

    //加载AB包中所有资源
    public void LoadAllResourceInAB<T>(string abName, System.Action<T[]> finishLoadHandler = null, string abFolderPath = null) where T : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(abName)) return;
        AssetBundleManager.Instance.LoadAsync(abName, TypeInts.AssetBundle, (AssetBundle _ab) =>
        {
            /*AssetBundleRequest request = _ab.LoadAllAssetsAsync<T>();
            var retList = request.allAssets as T[];
            finishLoadHandler(retList);*/
            try
            {
                if (finishLoadHandler != null)
                    finishLoadHandler(_ab.LoadAllAssets<T>());
            }
            catch (Exception e)
            {
                ClientLog.Instance.LogError("LoadAllResourceInAB error abName:" + abName + " error:" + e);
            }
        }, abFolderPath);
    }
    //卸载AB包
    public void UnLoadAllResourceInAB(string abName)
    {
        if (string.IsNullOrEmpty(abName)) return;
        AssetBundleManager.Instance.Unload(abName);
    }

#if UNITY_EDITOR
    /// <summary>
    /// PCEditor 走的
    /// </summary>
    /// <param name="path"></param>
    /// <param name="intType"></param>
    /// <param name="finishLoadObjectHandler"></param>
    /// <param name="paramData"></param>
    public void LoadResourceAssetDatabase(string resName, int intType, Action<UnityEngine.Object, ParamData> finishLoadObjectHandler, ParamData paramData = null)
    {
        //Shader/Mesh/Material/Texture/Atlas类型资源无须再次实例化
        UnityEngine.Object obj = null;
        string path = EditorDebug.Path(resName);
        switch (intType)
        {
            case TypeInts.GameObject:
            case TypeInts.Object:
            case TypeInts.AudioClip:
            case TypeInts.AnimClip:
                GameObject resObj = UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                obj = GameObject.Instantiate(resObj);
                break;
            case TypeInts.Sprite:
            case TypeInts.Material:
            case TypeInts.TextAsset:
            case TypeInts.Texture:
            case TypeInts.Texture2D:
            case TypeInts.Atlas:
            case TypeInts.ColorTransform:
            case TypeInts.Shader:
                obj = UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(object)); ;
                break;
        }

        finishLoadObjectHandler?.Invoke(obj, paramData);
    }
#endif
}
