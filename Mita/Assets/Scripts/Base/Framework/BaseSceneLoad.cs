using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSceneLoad
{
    public abstract void StartLoadingScene(BaseScene curScene);
    public abstract void DisposeLoadingScene();
    public abstract bool IsLoadingSceneReady();
    public abstract float GetLoadingSceneProgress();
    public abstract void StartPreLoadNewScene(BaseScene curscene);
    /// <summary>
    /// 记载在Prefab 目前没有用
    /// </summary>
    public abstract void GetScenePrefab();
    public abstract bool IsNewSceneLoadReady();
    public abstract float GetNewSceneLoadProgress();

    /// <summary>
    /// 释放处理
    /// </summary>
    public abstract void Dispose();
    /// <summary>
    /// 完全释放前处理
    /// </summary>
    public abstract void DestoryPreDispose();
    /// <summary>
    /// 完全释放
    /// </summary>
    public abstract void DestoryDispose();
}
