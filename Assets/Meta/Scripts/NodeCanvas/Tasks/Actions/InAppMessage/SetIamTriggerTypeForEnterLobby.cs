using ParadoxNotion.Design;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode.Task.Actions
{
    [Category("★ BagelCode/IAM")]
    public class SetIamTriggerTypeForEnterLobby : ActionTask<Blackboard>
    {
        public BBParameter<string> typeName;
        public BBParameter<InAppMessageTriggerType> saveAs;

        protected override string info
        {
            get { return "SetIamTriggerTypeForEnterLobby"; }
        }

        protected override void OnExecute()
        {
            var enterLobbyIamTriggerTypeVarriable = BlackboardUtils.GetOrCreateVariable<InAppMessageTriggerType>(
                agent, typeName.value);

            if (enterLobbyIamTriggerTypeVarriable.value != InAppMessageTriggerType.UNKNOWN)
                saveAs.value = enterLobbyIamTriggerTypeVarriable.value;

            enterLobbyIamTriggerTypeVarriable.value = InAppMessageTriggerType.UNKNOWN;

            EndAction();
        }
    }
}
