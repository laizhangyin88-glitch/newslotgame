using BagelCode;
using fishMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishFixedScreenFishType : FishBombBaseFish
    {
        public override void SetFishDieProcess(KillFishRsp hitFishMsg, FishFishBase fishIns, FishPlayerInfo playerIns)
        {
            BaseFishDie(fishIns, playerIns, hitFishMsg);
            BaseShakeOrVibrate(hitFishMsg, fishIns);
            BaseShowPlusTipsEffect(fishIns, playerIns, hitFishMsg);
            BaseShowFishSpecialEffect(fishIns);
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
            List<FishFishBase> tempList = new List<FishFishBase>();
            if (fishMsg.fishes.Count > 0)
            {
                for (int i = 0; i < fishMsg.fishes.Count; i++)
                {
                    FishFishBase tempFish = FishFishManager.Instance.GetUsingFishByFishUID((int)fishMsg.fishes[i].fish_uid);
                    if (tempFish != null)
                        tempList.Add(tempFish);
                }
            }

            if (fishMsg.IsFreeze)
                FishFishManager.Instance.PauseAssignationFish(tempList);
            else
                FishFishManager.Instance.ResumeAssignationFish(tempList);
            Destroy();
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
