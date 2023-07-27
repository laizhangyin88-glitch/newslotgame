#if DEV
using System;
using System.Collections;
using System.Collections.Generic;

namespace SlotMaker.TestSuite
{
	public static class TestSuiteServer
	{
		private static ITestSuiteServer testSuiteServer;

		public static void InitializeServer(ITestSuiteServer server)
		{
			if (testSuiteServer != null)
				throw new Exception("Already initialized TestSuite Server");

			testSuiteServer = server;
		}

		public static string GetUserId()
		{
			return testSuiteServer.GetUserId();
		}

		public static LogFilter GetLogFilter()
		{
			return testSuiteServer.GetLogFilter();
		}

		public static void Login(Action successCallback, Action errorCallback)
		{
			testSuiteServer.Login(successCallback, errorCallback);
		}

		public static void Logout()
		{
			testSuiteServer.Logout();
		}

		public static bool HasUserData(string userDataName)
		{
			return testSuiteServer.HasUserData(userDataName);
		}

		public static string GetUserData(string userDataName)
		{
			return testSuiteServer.GetUserData(userDataName);
		}

		public static void UpdateUserData(Dictionary<string, string> userData, Action successCallback, Action errorCallback)
		{
			testSuiteServer.UpdateUserData(userData, successCallback, errorCallback);
		}

		public static bool HasApplication(string applicationName)
		{
			return testSuiteServer.HasApplication(applicationName);
		}

		public static void GetApplicationList(Action successCallback, Action errorCallback)
		{
			testSuiteServer.GetApplicationList(successCallback, errorCallback);
		}

		public static void InstallApplication(string applicationName, Action successCallback, Action errorCallback)
		{
			testSuiteServer.InstallApplication(applicationName, successCallback, errorCallback);
		}

		public static void UninstallApplication(string applicationName, Action successCallback, Action errorCallback)
		{
			testSuiteServer.UninstallApplication(applicationName, successCallback, errorCallback);
		}

		public static void GetTestSuiteReport(string reportType, Action<TestSuiteReportResult> successCallback, Action errorCallback)
		{
			testSuiteServer.GetTestSuiteReport(reportType, successCallback, errorCallback);
		}

		public static void BeginGame(Dictionary<string, object> parameter, Action successCallback, Action errorCallback)
		{
			testSuiteServer.BeginGame(parameter, successCallback, errorCallback);
		}

		public static void EndGame(Dictionary<string, object> parameter, Action successCallback, Action errorCallback)
		{
			testSuiteServer.EndGame(parameter, successCallback, errorCallback);
		}

		public static void UpdateGameWeight(Dictionary<string, object> parameter, Action successCallback, Action errorCallback)
		{
			testSuiteServer.UpdateGameWeight(parameter, successCallback, errorCallback);
		}
	}
}
#endif