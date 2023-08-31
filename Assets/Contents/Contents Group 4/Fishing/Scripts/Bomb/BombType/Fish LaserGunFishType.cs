using fishMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishLaserGunFishType : FishBombBaseFish
    {
        public override void FixedScreenProcess(FreezeFishesRsp fishMsg)
        {
            
        }

        public override void SetFishDieProcess(KillFishRsp hitFishMsg, FishFishBase fishIns, FishPlayerInfo playerIns)
        {
            BaseFishDie(fishIns, playerIns, hitFishMsg);
            BaseShakeOrVibrate(hitFishMsg, fishIns);
            SubFishDieProcess(fishIns, playerIns, hitFishMsg);
            Destroy();
        }

        public override void SubFishDieProcess(FishFishBase tempFish, FishPlayerInfo playerIns, KillFishRsp hitFishMsg)
        {
            if (hitFishMsg.SubFishes != null)
            {
                for (int i = 0; i < hitFishMsg.SubFishes.Count; i++)
                    FishBombManager.Instance.CheckFishState(hitFishMsg.SubFishes[i]);
            }
        }

        public override void Destroy()
        {
            BaseDestroy();
        }

        public override void RemoveFishPartProcess(FishPartFish fishIns, List<CrabPart> crabParts)
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
