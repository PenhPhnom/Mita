using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// 音频播放 
/// 外部加载音频的话 要么加载打成AB包的AudioClip 或者 只能加载 .wav 格式的音频
/// </summary>
public class AudioPlayManager : MonoSingleton<AudioPlayManager>
{
    private AudioSourceControl m_AudioSourceControl;
    private float m_NextTime = 5f;
    private float m_Time = 0f;

    private void Start()
    {
        m_AudioSourceControl = new AudioSourceControl(gameObject);
    }

    /// <summary>
    /// 每过五秒释放一次AudioSource
    /// </summary>
    private void Update()
    {
        m_Time += Time.deltaTime;
        if (m_Time > m_NextTime)
        {
            m_Time = 0;
            if (m_AudioSourceControl != null)
                m_AudioSourceControl.ReleaseFreeAudioSource();
        }
    }

    /// <summary>
    /// 释放空闲的AudioSource
    /// </summary>
    public void ReleaseFreeAudioSource()
    {
        if (m_AudioSourceControl != null)
            m_AudioSourceControl.ReleaseFreeAudioSource();
    }

    /// <summary>
    /// 播放音乐（需要调用StopPlay关闭）
    /// </summary>
    /// <param name="audioClipName">音频片段名称</param>
    /// <param name="isBG">是否是背景音乐</param>
    /// <returns></returns>
    public AudioSource PlayAudio(AudioClip audioClip, bool isBG = false, bool isMore = false)
    {
        AudioSource audioSource = null;
        if (isBG)
        {
            audioSource = m_AudioSourceControl.GetBGAudioSource();
        }
        else
        {
            if (!isMore)
            {
                audioSource = CheckExisting(audioClip.name);
            }

            if (audioSource == null)
                audioSource = m_AudioSourceControl.FindFreeAudioSource();
        }
        audioSource.clip = audioClip;
        audioSource.Play();

        return audioSource;
    }

    /// <summary>
    /// 设置背景音乐音量
    /// </summary>
    /// <param name="volume"></param>
    public void SetBGVolume(float volume)
    {
        AudioSource bgAudioSource = m_AudioSourceControl.GetBGAudioSource();
        if (bgAudioSource != null)
            bgAudioSource.volume = volume;
    }

    /// <summary>
    /// 是否将背景音乐禁音
    /// </summary>
    /// <param name="isMute"></param>
    public void SetMuteBG(bool isMute)
    {
        AudioSource bgAudioSource = m_AudioSourceControl.GetBGAudioSource();
        if (bgAudioSource != null)
            bgAudioSource.mute = isMute;
    }

    /// <summary>
    /// 设置音效音量
    /// </summary>
    /// <param name="volume"></param>
    public void SetAllEffVolum(float volume)
    {
        List<AudioSource> _audioSources = m_AudioSourceControl.GetEffAudioSource();
        for (int i = 1; i < _audioSources.Count; i++)
        {
            _audioSources[i].volume = volume;
        }
    }


    /// <summary>
    /// 设置音效音量
    /// </summary>
    /// <param name="volume"></param>
    public void SetEffVolum(string audioClipName, float volume)
    {
        AudioSource audioSource = CheckExisting(audioClipName);
        if (audioSource)
            audioSource.volume = volume;
    }

    /// <summary>
    /// 设置音效是否禁音
    /// </summary>
    /// <param name="isMute"></param>
    public void SetMuteEff(bool isMute)
    {
        List<AudioSource> _audioSources = m_AudioSourceControl.GetEffAudioSource();
        for (int i = 1; i < _audioSources.Count; i++)
        {
            _audioSources[i].mute = isMute;
        }
    }

    public void StopAudio(string audioClipName)
    {
        AudioSource audioSource = CheckExisting(audioClipName);
        if (audioSource)
            audioSource.Stop();
    }

    /// <summary>
    /// 暂停所有音频
    /// </summary>
    public void PauseAllAudio()
    {
        m_AudioSourceControl.PauseAll();
    }

    /// <summary>
    /// 恢复所有音频
    /// </summary>
    public void ResumeAllAudio()
    {
        m_AudioSourceControl.ResumeAll();
    }

    /// <summary>
    /// 停止所有音频
    /// </summary>
    public void StopAllAudio()
    {
        m_AudioSourceControl.StopAll();
    }

    /// <summary>
    /// 播放所有音频
    /// </summary>
    public void PlayAllAudio()
    {
        m_AudioSourceControl.PlayAll();
    }

    /// <summary>
    /// 重置所有AudioSource
    /// </summary>
    public void ResetAllAudio()
    {
        m_AudioSourceControl.ResetAll();
    }


    /// <summary>
    /// 查找是否存在正在播放的AudioSource
    /// </summary>
    /// <param name="singleAudioClip"></param>
    /// <returns></returns>
    private AudioSource CheckExisting(string audioClipName)
    {
        AudioSource audioSource = m_AudioSourceControl.FindSameAudioSource(audioClipName);
        return audioSource;
    }

    private void OnDestroy()
    {
        if (m_AudioSourceControl != null)
        {
            m_AudioSourceControl.OnRelease();
            m_AudioSourceControl = null;
        }
    }

    /// <summary>
    /// AB包加载音乐
    /// </summary>
    /// <param name="resName"></param>
    /// <param name="abName"></param>
    /// <param name="abFolderPath">
    /// 1.Android ab包有替换jar:file:// 如果加会被替换掉，会找不到路径
    /// 2.iOS需要加file:// 如果是streammingasset下的不传路径路径可以忽略，ab包策略会添加上
    /// </param>
    /// <param name="paramData"></param>
    public void PlaySound(string resName, string abName = null, string abFolderPath = null, ParamData paramData = null)
    {
        ResourceMgr.Instance.LoadResourceAsync(resName, TypeInts.AudioClip, (obj, param) =>
        {
            AudioClip audioClip = obj as AudioClip;
            PlayAudio(audioClip, param != null && param.bParam, param != null && param.bParam2);
        }, abName, abFolderPath, paramData);
    }

    /// <summary>
    /// 加载音乐
    /// </summary>
    /// <param name="name"></param>
    /// <param name="url">
    /// 1.Android 可以加 jar:file:// 也可以不加
    /// 2.iOS需要加file://
    /// </param>
    /// <param name="isBG"></param>
    /// <param name="isMore"></param>
    /// <param name="AudioType"></param>
    public void PlaySound(string name, string url, bool isBG = false, bool isMore = false, AudioType AudioType = AudioType.WAV)
    {
        StartCoroutine(PlaySoundWAV(name, url, isBG, isMore, AudioType));
    }

    /// <summary>
    /// 外部的音频格式
    /// </summary>
    /// <param name="name"></param>
    /// <param name="path"></param>
    public IEnumerator PlaySoundWAV(string name, string url = null, bool isBG = false, bool isMore = false, AudioType AudioType = AudioType.WAV)
    {
        url += (name + ".wav");
        UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType);
        yield return request.SendWebRequest();
        try
        {
            if (request.isDone)
            {
                if (request.result == UnityWebRequest.Result.Success)
                {
                    AudioClip audioClip = DownloadHandlerAudioClip.GetContent(request);
                    audioClip.name = name;
                    PlayAudio(audioClip, isBG, isMore);
                }
            }
        }
        catch (Exception e)
        {
            ClientLog.Instance.LogError(e.Message);
        }
    }
}
