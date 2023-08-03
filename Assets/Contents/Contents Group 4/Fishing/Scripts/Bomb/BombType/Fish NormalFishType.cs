using fishMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishNormalFishType : FishBombBaseFish
    {
        public void SetCustpmEffectPos(FishFishBase fishIns)
        {
            List<Vector3> effectPosList = fishIns.GetEffectPoint();

            if (effectPosList != null)
                customEffectPosList = fishIns.GetEffectPoint();
        }

        public override void SetFishDieProcess(KillFishRsp hitFishMsg, FishFishBase fishIns, FishPlayerInfo playerIns)
        {
            BaseFishDie(fishIns, playerIns, hitFishMsg);
            BaseShakeOrVibrate(hitFishMsg, fishIns);
            BaseShowFishSpecialEffect(fishIns);
            BaseDestroy();
        }

        public override void SubFishDieProcess(FishFishBase tempFish, FishPlayerInfo playerInS, KillFishRsp hitFishMsg)
        {
            if (hitFishMsg.subFishCount > 0)
            {
                for (int i = 0; i < hitFishMsg.SubFishes.Count; i++)
                {
                    FishBombManager.Instance.CheckFishState(hitFishMsg.SubFishes[i]);
                }
            }
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
