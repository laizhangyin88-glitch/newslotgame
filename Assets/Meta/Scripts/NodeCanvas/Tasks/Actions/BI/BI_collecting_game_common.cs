using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_collecting_game_common : ActionTask<Blackboard>
{
    public BBParameter<string> eventName;
    public BBParameter<List<string>> dataKeyList;
    public List<BBParameter<string>> dataObjectList;

    protected override string info
    {
        get
        {
            string resultString = string.Format("BI Collecting Game Common {0}(string only)", eventName);

            if( dataKeyList.value != null
                && dataKeyList.value.Count > 0
                && dataObjectList != null
                && dataObjectList.Count > 0
                && dataKeyList.value.Count == dataObjectList.Count
                )
            {
                resultString += "\nData {";
                for(int i=0; i < dataKeyList.value.Count; ++i)
                {
                    resultString += string.Format("{0}{1} = {2}", i==0?"":", ", dataKeyList.value[i], dataObjectList[i].value == null ? dataObjectList[i] : dataObjectList[i].value);
                }
                resultString += "}";
            }

            return resultString;
        }
    }

    protected override void OnExecute()
    {
        Dictionary<string, object> customData = new Dictionary<string, object>();

        if( dataKeyList.value != null && dataKeyList.value.Count > 0
            && dataObjectList != null && dataObjectList.Count > 0
            && dataKeyList.value.Count == dataObjectList.Count )
        {
            for(int i=0; i < dataKeyList.value.Count; ++i)
            {
                customData[dataKeyList.value[i]] = dataObjectList[i].value.ToString();
            }
        }

        BiEventUtils.AppendCollectingGameEventData(customData);

        Analytics.CustomEvent(eventName.value, customData);

        EndAction();
    }
}

}
