using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Blackboard")]
public class GetStringFromStringTable : ActionTask<Blackboard>
{
    public BBParameter<string> key;
    public BBParameter<List<string>> argsList;
    public StringTable.StringTableType tableType;

    public BBParameter<string> saveAs;

    protected override string info
    {
        get { return string.Format("GetString({0}) save as {1}", key, saveAs); }
    }

    protected override void OnExecute()
    {
        bool error = true;
        object[] args = null;

        if(argsList.value != null && argsList.value.Count > 0)
        {
            args = new object[argsList.value.Count];

            for(int i=0; i<argsList.value.Count; ++i)
            {
                object argValue = BlackboardUtils.FindValue(agent, argsList.value[i]);
                if(argValue == null)
                {
                    args[i] = "";
                }
                else
                {
                    //args[i] = argValue.ToString();
                    args[i] = argValue;
                }
            }

            saveAs.value = StringTableUtils.GetString(tableType, key.value, out error, args);
        }
        else
        {
            saveAs.value = StringTableUtils.GetString(tableType, key.value, out error);
        }

        EndAction(!error);
    }
}

}
