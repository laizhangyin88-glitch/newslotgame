using fishMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishSpawnType : FishBombBaseFish
    {

        public override void SetFishDieProcess(KillFishRsp hitFishMsg, FishFishBase fishIns, FishPlayerInfo playerIns)
        {
            BaseFishDie(fishIns, playerIns, hitFishMsg);
            BaseShakeOrVibrate(hitFishMsg, fishIns);
            BaseShowPlusTipsEffect(fishIns, playerIns, hitFishMsg);
            BaseShowFishSpecialEffect(fishIns);
            Destroy();
        }

        public override void SubFishDieProcess(FishFishBase tempFish, FishPlayerInfo playerIns, KillFishRsp hitFishMsg)
        {
            
        }

        public override void Destroy()
        {
            BaseDestroy();
        }

        public override void FixedScreenProcess(FreezeFishesRsp fishMsg)
        {
           
        }

        public override void RemoveFishPartProcess(FishFishBase fishIns, List<CrabPart> crabParts)
        {

        }

        public override void AddEventListener()
        {
            
        }

        public override void RemoveEventListener()
        {
            
        }
    }
}
