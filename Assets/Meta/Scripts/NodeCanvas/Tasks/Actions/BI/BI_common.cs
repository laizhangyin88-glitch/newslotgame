using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{
    [Category("★ BagelCode/BI")]
    public class BI_common : ActionTask<Blackboard>
    {
        public BBParameter<string> eventName;
        public BBParameter<List<string>> dataKeyList;
        public List<BBParameter<string>> dataObjectList;

        protected override string info
        {
            get
            {
                string resultString = string.Format("BI Common {0}(string only)", eventName);

                if (dataKeyList.value != null
                    && dataKeyList.value.Count > 0
                    && dataObjectList != null
                    && dataObjectList.Count > 0
                    && dataKeyList.value.Count == dataObjectList.Count
                    )
                {
                    resultString += "\nData {";
                    for (int i = 0; i < dataKeyList.value.Count; ++i)
                    {
                        resultString += string.Format("{0}{1} = {2}", i == 0 ? "" : ", ", dataKeyList.value[i], dataObjectList[i].value == null ? dataObjectList[i] : dataObjectList[i].value);
                    }
                    resultString += "}";
                }

                return resultString;
            }
        }

        protected override void OnExecute()
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();

            if (dataKeyList.value != null && dataKeyList.value.Count > 0
                && dataObjectList != null && dataObjectList.Count > 0
                && dataKeyList.value.Count == dataObjectList.Count)
            {
                for (int i = 0; i < dataKeyList.value.Count; ++i)
                {
                    var value = dataObjectList[i].value;
                    if (value == null) customData[dataKeyList.value[i]] = null;
                    else customData[dataKeyList.value[i]] = dataObjectList[i].value.ToString();
                }
            }

            Analytics.CustomEvent(eventName.value, customData);

            EndAction();
        }
    }
}
