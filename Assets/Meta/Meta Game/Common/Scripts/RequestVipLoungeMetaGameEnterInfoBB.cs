using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Common")]
    public class RequestVipLoungeMetaGameEnterInfoBB : ActionTask<Blackboard>
    {
        protected override string info
        {
            get { return string.Format("Request Meta Game Enter Info(VIP Lounge)"); }
        }

        protected override void OnExecute()
        {
            // Passive Event or Active mode open => always call api
            BagelCodeClientAPI.RequestVegasDreamInfo(
                (response) =>
                {
                    BlackboardQueryUtils.UpdateRequestVegasDreamInfo(response);
                    EndAction();
                },
                (error) =>
                {
                    if (agent != null)
                    {
                        //GlobalErrorHandler.GlobalError(error);
                        EndAction(false);
                    }
                });
        }
    }
}
