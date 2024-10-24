using CryPrinter;
using SkiaSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TesterPrinter : MonoBehaviour
{
    private const string TEST_PORT = "COM3";
    private const string PORT = "/dev/ttyS1";
    PhoenixPrinter printer = null;
    private Button button;
    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClickPrinter);
        if (Application.isEditor)
        {
            printer = new PhoenixPrinter(TEST_PORT);
        }
        else
        {
            printer = new PhoenixPrinter(PORT);
        }
    }

    private void OnClickPrinter()
    {
        printer.Reinitialize();
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
        Texture2D tex = CryPrinter.ZXingQrCode.GenerateQRImageWithColor("www.bing.com", 256, 256, Color.black);

        using var qrCodeBitmap = SKBitmap.Decode(tex.EncodeToPNG());
        using var printerImage = new PrinterImage(qrCodeBitmap);
        printer.SetImage(printerImage, document, 1);
        document.Sections.Add(storeIdSection);
        printer.PrintDocument(document);
        printer.FormFeed();
    }
}
