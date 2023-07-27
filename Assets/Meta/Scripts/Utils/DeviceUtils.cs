namespace BagelCode.Utils
{
    public class DeviceUtils
    {
        public static int GetDeviceOSVersion()
        {
            string versionAsString = "0.0";

#if UNITY_IOS && !UNITY_EDITOR
            versionAsString = UnityEngine.iOS.Device.systemVersion;
#endif
            bool successfulConversion = int.TryParse(versionAsString.Split('.')[0], out int version);

            return successfulConversion ? version : 0;
        }
    }    
}
