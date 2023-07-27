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
    public class BossRaidersMonsterSpineUtils : BossRaidersMonsterUtilsBase
    {
        public override string GetMonsterPrefabName(BossRaidersBossType type, MonsterData monsterData = null)
        {
            if (monsterData == null)
                return "";

            return monsterData.prefabName;
        }
    }
}