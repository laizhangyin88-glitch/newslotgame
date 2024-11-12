using UnityEngine;
using System;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context/Debug")]
public class LogMessage : ActionTask
{
    public BBParameter<string> logMessage;
    public BBParameter<bool> isError;

    protected override string info
    {
        get { return string.Format("Debug Log({0})", logMessage); }
    }

    protected override void OnExecute()
    {
#if DEV
        if(isError.value)
        {
            Debug.LogError( string.Format("[Action][Debug] : {0}", logMessage.value) );
        }
        else
        {
            Debug.Log( string.Format("[Action][Debug] : {0}", logMessage.value) );
        }
#endif
        EndAction();
    }
}

}
