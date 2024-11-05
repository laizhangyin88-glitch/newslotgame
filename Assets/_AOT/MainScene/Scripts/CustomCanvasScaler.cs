using UnityEngine;
using UnityEngine.UI;

namespace SlotMaker
{
    [RequireComponent(typeof(CanvasScaler))]
    public class CustomCanvasScaler : MonoBehaviour
    {
        private void Start()
        {
            CustomScreenInfo.SetCustomReferenceResolution(GetComponent<CanvasScaler>());
            Destroy(this);
        }
    }
}
