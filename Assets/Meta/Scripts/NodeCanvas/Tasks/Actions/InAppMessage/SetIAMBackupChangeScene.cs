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
    public class SetIAMBackupChangeScene : ActionTask<Blackboard>
    {
        public BBParameter<ActionChangeSceneType> changeSceneType;
        public BBParameter<InAppMessageTriggerType> triggerType;
        public BBParameter<List<InAppMessageTriggerType>> checkTriggerTypes;

        public BBParameter<string> saveMessage;
        public BBParameter<bool> isRemove;

        protected override string info
        {
            get { return string.Format("Set {0} IAM Change Scene Backup", changeSceneType); }
        }

        protected override void OnExecute()
        {
            var iamBackupChangeScene = BlackboardUtils.GetOrCreateVariable<Dictionary<ActionChangeSceneType, KeyValuePair<InAppMessageTriggerType, string>>>(MainBlackboard.Get(), "iamBackupChangeScene");

            if (iamBackupChangeScene == null || iamBackupChangeScene.value == null)
                iamBackupChangeScene.value = new Dictionary<ActionChangeSceneType, KeyValuePair<InAppMessageTriggerType, string>>();

            if (isRemove.value == true)
            {
                if (iamBackupChangeScene.value.Count > 0 && iamBackupChangeScene.value.ContainsKey(changeSceneType.value))
                    iamBackupChangeScene.value.Remove(changeSceneType.value);
            }

            if (changeSceneType.value != ActionChangeSceneType.UNKNOWN &&
                triggerType.value != InAppMessageTriggerType.UNKNOWN &&
                checkTriggerTypes != null &&
                checkTriggerTypes.value != null)
            {
                int typesCount = checkTriggerTypes.value.Count;
                for (int i = 0; i < typesCount; ++i)
                {
                    if (triggerType.value == checkTriggerTypes.value[i])
                    {
                        var data = new KeyValuePair<InAppMessageTriggerType, string>(checkTriggerTypes.value[i], saveMessage.value);
                        if (iamBackupChangeScene.value.ContainsKey(changeSceneType.value))
                            iamBackupChangeScene.value[changeSceneType.value] = data;
                        else
                            iamBackupChangeScene.value.Add(changeSceneType.value, data);

                        break;
                    }
                }
            }

            EndAction();
        }
    }
}