
namespace CryPrinter
{
    /// <summary>
    /// All available types of status requests. There is a lot of overlap and redundancy but this
    /// is how things are done in ESC/POS
    /// </summary>
    enum PhoenixStatusRequests
    {
        /// <summary>
        /// Transmit the printer status
        /// </summary>
        Status = 1,

        /// <summary>
        /// Transmit the off-line printer status
        /// </summary>
        OffLineStatus = 2,

        /// <summary>
        /// Transmit error status
        /// </summary>
        ErrorStatus = 3,

        /// <summary>
        /// Transmit paper roll sensor status
        /// </summary>
        PaperRollStatus = 4,

        /// <summary>
        /// Request all statuses
        /// </summary>
        FullStatus = 20,
    }
}
