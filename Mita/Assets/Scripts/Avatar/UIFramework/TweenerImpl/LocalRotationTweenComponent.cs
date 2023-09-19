/// <summary>
/// RotationTweenComponent.cs
/// Desc:   
/// </summary>

using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class LocalRotationTweenComponent : ITweenerComponent
{
    [SerializeField]
    bool m_enabled = false;
    Vector3 m_DestLocalRotation;

    public Vector3 m_SrcLocalRotation = Vector3.zero;

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
        m_DestLocalRotation = m_RectTr.localRotation.eulerAngles;

    }
    [Button]
    public void Play()
    {
        var tweenerCore = DOTween.To(SrcRotationGetter, RotationSetter, DestRotationGetter(), duration).SetDelay(delay);
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
        var tweenerCore = DOTween.To(DestRotationGetter, RotationSetter, SrcRotationGetter(), duration).SetDelay(rewindDelay);
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

    Vector3 SrcRotationGetter()
    {
        return m_SrcLocalRotation;
    }
    Vector3 DestRotationGetter()
    {

        return m_DestLocalRotation;
    }
    void RotationSetter(Vector3 localRotation)
    {
        m_RectTr.localRotation = Quaternion.Euler(localRotation);
    }
    public void Prepare(GameObject owner)
    {
        m_RectTr = owner.transform as RectTransform;

    }

    public void SetToStart()
    {
        Release();
        RotationSetter(SrcRotationGetter());
    }

    public void SetToEnd()
    {
        Release();
        RotationSetter(DestRotationGetter());
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
        if (com is LocalRotationTweenComponent)
        {
            LocalRotationTweenComponent targetCom = com as LocalRotationTweenComponent;
            if (targetCom.m_enabled == this.m_enabled && targetCom.enableEaseCurve == this.enableEaseCurve
                && targetCom.easeMode == this.easeMode && targetCom.easeCurve.Equals(this.easeCurve)
                && targetCom.enableRewindCurve == this.enableRewindCurve && targetCom.rewindEaseMode == this.rewindEaseMode
                && targetCom.rewindCurve.Equals(this.rewindCurve) && targetCom.m_SrcLocalRotation == this.m_SrcLocalRotation
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
