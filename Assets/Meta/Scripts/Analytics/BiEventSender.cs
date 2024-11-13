using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Networking;
using NodeCanvas.Framework;
using ParadoxNotion.Services;
using SlotMaker;
using SlotMaker.Json;

#if DEV
using SlotMaker.TestSuite;
#endif

namespace BagelCode
{

public class BiEventSender : MonoWeakSingleton<BiEventSender>
{
    public string accessKey = String.Empty;
    public string secretKey = String.Empty;
    public string addressFull = String.Empty;
    public string appName = String.Empty;
    public string env = String.Empty;

    public string addressHost = String.Empty;
    public string addressEnd  = String.Empty;

    private static SHA256 sha256 = SHA256.Create();

    public int maxRetryCount = 10;

    private const long DEFAULT_WAIT_INTERVAL = 200;
    private const long MAX_WAIT_INTERVAL = 5000;

    private const string BI_RESPONSE_KEY_DATA = "data";
    private const string BI_RESPONSE_KEY_FAILEDRECORDCOUNT = "FailedRecordCount";

    void Awake()
    {
    }

    public bool IsInitialized()
    {
        return (accessKey != String.Empty && secretKey != String.Empty && addressFull != String.Empty && appName != String.Empty && env != String.Empty /*&& addressHost != String.Empty && addressEnd != String.Empty*/);
    }

    public void Initialize(string accessKey, string secretKey, string addressFull, string appName, string env)
    {
        this.accessKey   = accessKey;
        this.secretKey   = secretKey;
        this.addressFull = addressFull;
        this.appName     = appName;
        this.env         = env;
        this.addressHost = String.Empty;
        this.addressEnd  = String.Empty;

        if(!string.IsNullOrEmpty(addressFull) && addressFull.Contains("https://") && addressFull.Contains(".com"))
        {
            string[] stringSeparators = new string[] {".com"};
            string[] result = addressFull.Split(stringSeparators, StringSplitOptions.None);

            if(result != null && result.Length > 1)
            {
                addressHost = result[0] + ".com";
                addressEnd  = result[1];
                addressHost = addressHost.Replace("https://", "");
            }
        }
    }
    
    public static void SendBiEvent(string eventColumns)
    {
        if (ApplicationSettings.LogAnalytics())
            Debug.Log(string.Format("[BI][{0}]", eventColumns)); //Logging For Test

        if (!Instance.IsInitialized())
        {
            if (ApplicationSettings.LogAnalytics())
                Debug.LogWarning("[BiEventSender] Called before Initialization.");

            return;
        }
        
        Instance.StartToPutRecord(eventColumns);
    }

    private void StartToPutRecord(string recordDataStr)
    {
        string body = GetRecordBody(recordDataStr);
        StartCoroutine(PutRecord(body, false, 0));
    }

    private void StartToPutRecordList(List<string> recordDataList)
    {
        string body = GetRecordBatchBody(recordDataList);
        StartCoroutine(PutRecord(body, true, 0));
    }

    private IEnumerator PutRecord(string body, bool isBatch, int retriedCount)
    {
        if (retriedCount > 0)
        {
            long waitTimeMs = GetWaitTimeMs(retriedCount);
            yield return new WaitForSeconds((float)waitTimeMs / 1000.0f);
        }

        if (ApplicationSettings.LogAnalytics() && retriedCount > 0)
        {
            Debug.Log(string.Format("[BiEventSender] Retry({0}): Putting record with body '{1}' to Kinesis firehose.", retriedCount, body));
        }

        using (UnityWebRequest webRequest = GetSignedRequest(body, isBatch)) {
            yield return webRequest.SendWebRequest();

            if (!webRequest.isNetworkError && webRequest.responseCode == 200)
            {
                int failedCount = GetRecordResponseFailedCount(webRequest.downloadHandler.text);

                if (ApplicationSettings.LogAnalytics())
                {
                    if(failedCount == 0)
                    {
                        Debug.Log(String.Format("[BiEventSender] Sent Successfully. body:{0}", webRequest.downloadHandler.text));
                    }
                    else
                    {
                        ++retriedCount;
                        if (retriedCount < maxRetryCount)
                        {
                            StartCoroutine(PutRecord(body, isBatch, retriedCount));
                        }
                        else
                        {
                            if (ApplicationSettings.LogAnalytics())
                                Debug.Log(string.Format("[BiEventSender] Retry Over: failed record '{0}'", body));
                        }
                    }
                }
            }
            else
            {
                // Debug.LogError(string.Format("error : {0}, response Code : {1}", webRequest.isNetworkError, webRequest.responseCode));

                if (ApplicationSettings.LogAnalytics())
                    Debug.Log(String.Format("[BiEventSender] status code:{0} error:{1}", webRequest.responseCode, webRequest.error));

                ++retriedCount;
                if (retriedCount < maxRetryCount)
                {
                    StartCoroutine(PutRecord(body, isBatch, retriedCount));
                }
                else
                {
                    if (ApplicationSettings.LogAnalytics())
                        Debug.Log(string.Format("[BiEventSender] Retry Over: abandon to put record with body '{0}' to Kinesis firehose.", body));
                }
            }
        }
    }

    private byte[] HmacSHA256(String data, byte[] key)
    {
        String algorithm = "HmacSHA256";
        KeyedHashAlgorithm kha = KeyedHashAlgorithm.Create(algorithm);
        kha.Key = key;

        return kha.ComputeHash(Encoding.UTF8.GetBytes(data));
    }

    private byte[] GetSignatureKey(String key, String dateStamp, String appName, String env)
    {
        byte[] kSecret  = Encoding.UTF8.GetBytes(("BAGEL1" + key).ToCharArray());
        byte[] kDate    = HmacSHA256(dateStamp, kSecret);
        byte[] kAppName = HmacSHA256(appName, kDate);
        byte[] kenv     = HmacSHA256(env, kAppName);
        byte[] kSigning = HmacSHA256("bagel1_request", kenv);

        return kSigning;
    }

    private string GetRecordBody(string recordData)
    {
#if DEV
        var bytes = Encoding.UTF8.GetBytes(recordData);
        bool ascii = true;
        foreach (var bt in bytes)
        {
            if ((bt & 0x80) > 0)
            {
                ascii = false;
                break;
            }
        }

        if (!ascii)
        {
            var report = new Dictionary<string, object>();
            report["name"] = "GetRecordBody";
            report["notes"] = "recordData could not include none asscii characters!";
            report["screenshot"] = null;
            report["type"] = "bug";
            report["content"] = "meta";
            var reportDetails = new Dictionary<string, object>();
            reportDetails["dump"] = recordData;
            report["details"] = reportDetails;

#if !NEW_NET
            TestSuiteManager.Instance.UpdateReportHeader(report);
            TestSuiteManager.Instance.Report(report);
            Debug.LogError(recordData);
#endif

            }



#endif

        StringBuilder sb = new StringBuilder();
        string recordDataBase64EncodedStr = System.Convert.ToBase64String(Encoding.UTF8.GetBytes(recordData));
        sb.Append("[");
        sb.Append(recordData);
        sb.Append("]");

        return sb.ToString();
    }

    private string GetRecordBatchBody(List<string> recordDataList) 
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("[");

        for (int i = 0; i < recordDataList.Count; ++i)
        {
            sb.Append(recordDataList[i]);
            if (i != recordDataList.Count - 1)
                sb.Append(",");
        }
        sb.Append("]");
        
        return sb.ToString();
    }

    private UnityWebRequest GetSignedRequest(string body, bool isRecordBatch)
    {
        const string method = "POST";
        const string contentType = "application/json; charset=utf-8";
        // const string service = "firehose";

        DateTime utcNow = DateTime.UtcNow;
        string bagelDateTime = utcNow.ToString("yyyyMMddTHHmmssZ");
        string dateStamp = utcNow.ToString("yyyyMMdd");

        byte[] bodyBytes= Encoding.UTF8.GetBytes(body);

        const string canonicalQueryString = ""; 
        string canonicalHeaders = String.Format("content-type:{0}\nhost:{1}\nx-bagel-date:{2}",contentType, addressHost, bagelDateTime);
        string signedHeaders = "content-type;host;x-bagel-date";
        byte[] payloadHashBytes = sha256.ComputeHash(bodyBytes);
        string payloadHash = BitConverter.ToString(payloadHashBytes).Replace("-", string.Empty).ToLower();

        string canonicalRequest = String.Format("{0}\n{1}\n{2}\n{3}\n\n{4}\n{5}", method, addressEnd, canonicalQueryString, canonicalHeaders, signedHeaders, payloadHash);

        const string algorithm = "BAGEL1-HMAC-SHA256";
        string credentialScope = String.Format("{0}/{1}/{2}/bagel1_request", dateStamp, appName, env);
        string stringToSign = String.Format("{0}\n{1}\n{2}\n{3}", algorithm, bagelDateTime, credentialScope, BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(canonicalRequest))).Replace("-", string.Empty).ToLower());

        byte[] signingKey = GetSignatureKey(secretKey, dateStamp, appName, env);
        byte[] signatureBytes = HmacSHA256(stringToSign, signingKey);
        string signature = BitConverter.ToString(signatureBytes).Replace("-", string.Empty).ToLower(); 
        string authorizationHeader = String.Format("{0} Credential={1}/{2}, SignedHeaders={3}, Signature={4}", algorithm, accessKey, credentialScope, signedHeaders, signature);

        WWWForm form = new WWWForm();
        form.AddField("", "");

        UnityWebRequest webRequest = UnityWebRequest.Post(addressFull, form);
        webRequest.SetRequestHeader("Content-Type", contentType);
        webRequest.SetRequestHeader("X-Bagel-Date", bagelDateTime);
        webRequest.SetRequestHeader("Authorization", authorizationHeader);
        webRequest.uploadHandler = new UploadHandlerRaw(bodyBytes);

        return webRequest;
    }

    private long GetWaitTimeMs(int retriedCount)
    {
        long waitTime = (long)(Math.Pow(2, retriedCount - 1) * DEFAULT_WAIT_INTERVAL);
        if(waitTime > MAX_WAIT_INTERVAL)
            waitTime = MAX_WAIT_INTERVAL;
        return waitTime;
    }

    private int GetRecordResponseFailedCount(string textJson)
    {
        int failedCount = 0;
        var responseJsonObj = SlotSimpleJson.DeserializeObject<Dictionary<string, object>>(textJson);
        
        if(responseJsonObj.Count > 0 && responseJsonObj.ContainsKey(BI_RESPONSE_KEY_DATA))
        {
            var dataJsonObj = SlotSimpleJson.DeserializeObject<Dictionary<string, object>>(responseJsonObj[BI_RESPONSE_KEY_DATA].ToString());
            if(dataJsonObj.Count > 0 && dataJsonObj.ContainsKey(BI_RESPONSE_KEY_FAILEDRECORDCOUNT))
            {
                failedCount = System.Convert.ToInt32(dataJsonObj[BI_RESPONSE_KEY_FAILEDRECORDCOUNT]);
            }
        }

        return failedCount;
    }
}

}
