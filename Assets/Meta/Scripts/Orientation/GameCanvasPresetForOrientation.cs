using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode
{

    public class GameCanvasPresetForOrientation : MonoBehaviour
    {
        public Vector2 landscape;
        public Vector2 landscapeNotch;

        public Vector2 vertical;
        public Vector2 verticalLandscape;

        public CanvasScaler canvasScaler;

        private void Awake()
        {
            canvasScaler = GetComponent<CanvasScaler>();
            canvasScaler.referenceResolution = ScreenSafeAreaUtils.IsNotch() ? landscapeNotch : landscape;
        }

        public void ChangeOrientation(ScreenOrientation targetOrientation)
        {
            if(OrientationUtils.Instance.contentOrientation == ScreenOrientation.LandscapeLeft)
            {
                canvasScaler.referenceResolution = ScreenSafeAreaUtils.IsNotch() ? landscapeNotch : landscape;
            }
            else
            {
                if(OrientationUtils.Instance.PossibleChangeOrientation())
                    canvasScaler.referenceResolution = vertical;
                else
                    canvasScaler.referenceResolution = verticalLandscape;
            }
        }
    }

}
