using fishMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishGameData
    {
        //玩家自己的座位
        public int playerChairId = 0;
        public int playerTotalCount = 4;
        public int userId = 0;
        public FishGameConfig GameConfig;

        public float ResolutionHeight;
        public float ResolutionHeightHalf;
        public float ResolutionWidth;
        public float ResolutionWidthHalf;

        public string[] FishDieAudio;
        public List<CannonInfo> GunLevelConfig;
        public List<FishGunConfig> GunConfigList;
        public List<FishRoomConfig> RommConfigList;
        public List<FishSoundConfig> SoundConfigList;
        public Dictionary<int, FishFishConfig> FishConfigList;
        public Dictionary<int, FishCoinEffectConfig> CoinEffectConfigList;
        public Dictionary<int, FishDieEffectConfig> DieEffectConfigList;
        public Dictionary<int, FishScoreEffectConfig> ScoreEffectConfigList;
        public Dictionary<int, FishPlusTipsEffectConfig> PlusTipsEffectConfigList;
        public Dictionary<int, FishSpecialDeclareConfig> SpecialDeclareConfigList;
        public List<FishInfo> fishRawDataList;
        

        public FishGameData()
        {
            InitData();
        }

        private void InitData()
        {
            InitGameData();
            InitFishData();
        }
        private void InitGameData()
        {
            playerChairId = 0;
            playerTotalCount = 4;
            userId = 0;
            GunLevelConfig = new List<CannonInfo>();
        }

        private void InitFishData()
        {
        }
    }
}
