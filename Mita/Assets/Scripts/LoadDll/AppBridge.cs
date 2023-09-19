using System.Runtime.InteropServices;
using UnityEngine;
using System;
using AOT;
//using HybridCLR;

/// <summary>
/// 处理给 Unity---> App消息
/// </summary>

//目前是因为Android 不想给so文件 只能这么写了 要自己做好管理 别乱了 本应该一个 [DllImport("MyLibrary")] 完美的解决 
//不要写两个脚本 一个安卓一个IOS 直接在此脚本写 
public static class AppBridge
{
    public static void SendMessageToAvatarApp(string msg)
    {
#if UNITY_IPHONE && !UNITY_EDITOR
        sendMessageToAvatarApp(msg);
#elif UNITY_ANDROID && !UNITY_EDITOR
        sendMessageToAvatarAppAndro(msg);
#endif
        return;
    }

    //******************************************IOS跟Android 接口***********************************************//
#if UNITY_IOS
    ////例如 PS
    [DllImport("__Internal")]
    public static extern void sendMessageToAvatarApp(string msg);

    //[DllImport("__Internal")]
    //public extern static void AVATARRegisterFaceDrivingDataCallback(AVATARFaceDrivingDataCallbackDelegate callback);

    //[DllImport("__Internal")]
    //public extern static void AVATARRemoveFaceDrivingDataCallback();
#elif UNITY_ANDROID && !UNITY_EDITOR
    public static AndroidJavaClass call;
    public static void sendMessageToAvatarAppAndro(string msg)
    {
        if (call == null)
            call = new AndroidJavaClass("com.wemomo.avatar.unity.UnityMessenger");

        call.CallStatic("sendMessageToApp", msg);
    }
#endif




}

