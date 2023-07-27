using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.TheFallenGames.OSA.Core;

namespace BagelCode
{
    public class MetaSlotListFullExtender : RectTransformFullExtender
    {
        public IOSA osaController;
#if UNITY_IOS || USE_SAFE_AREA
        override protected void Start()
        {
            isTracking = false;

            base.Start();

    #if !UNITY_EDITOR
            if(!ScreenSafeAreaUtils.IsNotch()) return;
    #endif

            osaController = GetComponent<IOSA>();
            if(osaController != null)
            {
                if(conformX)
                {
                    osaController.BaseParameters.contentPadding.left += (int)(offsetLeftRight.x);
                    osaController.BaseParameters.contentPadding.right = (int)(offsetLeftRight.y);
                    // ownerRect.anchoredPosition = Vector2.zero;
                }
            }
        }

        override public void UpdateRect(bool force = false)
        {
            if(force) return;

            // prevRootWidth = rootRectTransform.rect.width;
            // prevRootHeight = rootRectTransform.rect.height;

            if( ownerRect.rect.width == rootRectTransform.rect.width
                &&  ownerRect.rect.height == rootRectTransform.rect.height) return;
            
            safeArea = ScreenSafeAreaUtils.GetSafeArea();
            offsetLeftRight = Vector2.zero;
            offsetSizeDelta = Vector2.zero;

            Vector2 newSizeDelta = origSizeDelta;
            Vector2 newAnchoredPosition = origAnchoredPosition;

            Vector2 rootSize = new Vector2(rootRectTransform.rect.width, rootRectTransform.rect.height);

            rootSize += (rootSize - (rootSize * ownerRect.localScale));
            Vector2 screenToCanvasRatio = new Vector2(rootRectTransform.rect.width/Screen.width, rootRectTransform.rect.height/Screen.height);

            if(conformX)
            {
                float diffWidth = rootRectTransform.rect.width - (safeArea.width * screenToCanvasRatio.x);
                float diffHalfWidth = diffWidth/2f;

                float leftSafeWidth = safeArea.x * screenToCanvasRatio.x;
                float rightSafeWidth = diffWidth - leftSafeWidth;

                if(leftSafeWidth < diffHalfWidth)
                {
                    // +Position
                    newAnchoredPosition.x = diffHalfWidth - leftSafeWidth;
                }
                else if(rightSafeWidth < diffHalfWidth)
                {
                    // -Position
                    newAnchoredPosition.x = rightSafeWidth - diffHalfWidth;
                }

                float diff = leftSafeWidth - rightSafeWidth;

                offsetLeftRight.x = leftSafeWidth;
                offsetLeftRight.y = rightSafeWidth;
                offsetSizeDelta.x = rootSize.x - origSize.x;
            }

            // Nothing to do
            // if(conformY)
            // {
            // }

            ownerRect.anchoredPosition = newAnchoredPosition;
            ownerRect.sizeDelta = newSizeDelta + offsetSizeDelta + new Vector2(10f, 0f);
        }
#endif
    }
}
