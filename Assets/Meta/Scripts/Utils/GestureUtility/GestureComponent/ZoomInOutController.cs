using UnityEngine;
using SlotMaker;
using ParadoxNotion;

namespace BagelCode
{
    public class ZoomInOutController : MonoBehaviour
    {
        // Settings
        public readonly float MIN_ZOOM_SCALE = 1f;
        public readonly float MAX_ZOOM_SCALE = 2f;
#if UNITY_EDITOR || UNITY_WSA || UNITY_WEBGL // Mouse
        private readonly float ZOOM_SENSITIVITY = 0.1f;
#elif UNITY_ANDROID || UNITY_IPHONE // Touch
        private readonly float ZOOM_SENSITIVITY = 0.1f;
#endif

        public const string ON_CHANGE_SCREEN_SIZE = "OnChangeScreenSize";

        public Transform rootTransform;
        public Transform areaTransform;

        private float xMinBoundary;
        private float xMaxBoundary;
        private float yMinBoundary;
        private float yMaxBoundary;

        public float currentZoomScale = 1f;

        private void Start()
        {
            if (rootTransform == null)
                rootTransform = transform;

            if (areaTransform == null)
                areaTransform = transform.GetChild(0);

            InitBoundary();
        }

        private void OnEnable()
        {
            MessageDispatcher.Register(GestureManager.ON_GESTURE_EVENT, OnGestureEvent);
        }

        private void OnDisable()
        {
            MessageDispatcher.UnRegister(GestureManager.ON_GESTURE_EVENT, OnGestureEvent);
        }

        private void OnGestureEvent(EventData eventData)
        {
            var gestureType = Common.GetEnumTypeByString<GestureManager.GestureType>(eventData.name);

            // Zoom, Scroll
            switch (gestureType)
            {
                case GestureManager.GestureType.ZOOM_IN:
                case GestureManager.GestureType.ZOOM_OUT:
                    {
                        if (eventData.value is GestureZoomInOutData zoomInOutData)
                        {
                            Vector2 zoomPoint = zoomInOutData.zoomPoint;
                            float zoomScale = zoomInOutData.zoomScale * ZOOM_SENSITIVITY;

                            Vector3 delta = zoomPoint - (Vector2)rootTransform.position;
                            rootTransform.position += delta;
                            areaTransform.position -= delta;

                            currentZoomScale = Mathf.Clamp(currentZoomScale + zoomScale, MIN_ZOOM_SCALE, MAX_ZOOM_SCALE);
                            rootTransform.localScale = new Vector3(currentZoomScale, currentZoomScale);

                            delta = Vector3.zero - rootTransform.position;
                            rootTransform.position = Vector3.zero;
                            areaTransform.position -= delta;

                            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, ON_CHANGE_SCREEN_SIZE);
                        }
                    }
                    break;
                case GestureManager.GestureType.SCROLL_MOVE:
                    {
                        if (eventData.value is GestureScrollData scrollData)
                        {
                            Vector2 movement = scrollData.movement;
                            areaTransform.position += (Vector3)movement;

                            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, ON_CHANGE_SCREEN_SIZE);
                        }
                    }
                    break;
                default:
                    return;
            }

            FitAreaToBoundary();
        }

        private void FitAreaToBoundary()
        {
            var pos = areaTransform.position;

            var widthHalf = MetaScreenManager.GetCameraWidthHalf() * currentZoomScale;
            var heightHalf = MetaScreenManager.GetCameraHeightHalf() * currentZoomScale;
            float xStart = pos.x - widthHalf;
            float xEnd = pos.x + widthHalf;
            float yStart = pos.y - heightHalf;
            float yEnd = pos.y + heightHalf;

            if (xMinBoundary < xStart) // Left
            {
                pos.x -= xStart - xMinBoundary;
            }
            else if (xEnd < xMaxBoundary) // Right
            {
                pos.x -= xEnd - xMaxBoundary;
            }

            if (yStart > yMinBoundary) // Down
            {
                pos.y -= yStart - yMinBoundary;
            }
            else if (yEnd < yMaxBoundary) // Up
            {
                pos.y -= yEnd - yMaxBoundary;
            }

            areaTransform.position = pos;
        }

        private void InitBoundary()
        {
            xMaxBoundary = MetaScreenManager.GetCameraWidthHalf();
            xMinBoundary = -xMaxBoundary;
            yMaxBoundary = MetaScreenManager.GetCameraHeightHalf();
            yMinBoundary = -yMaxBoundary;
        }
    }
}
