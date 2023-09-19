using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;


public enum ELoadMsgeType
{
    LOADDLL = 1999, // 加载Dll
}


public class DllParam
{
    public string path;
}
public class DllName
{
    public List<string> names;
    public List<string> updateHotDll;
}

/// <summary>
/// 游戏入口 加载热更新的DLL
/// </summary>
public class LoadDll : MonoBehaviour
{
    private List<string> m_MessageList = new List<string>();
    private List<string> m_MessageCurrent = new List<string>();

    private int LoadCount = 0;
    private void Start()
    {
        Debug.Log("加载");
        //测试代码
        //LoadDllAssets(Application.streamingAssetsPath + "/IOS/");
    }

    public void AppToUnity(string msg)
    {
        lock (m_MessageList)
        {
            m_MessageList.Add(msg);
            CacheMessageMgr.Instance.SetMessageQueue(msg);
        }
    }

    private void Update()
    {
        //
        lock (m_MessageList)
        {
            if (m_MessageList.Count <= 0) return;
            m_MessageCurrent.Clear();
            m_MessageCurrent.AddRange(m_MessageList);
            m_MessageList.Clear();
        }

        foreach (var s in m_MessageCurrent)
        {
            var data = JObject.Parse(s);
            HandleMessage(Convert.ToInt32(data["messageType"]), data["content"].ToString());
        }
    }

    private void HandleMessage(int type, string content)
    {
        switch ((ELoadMsgeType)type)
        {
            case ELoadMsgeType.LOADDLL:
                Debug.Log("收到加载消息");
                var dllParam = JsonConvert.DeserializeObject<DllParam>(content);
                Debug.Log("收到加载消息:" + dllParam.path);
                string path = dllParam.path;
                if (!path.EndsWith("/"))
                {
                    path += "/";
                }
                LoadDllAssets(path);
                break;
        }
    }

    public static List<string> AOTMetaAssemblyNames = new List<string>()
    {
        "mscorlib.dll",
        "System.dll",
        "System.Core.dll",
    };

    public static List<string> UpdateAssemblyNames = new List<string>()
    {
        "Assembly-CSharp.dll"
    };

    public void LoadDllAssets(string path)
    {
        if (!string.IsNullOrEmpty(path))
        {
            //找到下载的json文件 要热更新哪些dll 
            string dllStr = StreamReaderJsonFromPath(path + "updateDll.json");
            DllName avatarRes = JsonConvert.DeserializeObject<DllName>(dllStr);
            if (avatarRes != null)
            {
                if (avatarRes.names != null && avatarRes.names.Count > 0)
                {
                    AOTMetaAssemblyNames.Clear();
                    foreach (var item in avatarRes.names)
                    {
                        AOTMetaAssemblyNames.Add(item);
                    }
                }
                if (avatarRes.updateHotDll != null && avatarRes.updateHotDll.Count > 0)
                {
                    UpdateAssemblyNames.Clear();
                    foreach (var item in avatarRes.updateHotDll)
                    {
                        UpdateAssemblyNames.Add(item);
                    }
                }
            }
        }
        else
        {
            path = $"{Application.streamingAssetsPath}/";
        }

        //StartCoroutine(DownLoadAssets(path, this.StartGame));
        StartCoroutine(LoadDllAssetBundle(path, this.StartGame));
    }

    private static Dictionary<string, byte[]> s_assetDatas = new Dictionary<string, byte[]>();

    public static byte[] GetAssetData(string dllName)
    {
        return s_assetDatas[dllName];
    }

    private string GetAssetStreamPath(string path, string name)
    {
        path += name;
        if (!path.Contains("://"))
        {
            path = "file://" + path;
        }
        return path;
    }

    public string StreamReaderJsonFromPath(string url)
    {
        string path = GetWebRequestPath(url);
        //读取
        Encoding endoning = Encoding.GetEncoding("utf-8");//识别Json数据内容中文字段
        string str = "";
        try
        {
            StreamReader streamReader = new StreamReader(path, endoning);
            str = streamReader.ReadToEnd();
            streamReader.Close();
            streamReader.Dispose();
        }
        catch (Exception)
        {
            Debug.Log("StreamReaderJsonFromPath ERROR");
        }

        return str;
    }

    private string GetWebRequestPath(string url)
    {
        string str = Uri.UnescapeDataString(url);
        string path = str.Replace("file://", "");
        return path;
    }

    IEnumerator DownLoadAssets(string path, Action<string> onDownloadComplete)
    {
        LoadCount = 0;
        int totalCount = AOTMetaAssemblyNames.Count + UpdateAssemblyNames.Count;
        foreach (var asset in AOTMetaAssemblyNames)
        {
            Debug.Log("start download 1 asset:" + GetAssetStreamPath(path, asset) + ".bytes");
            UnityWebRequest www = UnityWebRequest.Get(GetAssetStreamPath(path, asset));
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("加载失败:" + www.error);
            }
            else
            {
                // Or retrieve results as binary data
                byte[] assetData = www.downloadHandler.data;
                Debug.Log($"dll:{asset}  size:{assetData.Length}");
                s_assetDatas[asset] = assetData;
                LoadCount++;
                if (LoadCount >= totalCount)
                {
                    onDownloadComplete?.Invoke(path);
                }
            }
        }
        foreach (var asset in UpdateAssemblyNames)
        {
            Debug.Log("start download 1 asset:" + GetAssetStreamPath(path, asset) + ".bytes");
            UnityWebRequest www = UnityWebRequest.Get(GetAssetStreamPath(path, asset));
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("加载失败:" + www.error);
            }
            else
            {
                // Or retrieve results as binary data
                byte[] assetData = www.downloadHandler.data;
                Debug.Log($"dll:{asset}  size:{assetData.Length}");
                s_assetDatas[asset] = assetData;
                LoadCount++;
                if (LoadCount >= totalCount)
                {
                    onDownloadComplete?.Invoke(path);
                }
            }
        }
    }

    IEnumerator LoadDllAssetBundle(string path, Action<string> onDownloadComplete)
    {
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(GetAssetStreamPath(path, "updatedlls"), 0);
        yield return request.SendWebRequest();
        try
        {
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
            if (bundle != null)
            {
                AssetBundleRequest abr = bundle.LoadAllAssetsAsync();
                System.Object[] assetList = abr.allAssets;
                foreach (System.Object asset in assetList)
                {
                    TextAsset assetBytes = (TextAsset)asset;
                    s_assetDatas[assetBytes.name] = assetBytes.bytes;
                    Debug.Log($"dll:{assetBytes.name}  size:{assetBytes.bytes.Length}");
                }
                bundle.Unload(false);
                onDownloadComplete?.Invoke(path);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }


    void StartGame(string path)
    {
        LoadMetadataForAOTAssemblies();

#if !UNITY_EDITOR
        LoadUpdateDll();
#endif
        LoadSceneAssetBundle();
    }

    void LoadUpdateDll()
    {
        foreach (var updateDllName in UpdateAssemblyNames)
        {
            try
            {
                Debug.Log($"LoadUpdateDll:{updateDllName}");
                System.Reflection.Assembly.Load(GetAssetData(updateDllName));
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }

    private string GetSceneABPath()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return Application.streamingAssetsPath + "/StandaloneWindows/";
#elif UNITY_IPHONE && !UNITY_EDITOR
                return Application.streamingAssetsPath + "/IOS/";
#elif UNITY_ANDROID && !UNITY_EDITOR
                return Application.streamingAssetsPath + "/Android/";
#endif
    }

    private void LoadSceneAssetBundle(ulong offset = 421)
    {
        AssetBundle bundle = AssetBundle.LoadFromFile(GetSceneABPath() + "gamemain", 0, offset);
        if (bundle == null)
        {
            string errormsg = string.Format("LoadDll.LoadSceneAssetBundle ab NULL error !");
            Debug.Log(errormsg);
            return;
        }
        AsyncOperation _sceneAsync = SceneManager.LoadSceneAsync("GameMain");
        _sceneAsync.completed += operation =>
        {
            //给App 发消息
            // AppBridge.SendMessageToAvatarApp("{\"messageType\":2999,\"content\":\"可以走原来的了\"}");
            Debug.Log("消息发送完毕");
            bundle.Unload(false);
        };
    }

    private IEnumerator LoadSceneAssetBundle(string path, ulong offset = 421)
    {
        var bundleLoadRequest = AssetBundle.LoadFromFileAsync(path, 0, offset);

        yield return bundleLoadRequest;

        var loadAssetBundle = bundleLoadRequest.assetBundle;
        if (loadAssetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            yield break;
        }
        else
        {
            AsyncOperation _sceneAsync = SceneManager.LoadSceneAsync("GameMain");
            _sceneAsync.completed += operation =>
            {
                //给App 发消息 
                //AppBridge.SendMessageToAvatarApp("{\"messageType\":2999,\"content\":\"可以走原来的了\"}");
            };
            loadAssetBundle.Unload(false);
        }
    }


    /// <summary>
    /// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
    /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
    /// </summary>
    private static void LoadMetadataForAOTAssemblies()
    {

    }
}
