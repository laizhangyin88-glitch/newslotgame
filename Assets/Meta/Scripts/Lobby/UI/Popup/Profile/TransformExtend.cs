using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtend
{
    public static void RemoveAllChildren(this Transform parent)
    {
        Transform transform;
        for (int i = 0; i < parent.childCount; i++)
        {
            transform = parent.GetChild(i);
            GameObject.Destroy(transform.gameObject);
        }
    }
}
