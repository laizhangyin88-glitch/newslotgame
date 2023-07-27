using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class ThumbnailChecker
    {
        // public static string thumbnailsFolderPath = "Assets/Contents/Contents Group 1/_Slot Images/Slot Thumbnail";
        // public static string imagesFolderPath = "Assets/Contents/Contents Group 1/_Slot Images/Slot Image";

        // public static string checkAssetName = "slotthumb1";

        // public static string checkSlotImageBigFormat = "Slot Image Big {0}";
        // public static string checkSlotImageSmallFormat = "Slot Image Small {0}";
        // public static string checkSlotThumbnailFormat = "Slot Thumbnail {0}";

        // public static List<GameInfo> gameInfoList = null;
        // public static BagelCodeReliabilitySystem seqSyste = new BagelCodeReliabilitySystem();

        // public static void LoginAPI()
        // {
        //     deviceID = SystemInfo.deviceUniqueIdentifier;

        //     LoginRequest request = new LoginRequest
        //     {
        //         blockseq = seqSyste.FetchBlockSeq(),
        //         ackMask = BagelCodeHTTP.GenerateAckBits(),
        //         clientOs = ApplicationSettings.GetPlatformName().ToUpper(),
        //         clientNumberVersion = ApplicationSettings.GetClientVersionNumber(),
        //         deviceId = SystemInfo.deviceUniqueIdentifier,
        //         deviceName = ApplicationSettings.GetDeviceModel(),
        //         assetVersion = ApplicationSettings.Instance.bundleVersion,
        //         loginTime = TimeUtils.GetTimeStamp(),
        //         timezoneOffset = TimeUtils.GetTimeZoneOffset(),
        //         language = ApplicationSettings.GetDeviceLanguage(),
        //         isServerMaintenanceIgnore = false
        //     };

        //     BagelCodeHTTP.MakeApiCall("/v0/main/login", null, request,
        //         LoginResponse.Deserialize, responseCallback, errorCallback);
        // }

        // private static bool IsReady()
        // {
        //     if(Application.isPlaying == false) 
        //     {
        //         Debug.LogError("Checker is only working to playing mode(after lobby).");
        //         return false;
        //     }

        //     if(gameInfoList == null)
        //     {
        //         return false;
        //     }

        //     // var gameInfoList = BlackboardUtils.FindVariable<List<GameInfo>>(null, "./gameInfoList");

        //     // if(gameInfoList == null)
        //     // {
        //     //     Debug.LogError("gameInfoList is null. retry after lobby.");
        //     //     return false;
        //     // }

        //     return true;
        // }

        // [MenuItem("Assets/Thumbnail/Check Thumbnails", false, 750)]
        // public static void CheckThumbnails()
        // {
        //     if(IsReady() == false) return;

        //     var prefabs = System.IO.Directory.GetFiles(folderPath, "*.prefab");

        //     if(prefabs != null)
        //     {
        //         for(int i=0 ; i<prefabs.Length; ++i)
        //         {
        //             // Check checkAssetName
        //         }
        //     }
        //     // if (Selection.assetGUIDs == null)
        //     //     return;

        //     // string[] selectPaths = Selection.assetGUIDs.Select(x => AssetDatabase.GUIDToAssetPath(x)).ToArray();

        //     // if(selectPaths != null && selectPaths.Length > 0)
        //     // {
        //     //     for(int i=0; i < selectPaths.Length; ++i)
        //     //     {
        //     //         // GameObject prefab = PrefabUtility.LoadPrefabContents(path);
        //     //         Debug.LogError(selectPaths[i]);
        //     //     }
        //     // }
        // }

        // [MenuItem("Assets/Thumbnail/Check Slot Images(long, short", false, 751)]
        // public static void CheckSlotImages()
        // {
        //     if(IsReady() == false) return;
        // }

        // public static bool CheckAsset()
        // {
        //     return false;
        // }
    }
}