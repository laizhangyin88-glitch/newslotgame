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
}

public delegate void HTTPErrorCallback(BagelCodeHTTPError error);

}
