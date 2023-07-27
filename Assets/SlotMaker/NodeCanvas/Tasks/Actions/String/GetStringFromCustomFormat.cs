using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Blackboard")]
public class GetStringFromCustomFormat : ActionTask<Blackboard>
{
    public BBParameter<string> format;
    public List<BBParameter<object>> argsList;

    public BBParameter<string> saveAs;

    protected override string info
    {
        get { return string.Format("{0} = string.Format(\"format\",argsList)", saveAs); }
    }

    protected override void OnExecute()
    {
        if(format != null && !string.IsNullOrEmpty(format.value))
        {
            List<object> args = new List<object>();
            if(argsList.Count > 0)
            {
                for(int i=0; i<argsList.Count; ++i)
                {
                    args.Add(argsList[i].value);
                }
            }

            saveAs.value = string.Format(format.value, args.ToArray());
        }

        EndAction();
    }
}

}
