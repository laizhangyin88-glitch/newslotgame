using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 自动持续旋转
/// </summary>
/// <remarks>
/// From:whh - 2024年9月21日
/// </remarks>
public class AutoRotate : MonoBehaviour
{
    [Tooltip("旋转方向和速度")]
    public Vector3 Speed = Vector3.zero;

    void Update()
    {
        if (Speed == Vector3.zero)
            return;

        transform.Rotate(Speed * Time.deltaTime);
    }
}
