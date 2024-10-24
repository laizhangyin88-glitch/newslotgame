namespace CryPrinter
{
#if UNITY_ANDROID
    public enum Parity
    {
        None = 0,
        Odd = 1,
        Even = 2,
        Mark = 3,
        Space = 4,
    }

    public enum StopBits
    {
        None = 0,
        One = 1,
        Two = 2,
        OnePointFive = 3,
    }

#endif//UNITY_ANDROID
}