using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 会将文字内容单独缓存的Text
/// </summary>
public class ContentStashText : MonoBehaviour
{
    //[SerializeField]
    //protected string m_StashedContent;

    //public string StashedContent { get => m_StashedContent; }

    //public void UpdateStash()
    //{
    //    if (string.IsNullOrEmpty(this.text))
    //        return;

    //    m_StashedContent = this.text;
    //    this.text = null;
    //}

    //public void ApplyStash()
    //{
    //    this.text = m_StashedContent;
    //}

    //public void DropStash()
    //{
    //    m_StashedContent = null;
    //}
    [SerializeField]
    public class TextData
    {
        public Text Text;

        public int TagId;

        public TextData(Text text, int langId)
        {
            Text = text;
            TagId = langId;
        }
    }

    public TextData TextDataClass;

    private void Awake()
    {
        //TextDataClass.Text = DataLoader.Instance.GetTranslate();
    }

    private void OnEnable()
    {
        //添加切换语言事件

    }

    private void OnDisable()
    {
        //移除切换语言事件

    }
}
