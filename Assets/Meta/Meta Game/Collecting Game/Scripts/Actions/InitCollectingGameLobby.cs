using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Scratcher.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Collecting Game")]
    public class InitCollectingGameLobby : ActionTask<ContextElement>
    {
        private GameObject badgeObj = null;
        private const string ON_OPEN_COLLECTING_GAME_LOADING_EVENT = "OnOpenCollectingGameLoading";

        protected override string info
        {
            get { return "Init Collecting Game Lobby"; }
        }

        protected override void OnExecute()
        {
            if(badgeObj != null)
                GameObject.Destroy(badgeObj);

            ContextElement badgeAreaElement = ContextUtils.FindElement(agent, "Badge Area", ContextSearchingType.ChildrenSearch);
            badgeObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Badge", badgeAreaElement.transform);
            badgeAreaElement.UpdateContext(true);
            MetaContextElementUtils.SetActive(badgeAreaElement, false);
            
            MetaContextElementUtils.SetClickable(
                agent,
                ON_OPEN_COLLECTING_GAME_LOADING_EVENT,
                false,
                false,
                SendEvent,
                ownerSystem
            );
            
            EndAction();
        }
    }
}
