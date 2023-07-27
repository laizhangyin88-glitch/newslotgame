using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker.IoC;

namespace SlotMaker.Slots
{
    [CreateAssetMenu(fileName="New ApplyPatch", menuName="SlotMaker2/Slot/Command/ApplyPatch")]
    public class ApplyPatch : CommandAsset
    {
        public bool patchFront = true;

        [Serializable]
        public class ApplyPatchCommand : Command<ReelInstance, ApplyPatch>
        {
            protected override void OnExecute(float deltaTime)
            {
                if (sharedCommand.patchFront)
                    agent.ApplyFrontPatch();
                else 
                    agent.ApplyBackPatch();
                
                EndCommand();
            }
        }

        public override Command Create()
        {
            return new ApplyPatchCommand {
                sharedCommand = this
            };
        }
    }
}