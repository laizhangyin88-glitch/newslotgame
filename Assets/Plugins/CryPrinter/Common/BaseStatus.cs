using Newtonsoft.Json;
namespace CryPrinter
{

    /// <summary>
    /// Encapsulates all ESC/POS status response fields
    /// </summary>
    public class StatusReport
    {
        /// <summary>
        /// Get or Sets flag indicating that this is an invalid report
        /// An invalid report is generated if an incomplete, malformed,
        /// or missing response is processed from a GetStatus command.
        /// </summary>
        public bool IsInvalidReport { get; set; }

        /// <summary>
        /// Printer is reporting online if value is true
        /// </summary>
        public IsOnlineVal IsOnline { get; set; }

        /// <summary>
        /// There is some paper present if this value is true. Note, the paper level 
        /// may be low but is still conidered present.
        /// </summary>
        public IsPaperPresentVal IsPaperPresent { get; set; }

        /// <summary>
        /// Paper level is at or above the low paper threshold if value is true
        /// </summary>
        public IsPaperLevelOkayVal IsPaperLevelOkay { get; set; }

        /// <summary>
        /// Paper is in the present position if this value is true
        /// </summary>
        public IsTicketPresentAtOutputVal IsTicketPresentAtOutput { get; set; }

        /// <summary>
        /// Printer head (cover) is closed if value is true
        /// </summary>
        public IsCoverClosedVal IsCoverClosed { get; set; }

        /// <summary>
        /// The paper motor is currently off if this value is true
        /// </summary>
        public IsPaperMotorOffVal IsPaperMotorOff { get; set; }

        /// <summary>
        /// The diagnostic button is NOT being pushed if this value is true
        /// </summary>
        public IsDiagButtonReleasedVal IsDiagButtonReleased { get; set; }

        /// <summary>
        /// The head temperature is okay if this value is true
        /// </summary>
        public IsHeadTemperatureOkayVal IsHeadTemperatureOkay { get; set; }

        /// <summary>
        /// Comms are okay, no errors, if this value is true
        /// </summary>
        public IsCommsOkayVal IsCommsOkay { get; set; }

        /// <summary>
        /// Power supply voltage is within tolerance if this value is true
        /// </summary>
        public IsPowerSupplyVoltageOkayVal IsPowerSupplyVoltageOkay { get; set; }

        /// <summary>
        /// Power supply voltage is within tolerance if this value is true
        /// </summary>
        public IsPaperPathClearVal IsPaperPathClear { get; set; }

        /// <summary>
        /// The cutter is okay if this value is true
        /// </summary>
        public IsCutterOkayVal IsCutterOkay { get; set; }

        /// <summary>
        /// Last paper feed was NOT due to diag push button if value is true
        /// </summary>
        public IsNormalFeedVal IsNormalFeed { get; set; }

        /// <summary>
        /// If the printer is reporting any error type, this value is true
        /// </summary>
        public HasErrorVal HasError { get; set; }

        /// <summary>
        /// There is a non-recoverable error state if this value is true
        /// </summary>
        public HasFatalErrorVal HasFatalError { get; set; }

        /// <summary>
        /// There is a recoverable error state if this value is true
        /// </summary>
        public HasRecoverableErrorVal HasRecoverableError { get; set; }

        /// <summary>
        /// Returns this object as a JSON string and optionally
        /// tab-format so it looks pretty.
        /// </summary>
        /// <param name="prettyPrint">True to pretty print, default false</param>
        /// <returns>JSON string</returns>
        public string ToJSON(bool prettyPrint = false)
        {
            JsonConvert.SerializeObject(this);
            return JsonConvert.SerializeObject(this); //Json.Serialize(this, prettyPrint);
        }

        /// <summary>
        /// Returns a status report with the IsValidReport flag set to false
        /// </summary>
        /// <returns></returns>
        public static StatusReport Invalid()
        {
            return new StatusReport
            {
                IsInvalidReport = true,
            };
        }
    }
}
