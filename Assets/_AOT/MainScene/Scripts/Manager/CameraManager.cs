using UnityEngine;
using System.Collections;

namespace SlotMaker
{
    public class CameraManager : MonoWeakSingleton<CameraManager>
    {
        public enum CameraType
        {
            MainCamera,
            PopupCamera
        };

        public Camera mainCamera;
        public Camera popupCamera;

        public static Camera Get(CameraType cameraType = CameraType.MainCamera)
        {
            switch (cameraType)
            {
            case CameraType.MainCamera:
                return Instance.mainCamera;
            case CameraType.PopupCamera:
                return Instance.popupCamera;
            }

            return null;
        }
    }
}
