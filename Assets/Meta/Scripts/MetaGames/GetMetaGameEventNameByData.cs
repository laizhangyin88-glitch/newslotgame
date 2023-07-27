using NodeCanvas.Framework;
using ParadoxNotion.Design;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/MetaGames")]

    public class GetMetaGameEventNameByData : ActionTask <Blackboard> 
    {
        public BBParameter<string> eventInfoTypeValue;
        public BBParameter<string> seasonIndexValue;

        public BBParameter<string> saveAsEventName;

        protected override string info
        { 
            get 
            { 
                return string.Format("Get Meta Game Event Name({0}, {1})", eventInfoTypeValue, seasonIndexValue);
            } 
        }

        protected override void OnExecute()
        {
            var eventInfoType = BlackboardUtils.FindVariable<EventInfoType>(agent, eventInfoTypeValue.value);
            var seasonIndex = BlackboardUtils.FindVariable<int>(agent, seasonIndexValue.value);

            saveAsEventName.value = BlackboardQueryUtils.GetMetaGameEventName(eventInfoType.value, seasonIndex.value);

            EndAction();
        }
    }

}
