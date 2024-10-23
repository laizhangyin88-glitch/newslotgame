
namespace CryPrinter
{
    using System.Collections.Generic;

    /// <summary>
    /// Contains an ordered sequence of sections
    /// </summary>
    public interface IDocument
    {
        /// <summary>
        /// Gets or Setsthe ordered list of sections in this document
        /// </summary>
        IList<ISection> Sections { get; set; }

        /// <summary>
        /// Gets or Sets the codepage for this document
        /// </summary>
        CodePages CodePage { get; set; }
    }
}
