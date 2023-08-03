using BagelCode.ClientModels;
using BagelCode.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BagelCode
{

    public class FishFishConfigList
    {
        public List<FishFishConfig> configList;
    }
    [Serializable]
    public class FishFishConfig
    {
        public int id;
        public string fishName;
        public int fishRuleType;
        public int clientBuildFishType;
        public float fishRadius;
        public int ratioMin;
        public int ratioMax;
        public int ratioDeath;
        public int doubleAwardMinRatio;
        public int childFishCount;
        public List<string> childFishIds;
        public int damageRadius;
        public string damageFishIds;
        public int layerMin;
        public int layerMax;
        public int fishDieLayer;
        public float fishDieTime;
        public string fishBornAudio;
        public List<int> FishDieAudio;
        public string fishMoveAnimationName;
        public string fishDieAnimationName;
        public string lockIcon;
        public int coinEffectCount;
        public int dieEffectId;
        public int fishDamageSmallFishDieEffectID;
        public float dieRotationAngle;
        public int fishOutTipsID;
        public string fishOutTipsResource;
        public string fishOutTipsAnimation;
        public float fishOutTipsTime;
        public int IsPhoneVibrate;
        public int IsShakeScreen;
        public string BornEffectName;
        public int BornEffectType;
        public float BornEffectLifeTime;
        public int isTipsContent;
        public float TCProbability;
        public float tCShowTime;
        public string TipsContentInfo;
    }

    public class FishCoinEffectConfigList
    {
        public List<FishCoinEffectConfig> configList;
    }

    [Serializable]
    public class FishCoinEffectConfig
    {
        public int id;
        public string coinRes;
        public string coinAnimName;
        public float coinDelayTime;
        public int coinShowAudioID;
        public float coinFlyTime;
        public int coinFlyAuidioID;
    }

    public class FishDieEffectConfigList
    {
        public List<FishDieEffectConfig> configList = new List<FishDieEffectConfig>();
    }

    [Serializable]
    public class FishDieEffectConfig
    {
        public int id;
        public int fishDieType;
        public int fishDieBehavior;
        public float hitFlyDistance;
        public int plusTipsID;
        public int winScoreID;
        public int coinEffectId;
        public int specialDeclareID;
        public float fishDieOneShowTime;
        public float fishDieTwoShowTime;
        public float fishDieThreeShowTime;
        public string childFishLineRes;
        public float childFishLineDelyTime;
        public float childFishLineLifeTime;
        public List<string> dieEffectName;
        public List<int> dieEffectType;
        public List<int> dieEffectPositionFlag;
        public List<float> dieEffectDelyTime;
        public List<float> dieEffectLifeTime;
        public List<string> dieAudio;
    }

    public class FishScoreEffectConfigList
    {
        public List<FishScoreEffectConfig> configList = new List<FishScoreEffectConfig>();
    }

    [Serializable]
    public class FishScoreEffectConfig
    {
        public int id;
        public int scoreType;
        public string scoreResource;
        public int scoreEffect;
        public float scoreDelayTime;
        public float scoreShowTime;
        public int scoreShowAudio;
    }


    public class FishGunConfigList
    {
        public List<FishGunConfig> configList = new List<FishGunConfig>();
    }

    [Serializable]
    public class FishGunConfig
    {
        public int id;
        public string normalGunRes;
        public string normalBulletRes;
        public string normalNetRes;
        public string doubleGunRes;
        public string doubleBulletRes;
        public string doubleNetRes;
        public string freeGunRes;
        public string freeBulletRes;
        public string freeNetRes;
        public int shootAudio;
        public int hitFishAuio;
        public int makeUpAudio;
    }

    public class FishRoomConfigList
    {
        public List<FishRoomConfig> configList = new List<FishRoomConfig>();
    }

    [Serializable]
    public class FishRoomConfig
    {
        public int id;
        public string roomName;
        public int roomMoney;
        public string roomRes;
        public int autoDeskChair;
        public int deskCount;
        public int chairCount;
    }


    public class FishSoundConfigList
    {
        public List<FishSoundConfig> configList = new List<FishSoundConfig>();
    }

    [Serializable]
    public class FishSoundConfig
    {
        public int id;
        public string soundName;
        public string soundPath;
    }

    public class FishPlusTipsEffectConfigList
    {
        public List<FishPlusTipsEffectConfig> configList = new List<FishPlusTipsEffectConfig>();
    }

    [Serializable]
    public class FishPlusTipsEffectConfig
    {
        public int id;
        public string plusTipsResource;
        public float plusTipsDelayTime;
        public float plusTipsShowTime;
        public int plusTipsShowAudio;
    }

    public class FishSpecialDeclareConfigList
    {
        public List<FishSpecialDeclareConfig> configList = new List<FishSpecialDeclareConfig>();
    }

    [Serializable]
    public class FishSpecialDeclareConfig
    {
        public int id;
        public string specialDeclareRes;
        public int specialDeclareAnimType;
        public float delayTime;
        public float scoreShowTime;
        public int specialDeclareShowAudio;
    }

}


