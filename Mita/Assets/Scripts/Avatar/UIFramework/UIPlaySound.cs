
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPlaySound : MonoBehaviour, IPointerClickHandler
{
    public enum SoundMode
    {
        Click = 1,
        Enable = 2,
    }

#if UNITY_EDITOR
    // 默认音效设置
    const string path = "Assets/Resources_moved/Common/UISound.asset";
#endif

    // 音效类型
    public SoundMode Mode = SoundMode.Click;

    private bool m_isvalid;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!m_isvalid)
            return;

        if (Mode == SoundMode.Click)
        {
            //AudioPlayManager.Instance.PlaySound();
        }
    }

    public void OnEnable()
    {
        m_isvalid = true;

        if (Mode == SoundMode.Enable)
        {

        }
    }

    private void OnDisable()
    {

    }

    private void OnDestroy()
    {
        m_isvalid = false;
    }
}