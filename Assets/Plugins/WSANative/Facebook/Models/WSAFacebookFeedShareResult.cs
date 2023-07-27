#if UNITY_WSA && !UNITY_EDITOR
namespace BagelCode.FacebookWSA
{
    public class WSAFacebookShareResult
    {
        /// <summary>
        /// Was the login successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Error message if the login was not successful
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
#endif