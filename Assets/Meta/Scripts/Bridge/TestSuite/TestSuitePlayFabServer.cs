#if DEV
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using SlotMaker.Json;
using SlotMaker.TestSuite;
using PlayFab;
using PlayFab.Internal;
using PlayFab.ClientModels;

namespace BagelCode
{
	public class TestSuitePlayFabServer : ITestSuiteServer
	{
		LoginResult loginResult;
        List<CatalogItem> catalog;

		public string GetUserId()
		{
			return loginResult.PlayFabId;
		}

		public LogFilter GetLogFilter()
		{
			UserDataRecord logFilter = null;
            if (loginResult.InfoResultPayload.UserData.TryGetValue("logFilter", out logFilter))
                return (LogFilter)Convert.ToInt32(logFilter.Value);

            loginResult.InfoResultPayload.UserData["logFilter"] = new UserDataRecord{ Value = "0" };
            return (LogFilter)0;
		}

		public void Login(Action successCallback, Action errorCallback)
		{
			var infoRequestParameters = new GetPlayerCombinedInfoRequestParams
    		{
    			GetUserAccountInfo = true,
    			GetUserInventory = true,
    			GetUserData = true,
    			UserDataKeys = new List<string>
    			{
    				"logFilter",
    				"lastGame",
                    "testCase",
                    "forceGuestMode",
                    "quit"
    			}
    		};

#if UNITY_IOS && !UNITY_EDITOR
            PlayFabClientAPI.LoginWithIOSDeviceID(
                new LoginWithIOSDeviceIDRequest
                {
                    DeviceId = NativeHelper.Instance.GetDeviceID(),
#elif UNITY_ANDROID && !UNITY_EDITOR
            PlayFabClientAPI.LoginWithAndroidDeviceID(
                new LoginWithAndroidDeviceIDRequest
                {
                    AndroidDeviceId = NativeHelper.Instance.GetDeviceID(),
#else
            PlayFabClientAPI.LoginWithCustomID(
                new LoginWithCustomIDRequest
                {
    #if UNITY_WSA && !UNITY_EDITOR
                    CustomId = NativeHelper.Instance.GetDeviceID(),
    #else
                    CustomId = SystemInfo.deviceUniqueIdentifier,
    #endif
#endif
                    CreateAccount = true,
                    InfoRequestParameters = infoRequestParameters
                },
                (result) =>
                {
                	loginResult = result;

                    TestSuiteManager.Instance.LoginResult();

                    if (ApplicationSettings.LogTestSuite())
                        Debug.Log(string.Format("[TestSuite] Login successed: {0}", SlotSimpleJson.SerializeObject(result)));

                    if (successCallback != null)
                    	successCallback();
                },
                (error) =>
                {
                    Debug.LogError("[TestSuite] " + error.ToString());

                    if (errorCallback != null)
                    	errorCallback();
                }
            );
		}

        public void Logout()
        {
            PlayFabHttp.ForgetClientCredentials();
        }

        public bool HasUserData(string userDataName)
        {
            return loginResult.InfoResultPayload.UserData.ContainsKey(userDataName);
        }

        public string GetUserData(string userDataName)
        {
            UserDataRecord data = null;
            if (loginResult.InfoResultPayload.UserData.TryGetValue(userDataName, out data))
                return data.Value;
            return null;
        }

		public void UpdateUserData(Dictionary<string, string> userData, Action successCallback, Action errorCallback)
		{
			foreach (var pair in userData)
			{
				loginResult.InfoResultPayload.UserData[pair.Key] = new UserDataRecord { Value = pair.Value };
			}

			PlayFabClientAPI.UpdateUserData(
				new UpdateUserDataRequest { Data = userData }, 
				(result) => { if (successCallback != null) successCallback(); },
				(error) => { if (errorCallback != null) errorCallback(); }
			);
		}

        public bool HasApplication(string applicationName)
        {
            return GetApplication(applicationName) != null;
        }

        ItemInstance GetApplication(string applicationName, bool pop = false)
        {
            var inventory = loginResult.InfoResultPayload.UserInventory;
            foreach (var itemInstance in inventory)
            {
                if (itemInstance.ItemClass.Equals("Editor"))
                {
                    if (itemInstance.ItemId.Equals(applicationName))
                    {
                        if (pop) 
                            inventory.Remove(itemInstance);

                        return itemInstance;
                    }
                }
            }
            return null;    
        }

        public void GetApplicationList(Action successCallback, Action errorCallback)
        {
            PlayFabClientAPI.GetCatalogItems(
                new GetCatalogItemsRequest { CatalogVersion = "EditorStore" },
                (result) => 
                { 
                    catalog = result.Catalog;

                    if (successCallback != null) 
                        successCallback(); 
                },
                (error) => { if (errorCallback != null) errorCallback(); }
            );
        }

        public void InstallApplication(string applicationName, Action successCallback, Action errorCallback)
        {
            PlayFabClientAPI.PurchaseItem(
                new PurchaseItemRequest
                {
                    ItemId = applicationName,
                    VirtualCurrency = "TC",
                    Price = 0,
                    CatalogVersion = "EditorStore"
                },
                (result) => 
                { 
                    loginResult.InfoResultPayload.UserInventory.AddRange(result.Items);

                    if (successCallback != null) successCallback(); 
                },
                (error) => { if (errorCallback != null) errorCallback(); }
            );
        }

        public void UninstallApplication(string applicationName, Action successCallback, Action errorCallback)
        {
            PlayFabClientAPI.ConsumeItem(
                new ConsumeItemRequest
                {
                    ItemInstanceId = GetApplication(applicationName, true).ItemInstanceId,
                    ConsumeCount = 1
                },
                (result) => { if (successCallback != null) successCallback(); },
                (error) => { if (errorCallback != null) errorCallback(); }
            );
        }

        public void GetTestSuiteReport(string reportType, Action<TestSuiteReportResult> successCallback, Action errorCallback)
        {
            PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
                {
                    StatisticName = reportType,
                    StartPosition = 0,
                    MaxResultsCount = 100
                },
                (response) =>
                {
                    var ret = new TestSuiteReportResult();
                    ret.Results = new List<TestSuiteReportEntry>(response.Leaderboard.Count);
                    foreach (var entry in response.Leaderboard)
                    {
                        ret.Results.Add(new TestSuiteReportEntry
                        {
                            ReportName = entry.DisplayName,
                            StatValue = entry.StatValue,
                            Position = entry.Position
                        });
                    }
                    ret.NextReset = response.NextReset;

                    if (successCallback != null)
                        successCallback(ret);
                },
                (error) => { if (errorCallback != null) errorCallback(); }
            );
        }

        public void BeginGame(Dictionary<string, object> parameter, Action successCallback, Action errorCallback)
        {
            ExecuteCloudScript("contentBegin", parameter, successCallback, errorCallback);
        }

        public void EndGame(Dictionary<string, object> parameter, Action successCallback, Action errorCallback)
        {
            ExecuteCloudScript("contentEnd", parameter, successCallback, errorCallback);
        }

        public void UpdateGameWeight(Dictionary<string, object> parameter, Action successCallback, Action errorCallback)
        {
            ExecuteCloudScript("contentWeight", parameter, successCallback, errorCallback);
        }

        void ExecuteCloudScript(string functionName, Dictionary<string, object> parameter, Action successCallback, Action errorCallback)
        {
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
            {
                FunctionName = functionName,
                FunctionParameter = parameter
            }, 
            (result) => { if (successCallback != null) successCallback(); },
            (error) => { if (errorCallback != null) errorCallback(); });    
        }
	}
}
#endif