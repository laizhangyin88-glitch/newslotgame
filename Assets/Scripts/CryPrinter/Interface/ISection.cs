
namespace CryPrinter
{
    public interface ISection
    {
        /// <summary>
        /// String content for this section
        /// </summary>
        string Content { get; set; }

        /// <summary>
        /// All effects to apply to content. This can be masked together to
        /// apply more than one effect. These effects will only
        /// be active during the printing of this document and then 
        /// they will be cleared.
        /// </summary>
        FontEffects Effects { get; set; }

        /// <summary>
        /// Gets or Sets justification for this document
        /// </summary>
        FontJustification Justification { get; set; }

        /// <summary>
        /// Gets or Sets the width scalar for this document
        /// </summary>
        FontWidthScalar WidthScalar { get; set; }

        /// <summary>
        /// Gets or Sets the height scalar for this document
        /// </summary>
        FontHeighScalar HeightScalar { get; set; }

        /// <summary>
        /// Gets or Sets the font to use for this section
        /// </summary>
        ThermalFonts Font { get; set; }

        /// <summary>
        /// Auto-apply a newline after this document
        /// </summary>
        bool AutoNewline { get; set; }

        /// <summary>
        /// Returns data portion of content as byte array
        /// </summary>
        /// <param name="codepage">Codepage to encode text</param>
        /// <returns></returns>
        byte[] GetContentBuffer(CodePages codepage);
    }
}
