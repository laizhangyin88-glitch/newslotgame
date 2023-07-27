#if DEV
using System;
using System.Collections;
using System.Collections.Generic;

namespace SlotMaker.TestSuite
{
	public interface ITestSuiteServer
	{
		string GetUserId();
		LogFilter GetLogFilter();
		
		void Login(Action successCallback, Action errorCallback);
		void Logout();

		bool HasUserData(string userDataName);
		string GetUserData(string userDataName);
		void UpdateUserData(Dictionary<string, string> userData, Action successCallback, Action errorCallback);
		
		bool HasApplication(string applicationName);
		void GetApplicationList(Action successCallback, Action errorCallback);
		void InstallApplication(string applicationName, Action successCallback, Action errorCallback);
		void UninstallApplication(string applicationName, Action successCallback, Action errorCallback);

		void GetTestSuiteReport(string reportType, Action<TestSuiteReportResult> successCallback, Action errorCallback);
		void BeginGame(Dictionary<string, object> parameter, Action successCallback, Action errorCallback);
		void EndGame(Dictionary<string, object> parameter, Action successCallback, Action errorCallback);
		void UpdateGameWeight(Dictionary<string, object> parameter, Action successCallback, Action errorCallback);
	}
}
#endif