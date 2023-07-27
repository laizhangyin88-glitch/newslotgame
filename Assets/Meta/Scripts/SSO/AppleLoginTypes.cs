namespace BagelCode.Sso.AppleLogin
{
    /// <summary>
    /// <c>AppleLoginTypes</c>.
    /// This class file contains all the types we require when interacting with Apple login.
    /// </summary>
    /// 
    public enum UserDetectionStatus
    {
        LikelyReal,
        Unknown,
        Unsupported
    }

    public enum UserCredentialState
    {
        Revoked,
        Authorized,
        NotFound
    }
}
