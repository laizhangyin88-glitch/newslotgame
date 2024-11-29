
namespace CryPrinter
{
    /// <summary>
    /// An implementation with zero side-effects
    /// </summary>
    public class Placeholder : ISection
    {
        public string Content { get { return string.Empty; } set { } }

        public FontEffects Effects { get { return FontEffects.None; } set { } }
   
        public FontJustification Justification { get { return FontJustification.NOP; } set { } }
   
        public FontWidthScalar WidthScalar { get { return FontWidthScalar.NOP; } set { } }

        public FontHeighScalar HeightScalar { get { return FontHeighScalar.NOP; } set { } }

        public ThermalFonts Font { get { return ThermalFonts.NOP; } set { } }

        public bool AutoNewline { get { return false; } set { } }

        /// <summary>
        /// REturns empty buffer
        /// </summary>
        /// <param name="codepage">Unused</param>
        /// <returns>zero length byte array</returns>
        public byte[] GetContentBuffer(CodePages codepage)
        {
            return new byte[0];
        }
    }
}
