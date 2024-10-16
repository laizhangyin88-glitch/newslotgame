using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZXing.QrCode.Internal;
using ZXing;
using BagelCode;
using System.Runtime.Remoting.Contexts;
using GameUtil;

public class ExchangeViewController : MonoBehaviour
{
    public RawImage QRCodeRawImage; // 绘制好的二维码

    private string QRCodeInfo = "";

    private string BankInfo = "";

    private List<GameObject> btnsList = new List<GameObject>();

    private TextMeshProUGUI value_txt;

    private Button btnClose;
    private Button ButtonQR;
    private BarcodeWriter barcodeWriter;

    private TMP_InputField QRInputField; 

    private float waitTime = 5.0f;

    public Button TestBtn;

    private Button buttonUp;
    private Button buttonPrint;
    private Button buttonClose;

    private Transform content1;

    private LoopTimer _loopTimer;

    private int outCreditRate;

    private void Start()
    {
        outCreditRate = BlackboardUtils.GetOrCreateVariable<int>(MainBlackboard.Get(), "OutCreditRate").value;

        content1 = transform.Find("content1");
        content1.gameObject.SetActive(false);
        buttonUp = transform.Find("content1/ButtonUp").GetComponent<Button>();
        buttonPrint = transform.Find("content1/ButtonPrint").GetComponent<Button>();
        buttonClose = transform.Find("content1/ButtonClose").GetComponent<Button>();

        buttonPrint.onClick.AddListener(OnClickBtnPrint);
        buttonUp.onClick.AddListener(OnClickBtnUp);
        buttonClose.onClick.AddListener(() => { content1.gameObject.SetActive(false); });

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
        if(TestBtn != null)
        {
            TestBtn.onClick.AddListener(OnClickTestBtn);
        }
        QRInputField = transform.Find("content/InputField").GetComponent<TMP_InputField>();
        QRInputField.gameObject.SetActive(false);
        //QRInputField.onValueChanged.AddListener(InputFieldChange);
    }

    private void OnClickBtnUp()
    {
        StartCoroutine(UseBankQRCode(BankInfo));
    }

    private void OnClickBtnPrint()
    {

    }


    private void InputFieldChange()
    {
        if(_loopTimer == null)
        {
            float temp = 0.2f;
            int length = -1;
            _loopTimer = this.LoopAction(Time.deltaTime, (interval) =>
            {
                if (QRInputField.text.Length > 0)
                {
                    if (length < 0)
                    {
                        length = QRInputField.text.Length;
                    }
                    if ((temp -= Time.deltaTime) < 0)
                    {
                        temp = 0.2f;
                        if (length == QRInputField.text.Length)
                        {
                            GetQRCodeInfo(QRInputField.text);
                            _loopTimer?.Cancel();
                            _loopTimer = null;
                        }
                        length = -1;
                    }
                }
            });
        }
    }

    private void GetQRCodeInfo(string input)
    {
        //yield return new WaitForSeconds(2f);
        string temp = input.Replace("\"", "");
        Dictionary<string, object> req = new Dictionary<string, object>
        {
            {"qr_code", temp},
        };

        NetManager.Instance.Post(RPCName.agent_check_qr_code_print_order, req, (res) =>
        {
            Debug.LogError("###验证支付二维码###success");
            BankInfo = res["bank_order_id"];
            //QRCodeInfo = temp;
            QRInputField.gameObject.SetActive(false);
            content1.gameObject.SetActive(true);

        },
        (error) =>
        {
            GlobalErrorHandler.GlobalError(error);
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
                QRCodeInfo = res["qr_code_key"];
                DrawQRCode(QRCodeInfo);
                QRCodeRawImage.transform.parent.gameObject.SetActive(true);
            },
            (error) =>
            {
                GlobalErrorHandler.GlobalError(error);
            });
        }
    }

    private void OnClickTestBtn()
    {
        QRInputField.gameObject.SetActive(true); 
        QRInputField.text = "";
        QRInputField.ActivateInputField();
        InputFieldChange();
    }

    private IEnumerator UseBankQRCode(string code)
    {
        yield return new WaitForSeconds(1); 
        Dictionary<string, object> req = new Dictionary<string, object>
        {
            {"bank_order_id", code},
        };
        NetManager.Instance.Post(RPCName.agent_check_bank_order, req, (res) =>
        {
            Debug.LogError(res.ToString());
            Debug.LogError("上分成功.........................."); 
        },
        (error) =>
        {
            GlobalErrorHandler.GlobalError(error);
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
        Texture2D texture = ShowQRCode(formatStr, 256, 256);
        QRCodeRawImage.texture = texture;
    }
}
