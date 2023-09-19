/// <summary>
/// UIViewHelper.cs
/// Desc:   动效控制器
/// </summary>
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
public class UIViewHelper : MonoBehaviour
{
    static int s_lastTweenerId = 0;
    public static int GetUniqueTweenID()
    {
        if (s_lastTweenerId == int.MaxValue)
            s_lastTweenerId = 0;
        int newID = s_lastTweenerId + 1;
        s_lastTweenerId = newID;

        return newID;
    }

    public enum ViewDestType
    {
        Src,
        End
    }

    public enum ViewPlayStatus
    {
        inView, //屏幕内
        outView, //屏幕外
        play,
        rewind,
        Unknown,
    }
    public List<UITweenerBase> tweenerArray;

    public float completeTime;

    private ViewDestType m_curDest = ViewDestType.End;

    public List<UIViewHelper> childrenViewHelper;

    private int m_runningCoroutineId = -1;
    [ShowInInspector, ReadOnly]
    private ViewPlayStatus m_curPlayStatus = ViewPlayStatus.Unknown;

    void Awake()
    {
        if (tweenerArray == null)
        {
            UpdateTweenerConfig();
        }

        if (completeTime == 0)
        {
            Debug.LogError("the Component is not Prepare!!!!!");
        }
        upDateCompleteTime();
    }
    void OnDestroy()
    {
        foreach (var item in tweenerArray)
        {
            if (item)
                item.Release();
        }
    }
    void OnEnable()
    {
        if (ScreenAdapterManager.Instance != null)
            ScreenAdapterManager.Instance.onOrientationChangedP2 += OnOrientationChanged;
    }

    void OnDisable()
    {
        if (ScreenAdapterManager.Instance != null)
            ScreenAdapterManager.Instance.onOrientationChangedP2 -= OnOrientationChanged;
    }

    private void UpdateTweenerConfig()
    {
        tweenerArray = new List<UITweenerBase>(GetComponentsInChildren<UITweenerBase>(true));
        CalCompleteTime();
    }

    private void CalCompleteTime()
    {
        float endTime = 0;
        foreach (var item in tweenerArray)
        {
            if (item.CompleteTime > endTime)
            {
                endTime = item.CompleteTime;
            }
        }

        completeTime = endTime;

        foreach (var item in tweenerArray)
        {
            item.UpdateRewindDelay(completeTime);
        }
    }

    private void upDateCompleteTime()
    {

    }

    [Sirenix.OdinInspector.Button("Play")]
    public void Play()
    {
        Play(null);
    }
    public void Play(TweenCallback onComplete)
    {
        if (m_curPlayStatus == ViewPlayStatus.Unknown)
        {
            m_curPlayStatus = ViewPlayStatus.outView;
        }

        //Debug.Log($"UIViewHelper{this} Play,frame[{Time.frameCount}], m_curPlayStatus:" + m_curPlayStatus);
        if (m_curPlayStatus == ViewPlayStatus.outView)
        {
            m_curDest = ViewDestType.End;
            m_curPlayStatus = ViewPlayStatus.play;
            //Debug.Log($"UIViewHelper{this} Play,frame[{Time.frameCount}]");
            if (onComplete != null && completeTime > 0)
            {
                System.Action action = () =>
                {
                    this.m_runningCoroutineId = -1;
                    m_curPlayStatus = ViewPlayStatus.inView;
                    onComplete();
                };

                this.m_runningCoroutineId = FreeCoroutine.StartCoroutineRequest(FreeCoroutine.WaitTime(completeTime, action));
            }
            else
            {
                m_curPlayStatus = ViewPlayStatus.inView;
            }

            foreach (var item in tweenerArray)
            {
                item.SetToStart();
                item.Play();
            }

            if (childrenViewHelper != null)
            {
                foreach (var item in childrenViewHelper)
                {
                    item.Play();
                }
            }
        }
        else
        {
            if (onComplete != null)
            {
                onComplete();
            }
        }
    }

    /// <summary>
    /// 将所有Tween设置为启动状态
    /// </summary>
    public void SetToStart()
    {

        m_curDest = ViewDestType.Src;
        m_curPlayStatus = ViewPlayStatus.outView;
        foreach (var item in tweenerArray)
        {
            item.SetToStart();
        }
        if (childrenViewHelper != null)
        {
            foreach (var item in childrenViewHelper)
            {
                item.SetToStart();
            }
        }
    }

    [Sirenix.OdinInspector.Button("Rewind")]
    public void Rewind()
    {
        Rewind(null);
    }
    public void Rewind(TweenCallback onComplete)
    {
        if (m_curPlayStatus == ViewPlayStatus.Unknown)
        {
            m_curPlayStatus = ViewPlayStatus.inView;
        }
        //Debug.Log($"UIViewHelper{this} Rewind,frame[{Time.frameCount}], m_curPlayStatus:"+ m_curPlayStatus); 
        if (m_curPlayStatus == ViewPlayStatus.inView)
        {
            m_curDest = ViewDestType.Src;
            m_curPlayStatus = ViewPlayStatus.rewind;

            if (onComplete != null && completeTime > 0)
            {
                //Debug.Log($"UIViewHelper2{this} Rewind,frame[{Time.frameCount}]");
                System.Action action = () =>
                {
                    onComplete();
                    this.m_runningCoroutineId = -1;
                    m_curPlayStatus = ViewPlayStatus.outView;
                };
                this.m_runningCoroutineId = this.m_runningCoroutineId = FreeCoroutine.StartCoroutineRequest(FreeCoroutine.WaitTime(completeTime, action));
            }
            else
            {
                m_curPlayStatus = ViewPlayStatus.outView;
            }

            foreach (var item in tweenerArray)
            {
                item.SetToEnd();
                item.Rewind();
            }

            if (childrenViewHelper != null)
            {
                foreach (var item in childrenViewHelper)
                {
                    item.Rewind();
                }
            }
        }
        else
        {
            if (onComplete != null)
            {
                onComplete();
            }
        }
    }
    /// <summary>
    /// 将所有Tween设置为结束状态
    /// </summary>
    public void SetToEnd()
    {
        m_curDest = ViewDestType.End;
        m_curPlayStatus = ViewPlayStatus.inView;
        foreach (var item in tweenerArray)
        {
            item.SetToEnd();
        }
        if (childrenViewHelper != null)
        {
            foreach (var item in childrenViewHelper)
            {
                item.SetToEnd();
            }
        }
    }

    public void Kill()
    {
        if (this.m_runningCoroutineId > 0)
        {
            FreeCoroutine.KillCoroutine(this.m_runningCoroutineId);
            this.m_runningCoroutineId = -1;
        }
    }

    [Sirenix.OdinInspector.Button("Prepare")]
    public void Prepare()
    {
        tweenerArray = new List<UITweenerBase>(GetComponentsInChildren<UITweenerBase>(true));
        foreach (var item in tweenerArray)
        {
            item.Prepare();
        }

        UpdateTweenerConfig();
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(gameObject);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }

    public void RewindEaseUsePlayEase()
    {
        tweenerArray = new List<UITweenerBase>(GetComponentsInChildren<UITweenerBase>(true));
        foreach (var item in tweenerArray)
        {
            item.RewindEaseUsePlayEase();

        }
    }

    public void OnOrientationChanged()
    {
        if (m_curDest == ViewDestType.Src)
        {
            SetToStart();
        }
        else
        {
            SetToEnd();
        }
    }

    public void AddChildViewHelper(UIViewHelper child)
    {
        if (childrenViewHelper == null)
            childrenViewHelper = new List<UIViewHelper>();
        childrenViewHelper.Add(child);
    }

    public void RemoveChildViewHelper(UIViewHelper child)
    {
        if (childrenViewHelper == null) return;
        if (childrenViewHelper.Contains(child))
            childrenViewHelper.Remove(child);
    }

    public void ClearChildern()
    {
        childrenViewHelper.Clear();
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Tools/RewindEaseUsePlayEase_Selected")]
    static public void RewindEaseUsePlayEase_Selected()
    {
        var prefabs = UnityEditor.Selection.GetFiltered(typeof(UnityEngine.Object), UnityEditor.SelectionMode.DeepAssets);
        float total = prefabs.Length;
        float count = 0;
        foreach (var item in prefabs)
        {
            count++;

            UnityEditor.EditorUtility.DisplayProgressBar("RewindEaseUsePlayEase_Selected", item.name, count / total);

            var go = item as GameObject;
            if (go == null)
                continue;

            var viewHelper = go.GetComponent<UIViewHelper>();
            if (viewHelper)
            {
                viewHelper.RewindEaseUsePlayEase();
                UnityEditor.EditorUtility.SetDirty(go);
            }
        }
        UnityEditor.EditorUtility.ClearProgressBar();
        UnityEditor.AssetDatabase.SaveAssets();
    }
#endif

}
