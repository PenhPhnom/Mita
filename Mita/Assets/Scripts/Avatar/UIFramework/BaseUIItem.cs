using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUIItem : MonoBehaviour
{
    private void Start()
    {
        OnStart();
    }

    private void Awake()
    {
        OnAwake();
    }

    // Update is called once per frame
    private void Update()
    {
        OnUpdate(Time.deltaTime);
    }

    private void LateUpdate()
    {
        OnLateUpdate();
    }

    private void OnDestroy()
    {
        OnRelease();
    }

    public virtual void OnStart() { }
    public virtual void OnAwake() { }
    public virtual void OnUpdate(float deltaTime) { }
    public virtual void OnLateUpdate() { }
    public abstract void OnRelease();
    public abstract void OnBlind();
    public abstract void SetData(object param, params object[] args);
}
