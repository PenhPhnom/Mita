/// <summary>
/// UIComponentTweener.cs
/// Desc:   动效组件
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DG.Tweening;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using Sirenix.Utilities;
#endif
using UnityEngine;
public class UIComponentTweener : UITweenerBase
{
    HashSet<ITweenerComponent> tweenerList = new HashSet<ITweenerComponent>();
    [BoxGroup("SingleTween")]
    public AnchoredPositionTweenComponent positionTweener;
    [BoxGroup("SingleTween")]
    public AlphaTweenerCompnent alphaTweener;
    [BoxGroup("SingleTween")]
    public ScaleTweenComponent scaleTweener;
    [BoxGroup("SingleTween")]
    public LocalRotationTweenComponent localRotationTweener;

    [BoxGroup("MultiTween")]
    [ListDrawerSettings(NumberOfItemsPerPage = 10, ShowIndexLabels = true)]
    public AnchoredPositionTweenComponent[] positionTweeners;
    [BoxGroup("MultiTween")]
    [ListDrawerSettings(NumberOfItemsPerPage = 10, ShowIndexLabels = true)]
    public AlphaTweenerCompnent[] alphaTweeners;
    [BoxGroup("MultiTween")]
    [ListDrawerSettings(NumberOfItemsPerPage = 10, ShowIndexLabels = true)]
    public ScaleTweenComponent[] scaleTweeners;
    [BoxGroup("MultiTween")]
    [ListDrawerSettings(NumberOfItemsPerPage = 10, ShowIndexLabels = true)]
    public LocalRotationTweenComponent[] localRotationTweeners;

    void Awake()
    {
        CollectTweener();

        foreach (var item in tweenerList)
        {
            item.Awake();
        }
    }

    public override void Release()
    {
        foreach (var item in tweenerList)
        {
            item.Release();
        }
    }

    private void AddToTweener(ITweenerComponent tweener)
    {
        if (!tweenerList.Contains(tweener) && tweener.enabled)
        {
            tweenerList.Add(tweener);
        }
    }

    private void CollectTweener()
    {
        tweenerList.Clear();
        if (positionTweeners != null && positionTweeners.Length > 0)
        {
            CollectTweeners(positionTweeners);
        }
        else
        {
            AddToTweener(positionTweener);
        }

        if (alphaTweeners != null && alphaTweeners.Length > 0)
        {
            CollectTweeners(alphaTweeners);
        }
        else
        {
            AddToTweener(alphaTweener);
        }

        if (scaleTweeners != null && scaleTweeners.Length > 0)
        {
            CollectTweeners(scaleTweeners);
        }
        else
        {
            AddToTweener(scaleTweener);
        }

        if (localRotationTweeners != null && localRotationTweeners.Length > 0)
        {
            CollectTweeners(localRotationTweeners);
        }
        else
        {
            AddToTweener(localRotationTweener);
        }
    }

    private void CollectTweeners(ITweenerComponent[] tweenArray)
    {
        if (tweenArray == null || tweenArray.Length <= 0)
        {
            return;
        }
        for (int iTween = 0; iTween < tweenArray.Length; iTween++)
        {
            var tween = tweenArray[iTween];
            AddToTweener(tween);
        }
    }

    [Button]
    public override void Play()
    {
        foreach (var item in tweenerList)
        {
            item.Play();
        }
    }

    [Button]
    public override void Rewind()
    {
        foreach (var item in tweenerList)
        {
            item.Rewind();
        }
    }

    public override void Prepare()
    {
        CollectTweener();

        foreach (var item in tweenerList)
        {
            if (!item.enabled)
                continue;
            item.Prepare(gameObject);
        }
    }

    public override void SetToStart()
    {
        foreach (var item in tweenerList)
        {
            if (item == null)
            {
                continue;
            }
            item.SetToStart();
        }

    }
    public override void SetToEnd()
    {
        foreach (var item in tweenerList)
        {
            if (item == null)
            {
                continue;
            }
            item.SetToEnd();
        }

    }
    public override float CompleteTime
    {
        get
        {
            float completeTime = 0;
            foreach (var item in tweenerList)
            {
                var itemCompleteTime = item.completeTime;
                if (itemCompleteTime > completeTime)
                    completeTime = itemCompleteTime;
            }
            return completeTime;
        }
    }

    public override void UpdateRewindDelay(float totalCompleteTime)
    {
        foreach (var item in tweenerList)
        {
            var delay = totalCompleteTime - item.completeTime;
            item.rewindDelayTime = delay;
        }
    }

    public override void RewindEaseUsePlayEase()
    {
        alphaTweener.rewindEaseMode = alphaTweener.easeMode;
        positionTweener.rewindEaseMode = positionTweener.easeMode;
        scaleTweener.rewindEaseMode = scaleTweener.easeMode;
        localRotationTweener.rewindEaseMode = localRotationTweener.easeMode;
        RewindEaseUsePlayEase<AnchoredPositionTweenComponent>(positionTweeners);
        RewindEaseUsePlayEase<AlphaTweenerCompnent>(alphaTweeners);
        RewindEaseUsePlayEase<ScaleTweenComponent>(scaleTweeners);
        RewindEaseUsePlayEase<LocalRotationTweenComponent>(localRotationTweeners);
    }

    private void RewindEaseUsePlayEase<T>(T[] tweeners) where T : ITweenerComponent
    {
        if (tweeners == null || tweeners.Length <= 0)
        {
            return;
        }
        Type type = typeof(T);
        FieldInfo rewindField = null;
        FieldInfo easeField = null;
        FieldInfo[] fieldInfos = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (fieldInfos != null && fieldInfos.Length > 0)
        {
            for (int iInfo = 0; iInfo < fieldInfos.Length; iInfo++)
            {
                FieldInfo info = fieldInfos[iInfo];
                if (info.Name.Equals("rewindEaseMode"))
                {
                    rewindField = info;
                }
                else if (info.Name.Equals("easeMode"))
                {
                    easeField = info;
                }
                if (rewindField != null && easeField != null)
                {
                    break;
                }
            }
        }
        if (rewindField == null || easeField == null)
        {
            return;
        }

        foreach (var tweener in tweeners)
        {
            rewindField.SetValue(tweener, easeField.GetValue(tweener));
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// 为了兼容之前的单个Tween，执行这个能把之前的Tween放到对于的数组中
    /// </summary>
    [Sirenix.OdinInspector.Button("Single->Multi")]
    private void InitTweens()
    {
        HashSet<AnchoredPositionTweenComponent> posTweenerHash = new HashSet<AnchoredPositionTweenComponent>();
        positionTweeners = AddToTweeners<AnchoredPositionTweenComponent>(posTweenerHash, positionTweeners, positionTweener);
        HashSet<AlphaTweenerCompnent> alphaTweenerHash = new HashSet<AlphaTweenerCompnent>();
        alphaTweeners = AddToTweeners<AlphaTweenerCompnent>(alphaTweenerHash, alphaTweeners, alphaTweener);
        HashSet<ScaleTweenComponent> scaleTweenerHash = new HashSet<ScaleTweenComponent>();
        scaleTweeners = AddToTweeners<ScaleTweenComponent>(scaleTweenerHash, scaleTweeners, scaleTweener);
        HashSet<LocalRotationTweenComponent> rotTweenerHash = new HashSet<LocalRotationTweenComponent>();
        localRotationTweeners = AddToTweeners<LocalRotationTweenComponent>(rotTweenerHash, localRotationTweeners, localRotationTweener);
    }

    private T[] AddToTweeners<T>(HashSet<T> tweenerHash, T[] tweens, T tween) where T : ITweenerComponent
    {
        tweenerHash.Clear();
        if (tweens != null)
        {
            tweenerHash.AddRange(tweens);
        }
        tweenerHash.RemoveWhere(x => x.Equal(tween));
        Sirenix.Utilities.Editor.Clipboard.Clear();
        Sirenix.Utilities.Editor.Clipboard.Copy(tween, Sirenix.Utilities.Editor.CopyModes.DeepCopy);
        tweenerHash.Add(Sirenix.Utilities.Editor.Clipboard.Paste<T>());
        return tweenerHash.ToArray();
    }
#endif
}
