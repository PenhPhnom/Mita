using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 会将文字内容单独缓存的Text
/// </summary>
public abstract class TextContentStashBase : MonoBehaviour
{
    protected abstract string text
    {
        get;
        set;
    }

    [SerializeField]
    [TextArea(3, 3)]
    protected string m_StashedContent;

    public string StashedContent { get => m_StashedContent; }

    [Button]
    public void UpdateStash()
    {
        if (string.IsNullOrEmpty(this.text))
            return;

        m_StashedContent = this.text;
        ValidateStashedContent();
        this.text = null;
    }

    [Button]
    public void ApplyStash()
    {
        this.text = m_StashedContent;
    }

    [Button]
    public void DropStash()
    {
        m_StashedContent = null;
    }

    [Button]
    void ValidateStashedContent()
    {
        var lines = m_StashedContent.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        if (lines.Length > 1)
        {
            ClientLog.Instance.LogError($"保存的Text内容不合法   [{m_StashedContent}]");
            m_StashedContent = null;
        }
    }
}
