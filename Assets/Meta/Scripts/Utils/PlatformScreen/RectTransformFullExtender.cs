using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class RectTransformFullExtender : MonoBehaviour
    {
        public RectTransform rootRectTransform;

        public bool conformX;
        public bool conformY;

        protected bool isTracking = false;

        protected RectTransform ownerRect;
        protected Rect safeArea;

        protected Vector2 origSizeDelta = Vector2.zero;
        protected Vector2 origSize = Vector2.zero;
        protected Vector2 origAnchoredPosition = Vector2.zero;

        protected Vector2 offsetLeftRight = Vector2.zero;
        protected Vector2 offsetTopBottom = Vector2.zero;
        protected Vector2 offsetSizeDelta = Vector2.zero;

        protected float prevRootWidth;
        protected float prevRootHeight;

#if UNITY_IOS || USE_SAFE_AREA
        virtual protected void Awake()
        {
            ownerRect = GetComponent<RectTransform>();
        }

        virtual protected void Start()
        {
    #if !UNITY_EDITOR
            if(!ScreenSafeAreaUtils.IsNotch()) return;
    #endif
            if(transform.root != null && rootRectTransform == null)
                rootRectTransform = transform.root.GetComponent<RectTransform>();
            if(rootRectTransform == null) return;

            origSize = new Vector2(ownerRect.rect.width, ownerRect.rect.height);
            origSizeDelta = ownerRect.sizeDelta;
            origAnchoredPosition = ownerRect.anchoredPosition;

            isTracking = true;
            UpdateRect();
        }

        virtual public void UpdateRect(bool force = false)
        {
            prevRootWidth = rootRectTransform.rect.width;
            prevRootHeight = rootRectTransform.rect.height;

            // Debug.LogError( string.Format("Root Canvas({0}) : w : {1}, h : {2}", rootRectTransform.gameObject.name, rootRectTransform.rect.width, rootRectTransform.rect.height) );
            // Debug.LogError( string.Format("Root Canvas({0}) : x : {1}, y : {2}", rootRectTransform.gameObject.name, rootRectTransform.sizeDelta.x, rootRectTransform.sizeDelta.y) );

            // Vector2 prevMin = ownerRect.anchorMin;
            // Vector2 prevMax = ownerRect.anchorMax;
            // Vector2 prevpivot = ownerRect.pivot;

            Vector2 newSizeDelta = origSizeDelta;

            if(conformX)
            {
                newSizeDelta.x = rootRectTransform.sizeDelta.x;
            }

            if(conformY)
            {
                newSizeDelta.y = rootRectTransform.sizeDelta.y;
            }

            ownerRect.anchorMin = new Vector2(0.5f, 0.5f);
            ownerRect.anchorMax = new Vector2(0.5f, 0.5f);
            ownerRect.pivot = new Vector2(0.5f, 0.5f);

            ownerRect.anchoredPosition = Vector2.zero;
            ownerRect.position = new Vector3(rootRectTransform.position.x, rootRectTransform.position.y, ownerRect.position.z);
            ownerRect.sizeDelta = newSizeDelta;

            // ownerRect.anchorMin = prevMin;
            // ownerRect.anchorMax = prevMax;
            // ownerRect.pivot = prevpivot;
        }
        // {
        //     // safeArea = ScreenSafeAreaUtils.GetSafeArea();
        //     // offsetX = 0f;
        //     // offsetY = 0f;

        //     // if(conformX)
        //     // {
        //     //     float widthRatio = (float)ownerRect.rect.width / (float)safeArea.width;
        //     //     offsetX = widthRatio * (float)safeArea.x * 2f;
        //     // }

        //     // if(conformY)
        //     // {
        //     //     float heightRatio = (float)ownerRect.rect.height / (float)safeArea.height;
        //     //     offsetY = heightRatio * (float)safeArea.y * 2f;
        //     // }

        //     // ownerRect.sizeDelta = new Vector2(ownerRect.sizeDelta.x + offsetX, ownerRect.sizeDelta.y + offsetY);
        //     if(transform.root != null && rootRectTransform == null)
        //         rootRectTransform = transform.root.GetComponent<RectTransform>();
        //     if(rootRectTransform == null) return;

        //     prevRootWidth = rootRectTransform.rect.width;
        //     prevRootHeight = rootRectTransform.rect.height;

        //     // if(force)
        //     //     Debug.LogError("Update1");

        //     if( !force && ownerRect.rect.width == rootRectTransform.rect.width
        //         &&  ownerRect.rect.height == rootRectTransform.rect.height) return;

        //     // if(force)
        //     //     Debug.LogError("Update2");
            
        //     safeArea = ScreenSafeAreaUtils.GetSafeArea();
        //     offsetLeftRight = Vector2.zero;
        //     offsetSizeDelta = Vector2.zero;

        //     Vector2 newSizeDelta = ownerRect.sizeDelta;
        //     // if (Screen.height > Screen.width)  // Portrait
        //     //     newSizeDelta = new Vector2(origSizeDelta.y, origSizeDelta.x);
        //     // else
        //     //     newSizeDelta = origSizeDelta;

        //     Vector2 newAnchoredPosition = origAnchoredPosition;
        //     if (Screen.height > Screen.width)  // Portrait
        //         newAnchoredPosition = new Vector2(origAnchoredPosition.y, origAnchoredPosition.x);
        //     else
        //         newAnchoredPosition = origAnchoredPosition;

        //     Vector2 rootSize = new Vector2(rootRectTransform.rect.width, rootRectTransform.rect.height);

        //     rootSize += (rootSize - (rootSize * ownerRect.localScale));

        //     Vector2 screenToCanvasRatio = new Vector2(rootRectTransform.rect.width/Screen.width, rootRectTransform.rect.height/Screen.height);
        //     // Vector2 canvasToScreenRatio = new Vector2(Screen.width/rootRectTransform.rect.width, Screen.height/rootRectTransform.rect.height);

        //     // Debug.LogError( string.Format("Root Canvas({0}) : {1}", rootRectTransform.gameObject.name, rootRectTransform.sizeDelta) );
        //     // Debug.LogError( string.Format("Safe Area({0}) : {1}, {2}, w : {3}, h : {4}", gameObject.name, safeArea.x, safeArea.y, safeArea.width, safeArea.height) );
        //     // Debug.LogError( string.Format("Screen to canvas Ratio({0}) : {1}, {2}", gameObject.name, screenToCanvasRatio.x, screenToCanvasRatio.y) );
        //     // Debug.LogError( string.Format("Canvas to screen Ratio({0}) : {1}, {2}", gameObject.name, canvasToScreenRatio.x, canvasToScreenRatio.y) );
        //     // Debug.LogError( string.Format("Screen({0}) : {1}, {2}", gameObject.name, Screen.width, Screen.height) );

        //     // Debug.LogError( string.Format("Root OffsetMin({0}) : {1}", gameObject.name, rootRectTransform.offsetMin) );
        //     // Debug.LogError( string.Format("Root OffsetMax({0}) : {1}", gameObject.name, rootRectTransform.offsetMax) );

        //     // Debug.LogError( string.Format("owner OffsetMin({0}) : {1}", gameObject.name, ownerRect.offsetMin) );
        //     // Debug.LogError( string.Format("owner OffsetMax({0}) : {1}", gameObject.name, ownerRect.offsetMax) );

        //     if(conformX)
        //     {
        //         float diffWidth = rootRectTransform.rect.width - (safeArea.width * screenToCanvasRatio.x);
        //         float diffHalfWidth = diffWidth/2f;

        //         float leftSafeWidth = safeArea.x * screenToCanvasRatio.x;
        //         float rightSafeWidth = diffWidth - leftSafeWidth;

        //         if(leftSafeWidth < diffHalfWidth)
        //         {
        //             // +Position
        //             newAnchoredPosition.x = diffHalfWidth - leftSafeWidth;
        //         }
        //         else if(rightSafeWidth < diffHalfWidth)
        //         {
        //             // -Position
        //             newAnchoredPosition.x = rightSafeWidth - diffHalfWidth;
        //         }

        //         float diff = leftSafeWidth - rightSafeWidth;

        //         // Debug.LogError( string.Format("MinMax X({0}) : {1}, {2}, diff : {3}", gameObject.name, leftSafeWidth, rightSafeWidth, diff) );
        //         Debug.LogError( string.Format("Anchored X({0}) : {1}", gameObject.name, newAnchoredPosition.x));

        //         offsetLeftRight.x = leftSafeWidth;
        //         offsetLeftRight.y = rightSafeWidth;
        //         offsetSizeDelta.x = rootSize.x - ownerRect.rect.width;
        //     }

        //     if(conformY)
        //     {
        //         float diffHeight = rootRectTransform.rect.height - (safeArea.height * screenToCanvasRatio.y);
        //         float diffHalfHeight = diffHeight/2f;

        //         float bottomSafeHeight = safeArea.y * screenToCanvasRatio.y;
        //         float topSafeHeight = diffHeight - bottomSafeHeight;

        //         if(bottomSafeHeight < diffHalfHeight)
        //         {
        //             // +Position
        //             newAnchoredPosition.y = diffHalfHeight - bottomSafeHeight;
        //         }
        //         else if(topSafeHeight < diffHalfHeight)
        //         {
        //             // -Position
        //             newAnchoredPosition.y = topSafeHeight - diffHalfHeight;
        //         }

        //         // Debug.LogError( string.Format("MinMax Y({0}) : {1}, {2}, diff : {3}", gameObject.name, bottomSafeHeight, topSafeHeight, diffHalfHeight) );
        //         // Debug.LogError( string.Format("Anchored Y({0}) : {1}", gameObject.name, newAnchoredPosition.y));

        //         offsetTopBottom.x = topSafeHeight;
        //         offsetTopBottom.y = bottomSafeHeight;

        //         offsetSizeDelta.y = rootSize.y - ownerRect.rect.height;
        //     }

        //     ownerRect.anchoredPosition = newAnchoredPosition;
        //     ownerRect.sizeDelta = newSizeDelta + offsetSizeDelta;
        //     // Debug.LogError(string.Format("Size Delta({0}) : {1}, {2}", gameObject.name, newSizeDelta, offsetSizeDelta));

        // }

        virtual protected void Update()
        {
            if(!isTracking) return;

            if( prevRootWidth != rootRectTransform.rect.width || prevRootHeight != rootRectTransform.rect.height)
            {
                UpdateRect(true);
            }
        }
#endif
    }
}