using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using Sirenix.OdinInspector;

namespace SlotMaker.IoC
{
    [CreateAssetMenu(fileName = "New WaitUntil", menuName = "SlotMaker2/Slot/Command/WaitUntil")]
    public class WaitUntil : CommandAsset
    {
        [Serializable]
        public class Condition
        {
            [InlineEditor]
            public VariableAsset parameter;
            public AnimatorControllerParameterType parameterType = AnimatorControllerParameterType.Bool;

            [ShowIf("ActiveCompareMethod")]
            public CompareMethod compareMethod = CompareMethod.EqualTo;

            [ShowIf("ActiveBoolValue")]
            public bool boolValue;
            [ShowIf("ActiveIntValue")]
            public int intValue;
            [ShowIf("ActiveFloatValue")]
            public float floatValue;

            public bool Verify()
            {
                switch (parameterType)
                {
                case AnimatorControllerParameterType.Trigger:
                    return true;
                case AnimatorControllerParameterType.Bool:
                    return (bool)parameter.value == boolValue;
                case AnimatorControllerParameterType.Int:
                    return OperationUtils.Compare((int)parameter.value, intValue, compareMethod);
                case AnimatorControllerParameterType.Float:
                    return OperationUtils.Compare((float)parameter.value, floatValue, compareMethod, Mathf.Epsilon);
                }
                return true;
            }

            private bool ActiveCompareMethod() { return parameterType == AnimatorControllerParameterType.Int || parameterType == AnimatorControllerParameterType.Float; } 
            private bool ActiveBoolValue() { return parameterType == AnimatorControllerParameterType.Bool; }
            private bool ActiveIntValue() { return parameterType == AnimatorControllerParameterType.Int; }
            private bool ActiveFloatValue() { return parameterType == AnimatorControllerParameterType.Float; }
        }
        public List<Condition> conditions = new List<Condition>();

        [Serializable]
        public class WaitUntilCommand : Command<Component, WaitUntil>
        {
            protected override void OnUpdate(float deltaTime)
            {
                foreach (var condition in sharedCommand.conditions)
                {
                    if (!condition.Verify())
                        return;
                }

                EndCommand();
            }
        }

        public override Command Create()
        {
            return new WaitUntilCommand
            {
                sharedCommand = this
            };
        }
    }
}