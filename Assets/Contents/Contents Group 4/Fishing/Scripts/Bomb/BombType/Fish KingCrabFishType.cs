using fishMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishKingCrabFishType : FishBombBaseFish
    {

        public override void Destroy()
        {
            BaseDestroy();
        }

        public override void FixedScreenProcess(FreezeFishesRsp fishMsg)
        {
            
        }

        public override void RemoveFishPartProcess(FishFishBase fishIns, List<CrabPart> aryKilledParts)
        {
            FishAudioManager.Instance.PlayNormalAudio(62);
            if (aryKilledParts.Count == 1)
            {
                FishPlayerInfo playerIns = FishPlayerManager.Instance.GetPlayerInsByChairId(aryKilledParts[0].usChairId);
                fishIns.RemoveFishPart(aryKilledParts[0].usPartId);
                FishSpiderCrabEffectManager.Instance.CreateSpiderCrabBosshurt(aryKilledParts[0].usChairId, 1, aryKilledParts[0].usScore, playerIns.specialDeclarePanel.position);
            }
            else if (aryKilledParts.Count == 2)
            {
                FishPlayerInfo playerIns = FishPlayerManager.Instance.GetPlayerInsByChairId(aryKilledParts[1].usChairId);
                fishIns.RemoveFishPart(aryKilledParts[1].usPartId);
                FishSpiderCrabEffectManager.Instance.CreateSpiderCrabBosshurt(aryKilledParts[1].usChairId, 2, aryKilledParts[1].usScore, playerIns.specialDeclarePanel.position);
            }
        }

        public void ShowFishSpecialEffect(FishFishBase fishIns, int partId)
        {
            FishFishEffectManager.Instance.ShowFishEffect(fishIns.GetEffectPoint()[0], 1, "burst_coin_small", 0, 2, "");
        }

        public override void SetFishDieProcess(KillFishRsp hitFishMsg, FishFishBase fishIns, FishPlayerInfo playerIns)
        {
            BaseFishDie(fishIns, playerIns, hitFishMsg);
            BaseShakeOrVibrate(hitFishMsg, fishIns);
            SubFishDieProcess(fishIns, playerIns, hitFishMsg);
            BaseShowPlusTipsEffect(fishIns, playerIns, hitFishMsg);
            BaseShowFishSpecialEffect(fishIns);
            Destroy();
        }

        public override void SubFishDieProcess(FishFishBase tempFish, FishPlayerInfo playerIns, KillFishRsp hitFishMsg)
        {
            if (hitFishMsg.SubFishes != null)
            {
                for (int i = 0; i < hitFishMsg.SubFishes.Count; i++)
                {
                    FishBombManager.Instance.CheckFishState(hitFishMsg.SubFishes[i]);
                }
            }
        }

        public override void AddEventListener()
        {
            
        }

        public override void RemoveEventListener()
        {
            
        }
    }
}
