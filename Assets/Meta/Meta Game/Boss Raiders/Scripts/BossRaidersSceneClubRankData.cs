using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using UnityEngine;

namespace BagelCode.BossRaiders
{
    public class BossRaidersSceneClubRankData
    {
        private ContextElement caller;
        private ContextElement clickElement;
        public long clubId = 0;

        public void OnInit(ContextElement rootElement)
        {
            clickElement = ContextUtils.FindElement(rootElement, "Cell Click Area", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetClickable(clickElement, ClickItem);
        }

        public void SetCaller(ContextElement _caller)
        {
            caller = _caller;
        }

        private void ClickItem()
        {
            Debug.Log("Click Club : " + clubId);
            if (clubId > 0)
                MetaContextElementUtils.SendEvent(clickElement, "OnClickClubInfo", clubId, caller, null);
        }
    }
}