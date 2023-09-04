using fishMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public abstract class FishBombBaseFish
    {
        public FishGameData gameData;
        public bool isUsing;
        public List<Vector3> customEffectPosList;
        public FishBombBaseFish()
        {
            BaseInit();
        }

        private void BaseInit()
        {
            BaseInitData();
            AddEventListener();
        }

        public abstract void AddEventListener();

        private void BaseInitData()
        {
            gameData = FishGameUIManager.Instance.gameData;
            customEffectPosList = new List<Vector3> {Vector3.zero, Vector3.zero, Vector3.zero};
        }

        public Vector3 GetCustormEffectPos(int index)
        { return customEffectPosList[index]; }

        public void BaseShakeOrVibrate(KillFishRsp hitFishMsg, FishFishBase fishIns)
        {
            if (hitFishMsg.bombUID != 0)
                return;
            if (gameData.playerChairId == hitFishMsg.chairId)
            {
                if (fishIns.fishVo.FishConfig.IsShakeScreen == 1)
                {
                    bool isVibrate = false;
                    if (fishIns.fishVo.FishConfig.IsPhoneVibrate == 1)
                    {
                        isVibrate = true;
                    }
                    FishGameUIManager.Instance.SetShake(isVibrate);
                }
            }
        }

        public bool UpdateFishDieEffectConfig(FishFishBase fishIns, KillFishRsp hitFishMsg)
        {
            if (hitFishMsg.bombUID != 0)
            {
                FishFishBase parentFish = FishFishManager.Instance.GetCacheFishById(hitFishMsg.bombUID);
                FishDieEffectConfig subFishDieEffectConfig;
                if (parentFish != null)
                    subFishDieEffectConfig = gameData.DieEffectConfigList[parentFish.fishVo.FishConfig.fishDamageSmallFishDieEffectID];
                else
                {
                    FishFishConfig fishConfig = gameData.FishConfigList[hitFishMsg.bombFishId];
                    subFishDieEffectConfig = gameData.DieEffectConfigList[fishConfig.fishDamageSmallFishDieEffectID];
                }
                fishIns.UpdateDieEffectConfig(subFishDieEffectConfig);
                return true;
            }
            return false;
        }

        public void BaseFishDie(FishFishBase fishIns, FishPlayerInfo playerIns, KillFishRsp hitFishMsg)
        {
            if (gameData.playerChairId == hitFishMsg.chairId)
                PlayBaseFishDieAudio(fishIns, hitFishMsg.bombUID);
            fishIns.FishNormalDie(playerIns, hitFishMsg);
        }

        public void PlayBaseFishDieAudio(FishFishBase fishIns, int bombUID)
        {
            //if (bombUID == 0)
            //{
            //    int count = fishIns.FishVo.FishConfig.FishDieAudio.Count;
            //    if (count > 0)
            //    {
            //        int audioIndex = fishIns.FishVo.FishConfig.FishDieAudio[Random.Range(0, count)];
            //        FishAudioManager.Instance.PlayNormalAudio(audioIndex);
            //    }
            //}
        }

        public void BaseShowPlusTipsEffect(FishFishBase fishIns, FishPlayerInfo playerIns, KillFishRsp hitFishMsg)
        {
            FishDieEffectConfig dieEffectConfig = FishFishEffectManager.Instance.GetFishEffectConfig(fishIns.fishVo.FishConfig.dieEffectId);
            if (dieEffectConfig != null)
            {
                if (dieEffectConfig.plusTipsID != 0)
                {
                    FishPlusTipsEffectConfig tempConfig = FishPlusTipsEffectManager.Instance.GetPlusTipsEffectConfig(dieEffectConfig.plusTipsID);
                    if (tempConfig != null)
                    {
                        int parentFishId = hitFishMsg.bombUID;
                        if (parentFishId == 0)
                            FishPlusTipsEffectManager.Instance.SetPlusTipsEffectShowMode(playerIns.GetPlayerChairId(), playerIns.flyScorePos, hitFishMsg.mainScore, tempConfig);
                    }
                }
            }
        }

        public void BaseShowFishSpecialEffect(FishFishBase fishIns)
        {
            FishDieEffectConfig dieEffectConfig = FishFishEffectManager.Instance.GetFishEffectConfig(fishIns.fishVo.FishConfig.dieEffectId);
            if (dieEffectConfig != null)
            {
                for (int i = 0; i < dieEffectConfig.dieEffectName.Count; i++)
                {
                    string name = dieEffectConfig.dieEffectName[i];
                    int type = dieEffectConfig.dieEffectType[i];
                    int positionFlag = dieEffectConfig.dieEffectPositionFlag[i];
                    float delayTime = dieEffectConfig.dieEffectDelyTime[i];
                    float lifeTime = dieEffectConfig.dieEffectLifeTime[i];
                    string effectAudio = dieEffectConfig.dieAudio[i];
                    if (!string.IsNullOrEmpty(name) && name != "nil")
                    {
                        Vector3 beginPos = Vector3.zero;
                        switch (positionFlag)
                        {
                            case 1:
                                beginPos = fishIns.gameObject.transform.position;
                                break;
                            case 2:
                                beginPos = FishGameObjectPoolManager.Instance.GetPoolParent(PoolType.EffectPool).transform.position;
                                break;
                            case 3:
                                beginPos = GetCustormEffectPos(i);
                                break;
                            default:
                                break;
                        }
                        FishFishEffectManager.Instance.ShowFishEffect(beginPos, type, name, delayTime, lifeTime, effectAudio);
                    }
                }
            }
        }

        public void BaseShowLightningEffect(FishFishBase fishIns, FishPlayerInfo playerIns, KillFishRsp hitFishMsg)
        {
            var tempConfig = FishLightningEffectManager.Instance.GetFishEffectConfig(fishIns.fishVo.FishConfig.dieEffectId);
            if (tempConfig != null)
            {
                int parentFishId = hitFishMsg.bombFishId;
                if (parentFishId == 0)
                {
                    if (hitFishMsg.subFishCount > 0)
                    {
                        for (int i = 0; i < hitFishMsg.SubFishes.Count; i++)
                        {
                            var tempSubFish = FishFishManager.Instance.GetCacheFishById(hitFishMsg.SubFishes[i].mainFishUID);
                            FishLightningEffectManager.Instance.SetLightningEffectShowMode(fishIns.gameObject.transform, tempSubFish.gameObject.transform, playerIns.GetPlayerChairId(), fishIns.fishVo.UID, tempConfig);
                        }
                    }
                }
            }
        }

        public void BaseDestroy()
        {
            isUsing = false;
            
        }

        public abstract void RemoveEventListener();
        public abstract void SetFishDieProcess(KillFishRsp hitFishMsg, FishFishBase fishIns, FishPlayerInfo playerIns);
        public abstract void SubFishDieProcess(FishFishBase tempFish, FishPlayerInfo playerIns, KillFishRsp hitFishMsg);
        public abstract void FixedScreenProcess(FreezeFishesRsp fishMsg);
        public abstract void RemoveFishPartProcess(FishPartFish fishIns, List<CrabPart> crabParts);
        public abstract void Destroy();


    }
}
