using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

namespace SlotMaker
{
    public static class ContentBlackboardUtils
    {
        public static void AddTurnCount(IBlackboard bb)
        {
            var turnCount = BlackboardUtils.GetOrCreateVariable<int>(null, "/turnCount");
            turnCount.value += 1;

            turnCount = BlackboardUtils.GetOrCreateVariable<int>(null, "./game/turnCount");
            turnCount.value += 1;

            BlackboardUtils.SetOrCreateValue<int>(bb, "turnIndex", turnCount.value - 1);
        }

        public static void AddEarnCredit(IBlackboard bb, long earnCredit)
        {
            bb.SetValue("earnCredit", bb.GetValue<long>("earnCredit") + earnCredit);
            bb.SetValue("singleCredit", bb.GetValue<long>("singleCredit") + earnCredit);

            var parent = bb.GetVariable<Blackboard>("parent");
            while (parent != null)
            {
                parent.value.GetVariable<long>("earnCredit").value += earnCredit;
                parent = parent.value.GetVariable<Blackboard>("parent");
            }

            BlackboardUtils.FindVariable<long>(null, "./game/earnCredit").value += earnCredit;
        }

        public static void AddSpentCredit(IBlackboard bb, long spentCredit)
        {
            BlackboardUtils.GetOrCreateVariable<long>(bb, "spentCredit").value += spentCredit;
            BlackboardUtils.FindVariable<long>(null, "./game/spentCredit").value += spentCredit;

            MetaSystem.SpentCredit(spentCredit);
        }

        public static Blackboard GetBonusResponse(Blackboard spin, int id)
        {
            var bonusList = BlackboardUtils.FindVariable<List<Blackboard>>(spin, "response/bonusResult")?.value;
            if (bonusList != null)
            {
                for (int i = 0; i < bonusList.Count; ++i)
                {
                    int bonusId = bonusList[i].GetValue<int>("bonusId");
                    if (id == bonusId)
                    {
                        return bonusList[i];
                    }
                }
            }
#if DEV
            // Prints error log so that we can detect this case which ./spin/response/bonusResult is null or not exist, 
            // but we want to do so only in development builds. 
            else
                Debug.LogError($"GetBonusResponse has called for bonus ID: {id} but there is no \"response/bonusResult\" on \"./spin\"!", spin);
#endif
            return null;
        }

        public static Blackboard GetBonusResponse1(Blackboard spin, string uid)
        {
            var bonusList = BlackboardUtils.FindVariable<List<Blackboard>>(spin, "response/bonusResult").value;
            for (int i = 0; i < bonusList.Count; ++i)
            {
                string bonusUid = bonusList[i].GetValue<string>("uid");
                if (bonusUid.Equals(uid))
                {
                    return bonusList[i];
                }
            }
            return null;
        }

        public static List<Blackboard> GetBonusResponseList(Blackboard spin, int id)
        {
            var result = new List<Blackboard>();
            var bonusList = BlackboardUtils.FindVariable<List<Blackboard>>(spin, "response/bonusResult").value;
            for (int i = 0; i < bonusList.Count; ++i)
            {
                int bonusId = bonusList[i].GetValue<int>("bonusId");
                if (id == bonusId) result.Add(bonusList[i]);
            }
            return result;
        }

        public static Blackboard GetNextUnusedBonusResponse(Blackboard spin)
        {
            var bonusList = BlackboardUtils.FindVariable<List<Blackboard>>(spin, "response/bonusResult").value;
            for (int i = 0; i < bonusList.Count; ++i)
            {
                var isUsed = BlackboardUtils.GetOrCreateVariable<bool>(bonusList[i], "used");
                if (!isUsed.value)
                {
                    return bonusList[i];
                }
            }
            return null;
        }

        public static Blackboard GetNextUnusedBonusResponse(Blackboard spin, int id)
        {
            var bonusList = BlackboardUtils.FindVariable<List<Blackboard>>(spin, "response/bonusResult").value;
            for (int i = 0; i < bonusList.Count; ++i)
            {
                int bonusId = bonusList[i].GetValue<int>("bonusId");
                if (id != bonusId) {
                    continue;
                }

                var isUsed = BlackboardUtils.GetOrCreateVariable<bool>(bonusList[i], "used");
                if (!isUsed.value)
                {
                    return bonusList[i];
                }
            }
            return null;
        }

        public static Variable<T> GetSpinResponseValue<T>(string key)
        {
            var spin = BlackboardUtils.FindVariable<Blackboard>(null, "./spin");
            if(spin == null)
            {
                return null;
            }

            var response = spin.value.GetValue<Blackboard>("response");
            var value = BlackboardUtils.FindVariable<T>(response, key);
            if(value == null)
            {
                return null;
            }
            return value;
        }
    }
}
