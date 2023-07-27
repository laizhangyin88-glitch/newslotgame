using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Scratcher.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Collecting Game")]
    public class UpdateCollectingGameInGame : ActionTask<ContextElement>
    {
        public BBParameter<ContextElement> collectingGameButtonElement;

        protected override string info
        {
            get{ return "Update Collecting Game In Game"; }
        }

        protected override void OnExecute ()
        {
            Blackboard metaGameEnterInfo = BlackboardQueryUtils.GetMetaGameEnterInfo();
            if (metaGameEnterInfo != null)
            {
                int possessions = metaGameEnterInfo.GetValue<int>("totalPossessions");
                int completeScratchers = metaGameEnterInfo.GetValue<int>("completeScratchers");
                int totalCount = possessions + completeScratchers;
                
                ContextElement badgeElement = ContextUtils.FindElement(collectingGameButtonElement.value, "Badge Area/Badge", ContextSearchingType.FullNameSearch);

                ContextElement tabBadgeTextElement = ContextUtils.FindElement(badgeElement, "Text", ContextSearchingType.ChildrenSearch);
                if (totalCount > 99)
                    MetaContextElementUtils.SetText(tabBadgeTextElement, "99+");
                else 
                    MetaContextElementUtils.SetText(tabBadgeTextElement, totalCount.ToString());

                ContextAnimator contextAnimator = badgeElement as ContextAnimator;
                contextAnimator.isPreserve = true;
                contextAnimator.SetIntProperty(totalCount);
                    
                collectingGameButtonElement.value.GetComponent<Animator>().SetBool("IsBadge", true);
            }
            EndAction();
        }
    }
}