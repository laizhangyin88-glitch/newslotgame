using UnityEngine;

namespace SlotMaker
{
    [RequireComponent(typeof(RectTransform))]
    public class CustomRectTransform : MonoBehaviour
    {
        private void Start()
        {
            CustomScreenInfo.SetCustomRectTransformAnchor(GetComponent<RectTransform>());
            Destroy(this);
        }
    }
}
