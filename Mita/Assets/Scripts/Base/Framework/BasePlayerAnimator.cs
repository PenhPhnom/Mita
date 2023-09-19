//挂载到角色身上
//动画播放控制器
//暂且没有完全处理完动画多层融合

using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimPlayInfo
{
    public string szName = "";
    public AnimationClip clip = null;
    public float fSpeed = 1.0f;
    public float NextCrossFadeTime = 0.25f;
    public bool BlendAnim = false;
    public AnimPlayFinishCallBack FinishCall = null;
    public object CallParam = null;
}

// 动作播放结束（正常结束、被打断均会调用，循环动作不调用)
public class AnimPlayCallBackInfo
{
    public string szName = "";
    public AnimationClip clip;
    public AnimPlayFinishCallBack Callback = null;
    public object CallParam = null;
}

/// <summary>
/// 主动作、融合动作的播放信息 
/// </summary>
public class AnimatorPlayInfo
{
    public int PlayLayer = 0;
    public bool LoopPlay = false;
    public bool IsPlaying = false;
    public float AnimTimeLength = 1.0f;
    public float CurPlayTime = 0.0f; //当前播放时间
    public int AnimatorLoopTimes = 0; //Animator已经循环的次数  
    public string CurPlayAnimName = string.Empty;
    public AnimationClip CurPlayAnimClip = null;
    public AnimatorPlayState AnimPlayState = null;
}

/// <summary>
/// 对应一个Animator State信息 
/// </summary>
public class AnimatorPlayState
{
    public int AnimStateId = 0;
    public int PlayLayer = 0;
    public float LastVisitTime = 0f;
    public string AnimStateClip = "";
    public string AnimAssetName = "";
    public AnimationClip AnimClip = null;
}

/// <summary>
/// 按Animator Layer 存储的播放信息，一个变量实例为一个Layer 
/// 动作模板名字与State命名必须相同，格式为 AnimState%x_%y, %x为Layer，%y为State序号 
/// </summary>
public class AnimatorPlayLayer
{
    // 缓存在角色身上待播放的动作数量，超出的时候将覆盖最久未访问的 
    public const int MAX_ANIM_STATE = 7;
    public int Layer = 0;
    public int StopAnimStateId = 0;
    public AnimatorPlayState LastPlayState = null;
    public AnimatorPlayState[] AnimState = new AnimatorPlayState[MAX_ANIM_STATE];

    public List<AnimatorPlayState> CustomState = new List<AnimatorPlayState>(); // 自定义State名字的 

    //
    public AnimatorPlayLayer()
    {
        for (int iLoop = 0; iLoop < MAX_ANIM_STATE; ++iLoop)
            AnimState[iLoop] = new AnimatorPlayState();
    }
}

public delegate void AnimPlayFinishCallBack(BasePlayer basePlayer, bool bPlayBreak, object callParam = null);

public partial class BasePlayer : MonoBehaviour
{
    public string m_nowAnimtorName = "";
    public AnimatorOverrideController m_AnimController;
    public Animator m_Animator;
    private AnimatorPlayInfo m_AnimPlayInfo = new AnimatorPlayInfo();
    private List<AnimPlayCallBackInfo> m_ListFinishCallAnim = new List<AnimPlayCallBackInfo>();
    private Dictionary<int, AnimatorPlayLayer> m_DicAnimatorPlayInfo = new Dictionary<int, AnimatorPlayLayer>();
    private string m_LastPlayAnim = ""; // 上次执行播放的动画，用来协助判断某动画是否正在播放的 
    private List<AnimPlayInfo> m_ListPlayAnims = new List<AnimPlayInfo>();
    private bool m_IsPause = false;
    private float m_SpeedOfPause = 0f;

    private void Start()
    {
        InitAnimator();
    }
    /// <summary>
    /// 
    /// </summary>
    public RuntimeAnimatorController GetAnimatorController()
    {
        return m_Animator.runtimeAnimatorController;
    }

    /// <summary>
    ///
    /// </summary>
    public void InitAnimator()
    {
        m_AnimPlayInfo.PlayLayer = 0;
        Animator pAnimator = gameObject.AddMissingComponent<Animator>();
        // 创建一个新的，不然会共用相同的一个 
        if (null != pAnimator)
        {
            m_AnimController = new AnimatorOverrideController(pAnimator.runtimeAnimatorController);
            m_AnimController.name = "PlayerOverrrideController";
            pAnimator.runtimeAnimatorController = m_AnimController;

            m_Animator = pAnimator;
            m_Animator.runtimeAnimatorController = m_AnimController;
        }
    }

    //-----------------------------------------------------------------------------

    public void PlayAnimation(string animClipName, string abName = null, string abFolderPath = null, ParamData paramData = null)
    {
        ResourceMgr.Instance.LoadResourceAsync(animClipName, TypeInts.AnimClip, (resObj, data) =>
        {
            AnimationClip clip = (AnimationClip)resObj;
            List<AnimPlayInfo> listPlay = new List<AnimPlayInfo>();
            AnimPlayInfo info = new AnimPlayInfo();
            info.clip = clip;
            info.szName = clip.name;
            info.FinishCall = (AnimPlayFinishCallBack)data.objectParam;
            listPlay.Add(info);
            PlayAnimation(listPlay);
        }, abName, abFolderPath, paramData);
    }


    public void LoadPlayAnimationClipAll(string animClipName)
    {
        ResourceMgr.Instance.LoadAllResourceInAB<AnimationClip>(animClipName);
    }

    public void UnLoadAnimationClip(AnimationClip clipObj)
    {
        ResourceMgr.Instance.UnLoadResource(clipObj, TypeInts.AnimClip);
    }


    private void Update()
    {
        UpdateMainAnimation(Time.deltaTime);
    }

    private void UpdateMainAnimation(float fDeltaTime)
    {
        if (!m_AnimPlayInfo.IsPlaying)
            return;

        UpdateAnimPlayTime(m_AnimPlayInfo, fDeltaTime);

        // 放在最后面，等最后帧播放完毕 
        if (m_AnimPlayInfo.CurPlayTime >= m_AnimPlayInfo.AnimTimeLength)
        {
            if (m_AnimPlayInfo.LoopPlay)
            {
                m_AnimPlayInfo.CurPlayTime -= m_AnimPlayInfo.AnimTimeLength;
            }
            else
            {
                // 仅清除，不调用Animator的stop，不然跟引擎Animator进度不完全一致可能会导致骨骼没有停在最后一帧 
                ClearAnimPlayInfo(m_AnimPlayInfo);
                AnimatorFinishCallback();
                PlayNextAnimation();
            }
        }
    }

    void UpdateAnimPlayTime(AnimatorPlayInfo pPlayInfo, float fDeltaTime)
    {
        if (null != pPlayInfo.AnimPlayState && null != m_Animator)
        {
            float fNormalizedTime = -1f;
            AnimatorStateInfo pCurInfo = m_Animator.GetCurrentAnimatorStateInfo(pPlayInfo.AnimPlayState.PlayLayer);
            if (pCurInfo.shortNameHash == pPlayInfo.AnimPlayState.AnimStateId)
            {
                fNormalizedTime = pCurInfo.normalizedTime;
            }
            else
            {
                AnimatorStateInfo pNextInfo = m_Animator.GetNextAnimatorStateInfo(pPlayInfo.AnimPlayState.PlayLayer);
                if (pNextInfo.shortNameHash == pPlayInfo.AnimPlayState.AnimStateId)
                {
                    fNormalizedTime = pNextInfo.normalizedTime;
                }
            }

            //
            if (fNormalizedTime >= 0f)
            {
                //  
                int iLoopTimes = (int)fNormalizedTime;
                fNormalizedTime -= (float)iLoopTimes;
                if (iLoopTimes > pPlayInfo.AnimatorLoopTimes)
                {
                    // Animator进入下一循环了，物理动作需要把最后的动作播放出来 
                    pPlayInfo.CurPlayTime = pPlayInfo.AnimTimeLength + 0.000001f;
                }
                else
                {
                    pPlayInfo.CurPlayTime = pPlayInfo.AnimTimeLength * fNormalizedTime;
                }

                pPlayInfo.AnimatorLoopTimes = iLoopTimes;
            }
            else
            {
                pPlayInfo.CurPlayTime += fDeltaTime;
            }
        }
        else
        {
            pPlayInfo.CurPlayTime += fDeltaTime;
        }
    }



    public void PlayAnimation(List<AnimPlayInfo> listPlay)
    {
        AnimatorFinishCallback(true);

        if (null == listPlay || listPlay.Count <= 0)
            return;

        m_ListPlayAnims.Clear();

        for (int iLoop = 1; iLoop < listPlay.Count; ++iLoop)
        {
            m_ListPlayAnims.Add(listPlay[iLoop]);
        }

        // 播放第一个
        AnimPlayInfo Info = listPlay[0];
        PlayTheAnimation(Info.clip, Info.szName, Info.fSpeed, GetCrossFadeTime(Info.NextCrossFadeTime), Info.BlendAnim, Info.FinishCall, Info.CallParam);
    }

    private void PlayTheAnimation(AnimationClip clip, string szName, float fSpeed, float fCrossFadeTime, bool bBlendAnim, AnimPlayFinishCallBack FinishCall, object callParam = null
  )
    {
        if (string.IsNullOrEmpty(szName) || null == m_Animator)
        {
            AddAnimFinishCallback(szName, null, FinishCall, callParam);
            return;
        }

        PlayCharaAnim(szName, clip, fSpeed, fCrossFadeTime, bBlendAnim, FinishCall, callParam);
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    private void PlayCharaAnim(string szAnimName, AnimationClip pClip, float fSpeed = 1f, float fTransDuration = 0.25f, bool bBlendAnim = false, AnimPlayFinishCallBack FinishCall = null, object callParam = null)
    {
        if (string.IsNullOrEmpty(szAnimName) || null == pClip || (pClip.isLooping && IsAnimPlaying(szAnimName)))
            return;

        PlayCharaAnimInternal(szAnimName, pClip, fSpeed, fTransDuration, bBlendAnim, FinishCall, callParam);
    }

    /// <summary>
    /// 播放主动作，仅供内部调用  
    /// </summary>
    void PlayCharaAnimInternal(string szAnimName, AnimationClip pClip, float fSpeed, float fTransDuration, bool bBlendAnim, AnimPlayFinishCallBack FinishCall, object callParam = null)
    {
        if (null == pClip)
            return;

        if (m_AnimPlayInfo.CurPlayAnimName != szAnimName)
            StopAnimation(false, pClip);

        bool bLoopPlay = pClip.isLooping;
        float fAnimLen = pClip.length;
        m_AnimPlayInfo.CurPlayAnimName = szAnimName;
        m_AnimPlayInfo.CurPlayAnimClip = pClip;
        m_AnimPlayInfo.IsPlaying = true;
        m_AnimPlayInfo.LoopPlay = bLoopPlay;
        m_AnimPlayInfo.AnimTimeLength = fAnimLen;

        DoPlayCharaAnim(szAnimName, pClip, m_AnimPlayInfo, fSpeed, fTransDuration, bBlendAnim, FinishCall, callParam);
    }

    private void DoPlayCharaAnim(string szAnimName, AnimationClip pClip, AnimatorPlayInfo pAnimPlayInfo, float fSpeed,
       float fTransDuration, bool bBlendAnim, AnimPlayFinishCallBack FinishCall, object callParam = null, string szForceAnimStateName = "")

    {
        if (null == pClip || null == m_Animator || null == m_AnimController || null == pAnimPlayInfo)
            return;

        // 先清空 
        pAnimPlayInfo.AnimPlayState = null;
        pAnimPlayInfo.AnimatorLoopTimes = 0;

        AnimatorPlayLayer pPlayLayer = GetAnimatorPlayLayer(pAnimPlayInfo.PlayLayer);
        if (null == pPlayLayer)
            return;

        AnimatorPlayState pToPlay = null;
        float fAnimLen;
        //设置播放速度(融合动作播放速度只能和主动作保持一致)
        if (!bBlendAnim)
        {
            SetAnimatorSpeed(fSpeed);
        }
        //如果中间加入，直接设置到指定动作时间

        if (null == pPlayLayer.LastPlayState || pPlayLayer.LastPlayState.AnimClip != pClip)
        {
            pToPlay = FindPlayState(pPlayLayer, pClip, szForceAnimStateName);
        }
        else
        {
            AnimatorStateInfo pInfo = m_Animator.GetCurrentAnimatorStateInfo(pPlayLayer.Layer);
            // 重复调用播放循环动作不重新播放。非循环动作只要调用就重新播放
            if (!pInfo.loop)
            {
                pAnimPlayInfo.AnimPlayState = pPlayLayer.LastPlayState;
                if (!bBlendAnim)
                {
                    m_Animator.PlayInFixedTime(pPlayLayer.LastPlayState.AnimStateId, pPlayLayer.Layer, 0);
                }
                else
                {
                    m_Animator.CrossFadeInFixedTime(pPlayLayer.LastPlayState.AnimStateId, fTransDuration, pPlayLayer.Layer);
                }
            }
            m_LastPlayAnim = szAnimName;
            AddAnimFinishCallback(szAnimName, pClip, FinishCall, callParam);
            //
            return;
        }

        if (null == pToPlay)
            return;

        // TODO ：注意，设置AnimationClip会导致“will trigger a reallocation of the animator's clip bindings.”
        // 后面需要按官方文档的方式，同时加载的动作文件等全部加载完再一次性Add 
        if (m_AnimController[pToPlay.AnimStateClip] != pClip)
            m_AnimController[pToPlay.AnimStateClip] = pClip;

        pToPlay.AnimClip = pClip;
        pToPlay.AnimAssetName = szAnimName;
        pPlayLayer.LastPlayState = pToPlay;
        pAnimPlayInfo.AnimPlayState = pToPlay;

        if (!bBlendAnim)
        {
            m_Animator.PlayInFixedTime(pToPlay.AnimStateId, pPlayLayer.Layer);
        }
        else
        {
            m_Animator.CrossFadeInFixedTime(pToPlay.AnimStateId, fTransDuration, pPlayLayer.Layer);
        }
        m_LastPlayAnim = szAnimName;
        AddAnimFinishCallback(szAnimName, pClip, FinishCall, callParam);
    }

    /// <summary>
    /// 添加动画结束回调, AnimState可以传null，下一次update将被回调 
    /// </summary>
    private void AddAnimFinishCallback(string szName, AnimationClip clip, AnimPlayFinishCallBack FinishCall, object callParam = null)
    {
        if (FinishCall == null)
            return;

        if (clip.isLooping)
            return;

        AnimPlayCallBackInfo Info = new AnimPlayCallBackInfo();
        Info.clip = clip;
        Info.Callback = FinishCall;
        Info.szName = szName;
        Info.CallParam = callParam;

        m_ListFinishCallAnim.Add(Info);
    }

    private void AnimatorFinishCallback(bool bBreak = false)
    {
        if (m_ListFinishCallAnim.Count < 1)
            return;
        //
        for (int iLoop = 0; iLoop < m_ListFinishCallAnim.Count; iLoop++)
        {
            AnimPlayCallBackInfo Info = m_ListFinishCallAnim[iLoop];
            if (Info.clip == null)
            {
                m_ListFinishCallAnim.Remove(Info);
                Info.Callback(this, true, Info.CallParam);
                continue;
            }

            if (Info.szName == m_LastPlayAnim)
            {
                m_ListFinishCallAnim.Remove(Info);
                Info.Callback(this, bBreak, Info.CallParam);
            }
        }
    }


    private void ClearAnimPlayInfo(AnimatorPlayInfo pPlayInfo)
    {
        if (null == pPlayInfo)
            return;

        pPlayInfo.CurPlayAnimName = string.Empty;
        pPlayInfo.CurPlayAnimClip = null;
        pPlayInfo.CurPlayTime = 0;
        pPlayInfo.IsPlaying = false;
    }


    /// <summary>
    /// 动作是否在播放
    /// </summary>
    public bool IsAnimPlaying(string szAnimName)
    {
        if (m_AnimPlayInfo.CurPlayAnimName == szAnimName && m_AnimPlayInfo.IsPlaying)
            return true;
        return false;
    }

    /// <summary>
    /// 停止主动作 
    /// </summary>
    private void StopAnimation(bool bStopBlendAnim = true, AnimationClip pNextPlayAnim = null)
    {
        DoStopAnimator(m_AnimPlayInfo.PlayLayer, pNextPlayAnim);
        ClearAnimPlayInfo(m_AnimPlayInfo);
    }

    /// <summary>
    /// 如果下一个播放的动作非空，则不停止骨骼动作，等过渡即可  
    /// </summary>
    private void DoStopAnimator(int iLayer, AnimationClip pNextPlayAnim = null)
    {
        if (null == m_Animator || null == m_AnimController)
            return;

        if (null != pNextPlayAnim)
            return;

        AnimatorPlayLayer pPlayLayer = GetAnimatorPlayLayer(iLayer);
        if (null == pPlayLayer)
            return;

        pPlayLayer.LastPlayState = null;
        m_Animator.Play(pPlayLayer.StopAnimStateId, pPlayLayer.Layer);
        m_Animator.Update(0f);
    }


    /// <summary>
    /// 获取或者新建一个播放Layer记录  
    /// </summary>
    private AnimatorPlayLayer GetAnimatorPlayLayer(int iLayer)
    {
        AnimatorPlayLayer pInfo = null;
        if (!m_DicAnimatorPlayInfo.ContainsKey(iLayer))
        {
            pInfo = new AnimatorPlayLayer();
            pInfo.Layer = iLayer;
            pInfo.LastPlayState = null;
            pInfo.StopAnimStateId = Animator.StringToHash(string.Format("StopState{0}", iLayer));
            for (int iLoop = 0; iLoop < AnimatorPlayLayer.MAX_ANIM_STATE; ++iLoop)
            {
                AnimatorPlayState pPlayState = pInfo.AnimState[iLoop];
                string szStateClip = string.Format("AnimState{0}_{1}", iLayer, iLoop);
                pPlayState.AnimStateClip = szStateClip;
                pPlayState.AnimStateId = Animator.StringToHash(szStateClip);
                pPlayState.PlayLayer = iLayer;
            }

            m_DicAnimatorPlayInfo.Add(iLayer, pInfo);
        }
        else
            pInfo = m_DicAnimatorPlayInfo[iLayer];

        return pInfo;
    }


    public void SetAnimatorSpeed(float fSpeed)
    {
        if (null != m_Animator && Math.Abs(m_Animator.speed - fSpeed) > 0.01f)
            m_Animator.speed = fSpeed;
    }


    /// <summary>
    ///
    /// </summary>
    void ResetAnimation()
    {
        if (null != m_Animator && m_Animator.enabled)
        {
            AnimatorStateInfo pCurInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
            m_Animator.Play(pCurInfo.shortNameHash);
            m_Animator.Update(0f);

            // 清理播放状态 
            StopAnimation();
        }
    }

    /// <summary>
    /// 查找已经存在的、动作一致的State，如果没有找到，则返回最长时间未访问的 
    /// 不得返回Null
    /// </summary>
    private AnimatorPlayState FindPlayState(AnimatorPlayLayer pPlayLayer, AnimationClip AnimClip, string szCustomStateName = "")
    {
        // 如果指定了播放的state 
        if (!string.IsNullOrEmpty(szCustomStateName))
        {
            int iCustomStateId = Animator.StringToHash(szCustomStateName);
            for (int iLoop = 0; iLoop < pPlayLayer.CustomState.Count; ++iLoop)
            {
                AnimatorPlayState pState = pPlayLayer.CustomState[iLoop];
                if (pState.AnimStateId == iCustomStateId)
                    return pState;
            }

            // Add New
            AnimatorPlayState pPlayState = new AnimatorPlayState();
            pPlayState.AnimStateClip = szCustomStateName;
            pPlayState.AnimStateId = Animator.StringToHash(szCustomStateName);
            pPlayState.PlayLayer = pPlayLayer.Layer;
            pPlayLayer.CustomState.Add(pPlayState);
            return pPlayState;
        }

        //
        int iFoundIndex = -1, iEmptyIndex = -1, iVisitTimeIndex = -1;
        float fVisitTime = float.MaxValue;
        for (int iLoop = 0; iLoop < AnimatorPlayLayer.MAX_ANIM_STATE; ++iLoop)
        {
            AnimatorPlayState pState = pPlayLayer.AnimState[iLoop];
            if (null != pState.AnimClip && pState.AnimClip == AnimClip)
            {
                iFoundIndex = iLoop;
                break;
            }
            else if (null == pState.AnimClip)
            {
                if (-1 == iEmptyIndex)
                    iEmptyIndex = iLoop;
            }
            else if (pState.LastVisitTime < fVisitTime)
            {
                fVisitTime = pState.LastVisitTime;
                iVisitTimeIndex = iLoop;
            }
        }

        //
        if (-1 == iFoundIndex)
        {
            if (-1 != iEmptyIndex)
                iFoundIndex = iEmptyIndex;
            else if (-1 != iVisitTimeIndex)
                iFoundIndex = iVisitTimeIndex;
        }

        // 预防，原则上不会发生 
        if (iFoundIndex < 0)
            iFoundIndex = 0;

        pPlayLayer.AnimState[iFoundIndex].LastVisitTime = Time.time;
        return pPlayLayer.AnimState[iFoundIndex];
    }

    private AnimatorPlayInfo GetAnimPlayInfo(string szAnimName)
    {
        if (m_AnimPlayInfo.CurPlayAnimName == szAnimName)
            return m_AnimPlayInfo;
        return null;
    }

    private void PlayNextAnimation()
    {
        if (m_ListPlayAnims.Count < 1)
            return;
        AnimPlayInfo Info = m_ListPlayAnims[0];
        m_ListPlayAnims.RemoveAt(0);
        string name = Info.clip.name;
        PlayTheAnimation(Info.clip, name, Info.fSpeed, GetCrossFadeTime(Info.NextCrossFadeTime), Info.BlendAnim, Info.FinishCall, Info.CallParam);
    }

    /// <summary>
    /// 得到动作融合时间
    /// </summary>
    /// <param name="_crossFadeTime">如果是-1，则使用，默认时间</param>
    public float GetCrossFadeTime(float _crossFadeTime = -1.0f)
    {
        if (null == m_Animator)
            return 0f;

        float fDefaultCrossTime = (_crossFadeTime < 0f) ? 0.1f : _crossFadeTime;
        return fDefaultCrossTime;
    }

    public void PauseAnimation(bool bIsPause)
    {
        m_IsPause = bIsPause;
        if (m_Animator != null)
        {
            if (bIsPause)
            {
                m_SpeedOfPause = m_Animator.speed;
            }

            float fSpeed = !m_IsPause ? m_SpeedOfPause : 0;
            m_Animator.speed = fSpeed;
        }
    }

    private void OnDestroy()
    {
        foreach (var item in m_ListFinishCallAnim)
        {
            if (item.Callback != null)
                item.Callback = null;
        }

        foreach (var item in m_ListPlayAnims)
        {
            if (item.FinishCall != null)
                item.FinishCall = null;
        }

        m_ListFinishCallAnim.Clear();
        m_ListPlayAnims.Clear();
        m_AnimPlayInfo = new AnimatorPlayInfo();
    }
}