using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceControl
{
    //第1个为背景音乐专用
    private List<AudioSource> m_AudioSources = new List<AudioSource>();

    private GameObject m_PlayLocation;

    private int m_FreeCount = 0;
    private List<AudioSource> m_TempSources = new List<AudioSource>();

    public AudioSourceControl(GameObject playLocation)
    {
        m_PlayLocation = playLocation;
        Initial();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    private void Initial()
    {
        // 初始化四个AudioSource，第一个AudioSource为背景音乐专用
        for (int i = 0; i < 4; i++)
        {
            m_AudioSources.Add(m_PlayLocation.AddComponent<AudioSource>());
        }
    }

    /// <summary>
    /// 找到空闲的AudioSource
    /// </summary>
    /// <returns></returns>
    public AudioSource FindFreeAudioSource()
    {
        for (int i = 1; i < m_AudioSources.Count; i++)
        {
            if (!m_AudioSources[i].isPlaying)
            {
                return m_AudioSources[i];
            }
        }

        // 没有空闲的AudioSource，创建新的AudioSource
        AudioSource tempSource = m_PlayLocation.AddComponent<AudioSource>();

        // 设置音量和是否禁音 添加到list
        tempSource.volume = m_AudioSources[1].volume;
        tempSource.mute = m_AudioSources[1].mute;
        m_AudioSources.Add(tempSource);

        return tempSource;
    }

    /// <summary>
    /// 一个名字对应一个Audisource 如果有多个 就需要修改了
    /// </summary>
    /// <param name="audioClipName"></param>
    /// <returns></returns>
    public AudioSource FindSameAudioSource(string audioClipName, bool isMore = false)
    {
        if (!isMore)
        {
            for (int i = 1; i < m_AudioSources.Count; i++)
            {
                if (m_AudioSources[i].clip != null && m_AudioSources[i].clip.name.Equals(audioClipName))
                {
                    return m_AudioSources[i];
                }
            }
        }

        return null;
    }

    /// <summary>
    /// 空闲数大于3个释放多余的AudioSource
    /// </summary>
    public void ReleaseFreeAudioSource()
    {
        m_FreeCount = 0;

        m_TempSources = new List<AudioSource>();

        // 记录有多少AudioSource没有播放
        for (int i = 1; i < m_AudioSources.Count; i++)
        {
            if (!m_AudioSources[i].isPlaying)
            {
                // 如果空闲AudioSource大于3个
                if (++m_FreeCount > 3)
                {
                    m_TempSources.Add(m_AudioSources[i]);
                }
            }
        }

        // 释放多余的AudioSource
        for (int i = 0; i < m_TempSources.Count; i++)
        {
            m_AudioSources.Remove(m_TempSources[i]);
            GameObject.Destroy(m_TempSources[i]);
        }

        // 释放临时List<>
        m_TempSources.Clear();
        m_TempSources = null;
    }

    /// <summary>
    /// 获取背景音乐专用AudioSource
    /// </summary>
    /// <returns></returns>
    public AudioSource GetBGAudioSource()
    {
        return m_AudioSources[0];
    }

    /// <summary>
    /// 获取音效AudioSource
    /// </summary>
    /// <returns></returns>
    public List<AudioSource> GetEffAudioSource()
    {
        return m_AudioSources;
    }

    /// <summary>
    /// 所有音频暂停
    /// </summary>
    public void PauseAll()
    {
        foreach (var item in m_AudioSources)
        {
            if (item.isPlaying)
                item.Pause();
        }
    }

    /// <summary>
    /// 所有音频继续播放
    /// </summary>
    public void ResumeAll()
    {
        foreach (var item in m_AudioSources)
        {
            if (!item.isPlaying)
                item.UnPause();
        }
    }

    /// <summary>
    /// 关闭所有音频
    /// </summary>
    public void StopAll()
    {
        foreach (var item in m_AudioSources)
        {
            item.Stop();
        }
    }

    /// <summary>
    /// 播放所有音频
    /// </summary>
    public void PlayAll()
    {
        foreach (var item in m_AudioSources)
        {
            item.Play();
        }
    }

    /// <summary>
    /// 将所有AudioSource属性设为默认值
    /// </summary>
    public void ResetAll()
    {
        foreach (var item in m_AudioSources)
        {
            item.clip = null;
            item.outputAudioMixerGroup = null;
            item.mute = false;
            item.loop = false;
            item.pitch = 1;
        }
    }

    public void OnRelease()
    {
        if (m_AudioSources != null)
        {
            m_AudioSources.Clear();
            m_AudioSources = null;
        }

        if (m_PlayLocation != null)
        {
            m_PlayLocation = null;
        }
    }
}
