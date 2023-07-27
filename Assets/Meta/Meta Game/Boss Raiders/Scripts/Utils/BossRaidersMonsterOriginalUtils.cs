using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

namespace BagelCode.BossRaiders
{
    public class BossRaidersMonsterOriginalUtils : BossRaidersMonsterUtilsBase
    {
        public override string GetMonsterPrefabName(BossRaidersBossType type, MonsterData monsterData = null)
        {
            string assetName = GetMonkeyName(type);

            if (string.IsNullOrEmpty(assetName))
                assetName = monsterData != null ? GetMonkeyName(monsterData.type, "Boss Raiders Monster Small Monkey") : "Boss Raiders Monster Small Monkey";

            return assetName;
        }

        private string GetMonkeyName(BossRaidersBossType type, string baseAssetName = "")
        {
            string assetName = baseAssetName;
            switch (type)
            {
                case BossRaidersBossType.SMALL_1:
                case BossRaidersBossType.SMALL_2:
                case BossRaidersBossType.SMALL_3:
                    assetName = "Boss Raiders Monster Small Monkey";
                    break;
                case BossRaidersBossType.BIG_1:
                    assetName = "Boss Raiders Monster Big Monkey";
                    break;
            }
            return assetName;
        }
    }
}