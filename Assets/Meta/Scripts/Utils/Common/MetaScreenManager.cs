using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    public class MetaScreenManager : MonoWeakSingleton<MetaScreenManager>
    {
        public Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        public static float GetCameraWidth()
        {
            return Instance.mainCamera.aspect * GetCameraHeight();
        }

        public static float GetCameraHeight()
        {
            return GetCameraHeightHalf() * 2f;
        }

        public static float GetCameraWidthHalf()
        {
            return Instance.mainCamera.aspect * GetCameraHeightHalf();
        }

        public static float GetCameraHeightHalf()
        {
            return Instance.mainCamera.orthographicSize;
        }

        public static bool IsWorldPointInScreen(Vector2 worldPoint)
        {
            float widthHalf = GetCameraWidthHalf();
            float heightHalf = GetCameraHeightHalf();

            return
                -widthHalf < worldPoint.x &&
                worldPoint.x < widthHalf &&
                -heightHalf < worldPoint.y &&
                worldPoint.y < heightHalf;
        }
    }
}
