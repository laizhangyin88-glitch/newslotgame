using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.U2D;

public static class GetSpriteAtlas
{
    [MenuItem("Tools/查看图片所属图集")]
    public static void GetSrpiteAtlas()
    {
        Object obj = Selection.activeObject;
        if(obj == null)
        {
            Debug.LogError("请选择精灵");
            return;
        }

        if (obj is Sprite == false && obj is Texture2D == false)
        {
            Debug.LogError("请选择精灵");
            return;
        }

        List<SpriteAtlas> ret = new List<SpriteAtlas>();
        SpriteAtlas[] atlases = Resources.FindObjectsOfTypeAll<SpriteAtlas>();
        foreach (SpriteAtlas atlas in atlases)
        {
            Sprite sprite = atlas.GetSprite(obj.name);
            if (sprite == null)
                continue;

            if (object.Equals(sprite, obj) == false && object.Equals(sprite.texture, obj) == false)
                continue;

            ret.Add(atlas);
        }

        if (ret.Count <= 0)
        {
            Debug.Log($"{obj.name}不存在任何图集中", obj);
        }
        else if (ret.Count > 1)
        {
            Debug.Log($"{obj.name}同时存在多个图集中,[{ret.Count}]:", obj);
            ret.ForEach((sa) => Debug.Log(AssetDatabase.GetAssetPath(sa), sa));
        }
        else
        {
            Debug.Log($"{obj.name}存在单个图集中：", obj);
            Debug.Log(AssetDatabase.GetAssetPath(ret[0]), ret[0]);
        }
    }
}
