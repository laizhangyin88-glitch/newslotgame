using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SimpleJSON;
using System.Security.Cryptography;
using System.Text;

public class NumericKeypadController : MonoBehaviour
{
    private TextMeshProUGUI _InputTxt;
    private string password;
    private Button confirmBtn;

    private void Start()
    {
        password = "";
        _InputTxt = transform.Find("content/bg/InputTxt").GetComponent<TextMeshProUGUI>();
        confirmBtn = transform.Find("content/BtnConfirm").GetComponent<Button>();
        _InputTxt.text = "";
        Transform btns = transform.Find("content/Btns");
        for (int i = 0; i < btns.childCount; i++)
        {
            Transform child = btns.GetChild(i);
            TextMeshProUGUI txt = child.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            Button button = child.GetComponent<Button>();
            int index = i + 1;
            txt.text = index.ToString();
            if(index == 10)
            {
                txt.text = "Cancel";
            }
            if(index == 11)
            {
                txt.text = "0";
            }
            if( index == 12)
            {
                txt.text = "Delete";
            }
            button.onClick.AddListener(() =>
            {
                OnClickButton(index);
            });
        }
        confirmBtn.onClick.AddListener(SendCheckPassword);
    }

    private void OnClickButton(int index)
    {
        switch (index)
        {
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
                InputNumber(index);
                break;
            case 10:
                InputCancle();
                break;
            case 11:
                InputNumber(0);
                break;
            case 12:
                InputDeleted();
                break;
        }
    }

    private void InputCancle()
    {
        Destroy(BackgroundManagerMainViewController.Instance.gameObject);
        Destroy(gameObject);
    }

    private void InputDeleted()
    {
        if (password.Length > 0)
        {
            string temp1 = password.Substring(0, password.Length - 1);
            password = temp1;
            _InputTxt.text = temp1;
        }
    }

    private void InputNumber(int index)
    {
        password += index.ToString();
        _InputTxt.text = password;
        //if(password.Length >= 8)
        //{
        //    //Destroy(gameObject);
        //    SendCheckPassword();
        //}
    }


    private void SendCheckPassword()
    {
        if (password.Length >= 6)
        {
            string passwordMD5 = ComputeMD5Hash(password);
            JSONNode data = JSONNode.Parse("{}");
            data["method"] = "login";
            JSONNode param = JSONNode.Parse("{}");
            param["password"] = passwordMD5;
            data["params"] = param;
            //JSONNode data = JSONNode.Parse("{\"method\":\"test\",\"params\":{\"p1\":\"v1\",\"p2\": \"v2\",\"p3\": \"v3\"}}");

            NetManager.Instance.Post(RPCName.user_php_interface, data, (res) =>
            {
                Debug.LogError(res);
            },
            (error) =>
            {
                Debug.LogError(error);
            });
        }
    }

    private string ComputeMD5Hash(string rawData)
    {
        // Create a new instance of MD5CryptoServiceProvider.
        using (MD5 md5 = MD5.Create())
        {
            // ComputeHash - returns byte array  
            byte[] inputBytes = Encoding.ASCII.GetBytes(rawData);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert byte array to a string   
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString().ToLower();
        }
    }
}
