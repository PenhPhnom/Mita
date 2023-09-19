/// <summary>
/// AnchoredPositionTweenComponent.cs
/// Desc:   
/// </summary>

using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class AnchoredPositionTweenComponent : ITweenerComponent
{
    public enum TweenMode
    {
        Anchored,
        Free,
    }

    [SerializeField]
    bool m_enabled = false;

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

    public TweenMode tweenMode;

    [ShowIf("tweenMode", TweenMode.Anchored)]
    public ScreenAdapterManager.EAnchorType anchorType;

    [ShowIf("tweenMode", TweenMode.Free)]
    public Vector2 destAnchoredPosition;

    [ShowIf("tweenMode", TweenMode.Free)]
    public Vector2 srcAnchoredPosition;

    int m_acteiveTweenerID = -1;
    public float delay = 0;
    public float duration = 1;

    // [HideInInspector]
    public RectTransform m_RectTr;

    [InfoBox("缺少screenAdapter，会导致适配失败！UIViewHelper需要Prepare", InfoMessageType.Error, "noAdapter")]
    public ScreenAdapter screenAdapter;

    bool noAdapter
    {
        get
        {
            return screenAdapter == null && this.tweenMode == TweenMode.Anchored;
        }
    }

    [ReadOnly]
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
        if (!m_enabled)
            return;

        if (tweenMode == TweenMode.Anchored)
        {
            destAnchoredPosition = m_RectTr.anchoredPosition;

            //switch (anchorType)
            //{
            //    case ScreenAdapterManager.EAnchorType.Left:
            //        srcAnchoredPosition = new Vector2(m_RectTr.anchoredPosition.x - m_RectTr.sizeDelta.x, m_RectTr.anchoredPosition.y);
            //        break;
            //    case ScreenAdapterManager.EAnchorType.Right:
            //        srcAnchoredPosition = new Vector2(m_RectTr.anchoredPosition.x + m_RectTr.sizeDelta.x, m_RectTr.anchoredPosition.y);
            //        break;
            //    case ScreenAdapterManager.EAnchorType.Top:
            //        srcAnchoredPosition = new Vector2(m_RectTr.anchoredPosition.x, m_RectTr.anchoredPosition.y + m_RectTr.sizeDelta.y);
            //        break;
            //    case ScreenAdapterManager.EAnchorType.Bottom:
            //        srcAnchoredPosition = new Vector2(m_RectTr.anchoredPosition.x, m_RectTr.anchoredPosition.y - m_RectTr.sizeDelta.y);
            //        break;
            //}

            switch (anchorType)
            {
                case ScreenAdapterManager.EAnchorType.Left:
                    srcAnchoredPosition = new Vector2(m_RectTr.anchoredPosition.x - m_RectTr.sizeDelta.x - ScreenAdapterManager.Instance.LeftEdge, m_RectTr.anchoredPosition.y);
                    break;
                case ScreenAdapterManager.EAnchorType.Right:
                    srcAnchoredPosition = new Vector2(m_RectTr.anchoredPosition.x + m_RectTr.sizeDelta.x + ScreenAdapterManager.Instance.RightEdge, m_RectTr.anchoredPosition.y);
                    break;
                case ScreenAdapterManager.EAnchorType.Top:
                    srcAnchoredPosition = new Vector2(m_RectTr.anchoredPosition.x, m_RectTr.anchoredPosition.y + m_RectTr.sizeDelta.y + ScreenAdapterManager.Instance.TopEdge);
                    break;
                case ScreenAdapterManager.EAnchorType.Bottom:
                    srcAnchoredPosition = new Vector2(m_RectTr.anchoredPosition.x, m_RectTr.anchoredPosition.y - m_RectTr.sizeDelta.y - ScreenAdapterManager.Instance.BottomEdge);
                    break;
            }
        }
    }
    [Button]
    public void Play()
    {
        var tweenerCore = DOTween.To(SrcPositionGetter, PositionSetter, DestPositionGetter(), duration).SetDelay(delay);
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
        var tweenerCore = DOTween.To(DestPositionGetter, PositionSetter, SrcPositionGetter(), duration).SetDelay(rewindDelay);
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

    Vector2 SrcPositionGetter()
    {
        return srcAnchoredPosition;
    }
    Vector2 DestPositionGetter()
    {
        //无论什么模式 只要有screenAdapter都得支持适配
        if (screenAdapter != null)
        {
            if (tweenMode == TweenMode.Anchored)
            {
                destAnchoredPosition = screenAdapter.CalcAnchoredPosition();
            }
            else if (tweenMode == TweenMode.Free)
            {
                destAnchoredPosition = screenAdapter.CalcFreePosition(destAnchoredPosition);
            }
        }
        return destAnchoredPosition;
    }
    void PositionSetter(Vector2 anchoredPos)
    {
        if (m_RectTr != null)
        {
            m_RectTr.anchoredPosition = anchoredPos;
        }
    }
    public void Prepare(GameObject owner)
    {
        m_RectTr = owner.transform as RectTransform;
        if (tweenMode == TweenMode.Anchored)
        {
            screenAdapter = owner.GetComponent<ScreenAdapter>();
        }
    }

    public void SetToStart()
    {
        Release();
        PositionSetter(SrcPositionGetter());
    }

    public void SetToEnd()
    {
        Release();
        PositionSetter(DestPositionGetter());
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
        if (com is AnchoredPositionTweenComponent)
        {
            AnchoredPositionTweenComponent targetCom = com as AnchoredPositionTweenComponent;
            if (targetCom.m_enabled == this.m_enabled && targetCom.enableEaseCurve == this.enableEaseCurve
                && targetCom.easeMode == this.easeMode && targetCom.easeCurve.Equals(this.easeCurve)
                && targetCom.enableRewindCurve == this.enableRewindCurve && targetCom.rewindEaseMode == this.rewindEaseMode
                && targetCom.rewindCurve.Equals(this.rewindCurve) && targetCom.tweenMode == this.tweenMode
                && targetCom.anchorType == this.anchorType && targetCom.screenAdapter == this.screenAdapter
                && targetCom.destAnchoredPosition == this.destAnchoredPosition && targetCom.srcAnchoredPosition == this.srcAnchoredPosition
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
