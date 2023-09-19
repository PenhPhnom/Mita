using UnityEngine;
using System.Collections;

public enum EPanelLayerType
{
    Bottom = 1, //底部常驻
    Panel = 2, //面板 全屏界面
    Pop = 3, //弹窗对话框 附带黑底遮罩
    Front = 4, //面板上层 子界面等 Pop和Front同层级
    Top = 5, //顶部常驻
    FlyTip = 6, //走马灯、广播
    Plot = 7, //剧情
    Guide = 8, //教学
    Loading = 9, //Loading界面
    NetError = 10//网络错误弹框
}

#region Global delegate 委托
public delegate void StateChangedEvent(object sender, EnumObjectState newState, EnumObjectState oldState);
public delegate void OnTouchEventHandle(GameObject _listener, object _args, params object[] _params);
#endregion

#region Global enum 枚举
/// <summary>
/// 对象当前状态 
/// </summary>
public enum EnumObjectState
{
    /// <summary>
    /// The none.
    /// </summary>
    None,
    /// <summary>
    /// The initial.
    /// </summary>
    Initial,
    /// <summary>
    /// The loading.
    /// </summary>
    Loading,
    /// <summary>
    /// The ready.
    /// </summary>
    Ready,
    /// <summary>
    /// the Showing 正在播放出现动画
    /// </summary>
    Showing,
    /// <summary>
    /// the Normal 正常状态
    /// </summary>
    Normal,
    /// <summary>
    /// the Disappearing 正在播放隐藏动画
    /// </summary>
    Disappearing,
    /// <summary>
    /// The disabled.
    /// </summary>
    Disabled,
    /// <summary>
    /// The closing.
    /// </summary>
    Closing
}

/// <summary>
/// Enum user interface type.
/// UI面板类型
/// </summary>
public enum EnumUIType : int
{
    None = -1,
    Login = 1,
    LOADING = 2,
    CRETEPLAYER = 3,
    CHATROOM = 4,
    GUIDE = 5,
    MESSAGEBOX = 6,
    MESSAGETIPS = 7,
    MAX = 999
}

public enum EnumTouchEventType
{
    OnClick,
    OnDoubleClick,
    OnDown,
    OnUp,
    OnEnter,
    OnExit,
    OnSelect,
    OnUpdateSelect,
    OnDeSelect,
    OnDrag,
    OnDragEnd,
    OnDrop,
    OnScroll,
    OnMove,
    OnLongPress,
}

public enum ESceneState
{
    Init,
    Loading,
    Loaded,
}

public enum EnumSceneType
{
    None = 0,
    StartGame,
    LoadingScene,
    LoginScene,
    MainScene
}

public enum EMaskNameType
{
    GUIDEMASK = 1,
}

public enum ELanguageType
{
    CHINA,
    ENGLISH
}


#endregion

#region Defines static class & cosnt

/// <summary>
/// 路径定义。
/// </summary>
public static class UIPathDefines
{
    /// <summary>
    /// 目前没走表示什么的  需要暂且先在这里定义
    /// </summary>
    public static string GetPrefabNameByType(EnumUIType uiType)
    {
        string pPanelName = string.Empty;
        switch (uiType)
        {
            case EnumUIType.LOADING:
                pPanelName = "loading_panel";
                break;
            case EnumUIType.CRETEPLAYER:
                pPanelName = "create_player_panel";
                break;
            case EnumUIType.Login:
                pPanelName = "guimain_panel";
                break;
            case EnumUIType.CHATROOM:
                pPanelName = "chatroom_panel";
                break;
            case EnumUIType.MESSAGEBOX:
                pPanelName = "GUIMessageBox";
                break;
            case EnumUIType.MESSAGETIPS:
                pPanelName = "GUIMessageTips";
                break;
            case EnumUIType.GUIDE:
                pPanelName = "GUIGuide_Panel";
                break;
            default:
                ClientLog.Instance.LogError($"Not Find EnumUIType! type: {uiType.ToString()}");
                break;
        }
        return pPanelName;
    }

    /// <summary>
    /// 
    /// </summary>
    public static BaseUI GetUIScriptByType(EnumUIType uiType)
    {
        BaseUI pScriptType = null;
        switch (uiType)
        {
            case EnumUIType.Login:
                pScriptType = new GUIMainPanel();
                break;
            case EnumUIType.MESSAGEBOX:
                pScriptType = new GUIMessageBox();
                break;
            case EnumUIType.MESSAGETIPS:
                pScriptType = new GUIMessageTips();
                break;
            case EnumUIType.GUIDE:
                pScriptType = new GUIGuide_Panel();
                break;
            default:
                ClientLog.Instance.Log("Not Find EnumUIType! type: ", uiType.ToString());
                break;
        }
        return pScriptType;
    }
}
#endregion
