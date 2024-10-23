
namespace CryPrinter
{
       
    [System.Flags]
    public enum ReturnCode
    {
        Success = 0,

        // Connection issues
        ConnectionAlreadyOpen,
        ConnectionNotFound,

        // Syntax issues
        UnsupportedCommand,
        InvalidArgument,

        // General failures
        ExecutionFailure,
    }
}
