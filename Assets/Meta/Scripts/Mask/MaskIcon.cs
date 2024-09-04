using System;
using UnityEngine;
public class MaskIcon : MonoBehaviour
{
    public bool isFoward = false;
    public float gapTimeMs = 50f;
    public float multiplier = 0.2f;
    private float angle = 0;
    private long lastTime = 0;
    void FixedUpdate()
    {
        long nowTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        if (nowTime - lastTime > gapTimeMs)
        {
            float dif = nowTime - lastTime;
            //Debug.Log($"dif = {dif}");
            lastTime = nowTime;
            if (isFoward)
            {
                angle -= dif* multiplier;
                if (angle < 0)
                {
                    angle = 360;
                }
            }
            else
            {
                angle += dif * multiplier;
                if (angle > 360)
                {
                    angle = 0;
                }
            }

            transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, angle);
        }
    }
}
