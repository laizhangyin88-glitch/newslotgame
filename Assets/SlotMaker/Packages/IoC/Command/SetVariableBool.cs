using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.IoC
{
    [CreateAssetMenu(fileName = "New SetVariableBool", menuName = "SlotMaker2/Slot/Command/SetVariableBool")]
    public class SetVariableBool : CommandAsset
    {
        public VariableBool target;
        public bool boolValue;

        [Serializable]
        public class SetVariableBoolCommand : Command<Component, SetVariableBool>
        {
            protected override void OnExecute(float deltaTime)
            {
                sharedCommand.target.value = sharedCommand.boolValue;
                EndCommand();
            }
        }

        public override Command Create()
        {
            return new SetVariableBoolCommand
            {
                sharedCommand = this
            };
        }
    }
}