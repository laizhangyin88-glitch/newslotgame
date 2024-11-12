using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{

    [Category("★ BagelCode/ClientAPI")]

    public class UserReport : ActionTask<Blackboard>
    {
        public BBParameter<string> userId;
        public BBParameter<BlockType> blockType;

        protected override string info
        {
            get
            {
                return string.Format("Block {0} {1}", userId, blockType);
            }
        }

        protected override void OnExecute()
        {
            if (userId.value != null)
            {
                BagelCodeClientAPI.UserBlock(userId.value, blockType?.value ?? BlockType.USER,
                (response) =>
                {
                    if (response.error == ClientModels.Error.OK)
                    {
                        BlackboardQueryUtils.AppendBlockedUserID(userId.value);
                        EndAction(true);
                    }
                    else
                    {
                        Debug.LogError(response.error);
                        EndAction(false);
                    }
                },
                (error) =>
                {
                    EndAction(false);
                });
            }
            else
            {
                Debug.LogError("No UserId found." + agent.gameObject.name);
            }
        }
    }
}
