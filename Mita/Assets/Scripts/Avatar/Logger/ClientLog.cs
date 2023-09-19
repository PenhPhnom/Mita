using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

public class ClientLog : Singleton<ClientLog>
{
    private string m_LogFilePath = "";
    private bool m_EnableLog = true;

    public override void Init()
    {
        if (!CanOutputLog())
            return;
        SRDebug.Init();
    }

    /// <summary>
    /// 是否可以输出log
    /// </summary>
    private bool CanOutputLog()
    {
        return m_EnableLog;
    }

    //
    public void LogError(object message)
    {
        if (null == message)
        {
            return;
        }

        LogToFile(message);
        LogErrorTag(message);
    }

    //
    public void LogWarning(object message)
    {
        if (null == message)
        {
            return;
        }

        LogToFile(message);
        try
        {
            Debug.LogWarning(message);
        }
        catch (System.Exception e)
        {
            Debug.Log(message);
        }
    }
    //
    public void Log(object message)
    {
        if (!CanOutputLog())
        {
            return;
        }

        if (null == message)
        {
            return;
        }
        LogToFile(message);
        LogTag((string)message);
    }


    public void Log(object message, params object[] args)
    {
        if (!CanOutputLog())
            return;

        string msg = (string)message;
        if (string.IsNullOrEmpty(msg) || args.Length == 0)
        {
            LogTag(message.ToString());
        }
        else
        {
            LogTag(string.Format(msg, args));
        }
    }

    public void Log(object message, params string[] args)
    {
        if (!CanOutputLog())
            return;

        string msg = (string)message;
        if (string.IsNullOrEmpty(msg) || args.Length == 0)
        {
            LogTag(message.ToString());
        }
        else
        {
            string str = "";
            for (int i = 0; i < args.Length; i++)
            {
                str += " " + args[i];
            }
            LogTag(msg + str);
        }
    }


    /// <summary>
    /// 写文件里log
    /// </summary>
    /// <param name="strLog"></param>
    private void LogToFile(object strLog)
    {
#if UNITY_IPHONE || UNITY_ANDROID
        return;
#endif
    }

    private void LogTag(object message)
    {
        Debug.Log($"-><b><color=#008B00><size=13>Unity Log：{message}</size></color></b>");
    }

    private void LogErrorTag(object message)
    {
        Debug.LogError("Unity: LogError： " + message);
    }

    public override void OnRelease()
    {

    }
}
