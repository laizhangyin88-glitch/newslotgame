using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtend
{
    /// <summary>
    /// 删除节点下所有子物体
    /// </summary>
    /// <param name="parent"></param>
    public static void RemoveAllChildren(this Transform parent)
    {
        Transform transform;
        for (int i = 0; i < parent.childCount; i++)
        {
            transform = parent.GetChild(i);
            GameObject.Destroy(transform.gameObject);
        }
    }

    /// <summary>
    /// 删除节点下所有子物体
    /// </summary>
    /// <remarks>
    /// 在主线程上立即执行
    /// </remarks>
    /// <param name="parent"></param>
    public static void RemoveAllChildrenImmediate(this Transform parent, bool allowDestroyingAssets = false)
    {
        int childCount = parent.childCount;
        if (childCount <= 0)
            return;

        for (int i = childCount-1; i >= 0; i--)
        {
            GameObject.DestroyImmediate(parent.GetChild(i).gameObject, allowDestroyingAssets);
        }
    }
}
