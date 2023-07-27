using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;

namespace BagelCode.Task.Actions
{

    [Category("★ BagelCode/IAM")]
    public class GetIAMByID : ActionTask<Blackboard>
    {
        public BBParameter<int> iamID;

        public BBParameter<Blackboard> saveIamInfo;
        public BBParameter<InAppMessageTriggerType> saveAstype;
        public BBParameter<bool> saveIsExist;

        protected override string info
        {
            get { return string.Format("{0} = Get IAM Info by ID : {1}", saveIamInfo, iamID); }
        }

        protected override void OnExecute()
        {
            var iamInfo = BlackboardQueryUtils.GetIAMBlackboard(iamID.value);

            if (iamInfo != null)
            {
                saveIamInfo.value   = iamInfo;
                saveIsExist.value   = true;

                var triggerV2List = BlackboardUtils.GetOrCreateBlackboardList(iamInfo, "triggerV2List");
                if(triggerV2List.Count > 0)
                {
                    saveAstype.value = triggerV2List[0].GetValue<InAppMessageTriggerType>("type");
                }
                else
                {
                    saveAstype.value = InAppMessageTriggerType.UNKNOWN;
                }
            }
            else
            {
                saveIamInfo.value   = null;
                saveIsExist.value   = false;
            }

            EndAction();
        }
    }

}
