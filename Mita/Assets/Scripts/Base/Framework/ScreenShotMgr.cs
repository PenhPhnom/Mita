using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class ScreenShotMgr : Singleton<ScreenShotMgr>
{
    public bool _busy = false;

    /// <summary>
    /// 截屏
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="callBack">回调</param>
    public void Shot(string filePath, System.Action callBack = null)
    {
        if (_busy == true)
            return;
        CoroutineController.Instance.StartCoroutine(ScreenShotTex(filePath, callBack));
    }

    /// <summary>
    /// UnityEngine自带截屏Api，只能截全屏
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <param name="callBack">截图完成回调</param>
    /// <returns>协程</returns>
    public IEnumerator ScreenShotTex(string fileName, System.Action callBack = null)
    {
        yield return new WaitForEndOfFrame();//等到帧结束，不然会报错
        Texture2D tex = UnityEngine.ScreenCapture.CaptureScreenshotAsTexture();//截图返回Texture2D对象
        byte[] bytes = tex.EncodeToPNG();//将纹理数据，转化成一个png图片
        System.IO.File.WriteAllBytes(fileName, bytes);//写入数据
        ClientLog.Instance.Log(string.Format("截取了一张图片: {0}", fileName));
        UnityEngine.Object.DestroyImmediate(tex, true);
        tex = null;
        callBack?.Invoke();
    }
    public override void OnRelease()
    {

    }

}