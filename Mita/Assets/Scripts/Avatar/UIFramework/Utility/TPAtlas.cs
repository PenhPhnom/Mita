/// <summary>
/// TPAtlas.cs
/// Desc:   索引了sprite的图集配置表
/// </summary>

using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "AssetTools/TPAtlas", fileName = "TPAtlas")]
public class TPAtlas : ScriptableObject
{
    public Texture2D texture;
    public List<Sprite> sprites;
#if UNITY_EDITOR
    [Sirenix.OdinInspector.Button]
    public void CollectSprites()
    {
        //选中TP图集更新texture的图集内容
        var target = this;
        var assets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(UnityEditor.AssetDatabase.GetAssetPath(target.texture));
        if (target.sprites == null)
            target.sprites = new List<Sprite>();
        target.sprites.Clear();
        foreach (var item in assets)
        {
            if (item is Sprite)
            {
                target.sprites.Add(item as Sprite);
            }
        }
        UnityEditor.EditorUtility.SetDirty(target);
        UnityEditor.AssetDatabase.SaveAssets();
    }
#endif
}
