#if UNITY_WSA && !UNITY_EDITOR
using System.Net;

namespace BagelCode.FacebookWSA
{
    public class HttpResponseMessage
    {
        public string Data { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public bool IsSuccessStatusCode
        {
            get { return ((int)StatusCode >= 200) && ((int)StatusCode <= 299); }
        }
    }
}
#endif