/// <summary>
/// AlphaTweenerCompnent.cs
/// Desc:   
/// </summary>

using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
[System.Serializable]
public class AlphaTweenerCompnent : ITweenerComponent
{
    [SerializeField]
    bool m_enabled = false;

    int m_acteiveTweenerID = -1;
    //[HideInInspector]
    public CanvasGroup canvasGroup;

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
    public float srcAlpha = 0;
    public float destAlpha = 1;
    public float delay = 0;
    public float duration = 1;

    [ReadOnly]
    [SerializeField]
    private float rewindDelay = 0;

    public bool enabled { get => m_enabled; set => m_enabled = value; }

    public float completeTime
    {
        get
        {
            return delay + duration;
        }
    }

    public float rewindDelayTime { set => rewindDelay = value; }

    [Button]
    public void Play()
    {
        var tweenerCore = DOTween.To(SrcAlphaGetter, AlphaSetter, destAlpha, duration).SetDelay(delay);
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
    float SrcAlphaGetter()
    {
        return srcAlpha;
    }
    float DestAlphaGetter()
    {
        return destAlpha;
    }
    void AlphaSetter(float a)
    {
        canvasGroup.alpha = a;
    }

    public void Prepare(GameObject owner)
    {
        canvasGroup = owner.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.LogError("Can not find CanvasGroup on [" + owner + "]", owner);
        }
    }

    public void Awake()
    {

    }
    [Button]
    public void Rewind()
    {
        var tweenerCore = DOTween.To(DestAlphaGetter, AlphaSetter, SrcAlphaGetter(), duration).SetDelay(rewindDelay);
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

    public void SetToStart()
    {
        Release();
        AlphaSetter(SrcAlphaGetter());
    }

    public void SetToEnd()
    {
        Release();
        AlphaSetter(DestAlphaGetter());
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
        if (com is AlphaTweenerCompnent)
        {
            AlphaTweenerCompnent targetCom = com as AlphaTweenerCompnent;
            if (targetCom.m_enabled == this.m_enabled && targetCom.canvasGroup == this.canvasGroup
                && targetCom.enableEaseCurve == this.enableEaseCurve && targetCom.easeMode == this.easeMode
                && targetCom.easeCurve.Equals(this.easeCurve) && targetCom.enableRewindCurve == this.enableRewindCurve
                && targetCom.rewindEaseMode == this.rewindEaseMode && targetCom.rewindCurve.Equals(this.rewindCurve)
                && targetCom.srcAlpha == this.srcAlpha && targetCom.destAlpha == this.destAlpha
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
