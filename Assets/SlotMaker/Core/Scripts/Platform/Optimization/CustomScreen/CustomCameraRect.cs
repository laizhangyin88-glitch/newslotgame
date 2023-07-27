using UnityEngine;

namespace SlotMaker
{
    [RequireComponent(typeof(Camera))]
    public class CustomCameraRect : MonoBehaviour
    {
        private void Start()
        {
            CustomScreenInfo.SetCustomCameraRect(GetComponent<Camera>());
            Destroy(this);
        }
    }
}
