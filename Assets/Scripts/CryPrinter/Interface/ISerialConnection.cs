
namespace CryPrinter
{
    /// <summary>
    /// Contract for all serialized connection types
    /// </summary>
    public interface ISerialConnection : System.IDisposable
    {
        /// <summary>
        /// Attempts to open serial device for communication. Returns
        /// Success only if device is found and opened successfully.
        /// </summary>
        /// <returns></returns>
        ReturnCode Open();

        /// <summary>
        /// Attempts to close and release the connection to the serial device.
        /// If there are no issues, Success will be returned.
        /// </summary>
        /// <returns></returns>
        ReturnCode Close();

        /// <summary>
        /// Gets or Sets the name that uniquely identifies this serial 
        /// device to the system
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets or Sets the read timeout in milliseconds
        /// </summary>
        int ReadTimeoutMS { get; set; }

        /// <summary>
        /// Gets or Sets the write timeout in milliseconds
        /// </summary>
        int WriteTimeoutMS { get; set; }

        /// <summary>
        /// Transmits payload to targets and returns count of 
        /// bytes that were written. The write operation will return
        /// once all bytes or written or if the period defined by
        /// WriteTimeoutMS expires.
        /// </summary>
        /// <param name="payload">Data to send</param>
        /// <returns>Count of bytes written</returns>
        int Write(byte[] payload);

        /// <summary>
        /// Read and return n count of bytes. This function
        /// will return once n bytes are received or ReadTimeoutMS
        /// has expired.
        /// </summary>
        /// <param name="n">count of bytes to read</param>
        /// <returns>bytes read</returns>
        byte[] Read(int n);
    }
}
