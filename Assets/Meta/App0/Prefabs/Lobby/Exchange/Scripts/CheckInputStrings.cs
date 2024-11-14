using BagelCode;
using GameUtil;
using ParadoxNotion;
using SBoxApi;
using SlotMaker;
using SlotMaker.Keno;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheckInputStrings : MonoBehaviour
{
    private string inputStrings;
    private Dictionary<string, long> QRCodeInfoDicti = new Dictionary<string, long>();
    private Dictionary<string, long> BankInfoDicti = new Dictionary<string, long>();


    public float clearInterval = 60;
    private float _clearInterval = 0;

    string patternBank = @"bank:([^&]*)&QRCodeEnd&"; 
    string patternQRCode = @"qr_code:([^&]*)&QRCodeEnd&";

    private bool isInUse = false;
    private int outCreditRate;
    private Event _Event;

    private InputField _InputField;

    private float _interval;
    private bool startInput = false;

    private string inputValue = "";

    private bool isCheckInput = false;

    private LobbyController lobbyController;

    private bool isShowWaitView = false;

    private bool isInitSQLite = false;

    private bool isSendReconnect = false;
    private string userId;
    private void Start()
    {
#if !UNITY_EDITOR
        if (!ApplicationSettings.Instance.isMachine)
        {
            gameObject.SetActive(false);
            return;
        }
#endif
        _interval = 2; 
        inputValue = "";
        isShowWaitView = false;
        isCheckInput = true;
        outCreditRate = BlackboardUtils.GetOrCreateVariable<int>(MainBlackboard.Get(), "OutCreditRate").value;
        _InputField = FindObjectOfType<CheckInputField>().GetComponent<InputField>();
        Debug.Log(_InputField.name);
        MessageDispatcher.Register("OnCustomEvent", OnListenerCloseXEvent);
        startInput = false;
        isInUse = false;
        _clearInterval = clearInterval;
    }

    private void OnListenerCloseXEvent(EventData eventData)
    {
        if(eventData.name == "OnResetIsInUse")
        {
            if(eventData.value != null)
            {
                isInUse = (bool)eventData.value;
            }
        }
    }

    private void Update()
    {
        lobbyController = FindObjectOfType<LobbyController>();
        if(lobbyController == null)
        {
            _InputField.text = "";
            isCheckInput = true;
            isInUse = false;
            _InputField.DeactivateInputField();
            return;
        }
        else
        {
            outCreditRate = BlackboardUtils.GetOrCreateVariable<int>(MainBlackboard.Get(), "OutCreditRate").value;
            InitSQLiteData();
        }
        if (isInUse)
        {
            _InputField.text = "";
            _InputField.DeactivateInputField();
            return;
        }
        if (_InputField != null)
        {
            if (!string.IsNullOrEmpty(_InputField.text))
            {
                isCheckInput = false;
                bool result = MatchInput(_InputField.text);
                if (result)
                {
                    _InputField.text = "";
                    _InputField.DeactivateInputField();
                    isCheckInput = true;
                }
            }
            if (isCheckInput)
            {
                _InputField.ActivateInputField();
            }
        }
        CheckQRCodeInfo();
    }

    private bool MatchInput(string input)
    {
        Match match = Regex.Match(input, patternBank);
        if (match.Success)
        {
            string result = match.Groups[1].Value;
            //正在加载界面，不弹窗
            GameLoadingSceneController gameLoadingSceneController = FindObjectOfType<GameLoadingSceneController>();
            if(gameLoadingSceneController != null)
            {
                return false;
            }
            ShowBankPopup("bank:" + result);
            _clearInterval = clearInterval;
            return true;
        }
        match = Regex.Match(input, patternQRCode);
        if (match.Success)
        {
            string result = match.Groups[1].Value;
            Debug.LogError("qr_code:" + result);
            GameLoadingSceneController gameLoadingSceneController = FindObjectOfType<GameLoadingSceneController>();
            if (gameLoadingSceneController != null)
            {
                return false;
            }
            CheckQRCode("qr_code:" + result);
            _clearInterval = clearInterval;
            return true;
        }
        input = "";
        inputValue = "";
        return false;
    }

    #region  使用银行凭证代码
    private void ShowBankPopup(string bankInfo)
    {
        if (isInUse)
        {
            _InputField.text = "";
            _InputField.DeactivateInputField();
            return;
        }
        isInUse = true;
        ConfirmBankInfo(bankInfo);
    }

    private void ConfirmBankInfo(string bankInfo)
    {
        ShowWaitView();
        Dictionary<string, object> req = new Dictionary<string, object>
        {
            {"bank_order_id", bankInfo},
        };
        NetManager.Instance.Post(RPCName.agent_query_bank_order, req, (res) =>
        {
            CloseWaitView();
            int state = (int)res["state"];
            if(state == 1)
            {
                long total_money = res["total_money"].AsLong;
                string temp = (total_money * outCreditRate).ToString("N0");
                ShowUseBankPopup(bankInfo, temp);
            }
            else
            {
                ShowErrorPopup("Bank Code has been use");
            }
        },
        (error) =>
        {
            CloseWaitView();
            ShowErrorPopup(error.error);
        });
    }

    private void ShowUseBankPopup(string bankInfo, string score)
    {
        ErrorPopupInfo info = new ErrorPopupInfo();
        info.type = ErrorPopupType.YesNo;
        info.text = "<size=32>Use this QR Bank Code can add " + score + " score to current account, Do you want to go on ?</size>";
        info.buttonText1 = "Confirm";
        info.buttonText2 = "Cancle";
        info.callback1 = delegate
        {
            ShowWaitView();
            UseBankInfo(bankInfo);
        };
        info.callback2 += delegate
        {
            isInUse = false;
        };
        ErrorPopupHandler.Instance.OpenError(info);
    }
    /// <summary>
    /// 使用银行凭证
    /// </summary>
    /// <param name="bankInfo"></param>
    private void UseBankInfo(string bankInfo)
    {
        Dictionary<string, object> req = new Dictionary<string, object>
        {
            {"bank_order_id", bankInfo},
        };
        NetManager.Instance.Post(RPCName.agent_check_bank_order, req, (res) =>
        {
            globalStore.newCredit = res["balance"].AsLong;
            BlackboardQueryUtils.SetMyCredit(globalStore.newCredit);
            MessageDispatcher.Dispatch("OnCreditEvent", new EventData<bool>("UpdateNaviCredit", true));
            this.DelayAction(2f, () =>
            {
                CloseWaitView();
                RemoveDictiElement();
                ShowSuccessResult("Success !");
            });
        },
        (error) =>
        {
            CloseWaitView();
            ShowErrorPopup(error.error);
        });
    }
    #endregion

    #region  使用积分兑换码
    private void CheckQRCode(string QRCodeInfo)
    {
        if (isInUse)
        {
            _InputField.text = "";
            _InputField.DeactivateInputField();
            return;
        }
        isInUse = true;
        ShowWaitView();
        Dictionary<string, object> req = new Dictionary<string, object>
        {
            {"qr_code", QRCodeInfo},
        };
        NetManager.Instance.Post(RPCName.agent_query_qr_code, req, (res) =>
        {
            CloseWaitView();
            long score = res["outcredit"].AsLong;
            ConfirmQRCodeInfo(QRCodeInfo, score);
        },
        (error) =>
        {
            CloseWaitView();
            ShowErrorPopup(error.error);
        });
    }
    
    private void ConfirmQRCodeInfo(string QRCodeInfo, long score)
    {
        ErrorPopupInfo info = new ErrorPopupInfo();
        info.type = ErrorPopupType.YesNoClose;

        info.text = $"<size=32>How do you want to use this QR code whose score are {score.ToString("N0")}?</size>";
        info.buttonText1 = "Print Bank QR Code";
        info.buttonText2 = "Add Score this Account";
        info.useXButton = true;
        info.callback1 = delegate
        {
            if (!QRCodeInfoDicti.ContainsKey(QRCodeInfo))
            {
                QRCodeInfoDicti.Add(QRCodeInfo, score);
            }
            SaveInfo();
            PrintBankQRCode(QRCodeInfo, score);
        };
        info.callback2 = delegate
        {
            UseQRCodeAddScore(QRCodeInfo);
        };
        info.callbackX += delegate
        {
            MessageDispatcher.Dispatch("OnCustomEvent", new EventData<bool>("OnResetIsInUse", false));
        };
        ErrorPopupHandler.Instance.OpenError(info);
    }

    private void PrintBankQRCode(string QRCodeInfo, long score, bool isMoneyValue = false)
    {
        ShowWaitView();
        Dictionary<string, object> req = new Dictionary<string, object>
        {
            {"qr_code", QRCodeInfo},
        };
        NetManager.Instance.Post(RPCName.agent_check_qr_code_print_order, req, (res) =>
        {
            string bankCode = res["bank_order_id"];
            
            long outCredite = score / outCreditRate;
            string orderInfo = res["show_order_id"];
            if (isMoneyValue)
            {
                outCredite = score;
            }

            TicketInfo ticketInfo = new TicketInfo()
            {
                BankInfo = bankCode,
                Money = outCredite,
                OrderText = orderInfo,
                TicketType = "Money Type",
            };
            PrinterController.Instance.PrintTicket(ticketInfo);
#if UNITY_EDITOR
            string data = bankCode + ":" + outCredite;
            MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_PRINT_BANK_INFO, data);
#endif
            this.DelayAction(15, () => { 
                CloseWaitView();
                RemoveDictiElement();
                ShowSuccessResult("Success !");
                isSendReconnect = false;
            });
        },
        (error) =>
        {
            ShowErrorPopup(error.error);
        });
    }

    private void UseQRCodeAddScore(string QRCodeInfo)
    {
        ShowWaitView();
        Dictionary<string, object> req = new Dictionary<string, object>
        {
            {"qr_code", QRCodeInfo},
        };
        NetManager.Instance.Post(RPCName.agent_check_qr_code_print_order, req, (res) =>
        {
            string bankCode = res["bank_order_id"];
            UseBankInfo(bankCode);
        },
        (error) =>
        {
            ShowErrorPopup(error.error);
        });
    }
    #endregion
    private void ShowWaitView()
    {
        if (isShowWaitView) return;
        isShowWaitView = true;
        var prefab = AssetBundleManager.LoadAsset<GameObject>("lobby0", "WaitForView");
        GameObject temp = Instantiate(prefab) as GameObject;
        temp.transform.SetParent(PopupManager.Instance.transform, false);
        PopupManager.Instance.Open(temp);
    }

    private void CloseWaitView()
    {
        StartCoroutine(CheckWaitView());
    }

    private IEnumerator CheckWaitView()
    {
        WaitForViewController controller = FindObjectOfType<WaitForViewController>();
        yield return controller != null;
        MessageDispatcher.Dispatch(EVTType.ON_CONTENT_EVENT, new EventData("CloseWaitForView"));
        isShowWaitView = false;
    }

    private void ShowErrorPopup(string error)
    {
        error = error.Replace("\"", "");
        ErrorPopupInfo info = new ErrorPopupInfo();
        info.type = ErrorPopupType.OK;
        info.text = $"<size=32>{error}</size>";
        info.buttonText1 = "OK";
        info.callback1 += delegate
        {
            isInUse = false;
            RemoveDictiElement();
        };
        ErrorPopupHandler.Instance.OpenError(info);
    }
    private void ShowSuccessResult(string value)
    {
        ErrorPopupInfo info = new ErrorPopupInfo();
        info.type = ErrorPopupType.OK;
        info.text = "<size=32>" + value + "</size>";
        info.buttonText1 = "OK";
        info.callback1 += delegate
        {
            isInUse = false;
        };
        ErrorPopupHandler.Instance.OpenError(info);
    }
    private void SaveInfo()
    {
        string temp1 = "";
        foreach (var item in QRCodeInfoDicti)
        {
            string result = item.Key + "credit:" + item.Value + "####";
            temp1 += result;
        }
        if (string.IsNullOrEmpty(userId))
        {
            userId = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "/me/userId").value;
        }
        Debug.LogError(" save success : temp1" + temp1);
        SQLiteManager.Instance.SetString(userId + "QRCODEINFOLIST", temp1);
    }
    private void InitSQLiteData()
    {
        if (isInitSQLite) return;
        isInitSQLite = true;
        if (string.IsNullOrEmpty(userId))
        {
            userId = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "/me/userId").value;
        }
        string qrCode = SQLiteManager.Instance.GetString(userId + "QRCODEINFOLIST", "");
        if (!string.IsNullOrEmpty(qrCode))
        {
            string[] node = qrCode.Split("###".ToCharArray());
            for (global::System.Int32 i = 0; i < node.Length; i++)
            {
                string temp = node[i];
                if (!string.IsNullOrEmpty(temp))
                {
                    string[] result = Regex.Split(temp, "credit:");
                    QRCodeInfoDicti.Add(result[0], long.Parse(result[1]));
                }
            }
        }
    }
    private void CheckQRCodeInfo()
    {
        if (isInUse) return;
        if(isSendReconnect) return;
        isSendReconnect = true;
        if (QRCodeInfoDicti.Count > 0)
        {
            var temp = QRCodeInfoDicti.First();
            ReconnectPrinter(temp.Key);
        }
    }


    private void ReconnectPrinter(string QRcode)
    {
        ShowWaitView();
        Dictionary<string, object> req = new Dictionary<string, object>
        {
            {"qr_code", QRcode},
        };
        NetManager.Instance.Post(RPCName.agent_query_qr_code, req, (res) =>
        {
            long score = res["total_money"].AsLong;
            Debug.LogError(score);
            PrintBankQRCode(QRcode, score, true);
        },
        (error) =>
        {
            CloseWaitView();
            ShowErrorPopup(error.error);
        });
    }

    private void RemoveDictiElement()
    {
        if (QRCodeInfoDicti.Count > 0)
        {
            var temp = QRCodeInfoDicti.First();
            QRCodeInfoDicti.Remove(temp.Key);
            SaveInfo();
        }
    }

    private void OnDestroy()
    {
        MessageDispatcher.UnRegister("OnCustomEvent", OnListenerCloseXEvent);
    }
}
