using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class AssetBundleManager : Singleton<AssetBundleManager>
{
    public delegate void AssetBundleLoadCallBack(AssetBundle ab);

    private class AssetBundleObject
    {
        public string _hashName;
        public string _abFolderPath;
        public int _intType;
        public int _refCount;
        public List<AssetBundleLoadCallBack> _callFunList = new List<AssetBundleLoadCallBack>();

        public AssetBundleCreateRequest _request;
        public UnityWebRequest _webRequest;
        public AssetBundle _ab;

        public int _dependLoadingCount;
        public List<AssetBundleObject> _depends = new List<AssetBundleObject>();

        public void OnRelease()
        {
            if (_ab != null)
            {
                _ab.Unload(true);
                _ab = null;
            }
            else if (_request != null && _request.assetBundle != null)
            {
                _request.assetBundle.Unload(true);
            }
            _request = null;
            _webRequest = null;
            _depends.Clear();
            _callFunList.Clear();
        }
    }


    private const int MAX_LOADING_COUNT = 10; //同时加载的最大数量

    private List<AssetBundleObject> tempLoadeds; //创建临时存储变量，用于提升性能

    private Dictionary<string, string[]> _dependsDataList = new Dictionary<string, string[]>();

    private Dictionary<string, AssetBundleObject> _readyABList = new Dictionary<string, AssetBundleObject>(); //预备加载的列表
    private Dictionary<string, AssetBundleObject> _loadingABList = new Dictionary<string, AssetBundleObject>(); //正在加载的列表
    private Dictionary<string, AssetBundleObject> _loadedABList = new Dictionary<string, AssetBundleObject>(); //加载完成的列表
    private Dictionary<string, AssetBundleObject> _unloadABList = new Dictionary<string, AssetBundleObject>(); //准备卸载的列表
    private readonly ulong offset = 421;
    public override void OnRelease()
    {
        _dependsDataList = new Dictionary<string, string[]>();

        if (_readyABList != null && _readyABList.Count > 0)
        {
            foreach (AssetBundleObject abObj in _readyABList.Values)
            {
                abObj.OnRelease();
            }
            _readyABList.Clear();
        }
        if (_loadingABList != null && _loadingABList.Count > 0)
        {
            foreach (AssetBundleObject abObj in _loadingABList.Values)
            {
                abObj.OnRelease();
            }
            _loadingABList.Clear();
        }
        if (_loadedABList != null && _loadedABList.Count > 0)
        {
            foreach (AssetBundleObject abObj in _loadedABList.Values)
            {
                abObj.OnRelease();
            }
            _loadedABList.Clear();
        }
        if (_unloadABList != null && _unloadABList.Count > 0)
        {
            foreach (AssetBundleObject abObj in _unloadABList.Values)
            {
                abObj.OnRelease();
            }
            _unloadABList.Clear();
        }
        _readyABList = new Dictionary<string, AssetBundleObject>();
        _loadingABList = new Dictionary<string, AssetBundleObject>();
        _loadedABList = new Dictionary<string, AssetBundleObject>();
        _unloadABList = new Dictionary<string, AssetBundleObject>();
        tempLoadeds = new List<AssetBundleObject>();
    }

    public override void Init()
    {
        OnRelease();
        LoadMainfest(GetMainName());
    }

    private string GetMainName()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return "StandaloneWindows";
#elif UNITY_IPHONE && !UNITY_EDITOR
        return "IOS";
#elif UNITY_ANDROID && !UNITY_EDITOR
        return "Android";
#endif
    }

    public void LoadMainfest(string mainName)
    {
        if (string.IsNullOrEmpty(mainName)) return;

        string path = GetAssetBundlePath(mainName);

        _dependsDataList.Clear();
        AssetBundle ab = AssetBundle.LoadFromFile(path, 0, offset);

        if (ab == null)
        {
            string errormsg = string.Format("LoadMainfest ab NULL error !");
            ClientLog.Instance.LogError(errormsg);
            return;
        }

        AssetBundleManifest mainfest = ab.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
        if (mainfest == null)
        {
            string errormsg = string.Format("LoadMainfest NULL error !");
            ClientLog.Instance.LogError(errormsg);
            return;
        }

        foreach (string assetName in mainfest.GetAllAssetBundles())
        {
            string hashName = assetName.Replace(".ab", "");
            string[] dps = mainfest.GetAllDependencies(assetName);
            for (int i = 0; i < dps.Length; i++)
                dps[i] = dps[i].Replace(".ab", "");
            _dependsDataList.Add(hashName, dps);
        }

        ab.Unload(true);
        ab = null;

        ClientLog.Instance.Log("AssetBundleManager dependsCount = ", _dependsDataList.Count.ToString());
    }

    private string GetHashName(string _assetName)
    {//读者可以自己定义hash方式，对内存有要求的话，可以hash成uint（或uint64）节省内存
        return _assetName.ToLower();
    }

    private string GetFileName(string _hashName)
    {//读者可以自己实现自己的对应关系
        return _hashName + ".ab";
    }

    // 获取一个资源的路径
    private string GetAssetBundlePath(string _hashName, int _intType = -1, string _abFolderPath = null)
    {
        //如果传了自己的地址，那么用自己特殊的
        if (!string.IsNullOrEmpty(_abFolderPath))
        {
            return GlobalFunction.UrlDecodeGetPath(_abFolderPath + _hashName);
        }
        //TODO:根据类型区分路径

#if UNITY_EDITOR || UNITY_STANDALONE
        //return "C:/momoroot/unityclient_avatar/AssetBundles/StandaloneWindows/" + _hashName;
        return Application.streamingAssetsPath + "/StandaloneWindows/" + _hashName;
#elif UNITY_IPHONE && !UNITY_EDITOR
                //return Application.persistentDataPath + "/"+bundlePath+"/IOS/" + _hashName;
                return Application.streamingAssetsPath +"/IOS/" + _hashName;
#elif UNITY_ANDROID && !UNITY_EDITOR
                //return "jar:file://" + Application.persistentDataPath +"/Android/" + _hashName;
                return "jar:file://" + Application.dataPath + "!/assets/" + "Android/" + _hashName;
#endif
    }

    public bool IsABExist(string _assetName)
    {
        string hashName = GetHashName(_assetName);
        return _dependsDataList.ContainsKey(hashName);
    }

    //同步加载
    public AssetBundle LoadSync(string _assetName)
    {
        string hashName = GetHashName(_assetName);
        var abObj = LoadAssetBundleSync(hashName);
        return abObj._ab;
    }

    //异步加载（已经加载直接回调），每次加载引用计数+1
    public void LoadAsync(string _assetName, int _intType, AssetBundleLoadCallBack callFun, string _abFolderPath = null)
    {
        string hashName = GetHashName(_assetName);
        LoadAssetBundleAsync(hashName, _intType, callFun, _abFolderPath);
    }
    //卸载（异步），每次卸载引用计数-1
    public void Unload(string _assetName)
    {
        string hashName = GetHashName(_assetName);
        UnloadAssetBundleAsync(hashName);
    }

    private AssetBundleObject LoadAssetBundleSync(string _hashName)
    {
        AssetBundleObject abObj = null;
        if (_loadedABList.ContainsKey(_hashName)) //已经加载
        {
            abObj = _loadedABList[_hashName];
            abObj._refCount++;

            foreach (var dpObj in abObj._depends)
            {
                LoadAssetBundleSync(dpObj._hashName); //递归依赖项，附加引用计数
            }

            return abObj;
        }
        else if (_loadingABList.ContainsKey(_hashName)) //在加载中,异步改同步
        {
            abObj = _loadingABList[_hashName];
            abObj._refCount++;

            foreach (var dpObj in abObj._depends)
            {
                LoadAssetBundleSync(dpObj._hashName); //递归依赖项，加载完
            }

            DoLoadedCallFun(abObj, false); //强制完成，回调

            return abObj;
        }
        else if (_readyABList.ContainsKey(_hashName)) //在准备加载中
        {
            abObj = _readyABList[_hashName];
            abObj._refCount++;

            foreach (var dpObj in abObj._depends)
            {
                LoadAssetBundleSync(dpObj._hashName); //递归依赖项，加载完
            }

            string path1 = GetAssetBundlePath(_hashName);
            abObj._ab = AssetBundle.LoadFromFile(path1, 0, offset);

            _readyABList.Remove(abObj._hashName);
            _loadedABList.Add(abObj._hashName, abObj);

            DoLoadedCallFun(abObj, false); //强制完成，回调

            return abObj;
        }

        //创建一个加载
        abObj = new AssetBundleObject();
        abObj._hashName = _hashName;

        abObj._refCount = 1;

        string path = GetAssetBundlePath(_hashName);
        abObj._ab = AssetBundle.LoadFromFile(path, 0, offset);

        if (abObj._ab == null)
        {
            try
            {
                //同步下载解决
                //byte[] bytes = AssetsDownloadMgr.I.DownloadSync(GetFileName(abObj._hashName));
                //if (bytes != null && bytes.Length != 0)
                //    abObj._ab = AssetBundle.LoadFromMemory(bytes);
            }
            catch (Exception ex)
            {
                ClientLog.Instance.LogError("LoadAssetBundleSync DownloadSync" + ex.Message);
            }
        }

        //加载依赖项
        string[] dependsData = null;
        if (_dependsDataList.ContainsKey(_hashName))
        {
            dependsData = _dependsDataList[_hashName];
        }

        if (dependsData != null && dependsData.Length > 0)
        {
            abObj._dependLoadingCount = 0;

            foreach (var dpAssetName in dependsData)
            {
                var dpObj = LoadAssetBundleSync(dpAssetName);

                abObj._depends.Add(dpObj);
            }

        }

        _loadedABList.Add(abObj._hashName, abObj);

        return abObj;
    }

    private void UnloadAssetBundleAsync(string _hashName)
    {
        AssetBundleObject abObj = null;
        if (_loadedABList.ContainsKey(_hashName))
            abObj = _loadedABList[_hashName];
        else if (_loadingABList.ContainsKey(_hashName))
            abObj = _loadingABList[_hashName];
        else if (_readyABList.ContainsKey(_hashName))
            abObj = _readyABList[_hashName];

        if (abObj == null)
        {
            string errormsg = string.Format("UnLoadAssetbundle error ! assetName:{0}", _hashName);
            ClientLog.Instance.LogError(errormsg);
            return;
        }

        if (abObj._refCount == 0)
        {
            string errormsg = string.Format("UnLoadAssetbundle refCount error ! assetName:{0}", _hashName);
            ClientLog.Instance.LogError(errormsg);
            return;
        }

        abObj._refCount--;

        foreach (var dpObj in abObj._depends)
        {
            UnloadAssetBundleAsync(dpObj._hashName);
        }

        if (abObj._refCount == 0)
        {
            _unloadABList.Add(abObj._hashName, abObj);
        }
    }


    private AssetBundleObject LoadAssetBundleAsync(string _hashName, int _intType, AssetBundleLoadCallBack _callFun, string _abFolderPath)
    {
        AssetBundleObject abObj = null;
        if (_loadedABList.ContainsKey(_hashName)) //已经加载
        {
            abObj = _loadedABList[_hashName];
            DoDependsRef(abObj);
            _callFun(abObj._ab);
            return abObj;
        }
        else if (_loadingABList.ContainsKey(_hashName)) //在加载中
        {
            abObj = _loadingABList[_hashName];
            DoDependsRef(abObj);
            abObj._callFunList.Add(_callFun);
            return abObj;
        }
        else if (_readyABList.ContainsKey(_hashName)) //在准备加载中
        {
            abObj = _readyABList[_hashName];
            DoDependsRef(abObj);
            abObj._callFunList.Add(_callFun);
            return abObj;
        }

        //创建一个加载
        abObj = new AssetBundleObject();
        abObj._hashName = _hashName;
        abObj._abFolderPath = _abFolderPath;
        abObj._intType = _intType;
        abObj._refCount = 1;
        abObj._callFunList.Add(_callFun);

        //ClientLog.Instance.Log("ZLLog:"+"创建加载"+ _hashName);

        //加载依赖项
        string[] dependsData = null;
        if (_dependsDataList.ContainsKey(_hashName))
        {
            dependsData = _dependsDataList[_hashName];
        }

        if (dependsData != null && dependsData.Length > 0)
        {
            abObj._dependLoadingCount = dependsData.Length;
            //ClientLog.Instance.Log("ZLLog:" + _hashName + "初始化依赖计数" + abObj._dependLoadingCount);

            foreach (var dpAssetName in dependsData)
            {
                var dpObj = LoadAssetBundleAsync(dpAssetName, _intType,
                    (AssetBundle _ab) =>
                    {
                        if (abObj._dependLoadingCount <= 0)
                        {
                            string errormsg = string.Format("LoadAssetbundle depend error ! assetName:{0}", _hashName);
                            ClientLog.Instance.LogError(errormsg);
                            return;
                        }

                        abObj._dependLoadingCount--;
                        //ClientLog.Instance.Log("ZLLog:" + _hashName + "减少依赖计数" + dpAssetName);

                        //依赖加载完
                        if (abObj._dependLoadingCount == 0 && ((abObj._request != null && abObj._request.isDone) || (abObj._webRequest != null && abObj._webRequest.isDone)))
                        {
                            DoLoadedCallFun(abObj);
                        }
                    }, _abFolderPath
                );

                abObj._depends.Add(dpObj);
            }

        }

        if (_loadingABList.Count < MAX_LOADING_COUNT) //正在加载的数量不能超过上限
        {
            DoLoad(abObj);
            //ClientLog.Instance.Log("ZLLog:" + _hashName + "执行加载");
            _loadingABList.Add(_hashName, abObj);
        }
        else _readyABList.Add(_hashName, abObj);

        return abObj;
    }

    private void DoDependsRef(AssetBundleObject abObj)
    {
        abObj._refCount++;

        if (abObj._depends.Count == 0) return;
        foreach (var dpObj in abObj._depends)
        {
            DoDependsRef(dpObj); //递归依赖项，加载完
        }
    }

    private void DoLoad(AssetBundleObject abObj)
    {
        string path = GetAssetBundlePath(abObj._hashName, abObj._intType, abObj._abFolderPath);
        abObj._request = AssetBundle.LoadFromFileAsync(path, 0, offset);
        if (abObj._request == null)
        {
            string errormsg = string.Format("LoadAssetbundle path error ! assetName:{0}", abObj._hashName);
            ClientLog.Instance.LogError(errormsg);
        }
        else if (abObj._request.assetBundle == null)    //使用LoadFromFileAsync加载不到资源，走网络加载
        {
            abObj._request = null;
            CoroutineController.Instance.StartCoroutine(LoadAssetBundleFromUrl(abObj));
        }
    }

    private IEnumerator LoadAssetBundleFromUrl(AssetBundleObject abObj)
    {
        string uri = GetAssetBundlePath(abObj._hashName, abObj._intType, abObj._abFolderPath);
        ClientLog.Instance.Log("uri:路径 ", uri);
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(uri, 0);
        yield return request.Send();
        try
        {
            abObj._webRequest = request;
        }
        catch (Exception e)
        {
            ClientLog.Instance.Log(e.Message);
        }
    }

    private void DoLoadedCallFun(AssetBundleObject abObj, bool isAsync = true)
    {
        //提取ab
        if (abObj._request != null)
        {
            abObj._ab = abObj._request.assetBundle; //如果没加载完，会异步转同步
            abObj._request = null;
            _loadingABList.Remove(abObj._hashName);
            _loadedABList.Add(abObj._hashName, abObj);
        }
        else if (abObj._webRequest != null)
        {
            try
            {
                abObj._ab = DownloadHandlerAssetBundle.GetContent(abObj._webRequest);
            }
            catch (Exception ex)
            {
                ClientLog.Instance.LogError("DoLoadedCallFun _webRequest Error " + ex.Message);
            }
            abObj._webRequest = null;
            _loadingABList.Remove(abObj._hashName);
            _loadedABList.Add(abObj._hashName, abObj);
        }


        if (abObj._ab == null)
        {
            string errormsg = string.Format("LoadAssetbundle _ab null error ! assetName:{0}", abObj._hashName);
            string path = GetAssetBundlePath(abObj._hashName, abObj._intType, abObj._abFolderPath);
            errormsg += "\n File " + File.Exists(path) + " Exists " + path;

            try
            {//尝试读取二进制解决
                if (File.Exists(path))
                {
                    byte[] bytes = File.ReadAllBytes(path);
                    if (bytes != null && bytes.Length != 0)
                        abObj._ab = AssetBundle.LoadFromMemory(bytes);
                }
            }
            catch (Exception ex)
            {
                ClientLog.Instance.LogError("LoadAssetbundle ReadAllBytes Error " + ex.Message);
            }

            if (abObj._ab == null)
            {
                ClientLog.Instance.LogError("LoadAssetbundle Fail :" + path);
            }
        }

        //运行回调
        foreach (var callback in abObj._callFunList)
        {
            try
            {
                callback(abObj._ab);
            }
            catch (Exception ex)
            {
                ClientLog.Instance.LogError("LoadAssetbundle callback " + abObj._hashName + " Error: " + ex.Message);
            }
        }
        abObj._callFunList.Clear();
    }

    private void UpdateLoad()
    {
        if (_loadingABList.Count == 0) return;
        //检测加载完的
        tempLoadeds.Clear();
        foreach (var abObj in _loadingABList.Values)
        {
            if (abObj._dependLoadingCount == 0 && ((abObj._request != null && abObj._request.isDone) || (abObj._webRequest != null && abObj._webRequest.isDone)))
            {
                tempLoadeds.Add(abObj);
            }
        }
        //回调中有可能对_loadingABList进行操作，提取后回调
        foreach (var abObj in tempLoadeds)
        {
            //加载完进行回调
            DoLoadedCallFun(abObj);
        }

    }

    private void DoUnload(AssetBundleObject abObj)
    {
        //这里用true，卸载Asset内存，实现指定卸载
        if (abObj._ab == null)
        {
            string errormsg = string.Format("LoadAssetbundle DoUnload error ! assetName:{0}", abObj._hashName);
            ClientLog.Instance.LogError(errormsg);
            return;
        }

        abObj._ab.Unload(true);
        abObj._ab = null;
        //ClientLog.Instance.Log("ZLLog:" + "卸载包"+ abObj._hashName);
    }

    private void UpdateUnLoad()
    {
        if (_unloadABList.Count == 0) return;

        tempLoadeds.Clear();
        foreach (var abObj in _unloadABList.Values)
        {
            if (abObj._refCount == 0 && abObj._ab != null)
            {//引用计数为0并且已经加载完，没加载完等加载完销毁
                DoUnload(abObj);
                _loadedABList.Remove(abObj._hashName);

                tempLoadeds.Add(abObj);
            }

            if (abObj._refCount > 0)
            {//引用计数加回来（销毁又瞬间重新加载，不销毁，从销毁列表移除）
                tempLoadeds.Add(abObj);
            }
        }

        foreach (var abObj in tempLoadeds)
        {
            _unloadABList.Remove(abObj._hashName);
        }
    }

    private void UpdateReady()
    {
        if (_readyABList.Count == 0) return;
        if (_loadingABList.Count >= MAX_LOADING_COUNT) return;

        tempLoadeds.Clear();
        foreach (var abObj in _readyABList.Values)
        {
            //ClientLog.Instance.Log("ZLLog:" + abObj._hashName + "Ready执行加载");
            DoLoad(abObj);

            tempLoadeds.Add(abObj);
            _loadingABList.Add(abObj._hashName, abObj);

            if (_loadingABList.Count >= MAX_LOADING_COUNT) break;
        }

        foreach (var abObj in tempLoadeds)
        {
            _readyABList.Remove(abObj._hashName);
        }
    }

    public void Update()
    {
        UpdateLoad();
        UpdateReady();
        UpdateUnLoad();
    }
}