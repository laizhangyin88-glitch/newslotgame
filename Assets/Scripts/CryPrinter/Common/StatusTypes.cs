
namespace CryPrinter
{
    public enum StatusTypes
    {
        /// <summary>
        /// Ready or not ready
        /// </summary>
        PrinterStatus,

        /// <summary>
        /// Physical state
        /// </summary>
        OfflineStatus,

        /// <summary>
        /// All error messages
        /// </summary>
        ErrorStatus,

        /// <summary>
        /// Paper low, paper present
        /// </summary>
        PaperStatus,

        /// <summary>
        /// Printing now or motor moving
        /// </summary>
        MovementStatus,

        /// <summary>
        /// All parameters
        /// </summary>
        FullStatus,
    }
}
