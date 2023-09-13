using UnityEngine;
using BagelCode.ClientModels;
using NodeCanvas.Framework;

namespace BagelCode
{
    public class EnterGameInfoBehaviour : MonoBehaviour
    {
        public int gameId;
        public string enterType;
        public string fromType;
        public string targetRoomId;

        public int slotStatus;

        public void SetEnterGameInfo()
        {
            if (slotStatus == 0)
            {
                if (gameId != 0)
                {
                    BlackboardQueryUtils.SetEnterGameInfo(gameId, enterType, fromType, targetRoomId, 0, null, false);
                    //if (gameId == 8)
                   if (gameId == 219)
                        gameObject.SendMessage("SendNow", "OnEnterFishRoom");
                    else
                        gameObject.SendMessage("SendNow", "OnEnterGame");
                }
            }
            else if (slotStatus == 5)
            {
                if (BlackboardQueryUtils.IsEarlyAccessAvailable())
                {
                    // Change Scene.........
                    BICustomEvents.EarlyAccessEnter("slot_list");
                    EventSender.SendGlobalEvent("OnEnterEarlyAccess");
                }
                else
                {
                    BagelCode.IAMRouter.Instance.TriggerIAM(InAppMessageTriggerType.EARLY_ACCESS_NON_SUBSCRIBER_SLOT_ENTER, null, "");
                }
            }
        }
    }
}
