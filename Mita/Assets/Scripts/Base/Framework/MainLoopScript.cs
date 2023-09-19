using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

///Unity开始运行 逻辑流程控制
public class MainLoopScript : MonoBehaviour
{
    class NextFrameCallInfo
    {
        public int iFrameCount = 0;
        public object objParam;
        public NextFrameCallHandler CallFunction;
    }
    //
    public delegate void UpdateHandler(float delta);
    public delegate void NextFrameCallHandler(object ObjParam);

    private static int mFrameCount = 0;
    private static float[] fCheckTime = new float[(int)EUpdatePriority.Max];
    private static float[] fElapseTime = new float[(int)EUpdatePriority.Max];
    private static Hashtable mapUpdateHandler = new Hashtable();
    private static List<NextFrameCallInfo> listNextFrameCallInfo = new List<NextFrameCallInfo>();
    private static List<System.Action> handles = new List<System.Action>();

    static public void AddUpdateHandler(EUpdatePriority priority, UpdateHandler h)
    {
        if (!mapUpdateHandler.Contains(priority)) mapUpdateHandler.Add(priority, new List<UpdateHandler>());
        List<UpdateHandler> listHandler = (List<UpdateHandler>)mapUpdateHandler[priority];
        if (!listHandler.Contains(h)) listHandler.Add(h);
    }

    /// <summary>
    /// 添加下一帧或者多帧再调用的函数，只调用一次 
    /// </summary>
    /// <param name="CallFunction"></param>
    /// <param name="objParam"></param>
    /// <param name="iDelayFrame"></param>
    static public void AddNextFrameCallHandler(NextFrameCallHandler CallFunction, object objParam, int iDelayFrame = 1, bool bClearSameCall = false)
    {
        if (bClearSameCall && listNextFrameCallInfo.Count > 0)
        {
            for (int iLoop = 0; iLoop < listNextFrameCallInfo.Count; ++iLoop)
            {
                if (listNextFrameCallInfo[iLoop].CallFunction == CallFunction)
                {
                    listNextFrameCallInfo.RemoveAt(iLoop);
                    break;
                }
            }
        }

        NextFrameCallInfo pInfo = new NextFrameCallInfo();
        pInfo.CallFunction = CallFunction;
        pInfo.objParam = objParam;
        pInfo.iFrameCount = mFrameCount + iDelayFrame; // GlobalValue.GetFrameCount() 非主线程不能调用GetFrameCount
                                                       //
        listNextFrameCallInfo.Add(pInfo);
    }

    //===============================================
    //
    //
    static public void DelUpdateHandler(UpdateHandler h)
    {
        if (mapUpdateHandler.Count < 1)
            return;

        foreach (DictionaryEntry entry in mapUpdateHandler)
        {
            List<UpdateHandler> listHandler = (List<UpdateHandler>)entry.Value;
            if (listHandler.Contains(h))
            {
                listHandler.Remove(h);
            }
        }
    }

    //===============================================
    //
    //
    void Awake()
    {
        Application.targetFrameRate = 30;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 后台处理
    /// </summary>
    /// <param name="focus"></param>
    void OnApplicationFocus(bool focus)
    {
        // ClientLog.Instance.Log("unity OnApplicationFocus：" + focus);
        if (focus)
        {
#if UNITY_ANDROID

#elif UNITY_IPHONE

#endif
        }
    }

    //===============================================
    //
    //
    void Start()
    {


        try
        {
            // 初始化一些东西
            UIManager.Instance.Init();
            SceneMgr.Instance.OnRelease();
            AssetBundleManager.Instance.OnRelease();
            ResourceMgr.Instance.OnRelease();
            ProfileMgr.Instance.OnRelease();
            MessageBox.Instance.OnRelease();
            UISpriteMgr.Instance.OnRelease();
            DataLoader.Instance.OnRelease();
            RedDotMgr.Instance.OnRelease();

            TimeMgr.Instance.Init();
            DataLoader.Instance.LoadTableData(); //加载表数据资源
            //GuideCore.Instance.OnInit();
            //
            fCheckTime[(int)EUpdatePriority.Realtime] = 0f;
            fCheckTime[(int)EUpdatePriority.High] = 1f;
            fCheckTime[(int)EUpdatePriority.Normal] = 5f;
            fCheckTime[(int)EUpdatePriority.Low] = 10f;

            fElapseTime[(int)EUpdatePriority.Realtime] = 0f;
            fElapseTime[(int)EUpdatePriority.High] = 0f;
            fElapseTime[(int)EUpdatePriority.Normal] = 0f;
            fElapseTime[(int)EUpdatePriority.Low] = 0f;

            ClientLog.Instance.Init();
        }
        catch (System.Exception ex)
        {
            ClientLog.Instance.LogError(ex.ToString());
        }
    }

    void OnDestroy()
    {
        SceneMgr.Instance.OnRelease();
        AssetBundleManager.Instance.OnRelease();
        ResourceMgr.Instance.OnRelease();
        ProfileMgr.Instance.OnRelease();
        MessageBox.Instance.OnRelease();
        UIManager.Instance.OnRelease();
        UISpriteMgr.Instance.OnRelease();
        DataLoader.Instance.OnRelease();
        RedDotMgr.Instance.OnRelease();
    }

    //===============================================
    //
    //
    void Update()
    {
        try
        {
            // 必须放在最前面 
            //资源下载的Update 放到最前面
            AssetBundleManager.Instance.Update();
            ResourceMgr.Instance.Update();
            EventMgr.Instance.Update();
            LaunchUpdateHandler(EUpdatePriority.Realtime);
            LaunchNextFrameHandler();
            LaunchUpdateHandler(EUpdatePriority.High);
            LaunchUpdateHandler(EUpdatePriority.Normal);
            LaunchUpdateHandler(EUpdatePriority.Low);
            //UIManager.Instance.Update();
            MessageBox.Instance.Update();
            RedDotMgr.Instance.Update();
        }
        catch (System.Exception ex)
        {
            ClientLog.Instance.LogError(ex.ToString());
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void LateUpdate()
    {
        try
        {
            //UIManager.Instance.LateUpdate();
        }
        catch (System.Exception ex)
        {
            ClientLog.Instance.LogError(ex.ToString());
        }
    }

    //===============================================
    //
    //
    static void LaunchUpdateHandler(EUpdatePriority priority)
    {
        try
        {
            if (!mapUpdateHandler.Contains(priority)) return;
            if (EUpdatePriority.Realtime != priority && fElapseTime[(int)priority] < fCheckTime[(int)priority])
            {
                fElapseTime[(int)priority] += Time.deltaTime;
                return;
            }

            List<UpdateHandler> listHandler = (List<UpdateHandler>)mapUpdateHandler[priority];
            float fDeltaTime = EUpdatePriority.Realtime == priority ? Time.deltaTime : fElapseTime[(int)priority];
            if (listHandler != null)
            {
                for (int iLoop = 0; iLoop < listHandler.Count; iLoop++)
                {
                    listHandler[iLoop](fDeltaTime);
                }
            }
            fElapseTime[(int)priority] = 0f;
        }
        catch (System.Exception ex)
        {
            ClientLog.Instance.LogError(ex.ToString());
        }
    }

    /// <summary>
    /// 处理隔帧调用 
    /// </summary>
    static void LaunchNextFrameHandler()
    {
        try
        {
            mFrameCount = Time.frameCount;
            for (int iLoop = 0; iLoop < listNextFrameCallInfo.Count;)
            {
                NextFrameCallInfo pInfo = listNextFrameCallInfo[iLoop];
                if (mFrameCount >= pInfo.iFrameCount)
                {
                    if (null != pInfo.CallFunction)
                        pInfo.CallFunction(pInfo.objParam);
                    //
                    listNextFrameCallInfo.RemoveAt(iLoop);
                }
                else
                {
                    ++iLoop;
                }
            }
        }
        catch (System.Exception ex)
        {
            ClientLog.Instance.LogError(ex.ToString());
        }
    }


    public static void AddLateUpdateHandle(System.Action action)
    {
        if (handles != null)
        {
            handles.Add(action);
        }
    }

    public static void RemoveLateUpdateHandle(System.Action action)
    {
        if (handles != null)
        {
            handles.Remove(action);
        }
    }
}
