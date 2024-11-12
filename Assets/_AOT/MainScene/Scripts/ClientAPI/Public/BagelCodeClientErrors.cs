using UnityEngine;
using System.Collections;

namespace BagelCode
{

public class BagelCodeHTTPError
{
	public string url;
	public long responseCode;
	public string error;
    public ClientModels.Error errorCode;
    public object errorDetailInfo = null;

    /// <summary>【NetManager】new field</summary>
    public string response;
}

public delegate void HTTPErrorCallback(BagelCodeHTTPError error);

}
