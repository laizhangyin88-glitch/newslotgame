using UnityEngine;
using NodeCanvas.Framework;
using System.Collections;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/VIP Lounge")]
    public class CheckWelcomeVipLounge : ActionTask<Blackboard>
    {
        public BBParameter<bool> isNotVIPLounge;

        protected override string info { get { return "CheckWelcomeVipLounge"; } }

        protected override void OnExecute()
        {
            StartCoroutine(ExecuteCoroutine());
        }

        private IEnumerator ExecuteCoroutine()
        {
            if (BlackboardQueryUtils.IsVipLoungeEnabled())
            {
                if (SlotMaker.ApplicationSettings.LogTest())
                    Debug.Log("CheckWelcomeVipLounge");

                if (isNotVIPLounge.value == false)
                {
                    var callbackTrigger = new EventTrigger(agent.gameObject, VipLounge.VipLounge.Events.ON_CLOSE_WELCOME_POPUP);
                    var eventData = new EventData<GameObject>(VipLounge.VipLounge.Events.CHECK_VIP_LOUNGE_OPEN, agent.gameObject);
                    EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, eventData);

                    yield return new WaitUntilTrigger(callbackTrigger);
                }
            }

            EndAction();
        }
    }
}
