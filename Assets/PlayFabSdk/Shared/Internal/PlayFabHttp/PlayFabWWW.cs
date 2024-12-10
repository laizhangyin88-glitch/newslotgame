#if !DISABLE_PLAYFABCLIENT_API
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PlayFab.Json;
using PlayFab.SharedModels;
using UnityEngine;
using UnityEngine.Networking;

#if !UNITY_WSA && !UNITY_WP8 && !UNITY_WEBGL
using Ionic.Zlib;
#endif

namespace PlayFab.Internal
{
    public class PlayFabWww : IPlayFabHttp
    {
        private int _pendingWwwMessages = 0;
        public bool SessionStarted { get; set; }
        public string AuthKey { get; set; }

        public void InitializeHttp() { }
        public void Update() { }
        public void OnDestroy() { }

        public void MakeApiCall(CallRequestContainer reqContainer)
        {
            reqContainer.RequestHeaders["Content-Type"] = "application/json";

#if !UNITY_WSA && !UNITY_WP8 && !UNITY_WEBGL
            if (PlayFabSettings.CompressApiData)
            {
                reqContainer.RequestHeaders["Content-Encoding"] = "GZIP";
                reqContainer.RequestHeaders["Accept-Encoding"] = "GZIP";

                using (var stream = new MemoryStream())
                {
                    using (var zipstream = new GZipStream(stream, Ionic.Zlib.CompressionMode.Compress, Ionic.Zlib.CompressionLevel.BestCompression))
                    {
                        zipstream.Write(reqContainer.Payload, 0, reqContainer.Payload.Length);
                    }
                    reqContainer.Payload = stream.ToArray();
                }
            }
#endif

            var request = new UnityWebRequest(reqContainer.FullUrl, "POST");
            request.uploadHandler = new UploadHandlerRaw(reqContainer.Payload);
            request.downloadHandler = new DownloadHandlerBuffer();

            foreach (var header in reqContainer.RequestHeaders)
            {
                request.SetRequestHeader(header.Key, header.Value);
            }

            // Debug.LogFormat("Posting {0} to Url: {1}", req.Trim(), reqContainer.FullUrl);
#if PLAYFAB_REQUEST_TIMING
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
#endif

            Action<string> wwwSuccessCallback = (response) =>
            {
                try
                {
#if PLAYFAB_REQUEST_TIMING
                    var startTime = DateTime.UtcNow;
#endif
                    var httpResult = JsonWrapper.DeserializeObject<HttpResponseObject>(response);

                    if (httpResult.code == 200)
                    {
                        reqContainer.JsonResponse = JsonWrapper.SerializeObject(httpResult.data);
                        reqContainer.DeserializeResultJson();
                        reqContainer.ApiResult.Request = reqContainer.ApiRequest;
                        reqContainer.ApiResult.CustomData = reqContainer.CustomData;

#if !DISABLE_PLAYFABCLIENT_API
                        var res = reqContainer.ApiResult as ClientModels.LoginResult;
                        var regRes = reqContainer.ApiResult as ClientModels.RegisterPlayFabUserResult;
                        if (res != null)
                            AuthKey = res.SessionTicket;
                        else if (regRes != null)
                            AuthKey = regRes.SessionTicket;

                        if (res != null || regRes != null)
                            PlayFabDeviceUtil.OnPlayFabLogin(res, regRes);
#endif
                        try
                        {
                            PlayFabHttp.SendEvent(reqContainer.ApiEndpoint, reqContainer.ApiRequest, reqContainer.ApiResult, ApiProcessingEventType.Post);
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }

#if PLAYFAB_REQUEST_TIMING
                        stopwatch.Stop();
                        var timing = new PlayFabHttp.RequestTiming {
                            StartTimeUtc = startTime,
                            ApiEndpoint = reqContainer.ApiEndpoint,
                            WorkerRequestMs = (int)stopwatch.ElapsedMilliseconds,
                            MainThreadRequestMs = (int)stopwatch.ElapsedMilliseconds
                        };
                        PlayFabHttp.SendRequestTiming(timing);
#endif
                        try
                        {
                            reqContainer.InvokeSuccessCallback();
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                    else
                    {
                        if (reqContainer.ErrorCallback != null)
                        {
                            reqContainer.Error = PlayFabHttp.GeneratePlayFabError(response, reqContainer.CustomData);
                            PlayFabHttp.SendErrorEvent(reqContainer.ApiRequest, reqContainer.Error);
                            reqContainer.ErrorCallback(reqContainer.Error);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            };

            Action<string> wwwErrorCallback = (errorCb) =>
            {
                reqContainer.JsonResponse = errorCb;
                if (reqContainer.ErrorCallback != null)
                {
                    reqContainer.Error = PlayFabHttp.GeneratePlayFabError(reqContainer.JsonResponse, reqContainer.CustomData);
                    PlayFabHttp.SendErrorEvent(reqContainer.ApiRequest, reqContainer.Error);
                    reqContainer.ErrorCallback(reqContainer.Error);
                }
            };

            PlayFabHttp.instance.StartCoroutine(Post(request, wwwSuccessCallback, wwwErrorCallback));
        }

        private IEnumerator Post(UnityWebRequest request, Action<string> wwwSuccessCallback, Action<string> wwwErrorCallback)
        {
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                wwwErrorCallback(request.error);
            }
            else
            {
                try
                {
#if !UNITY_WSA && !UNITY_WP8 && !UNITY_WEBGL
                    string encoding;
                    if (request.GetResponseHeaders().TryGetValue("Content-Encoding", out encoding) && encoding.ToLowerInvariant() == "gzip")
                    {
                        var stream = new MemoryStream(request.downloadHandler.data);
                        using (var gZipStream = new GZipStream(stream, Ionic.Zlib.CompressionMode.Decompress, false))
                        {
                            var buffer = new byte[4096];
                            using (var output = new MemoryStream())
                            {
                                int read;
                                while ((read = gZipStream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    output.Write(buffer, 0, read);
                                }
                                output.Seek(0, SeekOrigin.Begin);
                                var streamReader = new StreamReader(output);
                                var jsonResponse = streamReader.ReadToEnd();
                                wwwSuccessCallback(jsonResponse);
                            }
                        }
                    }
                    else
#endif
                    {
                        wwwSuccessCallback(request.downloadHandler.text);
                    }
                }
                catch (Exception e)
                {
                    wwwErrorCallback("Unhandled error in PlayFabWWW: " + e);
                }
            }
        }

        public int GetPendingMessages()
        {
            return _pendingWwwMessages;
        }
    }
}
#endif
