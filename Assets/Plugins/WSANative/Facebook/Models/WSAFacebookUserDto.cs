#if UNITY_WSA && !UNITY_EDITOR
using System;

namespace BagelCode.FacebookWSA
{
    [Serializable]
    public class WSAFacebookUserDto
    {
        public string id;
        public WSAFacebookAgeRangeDto age_range;
        public string name;
        public string first_name;
        public string last_name;
        public string link;
        public string gender;
        public string locale;
        public WSAFacebookPictureDto picture;
        public int timezone;
        public string email;
        public string birthday;
    }

    [Serializable]
    public class WSAFacebookAgeRangeDto
    {
        public int min;
        public int Max;
    }

    [Serializable]
    public class WSAFacebookPictureDto
    {
        public WSAFacebookPictureDataDto data;
    }

    [Serializable]
    public class WSAFacebookPictureDataDto
    {
        public string url;
        public bool is_silhouette;
        public int width;
        public int height;
    }
}
#endif