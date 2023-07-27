using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using SlotMaker.TestSuite;

namespace BagelCode.GemJackpot
{
    [Category("★ BagelCode/Meta Games/Gem Jackpot")]
    public class RequestGemJackpotEnter : ActionTask
    {
        public BBParameter<bool> saveAsSuccess;

        protected override string info
        {
            get { return "Request Gem Jackpot Enter"; }
        }

        protected override void OnExecute()
        {
            saveAsSuccess.value = false;

            EventInfo metaGameInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.GEM_JACKPOT);
            if (metaGameInfo == null)
            {
                EndAction(false);
                return;
            }

            BagelCodeClientAPI.RequestGemJackpotEnter(metaGameInfo.id,
                (response) =>
                {
                    var bb = GemJackpotUtils.GemJackpotInfo;
                    ClientAPI2Blackboard.Serialize(bb, response);

                    BlackboardUtils.SetOrCreateValue<int>(bb, "prevProgress", 0);
                    BlackboardUtils.SetOrCreateValue<int>(bb, "nextProgress", 0);

                    GemJackpotUtils.MaxFreeSpinCount = BlackboardUtils.FindValue<int>(bb, "freeSpinCount");
                    GemJackpotUtils.PrevGrandJackpotMultiplyNumerator = GemJackpotUtils.GrandJackpotMultiplyNumerator;

                    CreateDebugSpin();

                    saveAsSuccess.value = true;
                    EndAction();
                },
                (error) =>
                {
                    switch (error.errorCode)
                    {
                        case ClientModels.Error.INVALID_GEM_JACKPOT_REQUEST_ERROR:
                            EndAction(false);
                            break;
                        default:
                            GlobalErrorHandler.GlobalError(error);
                            break;
                    }
                });
        }

        protected void CreateDebugSpin()
        {
            var bb = GemJackpotUtils.GemJackpotInfo;
            var debugSpinsBB = BlackboardUtils.GetOrCreateBlackboard(bb, "debugSpins");

            List<DebugSpin> debugSpins = new List<DebugSpin>();

            // GemJackpotDebugSpinType
            debugSpins.Add(new DebugSpin() { code = (int)GemJackpotDebugSpinType.NONE,                      description = "NONE",                   DebugSequenceList = null });
            debugSpins.Add(new DebugSpin() { code = (int)GemJackpotDebugSpinType.NO_PROGRESSION,            description = "NO_PROGRESSION",         DebugSequenceList = null });
            debugSpins.Add(new DebugSpin() { code = (int)GemJackpotDebugSpinType.NEXT_ONE_PROGRESSION,      description = "NEXT_ONE_PROGRESSION",   DebugSequenceList = null });
            debugSpins.Add(new DebugSpin() { code = (int)GemJackpotDebugSpinType.NEXT_TWO_PROGRESSION,      description = "NEXT_TWO_PROGRESSION",   DebugSequenceList = null });
            debugSpins.Add(new DebugSpin() { code = (int)GemJackpotDebugSpinType.NEXT_THREE_PROGRESSION,    description = "NEXT_THREE_PROGRESSION", DebugSequenceList = null });
            debugSpins.Add(new DebugSpin() { code = (int)GemJackpotDebugSpinType.NEXT_FOUR_PROGRESSION,     description = "NEXT_FOUR_PROGRESSION",  DebugSequenceList = null });
            debugSpins.Add(new DebugSpin() { code = (int)GemJackpotDebugSpinType.NEXT_FIVE_PROGRESSION,     description = "NEXT_FIVE_PROGRESSION",  DebugSequenceList = null });
            debugSpins.Add(new DebugSpin() { code = (int)GemJackpotDebugSpinType.MINI_JACKPOT,              description = "MINI_JACKPOT",           DebugSequenceList = null });
            debugSpins.Add(new DebugSpin() { code = (int)GemJackpotDebugSpinType.MINOR_JACKPOT,             description = "MINOR_JACKPOT",          DebugSequenceList = null });
            debugSpins.Add(new DebugSpin() { code = (int)GemJackpotDebugSpinType.MAJOR_JACKPOT,             description = "MAJOR_JACKPOT",          DebugSequenceList = null });
            debugSpins.Add(new DebugSpin() { code = (int)GemJackpotDebugSpinType.GRAND_JACKPOT,             description = "GRAND_JACKPOT",          DebugSequenceList = null });

            BlackboardUtils.SetOrCreateValue<List<DebugSpin>>(debugSpinsBB, "list", debugSpins);
        }
    }
}