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
    Toggle remember;
    InputField registAccountInput;
    InputField[] registPasswordtInput = new InputField[2];
    InputField registCodeInput;
    GameObject loginPage;
    GameObject registPage;
    bool isRememberAccount = false;
    string account = "";
    string password = "";

    private void Awake()
    {
        loginPage = transform.Find("Login").gameObject;
        registPage = transform.Find("Regist").gameObject;
        accountInput = transform.Find("Login/Account").GetComponent<InputField>();
        passwordInput = transform.Find("Login/Password").GetComponent<InputField>();
        remember = transform.Find("Login/RememToggle").GetComponent<Toggle>();
        remember.onValueChanged.AddListener((bool value) =>
        {
            isRememberAccount = value;
        });

        registAccountInput = transform.Find("Regist/Account").GetComponent<InputField>();
        registPasswordtInput[0] = transform.Find("Regist/Password0").GetComponent<InputField>();
        registPasswordtInput[1] = transform.Find("Regist/Password1").GetComponent<InputField>();
        registCodeInput = transform.Find("Regist/Code").GetComponent<InputField>();
    }

    private void OnEnable()
    {
        loginPage.SetActive(true);
        registPage.SetActive(false);
        isRememberAccount = PlayerPrefs.GetInt("isRemember", 0) == 1;
        remember.isOn = isRememberAccount;
        accountInput.text = PlayerPrefs.GetString("account", "");
        passwordInput.text = PlayerPrefs.GetString("password", "");
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
        account = accountInput.text;
        loginDict["user_pwd"] = passwordInput.text;
        password = passwordInput.text;
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
            if (isRememberAccount)
            {
                PlayerPrefs.SetInt("isRemember", 1);
                PlayerPrefs.SetString("account", account);
                PlayerPrefs.SetString("password", password);
            }
            else
            {
                PlayerPrefs.SetInt("isRemember", 0);
                PlayerPrefs.SetString("account", "");
                PlayerPrefs.SetString("password", "");
            }
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
            password = "";
            passwordInput.text = password;
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
        if (string.IsNullOrEmpty(registAccountInput.text))
        {
            Debug.LogError("Account can not be empty!");
            return;
        }
        else if (string.IsNullOrEmpty(registPasswordtInput[0].text) || string.IsNullOrEmpty(registPasswordtInput[1].text))
        {
            Debug.LogError("Password can not be empty!");
            return;
        }
        else if (registPasswordtInput[0].text != registPasswordtInput[1].text)
        {
            Debug.LogError("Password is not same!");
            return;
        }

        Dictionary<string, string> registerDict = new Dictionary<string, string>();
        registerDict["user_name"] = registAccountInput.text;
        registerDict["user_pwd"] = registPasswordtInput[0].text;
        registerDict["user_code"] = registCodeInput.text;
        account = registAccountInput.text;
        password = registPasswordtInput[0].text;
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
            accountInput.text = account;
            loginPage.SetActive(true);
            registPage.SetActive(false);
            Debug.LogError("AccountRegistSucceed");
        }
        else
        {
            //error
            password = "";
            registPasswordtInput[0].text = password;
            registPasswordtInput[1].text = password;
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
