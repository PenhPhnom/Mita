/// <summary>
/// FreeCoroutine.cs
/// Desc:   自由携程，随意启动
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void CoroutineRequestDele(int id);

///	@mainpage   FreeCoroutine
/// FreeCoroutine is an easy use replacement of UnityCoroutine,can be called from anywhere,also support suspend/resume, kill,nesting and coroutine chain
/// 
///	@subpage    quick_start_page  @n
///		
/// 
/// @page quick_start_page  Quick Start
///     1.Create a GameObject. @n
///		2.Attach FreeCoroutine script to it. @n
///		3.Use FreeCoroutine.StartCoroutineRequest(IEnumerator coroutine) to start your coroutine any where you like. @n
///	

/// <summary>
/// Courtine state
/// </summary>
public enum CoroutineState
{
    Running,
    Suspend,
    Stop,
}
/// <summary>
/// Coroutine request.
/// </summary>
public class CoroutineRequest
{
    /// <summary>
    /// Occurs when coroutine request complete.
    /// </summary>
    public event CoroutineRequestDele OnCoroutineRequestComplete;

    /// <summary>
    /// Nesting Coroutine Stack.
    /// </summary>
    Stack<IEnumerator> mCoroutineStack = new Stack<IEnumerator>();

    /// <summary>
    /// The coroutine resquest waiting.
    /// Waiting CoroutineRequest will resume, when this CoroutineRequest Stoped.
    /// </summary>
    List<int> mChildrenCoroutineList;

    /// <summary>
    /// index of coroutine request.
    /// </summary>
    int mId;
    /// <summary>
    /// index of coroutine request.
    /// </summary>
    public int Id
    {
        get { return mId; }
    }
    /// <summary>
    /// the state of this coroutine request.
    /// </summary>
    CoroutineState mState;
    /// <summary>
    /// Gets the state.
    /// </summary>
    /// <value>The state.</value>
    public CoroutineState State
    {
        get { return mState; }
    }

    Coroutine m_runningCorotine;
    /// <summary>
    /// Initializes a new instance of the <see cref="CoroutineRequest"/> class.
    /// </summary>
    /// <param name="id">Identifier.</param>
    /// <param name="coroutine">Coroutine.</param>
    public CoroutineRequest(int id, IEnumerator coroutine)
    {
        mId = id;
        mCoroutineStack.Push(coroutine);
    }

    public void Start()
    {
        m_runningCorotine = FreeCoroutine.Instance.StartCoroutine(Update());
    }

    /// <summary>
    /// Suspend this CoroutineRequest.
    /// </summary>
    public void Suspend()
    {
        if (mState == CoroutineState.Running)
        {
            mState = CoroutineState.Suspend;
        }
    }
    /// <summary>
    /// Resume this CoroutineRequest.
    /// </summary>
    public void Resume()
    {
        if (mState == CoroutineState.Suspend)
        {
            mState = CoroutineState.Running;
        }
    }
    /// <summary>
    /// Stop this CoroutineRequest.
    /// </summary>
    public void Stop()
    {
        mState = CoroutineState.Stop;
    }

    public void Release()
    {
        if (mCoroutineStack != null)
        {
            mCoroutineStack.Clear();
            mCoroutineStack = null;
        }

        if (mChildrenCoroutineList != null)
        {
            mChildrenCoroutineList.Clear();
            mChildrenCoroutineList = null;
        }

        this.OnCoroutineRequestComplete = null;

        if (m_runningCorotine != null)
        {
            FreeCoroutine.Instance.StopCoroutine(this.m_runningCorotine);
            this.m_runningCorotine = null;
        }
    }
    /// <summary>
    /// Adds the child coroutine request.
    /// child coroutine request will be suspend
    /// </summary>
    /// <param name="id">Identifier.</param>
    public void AddChildCoroutineRequest(int id)
    {
        if (mChildrenCoroutineList == null)
        {
            mChildrenCoroutineList = new List<int>();
        }
        mChildrenCoroutineList.Add(id);
        FreeCoroutine.SuspendCoroutine(id);
    }

    /// <summary>
    /// Update this instance.
    /// </summary>
    IEnumerator Update()
    {
        while (State != CoroutineState.Stop)
        {
            if (State == CoroutineState.Suspend)
            {
                yield return null;
            }
            else
            {
                IEnumerator e = mCoroutineStack.Peek();
                bool move_next = e.MoveNext();
                if (move_next)
                {
                    if (e.Current is IEnumerator)
                    {
                        mCoroutineStack.Push(e.Current as IEnumerator);
                        continue;
                    }
                    yield return e.Current;
                }
                else
                {
                    if (State != CoroutineState.Stop)
                    {
                        //可以出现在回调中kill的情况
                        mCoroutineStack.Pop();
                        if (mCoroutineStack.Count == 0)
                        {
                            Stop();
                        }
                    }
                }
            }
        }

        //Resume children coroutine
        if (mChildrenCoroutineList != null)
        {
            foreach (int task_id in mChildrenCoroutineList)
            {
                FreeCoroutine.ResumeCoroutine(task_id);
            }
        }
        if (OnCoroutineRequestComplete != null)
            OnCoroutineRequestComplete(Id);
    }
}
/// <summary>
/// Free coroutine.
/// </summary>
public class FreeCoroutine : MonoBehaviour
{
    #region singleton
    /// <summary>
    /// The m instance.
    /// </summary>
    static FreeCoroutine mInstance;
    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static FreeCoroutine Instance
    {
        get
        {
            if (mInstance == null)
            {
                if (s_gameQuit)
                    return null;
                mInstance = GameObject.FindObjectOfType<FreeCoroutine>();
                if (mInstance == null)
                {
                    Debug.LogWarning("No FreeCoroutine find in scene, creating one");
                    GameObject freeCoroutineGO = new GameObject("FreeCoroutine", typeof(FreeCoroutine));
                    mInstance = freeCoroutineGO.GetComponent<FreeCoroutine>();
                }
            }
            return mInstance;
        }
    }

    static bool s_gameQuit = false;

    #endregion
    /// <summary>
    /// The coroutine dic.
    /// </summary>
    Dictionary<int, CoroutineRequest> mCoroutineDic = new Dictionary<int, CoroutineRequest>();
    /// <summary>
    /// The last id used,for CoroutineRequest id generation
    /// </summary>
    int mIdSeq = 0;

    void Awake()
    {
        Init();
    }

    void OnApplicationQuit()
    {
        s_gameQuit = true;
    }
    void HandleOnCoroutineRequestComplete(int id)
    {
        CoroutineRequest task = FindCoroutineInternal(id);
        if (task != null)
        {
            task.Release();
            this.mCoroutineDic.Remove(id);
        }
    }

    /// <summary>
    /// Gets the coroutine count.
    /// </summary>
    /// <value>The count.</value>
    public int Count
    {
        get { return mCoroutineDic.Count; }
    }

    /// <summary>
    /// Init this instance.
    /// for manual init;
    /// </summary>
    public void Init()
    {
        if (mInstance == null)
        {
            mInstance = this;
        }
    }

    /// <summary>
    /// Starts the coroutine request.
    /// </summary>
    /// <returns>The identifier of coroutine request.</returns>
    /// <param name="coroutine">Coroutine.</param>
    private int StartCoroutineRequestInternal(IEnumerator coroutine)
    {
        if (mIdSeq < 0)
        {
            mIdSeq = 0;
        }
        int id = mIdSeq++;
        CoroutineRequest task = new CoroutineRequest(id, coroutine);
        if (task.State != CoroutineState.Stop)
        {
            mCoroutineDic[id] = task;
            task.OnCoroutineRequestComplete += HandleOnCoroutineRequestComplete;
            task.Start();
            return id;
        }
        return -1;
    }

    /// <summary>
    /// Starts the coroutine request as a child of another coroutine request.
    /// if no parent coroutine found, the coroutine request will execute normally
    /// </summary>
    /// <returns>The identifier of coroutine request.</returns>
    /// <param name="coroutine">Coroutine.</param>
    /// <param name="parentCoroutineId">Parent coroutine identifier.</param>
    private int StartCoroutineRequestInternal(IEnumerator coroutine, int parentCoroutineId)
    {
        if (mIdSeq < 0)
        {
            mIdSeq = 0;
        }
        int id = mIdSeq++;
        CoroutineRequest task = new CoroutineRequest(id, coroutine);
        if (task.State != CoroutineState.Stop)
        {
            mCoroutineDic[id] = task;
            task.OnCoroutineRequestComplete += HandleOnCoroutineRequestComplete;
        }
        else
        {
            id = -1;
        }

        if (id > 0)
        {
            //find parent coroutine for valid coroutine only
            CoroutineRequest parentCR = FindCoroutineInternal(parentCoroutineId);
            if (parentCR == null)
            {
                Debug.LogWarning("Parent coroutine [" + parentCoroutineId + "] not found,starting normal coroutine");
            }
            else
            {
                parentCR.AddChildCoroutineRequest(id);
            }

            task.Start(); //if no parent found,continue start
        }

        return id;
    }

    /// <summary>
    /// Finds the coroutine specified by id.
    /// </summary>
    /// <returns>The CoroutineRequest.</returns>
    /// <param name="id">identifier of CoroutineRequest.</param>
    private CoroutineRequest FindCoroutineInternal(int id)
    {
        CoroutineRequest cr;
        mCoroutineDic.TryGetValue(id, out cr);
        return cr;
    }

    /// <summary>
    /// Kills the coroutine.
    /// </summary>
    /// <param name="id">Identifier.</param>
    private void KillCoroutineInternal(int id)
    {
        CoroutineRequest task = FindCoroutineInternal(id);
        if (task != null)
        {
            task.Stop();
            task.Release();
            this.mCoroutineDic.Remove(id);
        }
    }

    /// <summary>
    /// Suspends the coroutine.
    /// </summary>
    /// <param name="id">Identifier.</param>
    private void SuspendCoroutineInternal(int id)
    {
        CoroutineRequest task = FindCoroutineInternal(id);
        if (task != null)
        {
            task.Suspend();
        }
        else
        {
            Debug.LogError("Found coroutineRequest [" + id + "] Failed");
        }
    }
    /// <summary>
    /// Resumes the coroutine.
    /// </summary>
    /// <param name="id">Identifier.</param>
    private void ResumeCoroutineInternal(int id)
    {
        CoroutineRequest task = FindCoroutineInternal(id);
        if (task != null)
        {
            task.Resume();
        }
    }

    #region static interface

    /// <summary>
    /// Starts the coroutine request.
    /// </summary>
    /// <returns>The identifier of coroutine request.</returns>
    /// <param name="coroutine">Coroutine.</param>
    static public int StartCoroutineRequest(IEnumerator coroutine)
    {
        if (Instance == null)
            return -1;
        return Instance.StartCoroutineRequestInternal(coroutine);
    }

    /// <summary>
    /// Starts the coroutine request.
    /// </summary>
    /// <returns>The identifier of coroutine request.</returns>
    /// <param name="coroutine">Coroutine.</param>
    /// <param name="parentCoroutineId">Parent coroutine identifier.</param>
    static public int StartCoroutineRequest(IEnumerator coroutine, int parentCoroutineId)
    {
        if (Instance == null)
        {
            return -1;
        }
        return Instance.StartCoroutineRequestInternal(coroutine, parentCoroutineId);
    }

    /// <summary>
    /// Finds the coroutine specified by id.
    /// </summary>
    /// <returns>The CoroutineRequest.</returns>
    /// <param name="id">identifier of CoroutineRequest.</param>
    static public CoroutineRequest FindCoroutine(int id)
    {
        if (Instance == null)
            return null;
        return Instance.FindCoroutineInternal(id);
    }

    /// <summary>
    /// Kills the coroutine.
    /// </summary>
    /// <param name="id">Identifier.</param>
    static public void KillCoroutine(int id)
    {
        if (Instance == null)
            return;
        Instance.KillCoroutineInternal(id);
    }

    /// <summary>
    /// Suspends the coroutine.
    /// </summary>
    /// <param name="id">Identifier.</param>
    static public void SuspendCoroutine(int id)
    {
        if (Instance == null)
            return;
        Instance.SuspendCoroutineInternal(id);
    }
    /// <summary>
    /// Resumes the coroutine.
    /// </summary>
    /// <param name="id">Identifier.</param>
    static public void ResumeCoroutine(int id)
    {
        if (Instance == null)
            return;
        Instance.ResumeCoroutineInternal(id);
    }

    /// <summary>
    /// Waits the realtime.
    /// Utility function
    /// </summary>
    /// <returns>The realtime.</returns>
    /// <param name="waitTime">Wait time.</param>
    public static IEnumerator WaitRealtime(float waitTime)
    {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + waitTime)
        {
            yield return null;
        }
    }

    public static IEnumerator WaitRealtime(float waitTime, System.Action callback)
    {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + waitTime)
        {
            yield return null;
        }
        callback();
    }

    public static IEnumerator WaitTime(float waitTime, System.Action callback)
    {
        float start = Time.time;
        while (Time.time < start + waitTime)
        {
            yield return null;
        }
        callback();
    }

    public static IEnumerator WaitTime<T>(float waitTime, System.Action<T> callback, T data)
    {
        float start = Time.time;
        while (Time.time < start + waitTime)
        {
            yield return null;
        }
        callback(data);
    }
    #endregion
}