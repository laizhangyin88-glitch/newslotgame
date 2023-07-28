using System.Collections.Generic;
using System.Diagnostics;

namespace GameStudio.Slot.TRL.Utillity
{
    public class TRLLogger
    {
        static private Queue<string> logStack = new Queue<string>();

        static public string GetLogStackString()
        {
            string outputString = string.Empty;
            StackTrace st = new StackTrace(new StackFrame(true));
            for (int i = 0; i < st.FrameCount; i++)
            {
                outputString += $"Stack Trace Index {i}\n";
                StackFrame sf = st.GetFrame(i);
                outputString += $"[{sf.GetFileName()}.{sf.GetFileLineNumber() / sf.GetFileColumnNumber()}] - {sf.GetMethod()}\n";
            }
            return outputString;
        }
    }
}