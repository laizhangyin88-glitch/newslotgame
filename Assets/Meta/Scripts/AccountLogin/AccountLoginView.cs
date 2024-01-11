using BagelCode;
using com.adjust.sdk;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AccountLoginView : MonoBehaviour
{
    InputField accountInput;
    InputField passwordInput;
    InputField registAccountInput;
    InputField registPasswordtInput;
    GameObject loginPage;
    GameObject registPage;

    private void Awake()
    {
        loginPage = transform.Find("Login").gameObject;
        registPage = transform.Find("Regist").gameObject;
        accountInput = transform.Find("Login/Account").GetComponent<InputField>();
        passwordInput = transform.Find("Login/Password").GetComponent<InputField>();
        registAccountInput = transform.Find("Regist/Account").GetComponent<InputField>();
        registPasswordtInput = transform.Find("Regist/Password").GetComponent<InputField>();
    }

    private void OnEnable()
    {
        loginPage.SetActive(true);
        registPage.SetActive(false);
    }

    public void OnLoginClick()
    {
        if (string.IsNullOrEmpty(accountInput.text) || string.IsNullOrEmpty(passwordInput.text))
        {
            Debug.LogError("empty string is unlegal");
            return;
        }
        Dictionary<string, string> loginDict = new Dictionary<string, string>();
        loginDict["user_name"] = accountInput.text;
        loginDict["user_pwd"] = passwordInput.text;
        StartCoroutine(HttpPost(ApplicationSettings.Instance.loginUrl, "/passwd_login", loginDict, HandleLoginUserResponse));
    }

    void HandleLoginUserResponse(string response)
    {
        if (string.IsNullOrEmpty(response))
            return;
        AccountLoginRespone accountLoginRespone = JsonUtility.FromJson<AccountLoginRespone>(response);
        if (accountLoginRespone.err == 1)
        {
            //succeed
            Debug.LogError("AccountLoginSucceed");
            var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "userLoginInfo");
            LoginExtraData loginExtraData = new LoginExtraData()
            {
                login_token = accountLoginRespone.token
            };
            string strContent = JsonUtility.ToJson(loginExtraData);
            BlackboardUtils.SetOrCreateValue(bb, "loginInfo", strContent);
            EventSender.SendGlobalEvent("OnAccountLoginSucceed");
            Destroy(gameObject);
        }
        else
        {
            //error
            Debug.LogError("AccountLoginFail");
        }
    }

    public void OnLoginRegistClick()
    {
        loginPage.SetActive(false);
        registPage.SetActive(true);
    }

    public void OnRegistClick()
    {
        Dictionary<string, string> registerDict = new Dictionary<string, string>();
        registerDict["user_name"] = registAccountInput.text;
        registerDict["user_pwd"] = registPasswordtInput.text;
        StartCoroutine(HttpPost(ApplicationSettings.Instance.loginUrl, "/register_user", registerDict, HandleRegisterUserResponse));
    }

    public void OnReturnClick()
    {
        loginPage.SetActive(true);
        registPage.SetActive(false);
    }

    void HandleRegisterUserResponse(string response)
    {
        if (string.IsNullOrEmpty(response))
            return;
        AccountLoginRespone accountLoginRespone = JsonUtility.FromJson<AccountLoginRespone>(response);
        if (accountLoginRespone.err == 1)
        {
            //succeed
            Debug.LogError("AccountRegistSucceed");
        }
        else
        {
            //error
            Debug.LogError("AccountRegistFail");
        }
    }


    IEnumerator HttpPost(string url, string method, Dictionary<string, string> post_param, System.Action<string> callback)
    {
        // 创建一个表单
        JSONNode jsonNode = JSONNode.Parse("{}");

        foreach (var kvp in post_param)
        {
            jsonNode[kvp.Key] = kvp.Value;
        }
        string jsonBody = jsonNode.ToString();

        // 创建 UnityWebRequest 对象
        using (UnityWebRequest www = UnityWebRequest.Post(url + method, "POST"))
        {
            byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            www.uploadHandler = new UploadHandlerRaw(jsonBytes);
            www.downloadHandler = new DownloadHandlerBuffer();

            www.SetRequestHeader("Content-Type", "application/json");

            // 发送请求
            yield return www.SendWebRequest();

            // 检查是否有错误发生
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                // 请求成功，处理返回的数据
                string jsonResponse = www.downloadHandler.text;
                Debug.Log("Response: " + www.downloadHandler.text);
                callback?.Invoke(jsonResponse); // 调用回调函数，传递响应数据
            }
        }
    }

    public struct LoginExtraData
    {
        public string login_token;
    }

    public struct AccountLoginRespone
    {
        public int err;
        public string token;
    }

}
