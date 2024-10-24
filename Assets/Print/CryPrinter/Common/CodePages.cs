
namespace CryPrinter
{
    using System;

    /// <summary>
    /// Codepages supported by devices
    /// Note: Reliance Only
    /// </summary>
    public enum CodePages
    {
        /// <summary>
        /// Prints only ASCII 32-127
        /// </summary>
        CPSPACE = 32,
        /// <summary>
        /// Kurdish Arabic script
        /// </summary>
        CP600 = 600,
        /// <summary>
        /// Cyrillic + Euro symbol
        /// </summary>
        CP808 = 808,
        /// <summary>
        /// DOS Latin 1
        /// </summary>
        CP850 = 850,
        /// <summary>
        /// Quebec French
        /// </summary>
        CP863 = 863,
        /// <summary>
        /// ANSI Latin 1
        /// </summary>
        CP1252 = 1252,
        /// <summary>
        /// Georgian script
        /// </summary>
        CP4256 = 4256,

        /// <summary>
        /// Russian Cyrillic
        /// </summary>
        [Obsolete("Use 808")]
        CP771 = 0,

        /// <summary>
        /// Eurpoean and Greek
        /// </summary>
        CP437 = 437,

        /// <summary>
        /// Skips any non-ascii chars
        /// </summary>
        [Obsolete("Use CPSPACE")]
        ASCII
    }
}
