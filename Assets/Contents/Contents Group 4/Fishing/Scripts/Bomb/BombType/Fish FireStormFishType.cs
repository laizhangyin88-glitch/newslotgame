using fishMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishFireStormFishType : FishBombBaseFish
    {
        public override void AddEventListener()
        {
             
        }

        public override void Destroy()
        {
            BaseDestroy();
        }

        public override void FixedScreenProcess(FreezeFishesRsp fishMsg)
        {
            
        }

        public override void RemoveEventListener()
        {
             
        }

        public override void RemoveFishPartProcess(FishPartFish fishIns, List<CrabPart> crabParts)
        {
        }

        public override void SetFishDieProcess(KillFishRsp hitFishMsg, FishFishBase fishIns, FishPlayerInfo playerIns)
        {
            BaseFishDie(fishIns, playerIns, hitFishMsg);
            BaseShakeOrVibrate(hitFishMsg, fishIns);
            SubFishDieProcess(fishIns, playerIns, hitFishMsg);
            BaseShowFishSpecialEffect(fishIns);
            Destroy();
        }

        public override void SubFishDieProcess(FishFishBase tempFish, FishPlayerInfo playerIns, KillFishRsp hitFishMsg)
        {
            if (hitFishMsg.SubFishes != null)
                for (int i = 0; i < hitFishMsg.SubFishes.Count; i++)
                    FishBombManager.Instance.CheckFishState(hitFishMsg.SubFishes[i]);
        }
    }
}
