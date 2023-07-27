using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;
using BagelCode.Chat;

namespace BagelCode
{
    public class ProductUtils
    {
        public static Blackboard MakeBonusProduct(Blackboard actionBB, string itemType)
        {
            var gameId = actionBB.GetValue<int>("gameId");
            var ticketedBonusId = actionBB.GetValue<int>("ticketedBonusId");
            var bet = actionBB.GetValue<long>("bet");
            var extraBet = actionBB.GetValue<long>("extraBet");
            long totalBet = bet + extraBet;

            EventInfo bonusEventInfo = PassiveEventManager.Instance.GetBonusEventInfo(gameId);

            if (bonusEventInfo != null && bonusEventInfo.type == EventInfoType.BONUS_MULTIPLY)
            {
                long multiplierNumerator = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(bonusEventInfo);
                totalBet = IAMUtils.RemoveUnitOfDigit((bet + extraBet) * multiplierNumerator / NumberUtils.GetGlobalDenominator());
            }

            var gemProductID = string.Format("{0}|{1}|{2}", gameId, totalBet, ticketedBonusId);

            return MakeGemProduct(actionBB, gemProductID, itemType);
        }

        public static Blackboard MakeSlotUnlockProduct(Blackboard slotBB, long gemPrice)
        {
            var gemProductID = string.Format("slot_unlock|{0}", slotBB.GetValue<int>("gameID").ToString());
            var product = MakeGemProduct(slotBB, gemProductID, "slot_unlock");
            BlackboardUtils.SetOrCreateValue<long>(product, "gemPrice", gemPrice);

            return product;
        }

        public static Blackboard MakeSpeakerProduct(Blackboard bb, long speakerCount, long gemPrice)
        {
            // speaker|{global_chat_channel_id}|{purchased_speaker_count}
            var globalChannelID = ChatMetaManager.Instance.GetChannelID(ChannelType.Global);
            var gemProductID = string.Format("speaker|{0}|{1}", globalChannelID, speakerCount);
            var product = MakeGemProduct(bb, gemProductID, "speaker");
            BlackboardUtils.SetOrCreateValue<long>(product, "gemPrice", gemPrice);

            return product;
        }

        public static Blackboard MakeGemProduct(Blackboard parent, string gemProductId, string itemType)
        {
            Blackboard productBB = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(parent, "product");

            BlackboardUtils.SetOrCreateValue<string>(productBB, "gemProductId", gemProductId);
            BlackboardUtils.SetOrCreateValue<string>(productBB, "itemType", itemType);
            // productBB.transform.SetParent(parent.transform);

            // BlackboardUtils.SetOrCreateValue<Blackboard>(parent, "product", productBB);

            return productBB;
        }


        public static void MakeAEProduct(Blackboard productBB, string gemProductId, string itemType)
        {
            BlackboardUtils.SetOrCreateValue<string>(productBB, "gemProductId", gemProductId);
            BlackboardUtils.SetOrCreateValue<string>(productBB, "itemType", itemType);
        }
    }
}
