using fishMsg;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishBombManager : MonoSingleton<FishBombManager> //鱼死亡爆炸特效控制器
    {
        public enum FishBombype
        {
            NormalType = 1,
            SameType = 2,
            LightningChainType = 3,
            SpawnType = 4,
            FixedScreenType = 5,
            DoubleRewardType = 6,
            BulletType = 7,
            SmallGameType = 8,
            NormalBombType = 9,
            LaserGunType = 10,
            DrillGunType = 11,
            SerialDrillGunType = 12,
            FireStormType = 13,
            BisonType = 14,
            MultBombType = 15,
            kingCrabType = 16,
            LightningChainRandType = 17,
            DelayBombType = 18,
            ThunderHammer = 19,
        }

        public Dictionary<int, List<FishBombBaseFish>> BombFishTypeInsList;

        private void Awake()
        {
            BombFishTypeInsList = new Dictionary<int, List<FishBombBaseFish>>();
        }

        public void CheckFishState(KillFishRsp hitFishMsg)
        {
            FishPlayerInfo playerIns;
            FishFishBase tempFish = FishFishManager.Instance.GetCheckLockSaveFish(hitFishMsg, out playerIns);
            if (tempFish != null && playerIns != null)
                SetFishDieProcess(hitFishMsg, tempFish, playerIns);
            else
            {
                FishBombBaseFish bombIns = BuildBombInstance(hitFishMsg.mainFishType);
                if (bombIns != null)
                {
                    bombIns.SubFishDieProcess(tempFish, playerIns, hitFishMsg);
                    bombIns.Destroy();
                }
            }
        }

        public void KillPartFishSection(HaiWangCrabKilledPartRsp hitPartMsg)
        {
            FishFishBase hitFish = FishFishManager.Instance.GetUsingFishByFishUID(hitPartMsg.usHaiwangCrabId);
            FishBombBaseFish bombIns = BuildBombInstance((int)FishBombype.kingCrabType);
            bombIns.RemoveFishPartProcess(hitFish, hitPartMsg.aryKilledParts);
        }

        public void SetFixedScreenBombProcess(FreezeFishesRsp fishMsg)
        {
            var bombIns = BuildBombInstance((int)fishMsg.mainFishType);
            if (bombIns != null && fishMsg.mainFishType == (int)FishBombype.FixedScreenType)
            {
                bombIns.FixedScreenProcess(fishMsg);
            }
        }

        public void SetFishDieProcess(KillFishRsp hitFishMsg, FishFishBase hitFish, FishPlayerInfo playerIns)
        {
            var bombIns = BuildBombInstance(hitFishMsg.mainFishType);
            playerIns.UpdateLockFish();
            if (bombIns != null)
                bombIns.SetFishDieProcess(hitFishMsg, hitFish, playerIns);
        }

        public FishBombBaseFish BuildBombInstance(int mainFishType)
        {
            FishBombBaseFish bombIns = null;
            if (!BombFishTypeInsList.ContainsKey(mainFishType)
                || BombFishTypeInsList[mainFishType] == null)
                BombFishTypeInsList[mainFishType] = new List<FishBombBaseFish>();
            if (BombFishTypeInsList[mainFishType].Count > 0)
            {
                for (int i = 0; i < BombFishTypeInsList[mainFishType].Count; i++)
                {
                    if (!BombFishTypeInsList[mainFishType][i].isUsing)
                    {
                        BombFishTypeInsList[mainFishType][i].isUsing = true;
                        return BombFishTypeInsList[mainFishType][i];
                    }
                }
            }

            switch (mainFishType)
            {
                case (int)FishBombype.NormalType:
                    bombIns = new FishNormalFishType();
                    break;
                case (int)FishBombype.SameType:
                    bombIns = new FishSameFishType();
                    break;
                case (int)FishBombype.LightningChainType:
                case (int)FishBombype.LightningChainRandType:
                    bombIns = new FishLightningChainFishType();
                    break;
                case (int)FishBombype.SpawnType:
                    bombIns = new FishSpawnType();
                    break;
                case (int)FishBombype.FixedScreenType:
                    bombIns = new FishFixedScreenFishType();
                    break;
                case (int)FishBombype.DoubleRewardType:
                case (int)FishBombype.BulletType:
                case (int)FishBombype.SmallGameType:
                    Debug.LogError("未定义类型fishType==>" + mainFishType);
                    return null;
                case (int)FishBombype.NormalBombType:
                case (int)FishBombype.DelayBombType:
                case (int)FishBombype.MultBombType:
                case (int)FishBombype.ThunderHammer:
                    bombIns = new FishNormalBombFishType();
                    break;
                case (int)FishBombype.LaserGunType:
                    bombIns = new FishLaserGunFishType();
                    break;
                case (int)FishBombype.DrillGunType:
                    bombIns = new FishDrillGunFishType();
                    break;
                case (int)FishBombype.FireStormType:
                    bombIns = new FishFireStormFishType();
                    break;
                case (int)FishBombype.SerialDrillGunType:
                    bombIns = new FishSerialDrillGunFishType();
                    break;
                case (int)FishBombype.BisonType:
                    bombIns = new FishBisonFishType();
                    break;
                case (int)FishBombype.kingCrabType:
                    bombIns = new FishKingCrabFishType();
                    break;
                default:
                    Debug.LogError("未定义类型fishType==>" + mainFishType);
                    return null;
            }
            if (bombIns != null)
            {
                bombIns.isUsing = true;
                if (BombFishTypeInsList[mainFishType] == null)
                    BombFishTypeInsList[mainFishType] = new List<FishBombBaseFish>();
                BombFishTypeInsList[mainFishType].Add(bombIns);
            }
            return bombIns;
        }

        protected override void OnDestroy()
        {

        }
    }
}
