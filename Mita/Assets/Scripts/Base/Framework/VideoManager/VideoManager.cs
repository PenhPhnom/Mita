using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;


/// <summary>
/// 视频管理器  目前走 .MP4 格式的
/// </summary>
public class VideoManager : MonoSingleton<VideoManager>
{
    private VideoPlayer m_VideoPlayer;
    private RenderTexture m_RenderTextrue;
    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        if (m_VideoPlayer == null)
            m_VideoPlayer = gameObject.AddMissingComponent<VideoPlayer>();

        //如果 清除TargetTexture的残留帧
        if (m_VideoPlayer.targetTexture != null)
            m_VideoPlayer.targetTexture.Release();

        m_VideoPlayer.started += OnStarted; //每次播放后立刻启动方法
        // 监听视频播放结束
        m_VideoPlayer.loopPointReached += OnLoopPointReached; //每次到结尾，都会自己启动这个方法
    }

    public void SetVidoeRenderTexture(RawImage rawImage, int width, int height, int depth, RenderTextureFormat format)
    {
        if (m_VideoPlayer.targetTexture != null)
            m_VideoPlayer.targetTexture.Release();

        m_VideoPlayer.renderMode = VideoRenderMode.RenderTexture;
        m_RenderTextrue = new RenderTexture(width, height, depth, format);
        rawImage.texture = m_RenderTextrue;
        m_VideoPlayer.targetTexture = m_RenderTextrue;
    }

    /// <summary>
    /// 播放视频  Render
    /// </summary>
    public void PlayVideo(Renderer renderer, string vidoeName = null, string url = null)
    {
        m_VideoPlayer.renderMode = VideoRenderMode.MaterialOverride;
        m_VideoPlayer.targetMaterialRenderer = renderer;

        m_VideoPlayer.url = url + vidoeName + ".mp4";
        m_VideoPlayer.Play();
    }

    public void PlayVideo(RawImage rawImage, int width, int height, int depth, RenderTextureFormat format, string vidoeName = null, string url = null)
    {
        SetVidoeRenderTexture(rawImage, width, height, depth, format);
        m_VideoPlayer.url = url + vidoeName + ".mp4";
        m_VideoPlayer.Play();
    }

    /// <summary>
    /// 每次到结尾，都会自己启动这个方法
    /// </summary>
    /// <param name="source"></param>
    private void OnLoopPointReached(VideoPlayer source)
    {

    }

    /// <summary>
    /// 每次播放后立刻启动方法
    /// </summary>
    /// <param name="source"></param>
    private void OnStarted(VideoPlayer source)
    {

    }

    /// <summary>
    /// 停止播放
    /// </summary>
    public void Stop()
    {
        m_VideoPlayer.Stop();
    }

    /// <summary>
    /// 暂停播放
    /// </summary>
    public void Pause()
    {
        m_VideoPlayer.Pause();
    }

    /// <summary>
    /// 设置速度
    /// </summary>
    public void Prepare()
    {
        m_VideoPlayer.Prepare();
    }

    /// <summary>
    /// 设置时间
    /// </summary>
    public void SetTime(float time)
    {
        m_VideoPlayer.time = time;
    }


    public void OnRelease()
    {
        if (m_RenderTextrue != null)
        {
            Destroy(m_RenderTextrue);
            m_RenderTextrue = null;
        }
        if (m_VideoPlayer != null)
        {
            if (m_VideoPlayer.targetTexture != null)
                m_VideoPlayer.targetTexture.Release();

            // 监听视频播放结束
            m_VideoPlayer.loopPointReached -= OnLoopPointReached;
            m_VideoPlayer.started -= OnStarted;
            m_VideoPlayer = null;
        }
    }

    private void OnDestroy()
    {
        OnRelease();
    }

    private void OnGUI()
    {
        //if (GUILayout.Button("asdasdasd"))
        //{
        //    //PlayVideo(sss, "aaa", @"file://D:/App_UnitySampleProject/Assets/Resources_moved/Audio/");

        //    PlayVideo(img, 720, 1080, 0, RenderTextureFormat.Default, "aaa", @"file://D:/App_UnitySampleProject/Assets/Resources_moved/Audio/");
        //}
    }
}

