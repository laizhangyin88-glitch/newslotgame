using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode
{
    public class MetaSafeArea : MonoBehaviour
    {
        public CanvasScaler canvasScaler;

        private RectTransform ownerRect;
        private Rect lastSafeArea = new Rect (0, 0, 0, 0);
        private Vector2Int lastScreenSize = new Vector2Int (0, 0);
        private ScreenOrientation lastOrientation = ScreenOrientation.AutoRotation;

        private void Awake()
        {
            ownerRect = GetComponent<RectTransform>();
        }

        private void Start()
        {
            canvasScaler = transform.parent.GetComponent<CanvasScaler>();
            RefreshArea();
        }

        public void ChangeOrientation(ScreenOrientation targetOrientation)
        {
#if UNITY_IOS || UNITY_EDITOR
            StopCoroutine("ChangeArea");
            StartCoroutine("ChangeArea");
#endif
        }

        private IEnumerator ChangeArea()
        {
            float maxWait = 5f;
            while(lastScreenSize.x == Screen.width && lastScreenSize.y == Screen.height)
            {
                maxWait -= Time.deltaTime;
                if(maxWait < 0f)
                    yield break;

                yield return null;
            }

            RefreshArea();
        }

        private void RefreshArea()
        {
#if UNITY_IOS || USE_SAFE_AREA
            
    #if !UNITY_EDITOR
            if(!ScreenSafeAreaUtils.IsNotch()) return;
    #endif
            Rect safeArea = ScreenSafeAreaUtils.GetSafeArea();

            if (safeArea != lastSafeArea
                || Screen.width != lastScreenSize.x
                || Screen.height != lastScreenSize.y
                || Screen.orientation != lastOrientation)
            {
                // Fix for having auto-rotate off and manually forcing a screen orientation.
                // See https://forum.unity.com/threads/569236/#post-4473253 and https://forum.unity.com/threads/569236/page-2#post-5166467
                lastScreenSize.x = Screen.width;
                lastScreenSize.y = Screen.height;
                lastOrientation = Screen.orientation;

                ApplySafeArea(safeArea);
            }
#endif
        }

        private void ApplySafeArea(Rect newSafeArea)
        {
            lastSafeArea = newSafeArea;

            // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
            Vector2 anchorMin = newSafeArea.position;
            Vector2 anchorMax = newSafeArea.position + newSafeArea.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            ownerRect.anchorMin = anchorMin;
            ownerRect.anchorMax = anchorMax;
        }
    }
}