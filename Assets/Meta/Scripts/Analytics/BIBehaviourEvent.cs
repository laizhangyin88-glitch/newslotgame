using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode
{
    public class BIBehaviourEvent : MonoBehaviour
    {
        public void client_click_max_bet()
        {
            int gameId       = BlackboardUtils.FindVariable<int>("./game/gameId").value;
            long maxBetCredit = BlackboardUtils.FindVariable<long>("./maxBetCredit").value;

            Analytics.CustomEvent("client_click_max_bet", new Dictionary<string, object>
            {
                { "game_id", gameId },
                { "bet", maxBetCredit }
            });
        }
    }
}
