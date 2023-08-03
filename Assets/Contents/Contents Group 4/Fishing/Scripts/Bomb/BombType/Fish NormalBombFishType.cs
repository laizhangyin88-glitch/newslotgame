using fishMsg;
using ParadoxNotion;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishNormalBombFishType : FishBombBaseFish
    {
        KillFishRsp hitFishMsg;
        FishFishBase fishIns;
        FishPlayerInfo playerIns;
        public void SetCustomEffectPos(FishFishBase fishIns)
        {
            List<Vector3> effectPostList = fishIns.GetEffectPoint();
            if (effectPostList != null)
            {
                customEffectPosList = effectPostList;
            }
        }

        public override void FixedScreenProcess(FreezeFishesRsp fishMsg)
        {
            
        }

        public override void SetFishDieProcess(KillFishRsp hitFishMsg, FishFishBase fishIns, FishPlayerInfo playerIns)
        {
            BaseFishDie(fishIns, playerIns, hitFishMsg);
            BaseShakeOrVibrate(hitFishMsg, fishIns);
            //SubFishDieProcess(fishIns, playerIns, hitFishMsg);
            BaseShowFishSpecialEffect(fishIns);
            Destroy();
        }

        public override void SubFishDieProcess(FishFishBase tempFish, FishPlayerInfo playerIns, KillFishRsp hitFishMsg)
        {
            this.hitFishMsg = hitFishMsg;
            fishIns = tempFish;
            this.playerIns = playerIns;
        }

        public void SubFishTurelyDie()
        {
            for (int i = 0; i < hitFishMsg.SubFishes.Count; i++)
                FishBombManager.Instance.CheckFishState(hitFishMsg.SubFishes[i]);
        }

        public override void Destroy()
        {
            BaseDestroy();
        }

        public override void RemoveFishPartProcess(FishFishBase fishIns, List<CrabPart> crabParts)
        {
        }

        public override void AddEventListener()
        {
            MessageDispatcher.Register("FishBombEnd", (eventData) => OnFishBombEnd(eventData));
        }

        private void OnFishBombEnd(EventData eventData)
        {
            int uid = (int)eventData.value;
            if (hitFishMsg == null)
            {
                return;
            }
            if (uid == hitFishMsg.mainFishUID) {
                SubFishTurelyDie();
            }
        }

        public override void RemoveEventListener()
        {
            MessageDispatcher.UnRegister("FishBombEnd", (eventData) => OnFishBombEnd(eventData));
        }
    }
}
