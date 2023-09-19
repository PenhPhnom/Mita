using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorDebug
{
    /// <summary>
    /// UI预设。
    /// </summary>
    public const string UI_PREFAB = "Assets/Resources_moved/UI/Prefabs/Panel";
    /// <summary>
    /// Atlas。
    /// </summary>
    public const string UI_ATLAS = "Assets/Resources_moved/UI/Atlas";
    /// <summary>
    /// Color
    /// </summary>
    public const string UI_COLOR = "Assets/Resources_moved/UI/Color";
    /// <summary>
    /// icon路径
    /// </summary>
    public const string UI_IOCN_PATH = "Assets/Resources_moved/UI/Icon";

    /// <summary>
    /// Key 要加载的资源名字  value ：path 路径
    /// </summary>
    private static Dictionary<string, string> m_PathDic = new Dictionary<string, string>()
    {
        ["TestAtlas"] = $"{UI_ATLAS}/TestAtlas.asset",
        ["guimain_panel"] = $"{UI_PREFAB}/Loading/guimain_panel.prefab",
        ["GUIGuide_Panel"] = $"{UI_PREFAB}/Guide/GUIGuide_Panel.prefab",
        ["GUIMessageBox"] = $"{UI_PREFAB}/MessageBox/GUIMessageBox.prefab",
        ["GUIMessageTips"] = $"{UI_PREFAB}/MessageBox/GUIMessageTips.prefab",
        ["ColorTransform"] = $"{UI_COLOR}/ColorTransform.asset",
    };

    public static string Path(string name)
    {
        string path = m_PathDic.ContainsKey(name) ? m_PathDic[name] : "";
        if (string.IsNullOrEmpty(path))
            path = m_PathDic.ContainsKey(name.ToLower()) ? m_PathDic[name.ToLower()] : "";

        if (string.IsNullOrEmpty(path))
            ClientLog.Instance.LogError(path);

        return path;
    }
}
