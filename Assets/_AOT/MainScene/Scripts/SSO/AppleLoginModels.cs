namespace BagelCode.Sso.AppleLogin
{
    /// <summary>
    /// <c>AppleLoginModelss</c>.
    /// This class file contains all the models we require when interacting with Apple login, both via our native bridge
    /// and the non-native unity code.
    /// </summary>
    ///
    public class AppleUserInfoRequest
    {
        public AppleUserName name;
        public string email;
    }

    public class AppleUserName
    {
        public string firstName;
        public string lastName;
    }

    public struct UserInfo
    {
        public string userId;
        public string email;

        public string firstName;
        public string surname;

        public string authorisationCode;
        public string idToken;

        public string error;

        public UserDetectionStatus userDetectionStatus;
    }

    public struct AppleLoginCallbackArgs
    {
        /// <summary>
        /// The state of the user's authorization.
        /// </summary>
        public UserCredentialState credentialState;

        /// <summary>
        /// The logged in user info after the call is done.
        /// </summary>
        public UserInfo userInfo;

        /// <summary>
        /// Whether the call ends up with an error.
        /// </summary>
        public string error;
    }
}
   
