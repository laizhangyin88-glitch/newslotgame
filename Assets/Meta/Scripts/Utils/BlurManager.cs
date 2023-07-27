using UnityEngine;

namespace BagelCode
{
    public static class BlurManager
    {
        public static MobileBlur Blur
        {
            get
            {
                if(blur == null)
                    blur = Camera.main?.GetComponent<MobileBlur>();

                return blur;
            }
        }
        private static MobileBlur blur = null;

        public static void SetBlur(bool isActive)
        {
            Blur.enabled = isActive;
        }
    }
}
