using fishMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishSerialDrillGunFishType : FishBombBaseFish
    {
        public FishSerialDrillGunFishType()
        {

        }

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
            SubFishDieProcess(fishIns, playerIns, hitFishMsg);
            Destroy();
        }

        public override void SubFishDieProcess(FishFishBase tempFish, FishPlayerInfo playerIns, KillFishRsp hitFishMsg)
        {
            //var fishDamageSmallFishDieEffectConfig = gameData.DieEffectConfigList[tempFish.FishVo.FishConfig.fishDamageSmallFishDieEffectID];
            //var fishDamageBigFishDieEffectConfig = gameData.DieEffectConfigList[tempFish.FishVo.FishConfig.fishdam];
        }
    }
}

