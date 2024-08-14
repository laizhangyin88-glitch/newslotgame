using BagelCode;
using ParadoxNotion;
using SimpleJSON;
using SlotMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ResetPassword : MonoBehaviour
{
    private InputField oldInput;
    private InputField newInput;
    private InputField reEnterInput;
    private Text description;
    private PIDButton confirmBtn;

    private void Awake()
    {
        oldInput = transform.Find("Anchor/OLD/OLD").GetComponent<InputField>();
        newInput = transform.Find("Anchor/NEW/NEW").GetComponent<InputField>();
        reEnterInput = transform.Find("Anchor/ReEnter/ReEnter").GetComponent<InputField>();
        description = transform.Find("Anchor/Description").GetComponent<Text>();
        confirmBtn = transform.Find("Anchor/ConfirmBtn").GetComponent<PIDButton>();
    }

    private void Start()
    {
        description.text = "New Password must be 6 or more characters, including one letter and on number.";
    }



    public void OnConfirmClick()
    {
        if (string.IsNullOrEmpty(oldInput.text))
        {
            description.text = "Old password can not be empty.";
            return;
        }
        if (string.IsNullOrEmpty(newInput.text))
        {
            description.text = "New password can not be empty.";
            return;
        }
        if (string.IsNullOrEmpty(reEnterInput.text) || newInput.text != reEnterInput.text)
        {
            description.text = "The passwords entered twice are inconsistent.";
            return;
        }
        if (oldInput.text == newInput.text)
        {
            description.text = "New password must different from old.";
            return;
        }


        Dictionary<string, string> paramsDic = new Dictionary<string, string>
        {
            { "user_name", MainBlackboard.Get().GetValue<string>("account") },
            { "user_pwd", oldInput.text },
            { "new_user_pwd", newInput.text }
        };



        string autoUrl = TestManager.Instance.getAutoUrl();
        if (string.IsNullOrEmpty(autoUrl))
            autoUrl = ApplicationSettings.Instance.autoUrl;

        Debug.LogWarning(autoUrl);

        StartCoroutine(WWWGet(
            autoUrl,
            (address, err) =>
            {
                if (address == null)
                {
                    string errMsg = $"【ERR】：自动获取网络地址失败：{err}";
                    Debug.LogError(errMsg);
                    return;
                }
                address = "http://" + address;

                StartCoroutine(HttpPost(address, "/reset_passwd", paramsDic, ShowErrorDescription));
            }
        ));
    }

    public IEnumerator WWWGet(string url, Action<string, string> cb)
    {

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();
            string err = null;
            string addr = null;
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError("Error: " + www.error);
                err = www.error;
            }
            else
            {
                addr = www.downloadHandler.text;
            }
            if (cb != null) cb(addr, err);
        }
    }

    public void OnCloseBtnClick()
    {
        EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT,
                new EventData(MetaEventDefine.ON_LEAVE_RESET_PASSWORD));
        Destroy(gameObject);
    }

    private void ShowErrorDescription(string str)
    {
        JSONNode jsonNode = JSONNode.Parse(str);
        description.text = jsonNode["msg"];
        if (jsonNode["msg"] == "success")
            StartCoroutine(DelayClose());
    }

    private IEnumerator DelayClose()
    {
        confirmBtn.interactable = false;
        yield return new WaitForSeconds(3);
        OnCloseBtnClick();
    }

    private IEnumerator HttpPost(string url, string method, Dictionary<string, string> post_param, System.Action<string> callback)
    {
        url = url.Trim();

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

            byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            www.uploadHandler = new UploadHandlerRaw(jsonBytes);
            www.uploadHandler.contentType = "text/plain";
            www.downloadHandler = new DownloadHandlerBuffer();

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
                jsonResponse = AesManager.Instance.TryLocalDecrypt(jsonResponse);//使用local的key和iv解包
                Debug.Log("Response: " + jsonResponse);
                callback?.Invoke(jsonResponse); // 调用回调函数，传递响应数据
            }
        }
    }
}
