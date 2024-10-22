using System.Collections.Generic;
using UnityEngine;
using ZXing;
using ZXing.Common;

namespace CryPrinter
{
    public class ZXingQrCode
    {
        /// <summary>
        /// 生成2维码 方法一
        /// 经测试：只能生成256x256的
        /// </summary>
        /// <param name="content"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static Texture2D GenerateQRImage1(string content, int width, int height)
        {
            // 编码成color32
            EncodingOptions options = null;
            BarcodeWriter writer = new BarcodeWriter();
            options = new EncodingOptions
            {
                Width = width,
                Height = height,
                Margin = 1,
            };
            options.Hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
            writer.Format = BarcodeFormat.QR_CODE;
            writer.Options = options;
            Color32[] colors = writer.Write(content);

            // 转成texture2d
            Texture2D texture = new Texture2D(width, height);
            texture.SetPixels32(colors);
            texture.Apply();

            //存储成文件
            //byte[] bytes = texture.EncodeToPNG();
            //string path = System.IO.Path.Combine(Application.dataPath, "qr.png");
            //System.IO.File.WriteAllBytes(path, bytes);

            return texture;
        }

        /// <summary>
        /// 生成2维码 方法二
        /// 经测试：能生成任意尺寸的正方形
        /// </summary>
        /// <param name="content"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static Texture2D GenerateQRImageWithColor(string content, int width, int height, Color color)
        {
            BitMatrix bitMatrix;
            Texture2D texture = GenerateQRImageWithColor(content, width, height, color, out bitMatrix);
            return texture;
        }

        /// <summary>
        /// 生成2维码 方法二
        /// 经测试：能生成任意尺寸的正方形
        /// </summary>
        /// <param name="content"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static Texture2D GenerateQRImageWithColor(string content, int width, int height, Color color, out BitMatrix bitMatrix)
        {
            // 编码成color32
            MultiFormatWriter writer = new MultiFormatWriter();
            Dictionary<EncodeHintType, object> hints = new Dictionary<EncodeHintType, object>();
            //设置字符串转换格式，确保字符串信息保持正确
            hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
            // 设置二维码边缘留白宽度（值越大留白宽度大，二维码就减小）
            hints.Add(EncodeHintType.MARGIN, 1);
            hints.Add(EncodeHintType.ERROR_CORRECTION, ZXing.QrCode.Internal.ErrorCorrectionLevel.M);
            //实例化字符串绘制二维码工具
            bitMatrix = writer.encode(content, BarcodeFormat.QR_CODE, width, height, hints);

            // 转成texture2d
            int w = bitMatrix.Width;
            int h = bitMatrix.Height;
            Debug.Log(string.Format("w={0},h={1}", w, h));
            Texture2D texture = new Texture2D(w, h);
            for (int x = 0; x < h; x++)
            {
                for (int y = 0; y < w; y++)
                {
                    if (bitMatrix[x, y])
                    {
                        texture.SetPixel(y, x, color);
                    }
                    else
                    {
                        texture.SetPixel(y, x, Color.white);
                    }
                }
            }
            texture.Apply();

            //存储成文件
            //byte[] bytes = texture.EncodeToPNG();
            //string path = System.IO.Path.Combine(Application.dataPath, "qr.png");
            //System.IO.File.WriteAllBytes(path, bytes); 
            return texture;
        }

        /// <summary>
        /// 生成2维码 方法三
        /// 在方法二的基础上，添加小图标
        /// </summary>
        /// <param name="content"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Texture2D GenerateQRImageWithColorAndIcon(string content, int width, int height, Color color, Texture2D centerIcon)
        {
            BitMatrix bitMatrix;
            Texture2D texture = GenerateQRImageWithColor(content, width, height, color, out bitMatrix);
            int w = bitMatrix.Width;
            int h = bitMatrix.Height;
            // 添加小图
            int halfWidth = texture.width / 2;
            int halfHeight = texture.height / 2;
            int halfWidthOfIcon = centerIcon.width / 2;
            int halfHeightOfIcon = centerIcon.height / 2;
            int centerOffsetX = 0;
            int centerOffsetY = 0;
            for (int x = 0; x < h; x++)
            {
                for (int y = 0; y < w; y++)
                {
                    centerOffsetX = x - halfWidth;
                    centerOffsetY = y - halfHeight;
                    if (Mathf.Abs(centerOffsetX) <= halfWidthOfIcon && Mathf.Abs(centerOffsetY) <= halfHeightOfIcon)
                    {
                        texture.SetPixel(x, y, centerIcon.GetPixel(centerOffsetX + halfWidthOfIcon, centerOffsetY + halfHeightOfIcon));
                    }
                }
            }
            texture.Apply();
            // 存储成文件
            byte[] bytes = texture.EncodeToPNG();
            string path = System.IO.Path.Combine(Application.dataPath, "qr.png");
            System.IO.File.WriteAllBytes(path, bytes);
            return texture;
        }
    }
}