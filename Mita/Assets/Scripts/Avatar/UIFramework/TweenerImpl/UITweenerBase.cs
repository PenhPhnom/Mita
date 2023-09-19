/// <summary>
/// UIComponentTweener.cs
/// Desc:   动效组件
/// </summary>

using UnityEngine;
public abstract class UITweenerBase : MonoBehaviour
{
    public abstract float CompleteTime
    {
        get;
    }
    public abstract void Release();

    public abstract void Play();
    public abstract void Rewind();
    public abstract void Prepare();
    public abstract void SetToStart();
    public abstract void SetToEnd();

    public abstract void UpdateRewindDelay(float totalCompleteTime);

    public abstract void RewindEaseUsePlayEase();
}
