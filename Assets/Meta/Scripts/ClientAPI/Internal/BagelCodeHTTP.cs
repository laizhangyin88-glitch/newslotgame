using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using SlotMaker;
using SlotMaker.Json;
using BagelCode.ClientModels;
using BagelCode.Protobuf;

namespace BagelCode.Internal
{
    public class BagelCodeHTTP : MonoSingleton<BagelCodeHTTP>
    {
        private static BagelCodeReliabilitySystem reliabilitySystem = new BagelCodeReliabilitySystem();
        private static List<CallRequestContainer> pendingQueue = new List<CallRequestContainer>();

        private static CallRequestContainer longPoll = null;
        private static CallRequestContainer chatPoll = null;

        private static string cookie = null;
        private static string sid = null;

        public static long lastPacketTime = 0;

        public delegate void RetryEvent(CallRequestContainer reqContainer);

        public static event RetryEvent retryEventHandler;

        public delegate void CallbackCommon(IResponse<Error, CommonResponse> response);

        public static event CallbackCommon commonHandler;

    #if UNITY_WEBGL && !UNITY_EDITOR
        private const string X_SET_AUTHENTICATION = "X-Set-Cookie";
        private const string X_AUTHENTICATION = "X-Cookie";
    #else
        private const string X_SET_AUTHENTICATION = "set-cookie";
        private const string X_AUTHENTICATION = "Cookie";
    #endif

        public static void Reset()
        {
            StopRequest(longPoll);
            StopRequest(chatPoll);

            lastPacketTime = 0;

            cookie = null;
            sid = null;
            pendingQueue.Clear();

            reliabilitySystem.Reset();
        }

        public static string GetSessionId()
    	{
    		return sid;
    	}

        public static int GetPendingMessages()
        {
            return pendingQueue.Count;
        }

        protected internal static uint FetchBlockSeq()
        {
            return reliabilitySystem.FetchBlockSeq();
        }

        protected internal static int GenerateAckBits()
        {
            return reliabilitySystem.GenerateAckBits();
        }

        protected internal static void MakeApiCall<TRequest, TResult>(string apiEndpoint, Dictionary<string, string> requestHeadersDict,
            TRequest request, Func<byte[], TResult> deserializeResult,
            Action<TResult> successCallback, HTTPErrorCallback errorCallback, bool reliability = true)
            where TRequest : IProtoSerializable
            where TResult : IProtoSerializable
        {


            var reqContainer = new CallRequestContainer
            {
                sequence = reliabilitySystem.Sequence,
                reliability = reliability,
                status = CallRequestContainer.Status.Sent,
                headersDict = requestHeadersDict,
                apiEndpoint = apiEndpoint,
                fullUrl = ApplicationSettings.GetFullUrl(apiEndpoint),
                payload = request.Serialize(),
                apiRequest = request
            };

            RequestContainer(reqContainer, successCallback, deserializeResult, errorCallback);
        }

        protected internal static void MakeApiChatCall<TRequest, TResult>(string apiEndpoint, Dictionary<string, string> requestHeadersDict,
            TRequest request, Func<byte[], TResult> deserializeResult,
            Action<TResult> successCallback, HTTPErrorCallback errorCallback, bool reliability = true)
            where TRequest : IProtoSerializable
            where TResult : IProtoSerializable
        {
            var reqContainer = new CallRequestContainer
            {
                sequence = 0,
                reliability = reliability,
                status = CallRequestContainer.Status.Sent,
                headersDict = requestHeadersDict,
                apiEndpoint = apiEndpoint,
                fullUrl = ApplicationSettings.GetChattingFullUrl(apiEndpoint),
                payload = request.Serialize(),
                apiRequest = request
            };

            RequestContainer(reqContainer, successCallback, deserializeResult, errorCallback);
        }

        protected internal static void RequestContainer<TResult>(CallRequestContainer reqContainer,
            Action<TResult> successCallback,
            Func<byte[], TResult> deserializeResult,
            HTTPErrorCallback errorCallback)
            where TResult : IProtoSerializable
        {
/*
            string jsonString = JsonUtility.ToJson(reqContainer.apiRequest);
#if NEW_NET
            UnityEngine.Debug.LogWarning($"@@seaweed  上行：{reqContainer.apiEndpoint} ； 上行数据 == {jsonString}");
#else
            UnityEngine.Debug.Log($"@@seaweed  上行：{reqContainer.apiEndpoint} ； 上行数据 == {jsonString}");
#endif
*/
            reqContainer.callback = (unityWebRequest) =>
            {
                try
                {
                    if (ApplicationSettings.LogNetwork())
                    {
                        Debug.Log("[BagelCodeHTTP] Response: " + reqContainer.fullUrl +
                            "\ntime: " + reqContainer.stopwatch.Elapsed +
                            "\nresponseCode: " + unityWebRequest.responseCode);
                    }

                    if (unityWebRequest.responseCode == 200)
                    {
                        reqContainer.stopwatch.Stop();

                        if(reqContainer.sequence > 0)
                            reliabilitySystem.ProcessAck(reqContainer.sequence);
                            
                        reqContainer.status = CallRequestContainer.Status.Received;

                        reqContainer.response = unityWebRequest.downloadHandler.data;

                        // Hack
                        var errorCode = ReadErrorCode(reqContainer.response);

                        if (ApplicationSettings.LogNetwork()) Debug.Log(errorCode);

                        if (errorCode == ClientModels.Error.OK)
                        {
                            // Update ping time..
                            lastPacketTime = TimeUtils.GetTimeStamp();

                            reqContainer.apiResult = deserializeResult(reqContainer.response);

                            if (ApplicationSettings.LogNetwork())
                                Debug.Log(SlotSimpleJson.SerializeObject(reqContainer.apiResult));

                            string jsonString1 = JsonUtility.ToJson((TResult)reqContainer.apiResult);
                            UnityEngine.Debug.Log($"@@seaweed  下行：{reqContainer.apiEndpoint} ； 下行数据 response == {jsonString1}");

                            if (successCallback != null)
                                successCallback((TResult)reqContainer.apiResult);

                            if(commonHandler != null)
                            {
                                IResponse<Error, CommonResponse> response = reqContainer.apiResult as IResponse<Error, CommonResponse>;
                                if(response != null)
                                    commonHandler(response);
                            }
                        }
                        else
                        {
                            ErrorResponse errorResponse = ErrorResponse.Deserialize(reqContainer.response);

                            var error = new BagelCodeHTTPError
                            {
                                url = reqContainer.fullUrl,
                                responseCode = unityWebRequest.responseCode,
                                error = unityWebRequest.error,
                                errorCode = errorCode,
                                errorDetailInfo = errorResponse.errorDetailInfo
                            };

                            reqContainer.errorCallback(error);
                        }
                        
                        // unityWebRequest.Dispose();
                    }
                    else
                    {
                        if (!reqContainer.reliability)
                        {
                            reqContainer.status = CallRequestContainer.Status.Received;

                            var error = new BagelCodeHTTPError
                            {
                                url = reqContainer.fullUrl,
                                responseCode = unityWebRequest.responseCode,
                                error = unityWebRequest.error,
                                errorCode = ClientModels.Error.TIMEOUT_ERROR
                            };

                            reqContainer.errorCallback(error);
                            StopRequest(reqContainer);
                        }
                        else
                        {
                            StopRequest(reqContainer);

                            if (reqContainer.retry < ApplicationSettings.Instance.webRequestRetry)
                            {
                                RestartRequest(reqContainer);
                            }
                            else
                            {
                                if (ApplicationSettings.LogNetwork())
                                    Debug.Log("[BagelCodeHTTP] Retry Handler: " + reqContainer.fullUrl);

                                reqContainer.status = CallRequestContainer.Status.Idle;

                                if (retryEventHandler != null)
                                    retryEventHandler(reqContainer);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            };

            reqContainer.errorCallback = (error) =>
            {
                if (ApplicationSettings.LogNetwork())
                    Debug.Log(SlotSimpleJson.SerializeObject(error));
                
                if (errorCallback != null)
                    errorCallback(error);
            };

            reqContainer.coroutine = Instance.StartCoroutine(Instance.Post(reqContainer));

            pendingQueue.Add(reqContainer);
        }

        private IEnumerator Post(CallRequestContainer reqContainer)
        {
            if (ApplicationSettings.LogNetwork())
            {
                Debug.Log("[BagelCodeHTTP] Request: " + reqContainer.fullUrl);
                Debug.Log(SlotSimpleJson.SerializeObject(reqContainer.apiRequest));
            }

            string jsonString = JsonUtility.ToJson(reqContainer.apiRequest);
#if NEW_NET
            UnityEngine.Debug.LogWarning($"@@seaweed  上行：{reqContainer.apiEndpoint} ； 上行数据 == {jsonString}");
#else
            UnityEngine.Debug.Log($"@@seaweed  上行：{reqContainer.apiEndpoint} ； 上行数据 == {jsonString}");
#endif


            WWWForm form = new WWWForm();
            form.AddField("", "");

            reqContainer.webRequest = UnityWebRequest.Post(reqContainer.fullUrl, form);
            reqContainer.webRequest.SetRequestHeader("Content-Type", "application/protobuf");

            if(reqContainer.headersDict != null && reqContainer.headersDict.Count > 0)
            {
                foreach(KeyValuePair<string, string> data in reqContainer.headersDict)
                {
                    reqContainer.webRequest.SetRequestHeader(data.Key, data.Value);
                }
            }
            
            if(reqContainer.payload == null || reqContainer.payload.Length == 0)
            {
                throw new System.NullReferenceException(string.Format("{0}", reqContainer.fullUrl));
            }

            reqContainer.webRequest.uploadHandler = new UploadHandlerRaw(reqContainer.payload);
            
            if (!string.IsNullOrEmpty(cookie))
                reqContainer.webRequest.SetRequestHeader(X_AUTHENTICATION, cookie);

            yield return reqContainer.webRequest.SendWebRequest();

            if (!reqContainer.webRequest.isNetworkError && reqContainer.webRequest.responseCode == 200)
            {
                string newCookie = reqContainer.webRequest.GetResponseHeader(X_SET_AUTHENTICATION);
                if (!string.IsNullOrEmpty(newCookie))
                {
                    cookie = newCookie;

                    var groups = Regex.Match(cookie, @"connect.sid=(.*?);").Groups;
                    sid = groups.Count > 1 ? UnityWebRequest.UnEscapeURL(groups[1].Value) : "";
                }
            }

            reqContainer.callback(reqContainer.webRequest);
        }

        protected internal static void LongPoll<TRequest, TResult>(string apiEndpoint,
            TRequest request, Func<byte[], TResult> deserializeResult,
            Action<TResult> successCallback, HTTPErrorCallback errorCallback)
            where TRequest : IProtoSerializable
            where TResult : IProtoSerializable
        {
            StopRequest(longPoll);

            longPoll = new CallRequestContainer
            {
                sequence = reliabilitySystem.Sequence,
                status = CallRequestContainer.Status.Sent,
                headersDict = null,
                apiEndpoint = apiEndpoint,
                fullUrl = ApplicationSettings.GetFullUrl(apiEndpoint),
                payload = request.Serialize(),
                apiRequest = request
            };

            longPoll.callback = (unityWebRequest) =>
            {
                try
                {
                    longPoll.stopwatch.Stop();

                    if (ApplicationSettings.LogNetwork())
                        Debug.Log("[BagelCodeHTTP] Response: " + longPoll.fullUrl + ", time: " + longPoll.stopwatch.Elapsed);

                    reliabilitySystem.ProcessAck(longPoll.sequence);
                    longPoll.status = CallRequestContainer.Status.Received;

                    if (unityWebRequest.responseCode == 200)
                    {
                        longPoll.response = unityWebRequest.downloadHandler.data;

                        // Hack
                        var errorCode = ReadErrorCode(longPoll.response);

                        if (errorCode == ClientModels.Error.OK)
                        {
                            longPoll.apiResult = deserializeResult(longPoll.response);

                            if (ApplicationSettings.LogNetwork())
                                Debug.Log(SlotSimpleJson.SerializeObject(longPoll.apiResult));

                            successCallback((TResult)longPoll.apiResult);
                        }
                        else
                        {
                            ErrorResponse errorResponse = ErrorResponse.Deserialize(longPoll.response);

                            var error = new BagelCodeHTTPError
                            {
                                url = longPoll.fullUrl,
                                responseCode = unityWebRequest.responseCode,
                                error = unityWebRequest.error,
                                errorCode = errorCode,
                                errorDetailInfo = errorResponse.errorDetailInfo
                            };

                            longPoll.errorCallback(error);
                        }
                    }
                    else
                    {
                        var error = new BagelCodeHTTPError
                        {
                            url = longPoll.fullUrl,
                            responseCode = unityWebRequest.responseCode,
                            error = unityWebRequest.error,
                            errorCode = ClientModels.Error.UNKNOWN
                        };

                        longPoll.errorCallback(error);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            };

            longPoll.errorCallback = (error) =>
            {
                Debug.Log(SlotSimpleJson.SerializeObject(error));

                if (errorCallback != null)
                    errorCallback(error);
            };

            longPoll.coroutine = Instance.StartCoroutine(Instance.Post(longPoll));
        }

        protected internal static void ChatPoll<TRequest, TResult>(string apiEndpoint, Dictionary<string, string> requestHeadersDict,
            TRequest request, Func<byte[], TResult> deserializeResult,
            Action<TResult> successCallback, HTTPErrorCallback errorCallback)
            where TRequest : IProtoSerializable
            where TResult : IProtoSerializable
        {
            StopRequest(chatPoll);

            chatPoll = new CallRequestContainer
            {
                sequence = 0,
                status = CallRequestContainer.Status.Sent,
                headersDict = requestHeadersDict,
                apiEndpoint = apiEndpoint,
                fullUrl = ApplicationSettings.GetChattingFullUrl(apiEndpoint),
                payload = request.Serialize(),
                apiRequest = request
            };

            chatPoll.callback = (unityWebRequest) =>
            {
                try
                {
                    chatPoll.stopwatch.Stop();

                    if (ApplicationSettings.LogNetwork())
                        Debug.Log("[BagelCodeHTTP] Response: " + chatPoll.fullUrl + ", time: " + chatPoll.stopwatch.Elapsed);

                    // reliabilitySystem.ProcessAck(chatPoll.sequence);
                    chatPoll.status = CallRequestContainer.Status.Received;

                    if (unityWebRequest.responseCode == 200)
                            {
                        chatPoll.response = unityWebRequest.downloadHandler.data;

                        // Hack
                        var errorCode = ReadErrorCode(chatPoll.response);

                        if (errorCode == ClientModels.Error.OK)
                        {
                            chatPoll.apiResult = deserializeResult(chatPoll.response);

                            if (ApplicationSettings.LogNetwork())
                                Debug.Log(SlotSimpleJson.SerializeObject(chatPoll.apiResult));

                            successCallback((TResult)chatPoll.apiResult);
                        }
                        else
                        {
                            ErrorResponse errorResponse = ErrorResponse.Deserialize(chatPoll.response);

                            var error = new BagelCodeHTTPError
                            {
                                url = chatPoll.fullUrl,
                                responseCode = unityWebRequest.responseCode,
                                error = unityWebRequest.error,
                                errorCode = errorCode,
                                errorDetailInfo = errorResponse.errorDetailInfo
                            };

                            chatPoll.errorCallback(error);
                        }
                    }
                    else
                    {
                        var error = new BagelCodeHTTPError
                        {
                            url = chatPoll.fullUrl,
                            responseCode = unityWebRequest.responseCode,
                            error = unityWebRequest.error,
                            errorCode = ClientModels.Error.UNKNOWN
                        };

                        chatPoll.errorCallback(error);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            };

            chatPoll.errorCallback = (error) =>
            {
                Debug.Log(SlotSimpleJson.SerializeObject(error));

                if (errorCallback != null)
                    errorCallback(error);
            };

            chatPoll.coroutine = Instance.StartCoroutine(Instance.Post(chatPoll));
        }

        private void Update()
        {
            for (int i = 0; i < pendingQueue.Count; )
            {
                var req = pendingQueue[i];
                if (req.status == CallRequestContainer.Status.Received)
                {
                    pendingQueue.RemoveAt(i);
                }
                else
                {
                    if (req.status == CallRequestContainer.Status.Sent
                        && req.stopwatch.Elapsed > ApplicationSettings.Instance.webRequestTimeout)
                    {
                        StopRequest(req);

                        if (!req.reliability)
                        {
                            pendingQueue.RemoveAt(i);

                            var error = new BagelCodeHTTPError
                            {
                                url = req.fullUrl,
                                responseCode = 408, // TIMEOUT
                                error = "TIMEOUT_ERROR",
                                errorCode = ClientModels.Error.TIMEOUT_ERROR
                            };
                            req.errorCallback(error);
                            continue;
                        }
                        else if (req.retry < ApplicationSettings.Instance.webRequestRetry)
                        {
                            RestartRequest(req);
                        }
                        else
                        {
                            if (ApplicationSettings.LogNetwork())
                                Debug.Log("[BagelCodeHTTP] Timeout: " + req.fullUrl + ", time: " + req.stopwatch.Elapsed);

                            req.status = CallRequestContainer.Status.Idle;

                            if (retryEventHandler != null)
                                retryEventHandler(req);
                        }
                    }

                    ++i;
                }
            }

            if (longPoll != null)
            {
                if (longPoll.status == CallRequestContainer.Status.Sent)
                {
                    if (longPoll.stopwatch.Elapsed > ApplicationSettings.Instance.longPollTimeout)
                    {
                        StopRequest(longPoll);

                        var error = new BagelCodeHTTPError
                        {
                            url = longPoll.fullUrl,
                            errorCode = ClientModels.Error.TIMEOUT_ERROR
                        };

                        longPoll.errorCallback(error);
                        longPoll = null;
                    }
                }
            }

            if (chatPoll != null)
            {
                if (chatPoll.status == CallRequestContainer.Status.Sent)
                {
                    if (chatPoll.stopwatch.Elapsed > ApplicationSettings.Instance.longPollTimeout)
                    {
                        StopRequest(chatPoll);

                        var error = new BagelCodeHTTPError
                        {
                            url = chatPoll.fullUrl,
                            errorCode = ClientModels.Error.TIMEOUT_ERROR
                        };

                        chatPoll.errorCallback(error);
                        chatPoll = null;
                    }
                }
            }
        }

        protected internal static void StopRequest(CallRequestContainer req)
        {
            if (req == null) return;
            
            if (ApplicationSettings.LogNetwork())
                Debug.Log("[BagelCodeHTTP] Stop: " + req.fullUrl);

            req.stopwatch.Stop();
            Instance.StopCoroutine(req.coroutine);
            if (req.webRequest != null)
                req.webRequest.Dispose();
        }

        protected internal static void RestartRequest(CallRequestContainer req)
        {
            if (req == null) return;
            
            if (ApplicationSettings.LogNetwork())
                Debug.Log("[BagelCodeHTTP] Restart: " + req.fullUrl);

            ++req.retry;
            req.stopwatch.Reset();
            req.stopwatch.Start();
            req.coroutine = Instance.StartCoroutine(Instance.Post(req));
        }

        public static void RetryRequest(CallRequestContainer req)
        {
            if (req == null) return;
            
            if (ApplicationSettings.LogNetwork())
                Debug.Log("[BagelCodeHTTP] Retry: " + req.fullUrl);

            req.status = CallRequestContainer.Status.Sent;
            req.retry = 0;
            req.stopwatch.Reset();
            req.stopwatch.Start();
            req.coroutine = Instance.StartCoroutine(Instance.Post(req));
        }

        // Hack
        private static ClientModels.Error ReadErrorCode(byte[] data)
        {
            var reader = new BagelCode.Protobuf.ByteReader(data);
            while (reader.CanRead)
            {
                var __key = reader.ReadKey();
                switch (__key.Field)
                {
                case 1:
                    return (ClientModels.Error)ProtobufReader.ReadInt32(reader);
                default:
                    reader.Skip(__key.WireType);
                    break;
                }
            }

            return ClientModels.Error.UNKNOWN;
        }
    }

    public class CallRequestContainer
    {
        public enum Status
        {
            Sent,
            Received,
            Idle
        };

        public uint sequence;
        public int retry;
        public bool reliability = true;
        public Status status = Status.Idle; 
        public Dictionary<string, string> headersDict = null;

        public class ReqStopWatch
        {
            float beginTime;

            public ReqStopWatch()
            {
                Start();
            }

            public void Start()
            {
                beginTime = Time.realtimeSinceStartup;
            }

            public void Reset() {}
            public void Stop() {}

            public float Elapsed
            {
                get 
                {
                    return Time.realtimeSinceStartup - beginTime;
                }
            }
        }
        // public System.Diagnostics.Stopwatch stopwatch;
        public ReqStopWatch stopwatch;
        public Coroutine coroutine = null;
        public UnityWebRequest webRequest = null;

        public string apiEndpoint = null;
        public string fullUrl = null;
        public byte[] payload = null;
        public byte[] response = null;
        public object apiRequest = null;
        public object apiResult = null;
        public Action<UnityWebRequest> callback;
        public Action<BagelCodeHTTPError> errorCallback;

        public CallRequestContainer()
        {
            // stopwatch = System.Diagnostics.Stopwatch.StartNew();
            stopwatch = new ReqStopWatch();
        }
    }
}
