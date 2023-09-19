/// <summary>
/// AlphaTweenerCompnent.cs
/// Desc:   
/// </summary>

using UnityEngine;
[System.Serializable]
public class AnimationCompnent : ITweenerComponent
{
    [SerializeField]
    bool m_enabled = false;
    public float delay = 0;

    public Animation animation;

    public AnimationClip forwardClip;
    public AnimationClip backwardClip;

    public bool enabled { get => m_enabled; set => m_enabled = value; }

    public float completeTime
    {
        get
        {
            if (animation == null || animation.clip == null)
                return delay;
            return animation.clip.length + delay;
        }
    }

    public float rewindDelayTime { set => throw new System.NotImplementedException(); }

    public void Awake() { }

    public void Play()
    {
        var animationState = animation[forwardClip.name];
        animationState.speed = 1;
        animation.Play(forwardClip.name);
    }

    public void Prepare(GameObject owner)
    {
        animation = owner.GetComponent<Animation>();
        if (animation == null)
        {
            Debug.LogError("Can not find Animation on [" + owner + "]", owner);
        }
    }

    public void Rewind() { }

    public void SetToEnd()
    {

    }

    public void SetToStart()
    {
        if (forwardClip == null)
            return;
        var animationState = animation[forwardClip.name];
        animation.Play(forwardClip.name);
        animationState.time = 0;
        animationState.speed = 0;



        //111
    }

    public void Release()
    {
        // if (m_acteiveTweenerID >= 0)
        // {
        //     DOTween.Kill(m_acteiveTweenerID);
        // }
    }

    public bool Equal(ITweenerComponent com)
    {
        if (com == null)
        {
            return false;
        }
        if (com is AnimationCompnent)
        {
            AnimationCompnent targetCom = com as AnimationCompnent;
            if (targetCom.m_enabled == this.m_enabled && targetCom.delay == this.delay
                && targetCom.animation == this.animation && targetCom.forwardClip == this.forwardClip
                && targetCom.backwardClip == this.backwardClip)
            {
                return true;
            }
            return false;
        }
        return false;
    }
}
