using BagelCode;
using com.adjust.sdk;
using ParadoxNotion;
using SlotMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AccountLoginViewNew : MonoBehaviour
{
    InputField accountInput;
    InputField passwordInput;
    Toggle remember;
    InputField registAccountInput;
    InputField[] registPasswordtInput = new InputField[2];
    InputField registCodeInput;
    GameObject loginPage;
    GameObject registPage;
    GameObject tips;
    Text tipsText;
    bool isRememberAccount = false;
    string account = "";
    string password = "";
    float timers = 0;

    InputField inpNetWork; //NetWork
    Text txtNetWorkPlaceholder = null;
    Toggle tglAutoSever;
    Button bbtnClear;

    Text txtClientVersion = null;

    
    string _serverAddress = null;
    bool _isAutoSever = false;


    private bool isRun = false;
    private Queue<Action> taskQueue = new Queue<Action>();

    protected System.Timers.Timer _mechineConnectTimer = null;


    private void Update()
    {
        if (!isRun)
        {
            isRun = true;
            while (taskQueue.Count > 0)
            {
                var task = taskQueue.Dequeue();
                task.Invoke();
            }
            isRun = false;
        }
    }

    private void Awake()
    {
        txtClientVersion = transform.Find("Anchor/Login/clientVersion").GetComponent<Text>();
        txtClientVersion.text = ApplicationSettings.Instance.clientVersion;

        txtNetWorkPlaceholder = transform.Find("Anchor/Login/NetWork/Placeholder").GetComponent<Text>();
        inpNetWork = transform.Find("Anchor/Login/NetWork").GetComponent<InputField>();


        tglAutoSever = transform.Find("Anchor/Login/AutoSeverToggle").GetComponent<Toggle>();
        tglAutoSever.onValueChanged.AddListener((bool value) =>
        {
            _isAutoSever = value;
            if (_isAutoSever)
            {
                PlayerPrefs.SetInt("isAutoSever", 1);
            }
            else
            {
                PlayerPrefs.SetInt("isAutoSever", 0);
            }

        });

        bbtnClear = transform.Find("Anchor/Login/Clear").GetComponent<Button>();
        bbtnClear.onClick.AddListener(() =>
        {
            PlayerPrefs.SetInt("isAutoSever", 0);
            _isAutoSever = false;
            tglAutoSever.isOn = false;

            PlayerPrefs.SetString("serverAddress", "");
            inpNetWork.text = "";

            _serverAddress = ApplicationSettings.Instance.newLoginUrlApp;
            txtNetWorkPlaceholder.text = _serverAddress;
        });



        loginPage = transform.Find("Anchor/Login").gameObject;
        registPage = transform.Find("Anchor/Regist").gameObject;
        accountInput = transform.Find("Anchor/Login/Account").GetComponent<InputField>();
        passwordInput = transform.Find("Anchor/Login/Password").GetComponent<InputField>();
        remember = transform.Find("Anchor/Login/RememToggle").GetComponent<Toggle>();
        remember.onValueChanged.AddListener((bool value) =>
        {
            GSManager.Instance?.GetHandler("UI_Button_Normal").Play();
            isRememberAccount = value;
        });

        registAccountInput = transform.Find("Anchor/Regist/Account").GetComponent<InputField>();
        registPasswordtInput[0] = transform.Find("Anchor/Regist/Password0").GetComponent<InputField>();
        registPasswordtInput[1] = transform.Find("Anchor/Regist/Password1").GetComponent<InputField>();
        registCodeInput = transform.Find("Anchor/Regist/Code").GetComponent<InputField>();

        tips = transform.Find("Tips").gameObject;
        tipsText = transform.Find("Tips/Text").GetComponent<Text>();


        Debug.LogWarning($" 【版本号】： 客户端版本：{ApplicationSettings.Instance.clientVersion}，产品版本 {ProductSettings.Instance.productVersion}，是否机台包 {ApplicationSettings.Instance.isMachine}");
        NetManager.Instance.Init(new WebSock());
        FgBgManager.Instance.Init();
        // JsonUtilityTest.DoExample();
    }


    private void OnDestroy()
    {
        if (_mechineConnectTimer != null)
        {
            _mechineConnectTimer.Stop();
            _mechineConnectTimer.Dispose();
            _mechineConnectTimer = null;
        }
    }

    private void MechineAutoConnect()
    {
            /*
            Dictionary<string, string> loginDict = new Dictionary<string, string>();
            loginDict["user_name"] = "device2";
            loginDict["user_pwd"] =  "123456";
            string url = ApplicationSettings.Instance.newLoginUrlMechine;
            Debug.LogWarning($"@ connect : {url} user_name {loginDict["user_name"]} ");
            StartCoroutine(HttpPost(url, "/passwd_login", loginDict, HandleLoginUserResponse));*/

            Dictionary<string, string> loginDict = new Dictionary<string, string>();
            loginDict["device_id"] = NativeHelper.Instance.GetDeviceID();
            string url = ApplicationSettings.Instance.newLoginUrlMechine;
            Debug.LogWarning($"@ get device account : {url}/device_login  device_id = {loginDict["device_id"]} ");
            StartCoroutine(HttpPost(url, "/device_login", loginDict, HandleGetDeviceAccountResponse));
    }

    void HandleGetDeviceAccountResponse(string response)
    {
        if (string.IsNullOrEmpty(response))
            return;

        DeviceAccountRespone deviceAccount = JsonUtility.FromJson<DeviceAccountRespone>(response);
        if (deviceAccount.err == 0)
        {
            Dictionary<string, string> loginDict = new Dictionary<string, string>();
            loginDict["user_name"] = deviceAccount.user_name;
            loginDict["user_pwd"] = deviceAccount.user_pwd;
            //accountInput.text = loginDict["user_name"];
            //passwordInput.text = loginDict["user_pwd"];
            string url = ApplicationSettings.Instance.newLoginUrlMechine;
            Debug.LogWarning($"@ connect : {url}/passwd_login  user_name = {loginDict["user_name"]} ");
            StartCoroutine(HttpPost(url, "/passwd_login", loginDict, HandleLoginUserResponse));
        }
        else
        {
            Debug.LogError($"{deviceAccount.msg}");
            StartCoroutine(ShowTips($"{deviceAccount.msg}"));
        }
    }



    private void Start()
    {
        transform.Find("Anchor").gameObject.SetActive(!ApplicationSettings.Instance.isMachine);
        //transform.Find("Anchor").gameObject.SetActive(false);
        if (ApplicationSettings.Instance.isMachine)
        {
            MechineAutoConnect();
            _mechineConnectTimer = new System.Timers.Timer(15000);
            _mechineConnectTimer.AutoReset = true; // 是否重复执行
            _mechineConnectTimer.Elapsed += (object sender, ElapsedEventArgs e) => {
                taskQueue.Enqueue(() =>
                {
                    MechineAutoConnect();
                });
            };
            _mechineConnectTimer.Start();
        }
        else
        {
            loginPage.SetActive(true);
            registPage.SetActive(false);
            isRememberAccount = PlayerPrefs.GetInt("isRemember", 0) == 1;
            remember.isOn = isRememberAccount;
            accountInput.text = PlayerPrefs.GetString("account", "");
            passwordInput.text = PlayerPrefs.GetString("password", "");


            _isAutoSever = PlayerPrefs.GetInt("isAutoSever", 0) == 1;
            tglAutoSever.isOn = _isAutoSever;

            string addr = PlayerPrefs.GetString("serverAddress", "");
            _serverAddress = !string.IsNullOrEmpty(addr) ? addr : ApplicationSettings.Instance.newLoginUrlApp;
            txtNetWorkPlaceholder.text = _serverAddress;

        }
    }





    private void OnEnable() {}


    public IEnumerator WWW_Get(Action<string,string> cb)
    {
        bool getTargetUrl = false;
        //string finalStr = "http://8.138.117.128:9981/get_config?key=myApplication.new_login_url";
        //string finalStr = "http://8.138.117.128:9981/get_config?key=myApplication.xigua_login_url";
        //http://8.138.117.128:9981/get_config?key=myApplication.haicao_logic_url

        string finalStr = TestManager.Instance.getAutoUrl();
        if (string.IsNullOrEmpty(finalStr)) {
            finalStr = ApplicationSettings.Instance.autoUrl;
        } 

        Debug.LogWarning(finalStr);
        WWW www = new WWW(finalStr);
        yield return www;
        //如果error是空的，说明访问成功
        string addr = null;
        string err = null;
        if (string.IsNullOrEmpty(www.error))
        {
            //192.168.2.218:7501
            addr = "http://" + www.text;
            getTargetUrl = true;
        }
        else
        {
            //yield return ShowTips($"failed,msg:{www.error}");

            Debug.LogError($"failed,msg:{www.error}");
            err = www.error;
        }
        yield return new WaitUntil(() => getTargetUrl);

        if (cb != null) cb(addr,err);

    }

    //device_login

    public IEnumerator WWW_Get02(string url,Action<string, string> cb)
    {
        /*string finalStr = TestManager.Instance.getAutoUrl();
        if (finalStr == "")
        {
            finalStr = ApplicationSettings.Instance.autoUrl;
        }

        Debug.LogWarning(finalStr);
        */
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            //yield return www.Send();
            yield return www.SendWebRequest();
            string err = null;
            string addr = null;
            if (www.isNetworkError || www.isHttpError)
            {
                StartCoroutine(ShowTips($"{www.error}"));
                Debug.LogError("Error: " + www.error);
                err = www.error;
            }
            else
            {
                //addr = "http://" + www.downloadHandler.text;
                addr = www.downloadHandler.text;
            }
            if (cb != null) cb(addr, err);
        }
    }

    public void OnLoginClick()
    {
        if (string.IsNullOrEmpty(accountInput.text))//|| string.IsNullOrEmpty(passwordInput.text))
        {
            StartCoroutine(ShowTips("Account can not be empty!"));
            return;
        }
        if (string.IsNullOrEmpty(passwordInput.text))
        {
            StartCoroutine(ShowTips("Password can not be empty!"));
            return;
        }

        if (accountInput.text == "admin" && passwordInput.text == "@123456")
        {
            inpNetWork.transform.gameObject.SetActive(true);
            tglAutoSever.transform.gameObject.SetActive(true);
            bbtnClear.transform.gameObject.SetActive(true);
            return;
        }

        if(!string.IsNullOrEmpty(_serverAddress))
            _serverAddress = _serverAddress.Trim();
        if (!string.IsNullOrEmpty(inpNetWork.text))
            inpNetWork.text = inpNetWork.text.Trim();



        //string addr = PlayerPrefs.GetString("serverAddress", "");
       // _serverAddress = !string.IsNullOrEmpty(addr) ? addr : ApplicationSettings.Instance.newLoginUrl;

        if (!string.IsNullOrEmpty(inpNetWork.text) && !_isAutoSever)
        {

            PlayerPrefs.SetString("serverAddress", inpNetWork.text);
            _serverAddress = inpNetWork.text;
            StartCoroutine(ShowTips($"Server address = {inpNetWork.text}"));

            Dictionary<string, string> loginDict = new Dictionary<string, string>();
            loginDict["user_name"] = accountInput.text;
            account = accountInput.text;
            loginDict["user_pwd"] = passwordInput.text;
            password = passwordInput.text;
            Debug.LogWarning($"@ connect : {_serverAddress}");
            StartCoroutine(HttpPost(_serverAddress, "/passwd_login", loginDict, HandleLoginUserResponse));

        }
        else if (_isAutoSever || string.IsNullOrEmpty(_serverAddress)) //自动获取地址
        {
            string autoUrl = TestManager.Instance.getAutoUrl();
            if (string.IsNullOrEmpty(autoUrl))
                autoUrl = ApplicationSettings.Instance.autoUrl;

            Debug.LogWarning(autoUrl);

            StartCoroutine(WWW_Get02(
                autoUrl,
                (addr1,err) =>
                {
                    if (addr1 == null)
                    {
                        string errMsg = $"【ERR】：自动获取网络地址失败：{err}";
                        StartCoroutine(ShowTips(errMsg));
                        Debug.LogError(errMsg);
                        return;
                    }
                    _serverAddress = "http://" +addr1;
                    //_serverAddress = addr1;

                    Dictionary<string, string> loginDict = new Dictionary<string, string>();
                    loginDict["user_name"] = accountInput.text;
                    account = accountInput.text;
                    loginDict["user_pwd"] = passwordInput.text;
                    password = passwordInput.text;
                    Debug.LogWarning($"@ connect : {_serverAddress}");
                    StartCoroutine(HttpPost(_serverAddress, "/passwd_login", loginDict, HandleLoginUserResponse));

                }
            ));
        }
        else
        {

            Dictionary<string, string> loginDict = new Dictionary<string, string>();
            loginDict["user_name"] = accountInput.text;
            account = accountInput.text;
            loginDict["user_pwd"] = passwordInput.text;
            password = passwordInput.text;
            Debug.LogWarning($"@ connect : {_serverAddress}");
            StartCoroutine(HttpPost(_serverAddress, "/passwd_login", loginDict, HandleLoginUserResponse));
        }
    }

    public void OnForgotClick()
    {
        account = accountInput.text;
        Debug.LogError("Forgot Click");
        if (string.IsNullOrEmpty(account))
        {
            StartCoroutine(ShowTips("Account can not be empty!"));
        }
        else
        {
            MainBlackboard.Get().SetValue("account", account);
            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT,
                new EventData(MetaEventDefine.ON_ENTER_RESET_PASSWORD));
        }
    }

    void HandleLoginUserResponse(string response)
    {
        if (string.IsNullOrEmpty(response))
            return;

        AccountLoginRespone accountLoginRespone = JsonUtility.FromJson<AccountLoginRespone>(response);
        if (accountLoginRespone.err == 0)
        {
            if (!ApplicationSettings.Instance.isMachine)
            {
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
            }


            if (!NetManager.Instance.isConnect())
            {
                string logicUrl = string.Format("ws://{0}", accountLoginRespone.logic_ip);
                NetManager.Instance.Connect(new NetConnectOptions(logicUrl,99));
            }
            globalStore.gToken = accountLoginRespone.token_id;


            //Debug.LogError("AccountLoginSucceed");
            var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "userLoginInfo");
            LoginExtraData loginExtraData = new LoginExtraData()
            {
                login_token = accountLoginRespone.token_id
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
            Debug.LogError($"{accountLoginRespone.msg}");
            StartCoroutine(ShowTips($"{accountLoginRespone.msg}"));
        }
    }

    void res_login(ParadoxNotion.EventData eventData)
    {
        //Debug.Log(eventData);
        //Dictionary<string,object>data = eventData.value as Dictionary<string,object>;
        //int err = (int)data["err"];
        // if (((int)data["err"]) == 0)
        //if ( Convert.ToInt32(data["err"]) == 0)
        //{
               // Destroy(gameObject);
        //}
    }

    public void OnLoginRegistClick()
    {
        loginPage.SetActive(false);
        registPage.SetActive(true);
    }

    public void OnRegistClick()
    {
        //if (!ApplicationSettings.Instance.isNewNetwork)
        //{
        //    return;
        //}
        if (string.IsNullOrEmpty(registAccountInput.text))
        {
            StartCoroutine(ShowTips("Account can not be empty!"));
            return;
        }
        else if (string.IsNullOrEmpty(registPasswordtInput[0].text) || string.IsNullOrEmpty(registPasswordtInput[1].text))
        {
            StartCoroutine(ShowTips("Password can not be empty!"));
            return;
        }
        else if (registPasswordtInput[0].text != registPasswordtInput[1].text)
        {
            StartCoroutine(ShowTips("Password not the same!"));
            return;
        }

        Dictionary<string, string> registerDict = new Dictionary<string, string>();
        registerDict["user_name"] = registAccountInput.text;
        registerDict["user_pwd"] = registPasswordtInput[0].text;
        registerDict["invite_code"] = registCodeInput.text;
        account = registAccountInput.text;
        password = registPasswordtInput[0].text;
        StartCoroutine(HttpPost(_serverAddress, "/register_user", registerDict, HandleRegisterUserResponse));
    }

    public void OnReturnClick()
    {
        loginPage.SetActive(true);
        registPage.SetActive(false);
    }

    public void OnCloseClick()
    {
        Application.Quit();
    }

    void HandleRegisterUserResponse(string response)
    {
        if (string.IsNullOrEmpty(response))
            return;
        AccountLoginRegistRespone accountLoginRegistRespone = JsonUtility.FromJson<AccountLoginRegistRespone>(response);
        if (accountLoginRegistRespone.err == 0)
        {
            //succeed
            accountInput.text = account;
            loginPage.SetActive(true);
            registPage.SetActive(false);
            Debug.LogError("AccountRegistSucceed");
            StartCoroutine(ShowTips("Account registration successful, please log in."));
        }
        else
        {
            StartCoroutine(ShowTips($"{accountLoginRegistRespone.msg}"));
            //error
            password = "";
            registPasswordtInput[0].text = password;
            registPasswordtInput[1].text = password;
            Debug.LogError("AccountRegistFail");
        }
    }

    IEnumerator ShowTips(string str)
    {
        timers = 3;
        tipsText.text = str;
        tips.gameObject.SetActive(true);
        while (timers > 0)
        {
            timers -= 1;
            yield return new WaitForSeconds(1);
        }
        tips.gameObject.SetActive(false);
        tipsText.text = "";
    }

    IEnumerator HttpPost(string url, string method, Dictionary<string, string> post_param, System.Action<string> callback)
    {
        url = url.Trim();

        TestManager.Instance.SetTextServer(url);

        // 创建一个表单
        JSONNode jsonNode = JSONNode.Parse("{}");

        foreach (var kvp in post_param)
        {
            jsonNode[kvp.Key] = kvp.Value;
        }
        string jsonBody = jsonNode.ToString();

        AesManager aesManager = AesManager.Instance;
        jsonBody = aesManager.TryLocalEncrypt(jsonBody);

        // 创建 UnityWebRequest 对象
        using (UnityWebRequest www = UnityWebRequest.Post(url + method, "POST"))
        {
            /*byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            www.uploadHandler = new UploadHandlerRaw(jsonBytes);
            www.downloadHandler = new DownloadHandlerBuffer();

            www.SetRequestHeader("Content-Type", "application/json");*/


            byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            www.uploadHandler = new UploadHandlerRaw(jsonBytes);
            www.uploadHandler.contentType = "text/plain";
            www.downloadHandler = new DownloadHandlerBuffer();


            // 发送请求
            yield return www.SendWebRequest();

            // 检查是否有错误发生
            if (www.isNetworkError || www.isHttpError)
            {
                StartCoroutine(ShowTips($"{www.error}"));
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                // 请求成功，处理返回的数据
                string jsonResponse = www.downloadHandler.text;
                // Debug.LogError($"MyJson:{jsonResponse}");
                jsonResponse = AesManager.Instance.TryLocalDecrypt(jsonResponse);//使用local的key和iv解包
                Debug.Log("Response: " + jsonResponse);
                callback?.Invoke(jsonResponse); // 调用回调函数，传递响应数据
            }
        }
    }

    public struct LoginExtraData
    {
        public string login_token;
    }

    public struct AccountLoginRegistRespone
    {
        public int err;
        public string msg;
    }
    public struct AccountLoginRespone
    {
        public int err;
        public string msg;
        public string token_id;
        public string logic_ip;
    }

    public struct DeviceAccountRespone
    {
        public int err;
        public string msg;
        public string user_name;
        public string user_pwd;
    }

}
