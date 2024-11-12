using System.Diagnostics;
using System.Text;

namespace CryPrinter
{
    using UnityEngine;
    /// <summary>
    /// Standard document implementation
    /// </summary>
    public class StandardSection : ISection
    {
        static StandardSection()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
        
        public virtual string Content { get; set; }

        public virtual FontEffects Effects { get; set; }

        public virtual FontJustification Justification { get; set; }

        public virtual FontWidthScalar WidthScalar { get; set; }

        public virtual FontHeighScalar HeightScalar { get; set; }

        public virtual ThermalFonts Font { get; set; }

        public virtual bool AutoNewline { get; set; }

        public virtual byte[] GetContentBuffer(CodePages codepage)
        {
            if (string.IsNullOrEmpty(Content))
            {
                return new byte[0];
            }

            Debug.Log("GetContentBuffer codepage is " + codepage);
            
            Encoding encoder;
            switch (codepage)
            {
                case CodePages.CP771:
                    // This is the most similar to 771
                    encoder = System.Text.Encoding.GetEncoding(866);
                    break;

                case CodePages.CP437:
                    encoder = System.Text.Encoding.GetEncoding(437);
                    break;

                case CodePages.CPSPACE:
                    encoder = System.Text.Encoding.UTF8;
                    break;
                case CodePages.ASCII:
                    Content = System.Text.RegularExpressions.Regex.Replace(Content,
                        @"[^\u0020-\u007E]", string.Empty);
                    encoder = System.Text.ASCIIEncoding.ASCII;
                    break;

                default:
                    encoder = System.Text.Encoding.GetEncoding(866);
                    break;
            }

            return encoder.GetBytes(Content);
        }
    }
}