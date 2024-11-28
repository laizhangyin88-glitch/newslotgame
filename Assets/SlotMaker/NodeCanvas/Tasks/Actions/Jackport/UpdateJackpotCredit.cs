using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/JackpotChase")]
    public class UpdateJackpotCredit : ActionTask
    {
        public BBParameter<ContextElement> creditText;

        public BBParameter<double> jackpotMultiplier;
        public BBParameter<long>  prev;
        public BBParameter<long>  current;
        public BBParameter<float> endTime;
        public BBParameter<bool>  isBetChanged;

        public BBParameter<string> key;
        public StringTable.StringTableType tableType;
        public IContextText property;

        private string textFormat = null;

        protected override void OnExecute()
        {
            property = creditText.value as IContextText; 
            textFormat = StringTableUtils.GetVariable(tableType, key.value).value;

            var jackpotCredit = ownerAgent.GetComponent<ContentJackpotCredit>();
            jackpotCredit.Reset(property, textFormat,
                prev.value, current.value, endTime.value, jackpotMultiplier.value, isBetChanged.value);

            isBetChanged.value = false;

            EndAction();
        }
    }
}
