#if UNITY_WSA && !UNITY_EDITOR
using System;

namespace BagelCode.FacebookWSA
{
    [Serializable]
    public class WSAFacebookErrorDto
    {
        public string message;
        public string type;
        public string code;
    }
}
#endif