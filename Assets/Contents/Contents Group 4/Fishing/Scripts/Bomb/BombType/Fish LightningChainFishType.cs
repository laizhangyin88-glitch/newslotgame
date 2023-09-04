using fishMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishLightningChainFishType : FishBombBaseFish
    {
        int specialDeclareUID = 0;
        int score = 0;
        int multiple = 0;

        public override void SetFishDieProcess(KillFishRsp hitFishMsg, FishFishBase fishIns, FishPlayerInfo playerIns)
        {
            BaseFishDie(fishIns, playerIns, hitFishMsg);
            BaseShakeOrVibrate(hitFishMsg, fishIns);
            SubFishDieProcess(fishIns, playerIns, hitFishMsg);
            BaseShowPlusTipsEffect(fishIns, playerIns, hitFishMsg);
            ShowSpecialDeclareEffect(fishIns, playerIns, hitFishMsg);
            BaseShowFishSpecialEffect(fishIns);
            BaseShowLightningEffect(fishIns, playerIns, hitFishMsg);
            Destroy();
        }

        public override void SubFishDieProcess(FishFishBase tempFish, FishPlayerInfo playerIns, KillFishRsp hitFishMsg)
        {
            for (int i = 0; i < hitFishMsg.SubFishes.Count; i++)
            {
                FishBombManager.Instance.CheckFishState(hitFishMsg.SubFishes[i]);
            }
        }

        public void ShowSpecialDeclareEffect(FishFishBase fishIns, FishPlayerInfo playerIns, KillFishRsp hitFishMsg)
        {
            int parentFishId = hitFishMsg.bombUID;
            score = hitFishMsg.totalScore;
            multiple = hitFishMsg.totalRatio;

            if (parentFishId == 0)
            {
                FishSpecialDeclareConfig specialDeclareConfig = FishSpecialDeclareEffectManager.Instance.GetSpecialDeclareEffectConfig(fishIns.fishVo.DieEffectConfig.specialDeclareID);
                specialDeclareUID = FishSpecialDeclareEffectManager.Instance.SetSpecialDeclareEffectShowMode(playerIns.GetPlayerChairId(), playerIns.specialDeclarePanel.position, specialDeclareConfig);
                AsyncActionUtils.DelayedAction(FishBombManager.Instance, specialDeclareConfig.delayTime, ShowSpecialDeclareScore);
            }
        }

        public void ShowSpecialDeclareScore()
        {
            FishSpecialDeclareEffectManager.Instance.BeginSpecialDeclareEffectChangeScore(specialDeclareUID, score, multiple);
            Destroy();
        }

        public override void Destroy()
        {
            BaseDestroy();
        }

        public override void FixedScreenProcess(FreezeFishesRsp fishMsg)
        {
            
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
