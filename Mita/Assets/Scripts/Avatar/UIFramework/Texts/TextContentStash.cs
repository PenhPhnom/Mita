using UnityEngine.UI;

/// <summary>
/// 会将文字内容单独缓存的Text
/// </summary>
public class TextContentStash : TextContentStashBase
{
    public Text uiText;
    protected override string text
    {
        get
        {
            if (uiText == null)
            {
                uiText = GetComponent<Text>();
            }
            return uiText.text;
        }
        set
        {
            if (uiText == null)
            {
                uiText = GetComponent<Text>();
            }
            uiText.text = value;
        }
    }

}
