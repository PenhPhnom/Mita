/// <summary>
/// ScaleTweenComponent.cs
/// Desc:   
/// </summary>

using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
[System.Serializable]
public class ScaleTweenComponent : ITweenerComponent
{
    public enum TweenMode
    {
        Default,
        Free
    }
    [SerializeField]
    bool m_enabled = false;
    public TweenMode tweenMode;

    public Vector3 m_SrcLocalScale = Vector3.zero;
    [ShowIf("tweenMode", TweenMode.Free)]
    public Vector3 m_DestLocalScale;
    int m_acteiveTweenerID = -1;

    public bool enableEaseCurve;
    [HideIf("enableEaseCurve", true)]
    public Ease easeMode = Ease.Linear;
    [ShowIf("enableEaseCurve", true)]
    public AnimationCurve easeCurve;

    public bool enableRewindCurve;
    [HideIf("enableRewindCurve", true)]
    public Ease rewindEaseMode = Ease.Linear;
    [ShowIf("enableRewindCurve", true)]
    public AnimationCurve rewindCurve;

    public float delay = 0;
    public float duration = 1;

    [ReadOnly]
    [HideInInspector]
    public RectTransform m_RectTr;

    [SerializeField]
    private float rewindDelay;

    public bool enabled { get => m_enabled; set => m_enabled = value; }
    public float completeTime
    {
        get
        {
            return delay + duration;
        }
    }
    public float rewindDelayTime { set => rewindDelay = value; }
    public void Awake()
    {
        if (tweenMode == TweenMode.Default)
        {
            m_DestLocalScale = m_RectTr.localScale;
        }
    }
    [Button]
    public void Play()
    {
        var tweenerCore = DOTween.To(SrcScaleGetter, ScaleSetter, DestScaleGetter(), duration).SetDelay(delay);
        if (enableEaseCurve)
        {
            tweenerCore = tweenerCore.SetEase(easeCurve).Play();
        }
        else
        {
            tweenerCore = tweenerCore.SetEase(easeMode).Play();
        }
        tweenerCore.intId = UIViewHelper.GetUniqueTweenID();
        if (m_acteiveTweenerID >= 0)
        {
            DOTween.Kill(m_acteiveTweenerID);
        }
        m_acteiveTweenerID = tweenerCore.intId;
    }

    [Button]
    public void Rewind()
    {
        var tweenerCore = DOTween.To(DestScaleGetter, ScaleSetter, SrcScaleGetter(), duration).SetDelay(rewindDelay);
        if (enableRewindCurve)
        {
            tweenerCore = tweenerCore.SetEase(rewindCurve).Play();
        }
        else
        {
            tweenerCore = tweenerCore.SetEase(rewindEaseMode).Play();
        }
        tweenerCore.intId = UIViewHelper.GetUniqueTweenID();
        if (m_acteiveTweenerID >= 0)
        {
            DOTween.Kill(m_acteiveTweenerID);
        }
        m_acteiveTweenerID = tweenerCore.intId;
    }

    Vector3 SrcScaleGetter()
    {
        return m_SrcLocalScale;
    }
    Vector3 DestScaleGetter()
    {

        return m_DestLocalScale;
    }
    void ScaleSetter(Vector3 localScale)
    {
        m_RectTr.localScale = localScale;
    }
    public void Prepare(GameObject owner)
    {
        m_RectTr = owner.transform as RectTransform;

    }

    public void SetToStart()
    {
        Release();
        ScaleSetter(SrcScaleGetter());
    }

    public void SetToEnd()
    {
        Release();
        ScaleSetter(DestScaleGetter());
    }

    public void Release()
    {
        if (m_acteiveTweenerID >= 0)
        {
            DOTween.Kill(m_acteiveTweenerID);
        }
    }

    public bool Equal(ITweenerComponent com)
    {
        if (com == null)
        {
            return false;
        }
        if (com is ScaleTweenComponent)
        {
            ScaleTweenComponent targetCom = com as ScaleTweenComponent;
            if (targetCom.m_enabled == this.m_enabled && targetCom.enableEaseCurve == this.enableEaseCurve
                && targetCom.easeMode == this.easeMode && targetCom.easeCurve.Equals(this.easeCurve)
                && targetCom.enableRewindCurve == this.enableRewindCurve && targetCom.rewindEaseMode == this.rewindEaseMode
                && targetCom.rewindCurve.Equals(this.rewindCurve) && targetCom.m_SrcLocalScale == this.m_SrcLocalScale
                && targetCom.delay == this.delay && targetCom.duration == this.duration
                && targetCom.rewindDelay == this.rewindDelay)
            {
                return true;
            }
            return false;
        }
        return false;
    }
}
