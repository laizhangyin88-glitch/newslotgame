using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Scratcher.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Collecting Game")]
    public class UpdateCollectingGameLobby : ActionTask<ContextElement>
    {
        protected override string info
        {
            get { return "Update Collecting Game Lobby"; }
        }

        protected override void OnExecute()
        {
            Blackboard metaGameEnterInfo = BlackboardQueryUtils.GetMetaGameEnterInfo();
            ContextElement badgeAreaElement = ContextUtils.FindElement(agent, "Badge Area", ContextSearchingType.ChildrenSearch);

            if (metaGameEnterInfo != null)
            {
                EventInfoType type = metaGameEnterInfo.GetValue<EventInfoType>("type");
                if (type == EventInfoType.COLLECTING_GAME)
                {
                    int possessions = metaGameEnterInfo.GetValue<int>("totalPossessions");
                    int completeScratchers = metaGameEnterInfo.GetValue<int>("completeScratchers");
                    int totalCount = possessions + completeScratchers;
                    
                    ContextElement badgeElement = ContextUtils.FindElement(badgeAreaElement, "Badge", ContextSearchingType.ChildrenSearch);
                    ContextElement tabBadgeTextElement = ContextUtils.FindElement(badgeElement, "Text", ContextSearchingType.ChildrenSearch);
                    
                    if (totalCount > 99)
                        MetaContextElementUtils.SetText(tabBadgeTextElement, "99+");
                    else 
                        MetaContextElementUtils.SetText(tabBadgeTextElement, totalCount.ToString());
                    
                    MetaContextElementUtils.SetActive(badgeAreaElement, true);
    
                    ContextAnimator contextAnimator = badgeElement as ContextAnimator;
                    contextAnimator.isPreserve = true;
                    contextAnimator.SetIntProperty(totalCount);
                }
                else
                {
                    MetaContextElementUtils.SetActive(badgeAreaElement, false);
                }
            }
            else
            {
                MetaContextElementUtils.SetActive(badgeAreaElement, false);
            }
            EndAction();
            
        }
    }
}
