using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{
    [Category("★ BagelCode/BI")]
    public class BI_common_energy : ActionTask<Blackboard>
    {
        public BBParameter<string> eventName;
        public BBParameter<long> energy;
        public BBParameter<List<string>> dataKeyList;
        public List<BBParameter<string>> dataObjectList;
        protected override string info
        {
            get
            {
                return string.Format("{0}(string only) + energy(long) = {1}", eventName, energy.value);
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
                    customData[dataKeyList.value[i]] = dataObjectList[i].value.ToString();
                }
            }
            customData["energy"] = energy.value;

            Analytics.CustomEvent(eventName.value, customData);

            EndAction();
        }
    }
}