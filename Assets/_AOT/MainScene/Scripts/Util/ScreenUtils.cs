using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    public class ScreenUtils
    {
        public enum ScaleType
        {
            RelativeScale,
            FixedWidth,
            FixedHeight
        };

        public static Vector2 GetScaleSize(Vector2 origSize, ScaleType scaleType, float scaleFactor)
        {
            Vector2 scaleSize = Vector2.zero;

            if (scaleType == ScaleType.RelativeScale)
            {
                scaleSize.x = (int)(origSize.x * scaleFactor);
                scaleSize.y = (int)(origSize.y * scaleFactor);
            }
            else
            {
                float aspectRatio = origSize.x / origSize.y;
                if (scaleType == ScaleType.FixedWidth)
                {
                    scaleSize.x = (int)scaleFactor;
                    scaleSize.y = (int)(scaleFactor / aspectRatio);
                }
                else if (scaleType == ScaleType.FixedHeight)
                {
                    scaleSize.x = (int)(scaleFactor * aspectRatio);
                    scaleSize.y = (int)scaleFactor;
                }
            }

            return scaleSize;
        }

        // Perfect fit
        public static Vector2 GetScaleSize(Vector2 origSize, Vector2 scaleFactor)
        {
            Vector2 scaleSize = Vector2.zero;

            if(origSize.x > origSize.y)
            {
                scaleSize = ScreenUtils.GetScaleSize(origSize, ScreenUtils.ScaleType.FixedHeight, scaleFactor.y);

                if(scaleSize.x < scaleFactor.x)
                {
                    scaleSize = ScreenUtils.GetScaleSize(scaleSize, ScreenUtils.ScaleType.FixedWidth, scaleFactor.x);
                }
            }
            else
            {
                scaleSize = ScreenUtils.GetScaleSize(origSize, ScreenUtils.ScaleType.FixedWidth, scaleFactor.x);

                if(scaleSize.y < scaleFactor.y)
                {
                    scaleSize = ScreenUtils.GetScaleSize(scaleSize, ScreenUtils.ScaleType.FixedHeight, scaleFactor.y);
                }
            }

            return scaleSize;
        }
    }
}
