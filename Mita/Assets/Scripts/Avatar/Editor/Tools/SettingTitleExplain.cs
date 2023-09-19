/**
==========================
  - FileName: SettingTitleExplain.cs
  - CreatorName: 王唐新
  - CreateTime: 2023 三月 17 星期五
  - 功能描述: 设置标题注解
==========================
*/

using UnityEditor;
using System.IO;
using System;
using System.Text.RegularExpressions;
using static System.DayOfWeek;

public class SettingTitleExplain : AssetModificationProcessor
{
    private static string week;
    private static string currentUserName;

    private static void OnWillCreateAsset(string path)
    {
        path = path.Replace(".meta", "");
        if (path.EndsWith(".cs"))
        {
            string fileName = Regex.Match(path, @"[^/]*$").Value;
            string str = File.ReadAllText(path);
            DateTime dt = DateTime.Now;
            #region 得到当前周几
            //想不到什么好的方法 欢迎修改
            DayOfWeek dayOfWeek = (DayOfWeek) Enum.Parse(typeof(DayOfWeek), DateTime.Now.DayOfWeek.ToString());
            switch (dayOfWeek)
            {
                case Monday:
                    week = "星期一";
                    break;
                case Tuesday:
                    week = "星期二";
                    break;
                case Wednesday:
                    week = "星期三";
                    break;
                case Thursday:
                    week = "星期四";
                    break;
                case Friday:
                    week = "星期五";
                    break;
                case Saturday:
                    week = "星期六";
                    break;
                case Sunday:
                    week = "星期日";
                    break;
                default:
                    week = "显示周几出bug了，请联系作者，或自行修改SettingTitleExplain" ;
                    break;
            }

            #endregion
            string authorNameLine = ReadLineFromFile(path, 4);
            string authorName = authorNameLine.Substring(authorNameLine.IndexOf(":") + 1);
            if (authorName.Contains("AuthorName"))
                currentUserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            str = str.Replace("#AuthorName#", currentUserName)
                .Replace("#CreateTime#", dt.ToString($"yyyy MM dd {week} "))
                .Replace(dt.ToString("MM"), GetChineseMonth(dt.Month))
                .Replace("#FileName#", fileName).Replace("#path#", path);
            File.WriteAllText(path, str);
            AssetDatabase.Refresh();
        }
    }

    static readonly string[] chineseMonth =
        {"一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月"};

    private static string GetChineseMonth(int month) => chineseMonth[month - 1];

    private static string ReadLineFromFile(string filePath, int lineNumber)
    {
        string line = String.Empty;
        if (!File.Exists(filePath)) return line;
        using StreamReader reader = new StreamReader(filePath);
        for (int i = 1; i < lineNumber; i++)
            if (reader.ReadLine() == null) break;
        line = reader.ReadLine();
        return line;
    }
}