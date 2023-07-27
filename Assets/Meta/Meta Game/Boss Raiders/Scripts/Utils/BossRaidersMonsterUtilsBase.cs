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
    public abstract class BossRaidersMonsterUtilsBase : MonoBehaviour
    {
        public abstract string GetMonsterPrefabName(BossRaidersBossType type, MonsterData monsterData = null);
    }
}