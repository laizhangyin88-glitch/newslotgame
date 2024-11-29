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
    public class GetIAMBackupChangeScene : ActionTask<Blackboard>
    {
        public BBParameter<ActionChangeSceneType> changeSceneType;
        public BBParameter<List<InAppMessageTriggerType>> checkTriggerTypes;

        public BBParameter<string> saveValue;
        public BBParameter<string> saveMessage;
        public BBParameter<bool> isRemove;

        protected override string info
        {
            get { return string.Format("Get {0} IAM Change Scene Backup", changeSceneType); }
        }

        protected override void OnExecute()
        {
            var iamBackupChangeScene = BlackboardUtils.GetOrCreateVariable<Dictionary<ActionChangeSceneType, KeyValuePair<InAppMessageTriggerType, string>>>(MainBlackboard.Get(), "iamBackupChangeScene");

            saveValue.value = "";
            saveMessage.value = "";

            if (iamBackupChangeScene == null || iamBackupChangeScene.value == null || iamBackupChangeScene.value.Count == 0)
            {
                EndAction();
            }

            else if (iamBackupChangeScene.value.ContainsKey(changeSceneType.value))
            {
                var pairData = iamBackupChangeScene.value[changeSceneType.value];

                if (checkTriggerTypes != null && checkTriggerTypes.value != null)
                {
                    for (int i = 0; i < checkTriggerTypes.value.Count; ++i)
                    {
                        if (pairData.Key == checkTriggerTypes.value[i])
                        {
                            saveValue.value = pairData.Value;
                            SetStringMessage(pairData.Key);
                        }
                    }

                }


                if (isRemove.value)
                    iamBackupChangeScene.value.Remove(changeSceneType.value);
            }

            EndAction();
        }

        private void SetStringMessage(InAppMessageTriggerType triggerType)
        {
            switch (triggerType)
            {
                case InAppMessageTriggerType.INHOUSE_ADS_FOR_CLUB_ARENA:
                    saveMessage.value = "club_arena";
                    break;
                default:
                    break;
            }
        }
    }
}