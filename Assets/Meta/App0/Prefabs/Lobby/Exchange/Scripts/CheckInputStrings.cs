using BagelCode;
using GameUtil;
using ParadoxNotion;
using SlotMaker;
using SlotMaker.Keno;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class CheckInputStrings : MonoBehaviour
{
    private string inputStrings;

    private List<string> checkStringsList = new List<string>();


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
        startInput = false;
        if(outCreditRate <= 0)
        {
            outCreditRate = 100;///暂时写死
            
        }
        isInUse = false;
        _clearInterval = clearInterval;
    }

    private void Update()
    {
        lobbyController = FindObjectOfType<LobbyController>();
        if(lobbyController == null)
        {
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
                    isCheckInput = true;
                }
            }
            if (isCheckInput)
            {
                _InputField.ActivateInputField();
            }
        }
    }

    //private void OnGUI()
    //{
    //    CheckInput();
    //}

    private void CheckInput()
    {
        AccountLoginViewNew accountLoginViewNew = GameObject.FindObjectOfType<AccountLoginViewNew>();
        if(accountLoginViewNew != null)
        {
            isInUse = false;
            checkStringsList.Clear();
            return;
        }
        _Event = Event.current;
        if (_Event != null && _Event.isKey && Input.anyKeyDown && _Event.keyCode != KeyCode.None)
        {
            startInput = true;
            inputStrings = "" + _Event.keyCode.ToString().ToLower();
            checkStringsList.Add(inputStrings);
            inputValue = String.Join("", checkStringsList);
            //temp = temp.Replace("rightshiftsemicolon", ":");
            //temp = temp.Replace("alpha", "");
            //temp = temp.Replace("minus", "-");
            //temp = temp.Replace("rightshift7", "&");
            //temp = temp.Replace("&qrcodeend&", "&QRCodeEnd&");
        }
        if ((_clearInterval -= Time.deltaTime) <= 0 && checkStringsList.Count > 0)
        {
            checkStringsList.Clear();
            _clearInterval = clearInterval;
            isInUse = false;
        }
        if (startInput)
        {
            if((_interval -= Time.deltaTime) < 0)
            {
                _interval = 3;
                startInput = false;
                MatchInput(inputValue);
            }
        }
    }

    private bool MatchInput(string input)
    {

        //input = input.Replace("rightshiftsemicolon", ":");
        //input = input.Replace("rightshiftminus", "_");
        //input = input.Replace("alpha", "");
        //input = input.Replace("minus", "-");
        //input = input.Replace("rightshift7rightshift7rightshiftqrightshiftqrightshiftrrightshiftrrightshiftcrightshiftcooddeerightshifterightshiftennddrightshift7rightshift7", "&QRCodeEnd&");
        //Debug.LogError(input);
        //input = RemoveConsecutiveDuplicates(input);
        //Debug.LogError(input);
        Match match = Regex.Match(input, patternBank);
        if (match.Success)
        {
            string result = match.Groups[1].Value;
            Debug.LogError("bank:" + result);
            ShowBankPopup("bank:" + result);
            checkStringsList.Clear();
            _clearInterval = clearInterval;
            return true;
        }
        match = Regex.Match(input, patternQRCode);
        if (match.Success)
        {
            string result = match.Groups[1].Value;
            Debug.LogError("qr_code:" + result);
            //ShowQRCodePopup("qr_code:" + result);
            CheckQRCode("qr_code:" + result);
            checkStringsList.Clear();
            _clearInterval = clearInterval;
            return true;
        }
        input = "";
        inputValue = "";
        return false;
    }

    static string RemoveConsecutiveDuplicates(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        StringBuilder sb = new StringBuilder();
        char currentChar = input[0];
        int consecutiveCount = 1;

        sb.Append(currentChar); // 添加第一个字符  

        for (int i = 1; i < input.Length; i++)
        {
            if (input[i] == currentChar)
            {
                consecutiveCount++;

                // 仅在计数为奇数时添加字符，以跳过每对中的第二个字符  
                if (consecutiveCount % 2 != 0)
                {
                    sb.Append(input[i]);
                }
            }
            else
            {
                currentChar = input[i];
                consecutiveCount = 1;
                sb.Append(currentChar); // 添加新字符  
            }
        }

        return sb.ToString();
    }

    #region  使用银行凭证代码
    private void ShowBankPopup(string bankInfo)
    {
        if (isInUse) return;
        isInUse = true;
        ErrorPopupInfo info = new ErrorPopupInfo();
        info.type = ErrorPopupType.YesNo;
        info.text = "<size=32>Do you want to use this QR Bank Code?</size>";
        info.buttonText1 = "Confirm";
        info.buttonText2 = "Cancle";
        info.callback1 = delegate
        {
            ConfirmBankInfo(bankInfo);
        };
        info.callback2 += delegate
        {
            isInUse = false;
        };
        ErrorPopupHandler.Instance.OpenError(info);
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
                isInUse = false;
                ShowErrorPopup("Bank Code has been use");
            }
        },
        (error) =>
        {
            CloseWaitView();
            isInUse = false;
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
            UseBankInfo(bankInfo);
        };
        info.callback2 += delegate
        {
            isInUse = false;
        };
        ErrorPopupHandler.Instance.OpenError(info);
    }

    private void UseBankInfo(string bankInfo)
    {
        ShowWaitView();
        Dictionary<string, object> req = new Dictionary<string, object>
        {
            {"bank_order_id", bankInfo},
        };
        NetManager.Instance.Post(RPCName.agent_check_bank_order, req, (res) =>
        {
            globalStore.newCredit = res["balance"].AsLong;
            BlackboardQueryUtils.SetMyCredit(globalStore.newCredit);
            MessageDispatcher.Dispatch("OnCreditEvent", new EventData<bool>("UpdateNaviCredit", true));
            isInUse = false;
            this.DelayAction(2f, () =>
            {
                CloseWaitView();
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
        if (isInUse) return;
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
            ShowQRCodePopup(QRCodeInfo, score);
        },
        (error) =>
        {
            CloseWaitView();
            isInUse = false;
            ShowErrorPopup(error.error);
        });
    }


    private void ShowQRCodePopup(string QRCodeInfo, long score)
    {
        ErrorPopupInfo info = new ErrorPopupInfo();
        info.type = ErrorPopupType.YesNo;
        info.text = "<size=32>Do you want to use this QR code?</size>";
        info.buttonText1 = "Confirm";
        info.buttonText2 = "Cancle";
        info.callback1 = delegate
        {
            ConfirmQRCodeInfo(QRCodeInfo, score);
        };
        info.callback2 += delegate
        {
            isInUse = false;
        };
        ErrorPopupHandler.Instance.OpenError(info);
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
            Debug.LogError("print .................");
            ConfirmPrintBankPopup(QRCodeInfo, score);
        };
        info.callback2 = delegate
        {
            Debug.LogError("add score.................");
            ConfirmAddScore(QRCodeInfo, score);
        };
        info.callbackX += delegate
        {
            isInUse = false;
        };
        ErrorPopupHandler.Instance.OpenError(info);
    }

    private void ConfirmPrintBankPopup(string QRCodeInfo, long score)
    {
        ErrorPopupInfo info = new ErrorPopupInfo();
        info.type = ErrorPopupType.YesNo;
        info.text = $"<size=32>Do you want to print {score} score Bank QR Code ?</size>";
        info.buttonText1 = "Confirm";
        info.buttonText2 = "Cancle";
        info.callback1 = delegate
        {
            PrintBankQRCode(QRCodeInfo, score);
        };
        info.callback2 = delegate
        {
            isInUse=false;
        };
        ErrorPopupHandler.Instance.OpenError(info);
    }

    private void PrintBankQRCode(string QRCodeInfo, long score)
    {
        ShowWaitView();
        Dictionary<string, object> req = new Dictionary<string, object>
        {
            {"qr_code", QRCodeInfo},
        };
        NetManager.Instance.Post(RPCName.agent_check_qr_code_print_order, req, (res) =>
        {
            string bankCode = res["bank_order_id"];
            isInUse = false;
            long outCredite = score / outCreditRate; 
            TicketInfo ticketInfo = new TicketInfo()
            {
                BankInfo = bankCode,
                Money = outCredite,
                OrderText = "Test Order",
                TicketType = "Money Type",
            };
            PrinterController.Instance.PrintTicket(ticketInfo);
            CloseWaitView();
            ShowSuccessResult("Success !");
            /////打印银行凭证
        },
        (error) =>
        {
            isInUse = false;
            ShowErrorPopup(error.error);
            //SaveInfo();
        });
    }

    private void ConfirmAddScore(string QRCodeInfo, long score)
    {
        ErrorPopupInfo info = new ErrorPopupInfo();
        info.type = ErrorPopupType.YesNo;
        info.text = $"<size=32>Do you want to Add {score} Score to account ?</size>";
        info.buttonText1 = "Confirm";
        info.buttonText2 = "Cancle";
        info.callback1 = delegate
        {
            UseQRCodeAddScore(QRCodeInfo);
        };
        info.callback2 = delegate
        {
            isInUse = false;
        };
        ErrorPopupHandler.Instance.OpenError(info);
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
            CloseWaitView();
            UseBankInfo(bankCode);
        },
        (error) =>
        {
            ShowErrorPopup(error.error);
            //SaveInfo();
        });
    }

    #endregion




    private void ShowWaitView()
    {
        isShowWaitView = true;
        var prefab = AssetBundleManager.LoadAsset<GameObject>("lobby0", "WaitForView");
        GameObject temp = Instantiate(prefab) as GameObject;
        temp.transform.SetParent(PopupManager.Instance.transform, false);
        PopupManager.Instance.Open(temp);
    }

    private void CloseWaitView()
    {
        isShowWaitView= false;
        WaitForViewController.Instance.Close();
    } 

    private void ShowErrorPopup(string error)
    {
        isInUse = false;
        error = error.Replace("\"", "");
        ErrorPopupInfo info = new ErrorPopupInfo();
        info.type = ErrorPopupType.OK;
        info.text = $"<size=32>{error}</size>";
        info.buttonText1 = "OK";
        ErrorPopupHandler.Instance.OpenError(info);
    }
    private void ShowSuccessResult(string value)
    {
        isInUse = false;
        ErrorPopupInfo info = new ErrorPopupInfo();
        info.type = ErrorPopupType.OK;
        info.text = "<size=32>" + value + "</size>";
        info.buttonText1 = "OK";
        ErrorPopupHandler.Instance.OpenError(info);
    }

}
