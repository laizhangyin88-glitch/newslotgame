using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Scratcher.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Collecting Game")]
    public class RequestCollectingGameInfo : ActionTask
    {
        protected override string info
        {
            get { return "Request Collecting Game Info"; }
        }

        protected override void OnExecute()
        {
            EventInfo metaGameInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.COLLECTING_GAME);
            if (metaGameInfo == null) {
                EndAction();
            }
            MetaGameUtils.RequestCollectingGameInfo(() => {
                EndAction();
            });
        }
    }
}