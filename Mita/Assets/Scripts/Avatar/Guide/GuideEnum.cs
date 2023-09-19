using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//可以走表工具  定义枚举
/// <summary>
/// 引导判断条件类型
/// </summary>
public enum EGuideConditionType
{
    None = 0,
    TestTrue = 1,
    Level = 2, // 玩家等级
    OpenUI = 3, // 打开 UI
    CloseUI = 4, // 关闭 UI
    ClickBtn = 5,//点击一个UI按钮
}

/// <summary>
/// 新手引导 事件触发
/// </summary>
public enum EGuideTriggerEventType
{
    NONE,
    UI_OPEN_PANEL,// 打开 某个界面
    UI_MAIN_BTNXXXX, //主界面的 xxx按钮
}

public enum EGuideTargetType
{
    None = 0,
    FixUIWidget = 1,

}

/// <summary>
/// 标记所处环境，在选择使用何种相机和转换计算时使用
/// </summary>
public enum EGuideTargetEnvType
{
    None = 0,
    UIObject = 1,
    All = 99
}

public enum EMaskFade
{
    AlwaysOn = 0,
    FadeByClick = 1
}


public enum EMaskHighLightShape
{
    Circle = 0,
    Rect = 1,
    CornerRect = 2,
    FullScreen = 99,
    NoBlackMask = 100
}

public enum EGuideTargetMarkerType
{
    None = 0,
    Circle = 1,
    Rect = 2,
    Finger = 3,
    CircleFinger = 4,
    RectFinger = 5,
    SmallCircle = 6,
    RectFx = 7,
    DragArrow = 10
}

public enum EAnchorStretch
{
    Null = 0,
    LeftTop = 1,
    CenterTop = 2,
    RightTop = 3,
    LeftCenter = 4,
    Center = 5,
    RightCenter = 6,
    LeftBottom = 7,
    CenterBottom = 8,
    RightBottom = 9
}

// 0、没有引导板子
// 1、短句，箭头朝上
// 2、短句，箭头朝下
// 3、小头像，箭头朝左,头像在右
// 4、小头像，箭头朝右，头像在左
// 5、半身像，贴底部
public enum EPlotType
{
    None = 0,
    SmallDialogArrowUp = 1,
    SmallDialogArrowDown = 2,
    MidDialogIconRight = 3,
    MidDialogIconLeft = 4,
    HalfIcon = 5
}

/// <summary>
/// 引导工具类
/// </summary>
public class GuideEnum
{

}
