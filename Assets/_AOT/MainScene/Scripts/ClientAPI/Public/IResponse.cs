namespace BagelCode.Protobuf
{
public interface IResponse<TError, TCommon>
{
    TError error { get; set; }
    TCommon common { get; set; }
    long serverTime { get; set; }
}

public interface IChatResponse<TError>
{
    TError error { get; set; }
    long serverTime { get; set; }
}
}
