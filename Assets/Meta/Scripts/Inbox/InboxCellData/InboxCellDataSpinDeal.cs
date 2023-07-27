using System.Collections;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

namespace BagelCode
{
    public class InboxCellDataSpinDeal : InboxCellData
    {
        public InboxCellDataSpinDeal(InboxCellController _owner, Blackboard _inboxInfo)
            : base(_owner, _inboxInfo)
        {
            bool isChoice = inboxType == ClientModels.InboxTypes.SPIN_DEAL_V2;
            BlackboardUtils.SetOrCreateValue<bool>(inboxInfo, "isChoice", isChoice);
            gameId = inboxInfo.GetVariable<int>("gameId")?.value ?? -1;
        }

        public override IEnumerator InboxItemCheckExtraCoroutine()
        {
            gameId = inboxInfo.GetVariable<int>("gameId")?.value ?? -1;

            yield break;
        }

        public override IEnumerator OnAcceptSuccessCoroutine()
        {
            var responseBB = ownerBlackboard.GetValue<Blackboard>("response");
            bool isChoice = inboxInfo.GetValue<bool>("isChoice");

            // Make Spin Deal Control Popup
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = isChoice ? "Popup Choice Spins Deal Scene" :
                "Popup Control Spin Deal Select Bet & Spins Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject controlPopupObj = null;
            yield return cell.StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(bundle, asset, parent,
                (GameObject popupObj) => controlPopupObj = popupObj));

            var popupBB = controlPopupObj.GetComponent<Blackboard>();
            popupBB.AddVariable("inboxResponse", responseBB);
            popupBB.AddVariable("isChoice", isChoice);

            MetaObjectUtils.SetCalleeCaller(controlPopupObj, cell.gameObject);

            MetaPopupUtils.OpenPopup(controlPopupObj);

            // Wait Callback
            var callbackTrigger = new EventTrigger(cell, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);
        }

        protected override string GetInternalText()
        {
            gameId = BlackboardUtils.FindValue<int>(inboxInfo, "gameId");
            long totalBet = BlackboardUtils.FindValue<long>(inboxInfo, "totalBet");
            totalBet = LevelUtils.GetLevelMultiplierNumeratorValue(totalBet, "spinDeal");
            int spinCount = BlackboardUtils.FindValue<int>(inboxInfo, "spinCount");
            double multiplier = BlackboardUtils.FindValue<double>(inboxInfo, "multiplier");

            string gameTitleFormat = string.Format("GAME_TITLE_{0}", gameId.ToString());
            string gameTitle = StringTableUtils.GetString(GLOBAL, gameTitleFormat);

            string multiplierText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_SPIN_BOOST_MULTIPLIER", multiplier);

            string rewardText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_MESSAGE", title, message);
            TextDecoUtils.ConvertStringFormat(ref rewardText, "{spin_count}", spinCount.ToString("N0"));
            TextDecoUtils.ConvertStringFormat(ref rewardText, "{total_bet}", TextDecoUtils.ConvertCoinStyleText(totalBet));
            TextDecoUtils.ConvertStringFormat(ref rewardText, "{game_name}", gameTitle);
            TextDecoUtils.ConvertStringFormat(ref rewardText, "{multiplier}", multiplierText);

            return rewardText;
        }

        protected override string GetButtonText()
        {
            return StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_BUTTON_PLAY");
        }

        protected override IconType GetIconType()
        {
            return IconType.GAME_THUMBNAIL;
        }
    }
}
