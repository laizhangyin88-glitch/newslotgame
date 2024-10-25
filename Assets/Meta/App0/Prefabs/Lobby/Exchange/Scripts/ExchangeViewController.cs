using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using BagelCode;
using GameUtil;
using CryPrinter;
using SimpleJSON;
using ParadoxNotion;
using BmpSharp;
using SkiaSharp;
using System.Text.RegularExpressions;

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
    private const string TEST_PORT = "COM3";
    private const string PORT = "/dev/ttyS1";
    public RawImage QRCodeRawImage; // 绘制好的二维码
    private string CurrentQRCodeInfo = "";
    private string CurrentBankInfo = "";
    private List<GameObject> btnsList = new List<GameObject>();
    private TextMeshProUGUI value_txt;
    private Button btnClose;
    private Button ButtonQR;
    private BarcodeWriter barcodeWriter;
    private TMP_InputField QRInputField;
    private float waitTime = 5.0f;
    private Button ButtonUse;
    private Button TestBtn;
    private Button TestBtn1;
    private Button buttonUp;
    private Button buttonPrint;
    private Button clearBtn;
    private Transform content1;
    private Transform content2;
    private LoopTimer _loopTimer;
    private int outCreditRate;
    private int emptyLine = 3;
    private string userId;
    private List<string> QRCodeInfoList = new List<string>();
    private List<string> BankInfoList = new List<string>();
    private TMP_InputField testInput;

    PhoenixPrinter printer = null;

    private float interval = 10;
    private float tempInterval = 0;

    private Button CloseInputField;

    private void Start()
    {
        userId = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "/me/userId").value;
        outCreditRate = BlackboardUtils.GetOrCreateVariable<int>(MainBlackboard.Get(), "OutCreditRate").value;
        content1 = transform.Find("content1");
        content2 = transform.Find("content2");
        content1.gameObject.SetActive(false);
        content2.gameObject.SetActive(false);
        buttonUp = transform.Find("content1/ButtonUp").GetComponent<Button>();
        buttonPrint = transform.Find("content1/ButtonPrint").GetComponent<Button>();
        buttonPrint.onClick.AddListener(OnClickBtnPrint);
        buttonUp.onClick.AddListener(OnClickBtnUp);
        QRCodeRawImage = transform.Find("content/QR/RawImage").GetComponent<RawImage>();
        ButtonQR = QRCodeRawImage.transform.GetChild(0).GetComponent<Button>();
        ButtonQR.onClick.AddListener(() => { QRCodeRawImage.transform.parent.gameObject.SetActive(false); });
        QRCodeRawImage.transform.parent.gameObject.SetActive(false); 
        Transform btnParent = transform.Find("content/btns");
        value_txt = transform.Find("content/Image/value_txt").GetComponent<TextMeshProUGUI>();
        value_txt.text = "";
        btnClose = transform.Find("BtnClose").GetComponent<Button>();
        btnClose.onClick.AddListener(OnClickBtnClose);
        InitBtnsList(btnParent);
        ButtonUse = transform.Find("content/ButtonUse").GetComponent<Button>();
        TestBtn = transform.Find("content/TestBtn").GetComponent<Button>();
        TestBtn.onClick.AddListener(OnClickTestBtn);
        testInput = transform.Find("content/TestInputField").GetComponent<TMP_InputField>();

        TestBtn1 = transform.Find("content/TestBtn1").GetComponent<Button>();

        clearBtn = transform.Find("content/ClearBtn").GetComponent<Button>();

        TestBtn1.onClick.AddListener(OnClickTestBtn1);


        if (ButtonUse != null)
        {
            ButtonUse.onClick.AddListener(OnClickUseBtn);
        }
        QRInputField = transform.Find("content/InputField").GetComponent<TMP_InputField>();
        CloseInputField = QRInputField.transform.Find("CloseInputField").GetComponent<Button>();
        CloseInputField.onClick.AddListener(() => { QRInputField.gameObject.SetActive(false); });
        QRInputField.gameObject.SetActive(false);
        InitSQLiteData();
        CheckQRCodeInfo();
        CheckBankInfo();

        if (Application.isEditor)
        {
            printer = new PhoenixPrinter(TEST_PORT);
        }
        else
        {
            printer = new PhoenixPrinter(PORT);
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

    private void OnClickTestBtn()
    {
        UseExchangeQRCodeInfo(CurrentQRCodeInfo);
    }

    private void OnClickTestBtn1()
    {
        JudeUseInputField(testInput.text); 
    }

    private void OnClickClearBtn()
    {
        SBoxSanboxController.Instance.StopScrpitAllCoroutines(); 
    }

    private void CheckQRCodeInfo()
    {
        if(QRCodeInfoList.Count > 0)
        {
            CurrentQRCodeInfo = QRCodeInfoList[QRCodeInfoList.Count - 1];
            UseExchangeQRCodeInfo (CurrentQRCodeInfo);
        }
    }

    private void CheckBankInfo()
    {
        if(BankInfoList.Count > 0)
        {
            CurrentBankInfo = BankInfoList[BankInfoList.Count - 1];
            content1.gameObject.SetActive(true);
        }
    }
    private void OnClickBtnUp()
    {
        UseBankQRCode(CurrentBankInfo);
    }
    private void OnClickBtnPrint()
    {
        if(!string.IsNullOrEmpty(CurrentBankInfo))
        {
            PrintQRCodeInfo();
        }
    }
    private void PrintQRCodeInfo()
    {
        printer.Reinitialize();
        //printer.SetFont(ThermalFonts.C);
        //printer.SetScalars(FontWidthScalar.w8, FontHeighScalar.h5);
        //printer.AddEffect(FontEffects.Bold);
        //printer.PrintASCIIString("CurrentBankInfo QRCode");
        //printer.PrintNewline();
        //printer.Print2DBarcode(CurrentBankInfo);
        var document = new StandardDocument
        {
            CodePage = CodePages.CPSPACE,
        };

        var headerSection = new StandardSection
        {
            Content = "Header",
            Justification = FontJustification.JustifyCenter,
            HeightScalar = FontHeighScalar.h2,
            WidthScalar = FontWidthScalar.w2,
            Effects = FontEffects.Bold,
            Font = ThermalFonts.A,
            AutoNewline = true,
        };

        var storeIdSection = new StandardSection
        {
            Content = "# STORE: 1234",
            Justification = FontJustification.JustifyCenter,
            HeightScalar = FontHeighScalar.h2,
            WidthScalar = FontWidthScalar.w2,
            Effects = FontEffects.Bold,
            Font = ThermalFonts.A,
            AutoNewline = true,
        };

        document.Sections.Add(headerSection);
        Texture2D tex = CryPrinter.ZXingQrCode.GenerateQRImageWithColor(CurrentBankInfo + "&QRCodeEnd&", 256, 256, Color.black);

        using var qrCodeBitmap = SKBitmap.Decode(tex.EncodeToPNG());
        using var printerImage = new PrinterImage(qrCodeBitmap);
        printer.SetImage(printerImage, document, 1);
        document.Sections.Add(storeIdSection);
        printer.PrintDocument(document);
        printer.FormFeed();
        BankInfoList.Remove(CurrentBankInfo);
        //SaveInfo();
        content2.gameObject.SetActive(false);
        content1.gameObject.SetActive(false); 
    }

    private void InputFieldChange() 
    {
        if(_loopTimer == null)
        {
            _loopTimer = this.LoopAction(Time.deltaTime, (interval) =>
            {
                if(QRInputField.text.Length > 0)
                {
                    Debug.Log(QRInputField.text);
                }
                if (QRInputField.text.Contains("&QRCodeEnd&"))
                { 
                    QRInputField.DeactivateInputField(true);
                    Debug.Log("input end....." +  QRInputField.text);
                    _loopTimer?.Cancel();
                    _loopTimer = null;
                    string[] strings = Regex.Split(QRInputField.text, "&QRCodeEnd&");
                    JudeUseInputField(strings[0]);
                }
            });
            QRInputField.ActivateInputField();
        }
    }

    private void JudeUseInputField(string input)
    {
        Debug.Log(input);
        string[] splits = input.Split(':');
        if (splits[0] == "qr_code")
        {
            UseExchangeQRCodeInfo(input);
        }
        if (splits[0] == "bank")
        {
            UseBankQRCode(input);
        }
        content2.gameObject.SetActive(true);
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
        //SaveInfo();
        content2.gameObject.SetActive(true);
        string temp = input.Replace("\"", "");
        Dictionary<string, object> req = new Dictionary<string, object>
        {
            {"qr_code", temp},
        };
        NetManager.Instance.Post(RPCName.agent_check_qr_code_print_order, req, (res) =>
        {
            Debug.LogError("###验证支付二维码###success");
            CurrentBankInfo = res["bank_order_id"];
            if (!BankInfoList.Contains(CurrentBankInfo))
            {
                QRCodeInfoList.Remove(input);
                BankInfoList.Add(CurrentBankInfo);
            }
            //SaveInfo();
            content2.gameObject.SetActive(false);
            Debug.LogError("bank order  " + CurrentBankInfo);
            QRInputField.gameObject.SetActive(false);
            content1.gameObject.SetActive(true);
        },
        (error) =>
        {
            Debug.LogError(error);
            //GlobalErrorHandler.GlobalError(error);
            QRCodeInfoList.Remove(input);
            Debug.LogError("have error");
            ShowPopup(error.error);
            //SaveInfo();
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
            TextMeshProUGUI text = button.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            
            if(i == 9)
            {
                text.text = "X";
                button.onClick.AddListener(() =>
                {
                    OnClickBtn(10);
                });
            }
            else if(i == 10)
            {
                text.text = "0";
                button.onClick.AddListener(() =>
                {
                    OnClickBtn(0);
                });
            }
            else if (i == 11)
            {
                text.text = "OK";
                button.onClick.AddListener(() =>
                {
                    OnClickBtn(11);
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
            case 11:
                OnClickSure();
                break;
        }
    }
     
    private void OnClickZero()
    {
        if (string.IsNullOrEmpty(value_txt.text))
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
    private void OnClickSure()
    {
        if (!string.IsNullOrEmpty(value_txt.text))
        {
            Dictionary<string, object> req = new Dictionary<string, object>
            {
                {"outcredit", value_txt.text},
            };
            NetManager.Instance.Post(RPCName.agent_build_qr_code, req, (res) =>
            {
                CurrentQRCodeInfo = res["qr_code_key"];
                DrawQRCode(CurrentQRCodeInfo);
                QRCodeRawImage.transform.parent.gameObject.SetActive(true);
            },
            (error) =>
            {
                //GlobalErrorHandler.GlobalError(error);
                Debug.LogError(error);
                Debug.LogError("have error");
                ShowPopup(error.error);
            });
        }
    }

    private void OnClickUseBtn()
    {
        QRInputField.gameObject.SetActive(true); 
        QRInputField.text = "";
        QRInputField.ActivateInputField(); 
        InputFieldChange();
    }

    private void UseBankQRCode(string code)
    {
        content2.gameObject.SetActive(true);
        Dictionary<string, object> req = new Dictionary<string, object>
        {
            {"bank_order_id", code},
        };
        NetManager.Instance.Post(RPCName.agent_check_bank_order, req, (res) =>
        {
            Debug.LogError(res.ToString());
            globalStore.newCredit = res["balance"].AsLong;
            BankInfoList.Remove(CurrentBankInfo);
            //SaveInfo();
            Debug.LogError("上分成功..........................");
            content2.gameObject.SetActive(false);
            content1.gameObject.SetActive(false);
        },
        (error) =>
        {
            Debug.LogError(error.error);
            Debug.LogError("have error");
            ShowPopup(error.error);
            content2.gameObject.SetActive(false);
            content1.gameObject.SetActive(false);
            BankInfoList.Remove(CurrentBankInfo);
            //SaveInfo();
        });
    }

    private void OnClickNumber(int index)
    {
        value_txt.text += (index).ToString();
    }

    private void DeletedNumber() 
    {
        if (value_txt.text.Length > 0)
        {
            string temp = value_txt.text.Substring(0, value_txt.text.Length - 1);
            value_txt.text = temp;
        }
    }

    private void OnDestroy()
    {
        if(btnsList != null && btnsList.Count > 0)
        {
            for (int i = 0; i < btnsList.Count; i++)
            {
                Button btn = btnsList[i].GetComponent<Button>();
                btn.onClick.RemoveAllListeners();
            }
            btnsList.Clear();
        }
        btnClose.onClick.RemoveAllListeners();
        printer.Dispose();
        printer = null;
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
        Debug.LogError(" save success : temp1" + temp1 +"\n temp2" + temp2 );
        SQLiteManager.Instance.SetString(userId + "QRCODEINFOLIST", temp1);
        SQLiteManager.Instance.SetString(userId + "BANKINFOLIST", temp2);
    }

    private IEnumerator LoopCheck()
    {
        yield return new WaitUntil(() => globalStore.gameState == GameState.Hall);
        while (true)
        {
            if((tempInterval -= Time.deltaTime) < 0)
            {
                Debug.LogError("检测是否发送兑换协议.............");
                if(QRCodeInfoList.Count > 0)
                {

                }
                if(BankInfoList.Count > 0) 
                {

                }
            }
        }
    }
}
