using fishMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishSameFishType : FishBombBaseFish
    {
        int specialDeclareUID;
        int score;
        int multiple;

        public FishSameFishType()
        {
            gameData = FishGameUIManager.Instance.gameData;
        }

        public override void SetFishDieProcess(KillFishRsp hitFishMsg, FishFishBase fishIns, FishPlayerInfo playerIns)
        {
            BaseFishDie(fishIns, playerIns, hitFishMsg);
            BaseShakeOrVibrate(hitFishMsg, fishIns);
            SetCustomEffectPos(playerIns);
            SubFishDieProcess(fishIns, playerIns, hitFishMsg);
            BaseShowFishSpecialEffect(fishIns);
            BaseShowPlusTipsEffect(fishIns, playerIns, hitFishMsg);
            ShowSpecialDeclareEffect(fishIns, playerIns, hitFishMsg);
        }

        public void SetCustomEffectPos(FishPlayerInfo playerIns)
        {
            customEffectPosList[0] = playerIns.SwrilPanel.position;
        }


        public override void SubFishDieProcess(FishFishBase tempFish, FishPlayerInfo playerIns, KillFishRsp hitFishMsg)
        {
            if (hitFishMsg.subFishCount > 0)
            {
                for (int i = 0; i < hitFishMsg.SubFishes.Count; i++)
                {
                    FishBombManager.Instance.CheckFishState(hitFishMsg.SubFishes[i]);
                }
            }
        }

        public void ShowSpecialDeclareEffect(FishFishBase fishIns, FishPlayerInfo playerIns, KillFishRsp hitFishMsg)
        {
            int parentFishId = hitFishMsg.bombUID;
            score = hitFishMsg.totalScore;
            multiple = hitFishMsg.totalRatio;

            if (parentFishId == 0)
            {
                FishSpecialDeclareConfig specialDeclareConfig = FishSpecialDeclareEffectManager.Instance.GetSpecialDeclareEffectConfig(fishIns.FishVo.DieEffectConfig.specialDeclareID);
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

