using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 首行缩进文本
/// </summary>
public class IndentText : Text
{
    private const string space = "\u00A0\u00A0\u00A0";

    public string realText { get => base.text; }

    public override string text
    {
        get { return space + base.text; }
    }
}
