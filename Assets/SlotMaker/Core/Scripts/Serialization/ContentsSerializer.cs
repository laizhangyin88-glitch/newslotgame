//#define NEW_NET0
using NodeCanvas.Framework;
using SlotMaker.Json;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

#if DEV
using SlotMaker.TestSuite;
#endif

namespace SlotMaker.Contents
{
    public enum ContentsRequestType
    {
        Enter = 0,
        SlotSpin = 1,
        SlotClaimBonus = 2,
        VideoPokerDeal = 3,
        VideoPokerDraw = 4,
        VideoPokerClaimBonus = 5,
        GambleStart = 6,
        GambleDeal = 7,
        GambleTake = 8,
        TicketedBonusStart = 9,
        TicketedBonusClaim = 10,
        KenoPlay = 11,
        KenoClaimBonus = 12,
        ContentsStore = 13,
    };

    public static class ContentsSerializer
    {
        private delegate void DeserializeDelegate(IBlackboard bb, string contents);

        private static DeserializeDelegate[] deserializer = new DeserializeDelegate[]{
            DeserializeEnter,
            DeserializeSlotSpin,
            DeserializeClaimBonus,
            DeserializeVideoPokerDeal,
            DeserializeVideoPokerDraw,
            DeserializeClaimBonus,
            DeserializeGambleStart,
            DeserializeGambleDeal,
            DeserializeGambleTake,
            DeserializeTicketedBonusStart,
            DeserializeTicketedBonusClaim,
            DeserializeKenoPlay,
            DeserializeClaimBonus,
            DeserializeContentsStore,
        };

        public static string Serialize(object value)
        {
            return BlackboardJson.SerializeObject(value);
        }

        private static string XOR(string input)
        {
            string decryptKey = "22fc2777c44d33fb9a0cc14a680e86c1";
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
                sb.Append((char)(input[i] ^ decryptKey[(i % decryptKey.Length)]));

            String result = sb.ToString();
            return result;
        }

        public static void Deserialize(IBlackboard bb, bool withDecryption = true)
        {
            int requestType = bb.GetValue<int>("requestType");
            string contents = bb.GetValue<string>("contents");

#if NEW_NET
            string decryptedContents = (withDecryption == false || contents.StartsWith("{")) ? contents : XOR(contents);
#else
            string decryptedContents = withDecryption ? XOR(contents) : contents;
#endif

            //object testObj = SlotSimpleJson.DeserializeObject(decryptedContents);
            //string testStr = JsonUtility.ToJson(testObj);
            //Debug.Log($"@A testObj = {testObj}  - {testStr}");

#if UNITY_EDITOR
            Debug.Log($"@A contents = {decryptedContents}");
#endif

            deserializer[requestType](bb, decryptedContents);
        }

        public static string SerializeEnter(int gameId)
        {
            return Serialize(new
            {
                gameId = gameId,
                gameVersion = ContentsVersionManager.Instance.GetCurrentGameVersion(),
                contentsVersion = ContentsVersionManager.Instance.GetContentsVersion(),
            });
        }

        public static string SerializeSlotSpin(int gameId, long betCredit, long extraBetCredit, object customData)
        {
            return Serialize(new
            {
                gameId = gameId,
                bet = betCredit,
                extraBet = extraBetCredit,
                customData = (customData is int) ? new { extraInt = (int)customData } : customData,
                contentsVersion = ContentsVersionManager.Instance.GetContentsVersion(),
#if DEV && !NEW_NET
                debugParam = TestSuiteManager.Instance.DebugParam
#endif
            });
        }

        public static string SerializeClaimBonus(string claimId, object customData)
        {
            return Serialize(new
            {
                uid = claimId,
                decisionInfo = new { selectedIndex = (int)customData },
                contentsVersion = ContentsVersionManager.Instance.GetContentsVersion(),
            });
        }

        public static string SerializeVideoPokerDeal(int gameId, long betCredit, int handCount, object customData)
        {
            return Serialize(new
            {
                gameId = gameId,
                betPerHand = betCredit,
                handCount = handCount,
                customData = customData,
                contentsVersion = ContentsVersionManager.Instance.GetContentsVersion(),
#if DEV && !NEW_NET
                debugParam = TestSuiteManager.Instance.DebugParam
#endif
            });
        }

        public static string SerializeVideoPokerDraw(int gameId, List<bool> helds, object customData)
        {
            return Serialize(new
            {
                gameId = gameId,
                helds = helds,
                customData = customData,
                contentsVersion = ContentsVersionManager.Instance.GetContentsVersion(),
            });
        }

        public static string SerializeKenoPlay(int gameId, long betPerTicket, long extraBetPerTicket, int ticketCount, List<List<int>> pickInfoList, object customData)
        {
            return Serialize(new
            {
                gameId = gameId,
                betPerTicket = betPerTicket,
                extraBetPerTicket = extraBetPerTicket,
                ticketCount = ticketCount,
                pickInfoList = pickInfoList,
                customData = customData,
                contentsVersion = ContentsVersionManager.Instance.GetContentsVersion(),
#if DEV && !NEW_NET
                debugParam = TestSuiteManager.Instance.DebugParam
#endif
            });
        }

        public static string SerializeGambleStart(string ticketId)
        {
            return Serialize(new
            {
                ticketId = ticketId,
                contentsVersion = ContentsVersionManager.Instance.GetContentsVersion(),
            });
        }

        public static string SerializeGambleDeal(object customData)
        {
            return Serialize(new
            {
                selectedIndex = (int)customData,
                contentsVersion = ContentsVersionManager.Instance.GetContentsVersion(),
            });
        }

        public static string SerializeGambleTake()
        {
            return Serialize(new
            {
                contentsVersion = ContentsVersionManager.Instance.GetContentsVersion(),
            });
        }

        public static string SerializeTicketedBonusStart(string ticketId)
        {
            return Serialize(new
            {
                ticketId = ticketId,
                contentsVersion = ContentsVersionManager.Instance.GetContentsVersion(),
            });
        }

        public static string SerializeTicketedBonusClaim(string ticketId)
        {
            return Serialize(new
            {
                ticketId = ticketId,
                contentsVersion = ContentsVersionManager.Instance.GetContentsVersion(),
            });
        }

        private static void DeserializeEnter(IBlackboard bb, string contents)
        {
            BlackboardJson.DeserializeObject(
                BlackboardUtils.GetOrCreateBlackboard(bb, "game"),
                contents,
                "Enter"
            );
        }

        private static void DeserializeSlotSpin(IBlackboard bb, string contents)
        {
#if DEV && !NEW_NET
            TestSuiteManager.Instance.DebugParam = string.Empty;
#endif
            BlackboardJson.DeserializeObject(
                bb,
                contents,
                "SlotSpin"
            );
        }

        private static void DeserializeClaimBonus(IBlackboard bb, string contents)
        {
            BlackboardJson.DeserializeObject(
                bb,
                contents,
                "ClaimBonus"
            );

            UpdateBonusResult(bb);
            UpdateGambleTicket(bb);
        }

        private static void DeserializeVideoPokerDeal(IBlackboard bb, string contents)
        {
#if DEV && !NEW_NET
            TestSuiteManager.Instance.DebugParam = string.Empty;
#endif
            BlackboardJson.DeserializeObject(
                bb,
                contents,
                "VideoPokerDeal"
            );

            UpdateHandMetaInfoForBet(bb);
        }

        private static void DeserializeVideoPokerDraw(IBlackboard bb, string contents)
        {
            BlackboardJson.DeserializeObject(
                bb,
                contents,
                "VideoPokerDraw"
            );

            UpdateHandMetaInfoForBet(bb);
            UpdateGambleTicket(bb);
        }

        private static void DeserializeKenoPlay(IBlackboard bb, string contents)
        {
#if DEV && !NEW_NET
            TestSuiteManager.Instance.DebugParam = string.Empty;
#endif
            BlackboardJson.DeserializeObject(
                bb,
                contents,
                "KenoPlay"
            );
        }

        private static void DeserializeGambleStart(IBlackboard bb, string contents)
        {
            BlackboardJson.DeserializeObject(
                bb,
                contents,
                "GambleStart"
            );
        }

        private static void DeserializeGambleDeal(IBlackboard bb, string contents)
        {
            BlackboardJson.DeserializeObject(
                bb,
                contents,
                "GambleDeal"
            );
        }

        private static void DeserializeGambleTake(IBlackboard bb, string contents)
        {
            BlackboardJson.DeserializeObject(
                bb,
                contents,
                "GambleTake"
            );
        }

        private static void DeserializeTicketedBonusStart(IBlackboard bb, string contents)
        {
            BlackboardJson.DeserializeObject(
                bb,
                contents,
                "TicketedBonus"
            );
        }

        private static void DeserializeTicketedBonusClaim(IBlackboard bb, string contents)
        {
            BlackboardJson.DeserializeObject(
                bb,
                contents,
                "TicketedBonusClaim"
            );
        }

        private static void DeserializeContentsStore(IBlackboard bb, string contents)
        {
            BlackboardJson.DeserializeObject(
                bb,
                contents,
                "ContentsStoreInfo"
            );
        }

        #region Post-Processing

        private static void UpdateBonusResult(IBlackboard bb)
        {
            var newBonusResult = bb.GetVariable<List<Blackboard>>("bonusResult");
            if (newBonusResult != null && newBonusResult.value.Count > 0)
            {
                var spin = ContentBlackboard.Get().GetValue<Blackboard>("spin");
                var bonusResult = BlackboardUtils.GetOrCreateBlackboard(spin, "response");
                for (int i = 0; i < newBonusResult.value.Count; ++i)
                {
                    BlackboardUtils.AddToBlackboardList(bonusResult, "bonusResult", newBonusResult.value[i]);
                }
            }
        }

        private static void UpdateHandMetaInfoForBet(IBlackboard bb)
        {
            var handMetaInfoPerBet = BlackboardUtils.FindValue<List<Blackboard>>("./game/handMetaInfoPerBet");
            var handMetaInfoForBet = bb.GetValue<Blackboard>("handMetaInfoForBet");
            long betPerHand = handMetaInfoForBet.GetValue<long>("betPerHand");
            foreach (var oldHandMetaInfoForBet in handMetaInfoPerBet)
            {
                if (oldHandMetaInfoForBet.GetValue<long>("betPerHand") == betPerHand)
                {
                    BlackboardUtils.CopyBlackboard(handMetaInfoForBet, oldHandMetaInfoForBet);
                    break;
                }
            }
        }

        private static void UpdateGambleTicket(IBlackboard bb)
        {
            var ticketBB = bb.GetVariable<Blackboard>("gambleTicket");
            if (ticketBB != null)
            {
                var turn = ContentBlackboard.Get().GetValue<Blackboard>("turn");
                BlackboardUtils.SetOrCreateValue<Blackboard>(turn, "gambleTicket", ticketBB.value);
            }
        }

        #endregion Post-Processing
    }
}
