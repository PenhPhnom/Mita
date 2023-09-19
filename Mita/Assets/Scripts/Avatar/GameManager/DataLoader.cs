using Bright.Serialization;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using cfg;
using System;
using System.Threading.Tasks;

/// <summary>
/// 表工具用的 Luban
/// </summary>
public class DataLoader : Singleton<DataLoader>
{
    private Tables m_Tables;
    private readonly Dictionary<string, byte[]> tableDatasDic = new Dictionary<string, byte[]>();
    private float loadTableTime = 0f;

    /// <summary>
    /// 一次性加载完说有的表数据，加载表数据
    /// </summary>
    public void LoadTableData()
    {
        loadTableTime = Time.realtimeSinceStartup;
#if UNITY_EDITOR
        m_Tables = new Tables((file) =>
        {
            try
            {
                return new ByteBuf(File.ReadAllBytes($"{Application.dataPath}/Resources_moved/ResDatas/{file}.bytes"));
                //turn new ByteBuf(ReadAllBytesAsync(file).Result);
            }
            catch
            {
                return null;
            }
        });
        ClientLog.Instance.Log($"LoadTableData: 加载表数据使用的时间{Time.realtimeSinceStartup - loadTableTime }");
#else
        ResourceMgr.Instance.LoadAllResourceInAB<TextAsset>("datas", (obj) =>
         {
             for (int index = 0; index < obj.Length; index++)
             {
                 tableDatasDic.Add(obj[index].name, obj[index].bytes);
             }

             m_Tables = new Tables((name) =>
             {
                 try
                 {
                     ByteBuf byteBufData = new ByteBuf(tableDatasDic[name]);
                     return byteBufData;
                 }
                 catch
                 {
                     return null;
                 }
                
             });
             ResourceMgr.Instance.UnLoadAllResourceInAB("datas");
             ClientLog.Instance.Log($"LoadTableData: 加载表数据使用的时间{Time.realtimeSinceStartup - loadTableTime }");
         });
#endif
    }


    public cfg.Config.TbGuideConfig GetTbGuideConfig()
    {
        return m_Tables != null ? m_Tables.TbGuideConfig : null;
    }

    public cfg.Config.TbGuideDetailConfig GetTbGuideDetailConfig()
    {
        return m_Tables != null ? m_Tables.TbGuideDetailConfig : null;
    }

    public cfg.Config.GuideDetailConfig GetGuideDetailConfigByTag(string tag)
    {
        if (GetTbGuideDetailConfig().DataMap.ContainsKey(tag))
            return GetTbGuideDetailConfig().DataMap[tag];

        ClientLog.Instance.LogError($"GuideDetailConfig 表数据不存在{tag}");

        return null;
    }

    public cfg.Config.TbGuideShowConfig GetTbGuideShowConfig()
    {
        return m_Tables != null ? m_Tables.TbGuideShowConfig : null;
    }


    public cfg.Config.GuideShowConfig GetGuideShowConfigByTag(string tag)
    {
        if (GetTbGuideShowConfig().DataMap.ContainsKey(tag))
            return GetTbGuideShowConfig().DataMap[tag];

        ClientLog.Instance.LogError($"GuideShowConfig 表数据不存在{tag}");

        return null;
    }

    public cfg.Config.TbTranslateConfig GetTbTranslateConfig()
    {
        return m_Tables != null ? m_Tables.TbTranslateConfig : null;
    }
    public cfg.Config.TbSanXiaoConfig GetTbSanXiaoConfig()
    {
        return m_Tables != null ? m_Tables.TbSanXiaoConfig : null;
    }
    public string GetTranslateByID(string tag)
    {
        if (GetTbTranslateConfig().DataMap.ContainsKey(tag))
        {
            // 判断是哪种语言 返回哪种语言的内容 TODO 
            ELanguageType lan = ELanguageType.CHINA;
            string content = "";
            switch (lan)
            {
                case ELanguageType.CHINA:
                    content = GetTbTranslateConfig().DataMap[tag].Content;
                    break;
                case ELanguageType.ENGLISH:
                    content = GetTbTranslateConfig().DataMap[tag].ContentENG;
                    break;
            }

            return content;
        }

        return null;
    }


    public async Task<byte[]> ReadAllBytesAsync(string file)
    {
        byte[] v = await File.ReadAllBytesAsync($"{Application.dataPath}/Resources_moved/ResDatas/{file}.bytes");
        return v;
    }
    public override void OnRelease()
    {
        if (m_Tables != null)
            m_Tables = null;

        if (tableDatasDic != null)
            tableDatasDic.Clear();
    }
}