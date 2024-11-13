using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode
{

    public class MainCanvasPresetForOrientation : MonoBehaviour
    {
        public Vector2 landscape = Vector2.one;

        public Vector2 verticalMin = Vector2.one;
        public Vector2 verticalMax = Vector2.one;

        public float minRatio = 1.333f; // ex ) 4 : 3 = 1.333
        public float maxRatio = 1.777f; // ex ) 16 : 9 = 1.777

        public CanvasScaler canvasScaler;

        private void Awake()
        {
#if UNITY_ANDROID || UNITY_IOS
            canvasScaler = GetComponent<CanvasScaler>();
#endif
        }

        public void ChangeOrientation(ScreenOrientation targetOrientation)
        {
#if FIXABLE_LANDSCAPE
#else
    #if UNITY_ANDROID || UNITY_IOS
            if( !OrientationUtils.Instance.PossibleChangeOrientation() ) return;

            if(Screen.autorotateToLandscapeLeft || Screen.autorotateToLandscapeRight)
            {
                canvasScaler.referenceResolution = landscape;
                Screen.orientation = ScreenOrientation.LandscapeLeft;
            }
            else
            {
                canvasScaler.referenceResolution = GetVerticalScale();
                Screen.orientation = ScreenOrientation.Portrait;
            }

            Screen.orientation = ScreenOrientation.AutoRotation;
    #endif
#endif
        }

        public Vector2 GetVerticalScale()
        {
            float width = (float)Screen.width;
            float height = (float)Screen.height;
            float ratio = 1f;

            if(width >= height)
                ratio = width/height;
            else
                ratio = height/width;

            ratio = Mathf.Clamp(ratio, minRatio, maxRatio);

            if(ratio <= minRatio)
                return verticalMin;
            else if(ratio >= maxRatio)
                return verticalMax;

            ratio = (maxRatio - ratio)/(maxRatio - minRatio);
            return Vector2.Lerp(verticalMin, verticalMax, 1f - ratio);
        }
    }

}
