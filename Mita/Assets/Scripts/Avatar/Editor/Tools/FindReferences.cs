using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Reflection;

public static class FindReferences
{
    [MenuItem("Assets/Find References", false)]
    static void Find()
    {
        EditorSettings.serializationMode = SerializationMode.ForceText;
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (!string.IsNullOrEmpty(path))
        {
            ClientLog.Instance.Log("开始查找哪里引用到资源:" + path);
            string guid = AssetDatabase.AssetPathToGUID(path);
            //string guid = FindReferences.GetFileID(Selection.activeObject).ToString();
            // ClientLog.Instance.Log("guid : " + guid);
            List<string> withoutExtensions = new List<string>() { ".prefab", ".unity", ".mat", ".asset" };
            string[] files = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories)
                .Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();
            int startIndex = 0;

            EditorApplication.update = delegate ()
            {
                string file = files[startIndex];
                bool isCancel = EditorUtility.DisplayCancelableProgressBar("匹配资源中", file, (float)startIndex / (float)files.Length);

                if (Regex.IsMatch(File.ReadAllText(file), guid))
                {
                    Object find_obj = AssetDatabase.LoadAssetAtPath<Object>(GetRelativeAssetsPath(file));
                    ClientLog.Instance.Log(file + "引用到该资源", find_obj);
                    string extension = Path.GetExtension(file);
                    //isCancel = EditorUtility.DisplayDialog("找到了", file + "引用到该资源", "关闭", "继续查找");
                }

                startIndex++;
                if (isCancel || startIndex >= files.Length)
                {
                    EditorUtility.ClearProgressBar();
                    EditorApplication.update = null;
                    startIndex = 0;
                    ClientLog.Instance.Log("匹配结束");
                }

            };
        }
    }

    [MenuItem("Assets/Find References", true)]
    static private bool VFind()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        return (!string.IsNullOrEmpty(path));
    }

    static private string GetRelativeAssetsPath(string path)
    {
        return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
    }
}
