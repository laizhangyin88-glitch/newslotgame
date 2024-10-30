using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using BagelCode;
using GameUtil;

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
    public RawImage QRCodeRawImage; // 绘制好的二维码
    private string CurrentQRCodeInfo = "";
    private string CurrentBankInfo = "";
    private List<GameObject> btnsList = new List<GameObject>();
    private TextMeshProUGUI value_txt;
    private Button btnClose;
    private Button CloseButtonQR;
    private BarcodeWriter barcodeWriter;
    private Button QuickButton;
    private Transform content2;
    private int outCreditRate;
    private string userId;
    private List<string> QRCodeInfoList = new List<string>();
    private List<string> BankInfoList = new List<string>();
    private float tempInterval = 0;
    private Transform ContentQRCode;
    private Transform ContentPrint;
    private Transform ScrollViewQRCode;

    private List<Button> quickExchangeList = new List<Button>();
    private List<long> quickExchanges = new List<long>();

    private Button SureBtn;
    private Button valueBtn;

    private Transform SoftKeyboard;

    private bool isMachine = false;
    private Button SoftBtnClose;

    private TextMeshProUGUI soft_value_txt;

    private void Start()
    {
        isMachine = ApplicationSettings.Instance.isMachine;

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
        
        InitQuickQRCodeList();

        InitSQLiteData();
        CheckQRCodeInfo();
        CheckBankInfo();
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
        value_txt.text = credit.ToString();
    }

    private void OnClickSureBtn()
    {
        if (!string.IsNullOrEmpty(value_txt.text))
        {
            if (isMachine)
            {
                ConfirmPrintBankPopup(long.Parse(value_txt.text));
            }
            else
            {
                CreateQRCode(long.Parse(value_txt.text));
            }
        }
    }

    private void InitSQLiteData()
    {
        string qrCode = SQLiteManager.Instance.GetString(userId + "QRCODEINFOLIST", "");
        string bankCode = SQLiteManager.Instance.GetString(userId + "BANKINFOLIST", "");
        Debug.LogError(qrCode);
        Debug.LogError(bankCode);
        if (!string.IsNullOrEmpty(qrCode))
        {
            string[] node = qrCode.Split("###".ToCharArray());
            for (global::System.Int32 i = 0; i < node.Length; i++)
            {

                string temp = node[i];
                if (!string.IsNullOrEmpty(temp))
                {
                    QRCodeInfoList.Add(temp);
                }
            }
        }
        if (!string.IsNullOrEmpty(bankCode))
        {
            string[] node = bankCode.Split("###".ToCharArray());
            for (global::System.Int32 i = 0; i < node.Length; i++)
            {
                string temp = node[i];
                if (!string.IsNullOrEmpty(temp))
                {
                    BankInfoList.Add(temp);
                }
            }
        }
    }
    private void CheckQRCodeInfo()
    {
        if (QRCodeInfoList.Count > 0)
        {
            CurrentQRCodeInfo = QRCodeInfoList[QRCodeInfoList.Count - 1];
            UseExchangeQRCodeInfo(CurrentQRCodeInfo);
        }
    }
    private void CheckBankInfo()
    {
        if (BankInfoList.Count > 0)
        {
            CurrentBankInfo = BankInfoList[BankInfoList.Count - 1];
        }
    }

    private void PrintQRCodeInfo(string info)
    {
        long outCredite = long.Parse(value_txt.text) / outCreditRate;
        TicketInfo ticketInfo = new TicketInfo()
        {
            BankInfo = info,
            Money = outCredite,
            OrderText = "Test Order",
            TicketType = "Money Type",
        };

        BankInfoList.Remove(CurrentBankInfo);
        PrinterController.Instance.PrintTicket(ticketInfo);
        content2.gameObject.SetActive(false);
    }
    /// <summary>
    /// 使用兑换二维码
    /// </summary>
    /// <param name="input"></param>
    private void UseExchangeQRCodeInfo(string input)
    {
        if (!QRCodeInfoList.Contains(input))
        {
            QRCodeInfoList.Add(input);
        }
        SaveInfo();
        content2.gameObject.SetActive(true);
        string temp = input.Replace("\"", "");
        Dictionary<string, object> req = new Dictionary<string, object>
        {
            {"qr_code", temp},
        };
        NetManager.Instance.Post(RPCName.agent_check_qr_code_print_order, req, (res) =>
        {
            CurrentBankInfo = res["bank_order_id"];
            PrintQRCodeInfo(CurrentBankInfo);
            if (!BankInfoList.Contains(CurrentBankInfo))
            {
                QRCodeInfoList.Remove(input);
                BankInfoList.Add(CurrentBankInfo);
            }
            SaveInfo();
            this.DelayAction(2, () =>
            {
                ShowPopup("Success !");
                content2.gameObject.SetActive(false);
            });
        },
        (error) =>
        {
            QRCodeInfoList.Remove(input);
            ShowPopup(error.error);
            SaveInfo();
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
        value_txt.text = soft_value_txt.text;
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
            OnClickNumber(0);
        }
    }
    /// <summary>
    /// 生成下分的二维码
    /// </summary>
    private void CreateQRCode(long credit, bool isPrint = false)
    {
        long myCredit = BlackboardUtils.FindVariable<long>(null, "/me/credit").value;
        content2.gameObject.SetActive(true);
        if (credit <= myCredit)
        {
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
                    UseExchangeQRCodeInfo(CurrentQRCodeInfo);
                }

            },
            (error) =>
            {
                Debug.LogError(error);
                ShowPopup(error.error);
            });
        }
        else
        {
            ShowPopup("not enough money");
            content2.gameObject.SetActive(false);
        }
    }

    private void OnClickNumber(int index)
    {
        soft_value_txt.text += (index).ToString();
    }

    private void DeletedNumber()
    {
        if (soft_value_txt.text.Length > 0)
        {
            string temp = soft_value_txt.text.Substring(0, soft_value_txt.text.Length - 1);
            soft_value_txt.text = temp;
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
        ErrorPopupHandler.Instance.OpenError(info);
    }

    private void SaveInfo()
    {
        string temp1 = "";
        for (int i = 0; i < QRCodeInfoList.Count; i++)
        {
            temp1 += QRCodeInfoList[i] + "###";
        }
        string temp2 = "";
        for (int i = 0; i < BankInfoList.Count; i++)
        {
            temp2 += BankInfoList[i] + "###";
        }
        Debug.LogError(" save success : temp1" + temp1 + "\n temp2" + temp2);
        SQLiteManager.Instance.SetString(userId + "QRCODEINFOLIST", temp1);
        SQLiteManager.Instance.SetString(userId + "BANKINFOLIST", temp2);
    }

    private void ConfirmPrintBankPopup(long score)
    {
        ErrorPopupInfo info = new ErrorPopupInfo();
        info.type = ErrorPopupType.YesNo;
        info.text = $"<size=32>Do you want to print {score.ToString("N0")} score Bank QR Code ?</size>";
        info.buttonText1 = "Confirm";
        info.buttonText2 = "Cancle";
        info.callback1 = delegate
        {
            CreateQRCode(long.Parse(value_txt.text), true);
        };
        ErrorPopupHandler.Instance.OpenError(info);
    }
}
