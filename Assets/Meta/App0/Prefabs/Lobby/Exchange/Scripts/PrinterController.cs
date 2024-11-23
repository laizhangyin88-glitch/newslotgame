#if UNITY_ANDROID
using CryPrinter;
using GameUtil;

#endif
using SBoxApi;
using SboxSpace;
using SkiaSharp;
using SlotMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;


public class TicketInfo
{
    public string storeName;
    public long Money;
    public string TicketType;
    public string MoneyAmountEn;
    public string BankInfo;
    public string OrderText;
    public string StoreAddr;
    public string StoreEmail;
    public string StoreTel;
}

public class PrinterController
{

    private const string TEST_PORT = "COM3";
    private const string PORT = "/dev/ttyS1";
#if UNITY_ANDROID
    PhoenixPrinter printer = null;
#endif

    private static PrinterController _instance;

    private string companyName = null;
    private string companyAddress = null;
    private string companyEmail = null;
    private string telephone = null;
    private LoopTimer _loopTimer;
    private int count = 10;

    public static PrinterController Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new PrinterController();
            }
            return _instance;   
        }

    }

    private PrinterController()
    {
#if UNITY_ANDROID
        if (Application.isEditor)
        {
            printer = new PhoenixPrinter(TEST_PORT);
        }
        else
        {
            printer = new PhoenixPrinter(PORT);
        }
        companyName = BlackboardUtils.GetOrCreateVariable<string>(MainBlackboard.Get(), "CompanyName").value;
        companyAddress = BlackboardUtils.GetOrCreateVariable<string>(MainBlackboard.Get(), "CompanyAddress").value;
        companyEmail = BlackboardUtils.GetOrCreateVariable<string>(MainBlackboard.Get(), "CompanyEmail").value;
        telephone = BlackboardUtils.GetOrCreateVariable<string>(MainBlackboard.Get(), "telephone").value;

#endif
    }

    public void PrintTicket(TicketInfo ticketInfo)
    {

#if UNITY_ANDROID
        _loopTimer = Timer.LoopAction(1f, (intervale) =>
        {
            StatusReport PrinterStatusReport = PrinterController.Instance.printer.GetStatus(CryPrinter.StatusTypes.PrinterStatus); 
            StatusReport OfflineStatusReport = PrinterController.Instance.printer.GetStatus(CryPrinter.StatusTypes.OfflineStatus);
            StatusReport ErrorStatusReport = PrinterController.Instance.printer.GetStatus(CryPrinter.StatusTypes.ErrorStatus);
            StatusReport PaperStatusReport = PrinterController.Instance.printer.GetStatus(CryPrinter.StatusTypes.PaperStatus);
            StatusReport MovementStatusReport = PrinterController.Instance.printer.GetStatus(CryPrinter.StatusTypes.MovementStatus);
            StatusReport FullStatusReport = PrinterController.Instance.printer.GetStatus(CryPrinter.StatusTypes.FullStatus);

            Debug.LogError("PrinterStatusReport : " + PrinterStatusReport.HasError);
            Debug.LogError("OfflineStatusReport : " + OfflineStatusReport.HasError + "#isOnline" + OfflineStatusReport.IsOnline);
            Debug.LogError("ErrorStatusReport : " + ErrorStatusReport.HasError);
            Debug.LogError("PaperStatusReport : " + PaperStatusReport.HasError + " #paperStatus" + PaperStatusReport.IsPaperPresent + " #paperLevelOk:" + PaperStatusReport.IsPaperLevelOkay + "#IsPaperMotorOff" + PaperStatusReport.IsPaperMotorOff);
            Debug.LogError("MovementStatusReport : " + MovementStatusReport.HasError);
            Debug.LogError("FullStatusReport : " + FullStatusReport.HasError);

            Debug.LogError("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            if ((count -= 1) < 0)
            {
                _loopTimer.Cancel();
                count = 10;
            }
        });

        printer.Reinitialize();

        var document = new StandardDocument
        {
            CodePage = CodePages.CPSPACE,
        };

        var StoreNameSection = new StandardSection
        {
            Content = companyName,//ticketInfo.storeName,
            Justification = FontJustification.JustifyLeft,
            HeightScalar = FontHeighScalar.h1,
            WidthScalar = FontWidthScalar.w1,
            Effects = FontEffects.None,
            Font = ThermalFonts.C,
            AutoNewline = true,
        };

        var TicketTypeSection = new StandardSection
        {
            Content = ticketInfo.TicketType,
            Justification = FontJustification.JustifyCenter,
            HeightScalar = FontHeighScalar.h1,
            WidthScalar = FontWidthScalar.w1,
            Effects = FontEffects.Bold,
            Font = ThermalFonts.B,
            AutoNewline = true,
        };

        var MoneyAmountSection = new StandardSection
        {
            Content = ticketInfo.Money.ToString("N0"),
            Justification = FontJustification.JustifyCenter,
            HeightScalar = FontHeighScalar.h1,
            WidthScalar = FontWidthScalar.w1,
            Effects = FontEffects.Bold,
            Font = ThermalFonts.B,
            AutoNewline = true,
        };
        var MoneyAmountEnSection = new StandardSection
        {
            Content = NumberToEnglishString(ticketInfo.Money).ToUpper(),
            Justification = FontJustification.JustifyCenter,
            HeightScalar = FontHeighScalar.h1,
            WidthScalar = FontWidthScalar.w1,
            Effects = FontEffects.None,
            Font = ThermalFonts.C,
            AutoNewline = true,
        };
        //var QRCodeSection = new StandardSection
        //{
        //    ContentQRCode = PrintMsg.MoneyAmountEn,
        //    Justification = FontJustification.JustifyCenter,
        //    HeightScalar = FontHeighScalar.h1,
        //    WidthScalar = FontWidthScalar.w1,
        //    Effects = FontEffects.None,
        //    Font = ThermalFonts.C,
        //    AutoNewline = true,
        //};
        var PrintTimeSection = new StandardSection
        {
            Content = DateTime.Now.ToLongTimeString(),
            Justification = FontJustification.JustifyLeft,
            HeightScalar = FontHeighScalar.h1,
            WidthScalar = FontWidthScalar.w1,
            Effects = FontEffects.None,
            Font = ThermalFonts.C,
            AutoNewline = true,
        };

        var PrintDateSection = new StandardSection
        {
            Content = DateTime.Now.ToString("dd MMM yyyy dddd", CultureInfo.CreateSpecificCulture("en-GB")),
            Justification = FontJustification.JustifyLeft,
            HeightScalar = FontHeighScalar.h1,
            WidthScalar = FontWidthScalar.w1,
            Effects = FontEffects.None,
            Font = ThermalFonts.C,
            AutoNewline = true,
        };

        var OrderTextSection = new StandardSection
        {
            Content = ticketInfo.OrderText,
            Justification = FontJustification.JustifyLeft,
            HeightScalar = FontHeighScalar.h1,
            WidthScalar = FontWidthScalar.w1,
            Effects = FontEffects.None,
            Font = ThermalFonts.C,
            AutoNewline = true,
        };

        var StoreAddressSection = new StandardSection
        {
            Content = companyAddress,//ticketInfo.StoreAddr,
            Justification = FontJustification.JustifyLeft,
            HeightScalar = FontHeighScalar.h1,
            WidthScalar = FontWidthScalar.w1,
            Effects = FontEffects.None,
            Font = ThermalFonts.C,
            AutoNewline = true,
        };

        var StoreEmailSection = new StandardSection
        {
            Content = companyEmail,//ticketInfo.StoreEmail,
            Justification = FontJustification.JustifyLeft,
            HeightScalar = FontHeighScalar.h1,
            WidthScalar = FontWidthScalar.w1,
            Effects = FontEffects.None,
            Font = ThermalFonts.C,
            AutoNewline = true,
        };

        var StoreTelephoneSection = new StandardSection
        {
            Content = telephone,//ticketInfo.StoreTel,
            Justification = FontJustification.JustifyLeft,
            HeightScalar = FontHeighScalar.h1,
            WidthScalar = FontWidthScalar.w1,
            Effects = FontEffects.None,
            Font = ThermalFonts.C,
            AutoNewline = true,
        };
#if UNITY_EDITOR
        string data = ticketInfo.BankInfo + "#:#" + ticketInfo.Money + "#:#" + ticketInfo.OrderText; 
        MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_PRINT_BANK_INFO, data);
#endif
        document.Sections.Add(StoreNameSection);
        document.Sections.Add(TicketTypeSection);
        document.Sections.Add(MoneyAmountSection);
        document.Sections.Add(MoneyAmountEnSection);
        //生成二维码
        Texture2D tex = ZXingQrCode.GenerateQRImageWithColor(ticketInfo.BankInfo + "&QRCodeEnd&", 256, 256, Color.black);
        using var qrCodeBitmap = SKBitmap.Decode(tex.EncodeToPNG());
        using var printerImage = new PrinterImage(qrCodeBitmap);
        printer.SetImage(printerImage, document, 4);

        document.Sections.Add(PrintTimeSection);
        document.Sections.Add(PrintDateSection);
        document.Sections.Add(OrderTextSection);
        document.Sections.Add(StoreAddressSection);
        document.Sections.Add(StoreEmailSection);
        document.Sections.Add(StoreTelephoneSection);

        printer.PrintDocument(document);
        printer.FormFeed();
#endif
    }

    /// <summary>
    /// 数字转为英文字符串 不考虑负数
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public string NumberToEnglishString(long number)
    {
        if (number < 0)
        {
            return "";
        }
        if (number < 20) //0到19
        {
            switch (number)
            {
                case 0:
                    return "zero";
                case 1:
                    return "one";
                case 2:
                    return "two";
                case 3:
                    return "three";
                case 4:
                    return "four";
                case 5:
                    return "five";
                case 6:
                    return "six";
                case 7:
                    return "seven";
                case 8:
                    return "eight";
                case 9:
                    return "nine";
                case 10:
                    return "ten";
                case 11:
                    return "eleven";
                case 12:
                    return "twelve";
                case 13:
                    return "thirteen";
                case 14:
                    return "fourteen";
                case 15:
                    return "fifteen";
                case 16:
                    return "sixteen";
                case 17:
                    return "seventeen";
                case 18:
                    return "eighteen";
                case 19:
                    return "nineteen";
                default:
                    return "";
            }
        }
        if (number < 100) //20到99
        {
            if (number % 10 == 0) //20,30,40,...90的输出
            {
                switch (number)
                {
                    case 20:
                        return "twenty";
                    case 30:
                        return "thirty";
                    case 40:
                        return "forty";
                    case 50:
                        return "fifty";
                    case 60:
                        return "sixty";
                    case 70:
                        return "seventy";
                    case 80:
                        return "eighty";
                    case 90:
                        return "ninety";
                    default:
                        return "";
                }
            }
            else //21.22,....99 思路：26=20+6
            {
                return string.Format("{0} {1}", NumberToEnglishString(10 * (number / 10)),
                    NumberToEnglishString(number % 10));
            }
        }
        if (number < 1000) //100到999  百级
        {
            if (number % 100 == 0)
            {
                return string.Format("{0} hundred", NumberToEnglishString(number / 100));
            }
            else
            {
                return string.Format("{0} hundred and {1}", NumberToEnglishString(number / 100),
                    NumberToEnglishString(number % 100));
            }
        }
        if (number < 1000000) //1000到999999 千级
        {
            if (number % 1000 == 0)
            {
                return string.Format("{0} thousand", NumberToEnglishString(number / 1000));
            }
            else
            {
                return string.Format("{0} thousand and {1}", NumberToEnglishString(number / 1000),
                    NumberToEnglishString(number % 1000));
            }
        }
        if (number < 1000000000) //1000 000到999 999 999 百万级
        {
            if (number % 1000 == 0)
            {
                return string.Format("{0} million", NumberToEnglishString(number / 1000000));
            }
            else
            {
                return string.Format("{0} million and {1}", NumberToEnglishString(number / 1000000),
                    NumberToEnglishString(number % 1000000));
            }
        }
        if (number <= int.MaxValue) //十亿 级
        {
            if (number % 1000000000 == 0)
            {
                return string.Format("{0} billion", NumberToEnglishString(number / 1000000000));
            }
            else
            {
                return string.Format("{0} billion and {1}", NumberToEnglishString(number / 1000000000),
                    NumberToEnglishString(number % 1000000000));
            }
        }
        return "";
    }
}
