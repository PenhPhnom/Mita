/// <summary>
/// ITweenerComponent.cs
/// Desc:   动效元素接口
/// </summary>

using UnityEngine;
public interface ITweenerComponent
{
    bool enabled
    {
        get;
        set;
    }
    float completeTime
    {
        get;
    }
    float rewindDelayTime
    {
        set;
    }
    void Prepare(GameObject owner);
    void Awake();

    void Release();

    void Play();
    void Rewind();

    void SetToStart();

    void SetToEnd();

    bool Equal(ITweenerComponent com);
}
