using System;
using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{

    [Category("★ BagelCode/ClientAPI")]
    public class RequestInAppMessasgeInfo : ActionTask<Blackboard>
    {
        public BBParameter<int> requestIAMID;

        public BBParameter<InAppMessageTriggerType> saveAstype;
        public BBParameter<Blackboard> saveIamInfo;
        
        protected override string info
        {
            get { return String.Format("Request Iam Info Of {0}", requestIAMID); }
        }

        protected override void OnExecute()
        {
            BagelCodeClientAPI.InAppMessageInfoRequest(requestIAMID.value,  
                (response) =>
                {
                    if(agent != null)
                    {

                        if(response.inAppMessage != null)
                        {
                            BlackboardQueryUtils.RemoveIAMBlackboardById(requestIAMID.value);
                            
                            BlackboardQueryUtils.AddInAppMessage(response.inAppMessage);
                            
                            IAMRouter.Instance.UpdateIAMInfo();
                            
                            if (response.inAppMessage.triggerV2List.Count > 0)
                            {
                                saveAstype.value = response.inAppMessage.triggerV2List[0].type;
                            }
                            else
                            {
                                saveAstype.value = InAppMessageTriggerType.UNKNOWN;
                            }
                            
                            saveIamInfo.value = BlackboardQueryUtils.GetIAMBlackboard(requestIAMID.value);
                        }
                        else
                        {
                            saveAstype.value = InAppMessageTriggerType.UNKNOWN;
                        }

                        EndAction();
                    }
                },
                (error) =>
                {
                    if(agent != null)
                        GlobalErrorHandler.GlobalError(error);
                });
        }
    }

}
