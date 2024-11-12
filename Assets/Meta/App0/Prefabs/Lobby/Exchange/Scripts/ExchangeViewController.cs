using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using BagelCode;
using GameUtil;
using System.Text.RegularExpressions;
using System.Linq;
using ParadoxNotion;

public class QRCodeInfo
{
    public string QR_CodeID;
}

public class BankInfo
{
    public string BankInfoID;
}


public class ExchangeViewController : MonoBehaviour
{
    private RawImage QRCodeRawImage; // 绘制好的二维码
    private string CurrentQRCodeInfo = "";
    private string CurrentBankInfo = "";
    private List<GameObject> btnsList = new List<GameObject>();
    private TextMeshProUGUI value_txt;
    private Button btnClose;
    private Button CloseButtonQR;
    private BarcodeWriter barcodeWriter;
    private Button QuickButton;
    private Transform content2;
    public int outCreditRate;
    private string userId;
    private Dictionary<string, long> QRCodeInfoDicti = new Dictionary<string, long>();
    private float tempInterval = 0;
    private Transform ContentQRCode;
    private Transform ContentPrint;
    private Transform ScrollViewQRCode;
    private TextMeshProUGUI coinValueTxt;

    private List<Button> quickExchangeList = new List<Button>();
    private List<long> quickExchanges = new List<long>();

    private Button SureBtn;
    private Button valueBtn;

    private Transform SoftKeyboard;

    private bool isMachine = false;
    private Button SoftBtnClose;

    private TextMeshProUGUI soft_value_txt;
    private TextMeshProUGUI RateTxt;

    private void Start()
    {
        isMachine = ApplicationSettings.Instance.isMachine;

        MessageDispatcher.Register(EVTType.ON_CREDIT_EVENT, UpdateCredit);

        SureBtn = transform.Find("content/SureBtn").GetComponent<Button>();
        SureBtn.onClick.AddListener(OnClickSureBtn);
        userId = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "/me/userId").value;
        outCreditRate = BlackboardUtils.GetOrCreateVariable<int>(MainBlackboard.Get(), "OutCreditRate").value;
        content2 = transform.Find("content2");
        content2.gameObject.SetActive(false);
        QRCodeRawImage = transform.Find("content/QR/RawImage").GetComponent<RawImage>();
        CloseButtonQR = QRCodeRawImage.transform.GetChild(0).GetComponent<Button>();
        CloseButtonQR.onClick.AddListener(() => { QRCodeRawImage.transform.parent.gameObject.SetActive(false); });
        QRCodeRawImage.transform.parent.gameObject.SetActive(false);
        SoftKeyboard = transform.Find("SoftKeyboard");
        SoftKeyboard.gameObject.SetActive(false);
        SoftBtnClose = SoftKeyboard.Find("SoftBtnClose").GetComponent<Button>();
        SoftBtnClose.onClick.AddListener(() => {  SoftKeyboard.gameObject.SetActive(false); });
        soft_value_txt = SoftKeyboard.transform.Find("Image/soft_value_txt").GetComponent<TextMeshProUGUI>();
        Transform btnParent = transform.Find("SoftKeyboard/btns");
        value_txt = transform.Find("content/Image/value_txt").GetComponent<TextMeshProUGUI>();
        valueBtn = transform.Find("content/Image").GetComponent<Button>();
        valueBtn.onClick.AddListener(OnClickValueBtn);
        value_txt.text = "";
        btnClose = transform.Find("Base/BtnClose").GetComponent<Button>();
        btnClose.onClick.AddListener(OnClickBtnClose);
        InitBtnsList(btnParent);
        QuickButton = transform.Find("content/QuickButton").GetComponent<Button>();
        QuickButton.gameObject.SetActive(false);
        ContentQRCode = transform.Find("content/ScrollViewQRCode/GameObject/ContentQRCode");
        ScrollViewQRCode = transform.Find("content/parent/ScrollViewQRCode");
        coinValueTxt = transform.Find("content/coin/coinValue").GetComponent<TextMeshProUGUI>();

        coinValueTxt.text = BlackboardUtils.FindVariable<long>(null, "/me/credit").value.ToString("N0");
        RateTxt = transform.Find("content/RateTxt").GetComponent<TextMeshProUGUI>();
        RateTxt.text = string.Format("Redeem Rate : 1:{0}", outCreditRate);

        InitQuickQRCodeList();

    }

    private void UpdateCredit(EventData eventData)
    {
        if(eventData != null && eventData.name == "UpdateNaviCredit")
        {
            coinValueTxt.text = BlackboardUtils.FindVariable<long>(null, "/me/credit").value.ToString("N0");
        }
    }

    private void InitQuickQRCodeList()
    {
        quickExchanges = BlackboardUtils.GetOrCreateVariable<List<long>>(MainBlackboard.Get(), "AmountsPresetList").value;
        if (quickExchanges.Count > 0)
        {
            for (int i = 0; i < quickExchanges.Count; i++)
            {
                long value = quickExchanges[i];
                if (value >= outCreditRate)
                {
                    int index = i + 1;
                    Button go = Instantiate(QuickButton) as Button;
                    go.gameObject.SetActive(true);
                    TextMeshProUGUI textMeshProUGUI = go.transform.Find("Text").GetComponent<TextMeshProUGUI>();
                    textMeshProUGUI.text = value.ToString();
                    go.transform.SetParent(ContentQRCode.transform, false);
                    go.onClick.AddListener(() =>
                    {
                        OnClickQuickPrint(index);
                    });
                    quickExchangeList.Add(go);
                }
            }
        }
    }



    private void OnClickValueBtn()
    {
        SoftKeyboard.gameObject.SetActive(true);
        soft_value_txt.text = value_txt.text;
    }
    private void OnClickQuickPrint(int index)
    {
        long credit = quickExchanges[index - 1];
        string temp = (credit / outCreditRate).ToString("D");
        long result = long.Parse(temp) * outCreditRate;
        value_txt.text = result.ToString("N0");
    }

    private void OnClickSureBtn()
    {
        if (!string.IsNullOrEmpty(value_txt.text))
        {
            string temp = value_txt.text.Replace(",", "");
            long result = long.Parse(temp);
            long myCredit = BlackboardUtils.FindVariable<long>(null, "/me/credit").value;
            if (result <= myCredit)
            {
                if (isMachine)
                {
                    ConfirmPrintBankPopup(result);
                }
                else
                {
                    CreateQRCode(result);
                }
            }
            else
            {
                ShowPopup("not enough money");
            }
        }
    }

    private void PrintQRCodeInfo(string info, string orderInfo)
    {
        string temp = value_txt.text.Replace(",", "");
        long result = long.Parse(temp);
        long outCredite = result / outCreditRate; 
        TicketInfo ticketInfo = new TicketInfo()
        {
            BankInfo = info,
            Money = outCredite,
            OrderText = orderInfo,
            TicketType = "Money Type",
        };

        PrinterController.Instance.PrintTicket(ticketInfo);
    }
    /// <summary>
    /// 使用兑换二维码
    /// </summary>
    /// <param name="input"></param>
    private void UseExchangeQRCodeInfo(string input, long credit = 0)
    {
        if (!QRCodeInfoDicti.TryGetValue(input, out long value))
        {
            QRCodeInfoDicti.Add(input, credit);
        }
        content2.gameObject.SetActive(true);
        string temp = input.Replace("\"", "");
        Dictionary<string, object> req = new Dictionary<string, object>
        {
            {"qr_code", temp},
        };
        NetManager.Instance.Post(RPCName.agent_check_qr_code_print_order, req, (res) =>
        {
            CurrentBankInfo = res["bank_order_id"];
            string orderInfo = res["show_order_id"];
            PrintQRCodeInfo(CurrentBankInfo, orderInfo);
            this.DelayAction(15, () =>
            {
                ShowPopup("Success !");
                RemoveDictiElement();
                content2.gameObject.SetActive(false);
            });
        },
        (error) =>
        {
            RemoveDictiElement();
            ShowPopup(error.error);
            content2.gameObject.SetActive(false);
        });
    }
    private void OnClickBtnClose()
    {
        PopupManager.Instance.Close(this.gameObject);
        Destroy(this.gameObject);
    }

    private void InitBtnsList(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Button button = parent.GetChild(i).GetComponent<Button>();
            if (button != null)
            {
                TextMeshProUGUI text = button.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

                if (i == 9)
                {
                    text.text = "Delete";
                    button.onClick.AddListener(() =>
                    {
                        OnClickBtn(10);
                    });
                }
                else if (i == 10)
                {
                    text.text = "0";
                    button.onClick.AddListener(() =>
                    {
                        OnClickBtn(0);
                    });
                }
                else if (i == 11)
                {
                    text.text = "Clear All";
                    button.onClick.AddListener(() =>
                    {
                        OnClickClearAll();
                    });
                }
                else if(i == 12)
                {
                    text.text = "Confirm";
                    button.onClick.AddListener(() =>
                    {
                        OnClickConfirm();
                    });
                }
                else
                {
                    text.text = (i + 1).ToString();
                    int temp = i + 1;
                    button.onClick.AddListener(() =>
                    {
                        OnClickBtn(temp);
                    });
                }
            }
        }
    }

    private void OnClickClearAll()
    {
        soft_value_txt.text = "";
    }

    private void OnClickConfirm()
    {
        string temp = soft_value_txt.text.Replace(",", "");
        long value = long.Parse(temp);
        long result = long.Parse((value / outCreditRate).ToString("D")); 
        if(result == 0)
        {
            value_txt.text = "";
        }
        else
        {
            value_txt.text = (result * outCreditRate).ToString("N0");
        }
        
        SoftKeyboard.gameObject.SetActive(false);
    }

    private void OnClickBtn(int index)
    {
        switch (index)
        {
            case 0:
                OnClickZero();
                break;
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
                OnClickNumber(index);
                break;
            case 10:
                DeletedNumber();
                break;
        }
    }

    private void OnClickZero()
    {
        if (string.IsNullOrEmpty(soft_value_txt.text))
        {
             
        }
        else
        {
            if (soft_value_txt.text.Length >= 23) return;
            soft_value_txt.text += "0";
            string temp = soft_value_txt.text.Replace(",", "");
            long result = long.Parse(temp);
            soft_value_txt.text = result.ToString("N0");
        }
    }
    /// <summary>
    /// 生成下分的二维码
    /// </summary>
    private void CreateQRCode(long credit, bool isPrint = false)
    {
        content2.gameObject.SetActive(true);
        Dictionary<string, object> req = new Dictionary<string, object>
            {
                {"outcredit", credit},
            };
        NetManager.Instance.Post(RPCName.agent_build_qr_code, req, (res) =>
        {
            CurrentQRCodeInfo = res["qr_code_key"];
            if (!isPrint)
            {
                DrawQRCode(CurrentQRCodeInfo);
                QRCodeRawImage.transform.parent.gameObject.SetActive(true);
                content2.gameObject.SetActive(false);
            }
            else
            {
                if (!QRCodeInfoDicti.ContainsKey(CurrentQRCodeInfo))
                {
                    QRCodeInfoDicti.Add(CurrentQRCodeInfo, credit);
                    SaveInfo();
                }
                UseExchangeQRCodeInfo(CurrentQRCodeInfo, credit);
            }

        },
        (error) =>
        {
            Debug.LogError(error);
            ShowPopup(error.error);
        });

    }

    private void OnClickNumber(int index)
    {
        if (soft_value_txt.text.Length >= 23) return; 
        if (!string.IsNullOrEmpty(soft_value_txt.text))
        {
            string value = soft_value_txt.text.Replace(",", "");
            string result = value + index.ToString();
            long longResult = long.Parse(result);
            soft_value_txt.text = longResult.ToString("N0");
        }
        else
        {
            soft_value_txt.text += index.ToString();
        }
    }
    private void DeletedNumber()
    {
        if (!string.IsNullOrEmpty(soft_value_txt.text))
        {
            if (soft_value_txt.text.Length == 1)
            {
                soft_value_txt.text = "";
            }
            else if (soft_value_txt.text.Length > 1)
            {
                string temp = soft_value_txt.text.Replace(",", "");
                string temp1 = temp.Substring(0, temp.Length - 1);
                long result = long.Parse(temp1);
                soft_value_txt.text = result.ToString("N0");
            }
        }
    }

    private void OnDestroy()
    {
        if (btnsList != null && btnsList.Count > 0)
        {
            for (int i = 0; i < btnsList.Count; i++)
            {
                Button btn = btnsList[i].GetComponent<Button>();
                btn.onClick.RemoveAllListeners();
            }
            btnsList.Clear();
            btnsList = null;
        }
        if (quickExchangeList != null && quickExchangeList.Count > 0)
        {
            for (int i = 0; i < quickExchangeList.Count; i++)
            {
                Button btn = quickExchangeList[i].GetComponent<Button>();
                btn.onClick.RemoveAllListeners();
            }
            quickExchangeList.Clear();
            quickExchangeList = null;
        }
        btnClose.onClick.RemoveAllListeners();
        QRCodeInfoDicti.Clear();
    }
    private Color32[] GenerateQRCode(string formatStr, int width, int height)
    {
        ZXing.QrCode.QrCodeEncodingOptions options = new ZXing.QrCode.QrCodeEncodingOptions
        {
            CharacterSet = "UTF-8",
            Width = width,
            Height = height,
            Margin = 1
        };

        barcodeWriter = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = options
        };
        return barcodeWriter.Write(formatStr);
    }

    private Texture2D ShowQRCode(string str, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        Color32[] colors = GenerateQRCode(str, width, height);
        texture.SetPixels32(colors);
        texture.Apply();
        return texture;
    }

    private void DrawQRCode(string formatStr)
    {
        Texture2D texture = ShowQRCode(formatStr + "&QRCodeEnd&", 256, 256);
        QRCodeRawImage.texture = texture;
    }

    private void ShowPopup(string error)
    {
        ErrorPopupInfo info = new ErrorPopupInfo();
        info.type = ErrorPopupType.OK;
        info.text = $"<size=32>{error}</size>";
        info.buttonText1 = "OK";
        info.callback1 += delegate
        {
            content2.gameObject.SetActive(false);
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
        Debug.LogError(" save success : temp1" + temp1);
        SQLiteManager.Instance.SetString(userId + "QRCODEINFOLIST", temp1);
    }

    private void ConfirmPrintBankPopup(long score)
    {
        content2.gameObject.SetActive(true);
        ErrorPopupInfo info = new ErrorPopupInfo();
        info.type = ErrorPopupType.YesNo;
        info.text = $"<size=32>Do you want to print {score.ToString("N0")} score Bank QR Code ?</size>";
        info.buttonText1 = "Confirm";
        info.buttonText2 = "Cancle";
        info.callback1 = delegate
        {
            string temp = value_txt.text.Replace(",", "");
            long result = long.Parse(temp);
            CreateQRCode(result, true);
        };
        info.callback2 += delegate
        {
            content2.gameObject.SetActive(false);
        };
        ErrorPopupHandler.Instance.OpenError(info);
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

}
