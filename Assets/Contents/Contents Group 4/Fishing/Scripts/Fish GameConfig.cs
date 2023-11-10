using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BagelCode
{
    public class FishGameConfig
    {
        public enum FishType
        {
            Normal = 1,
            Combo = 2,
            Spine = 3,
            Tips = 4,
            Part = 5,
            Dragon = 6,
        }
        public string[] FishConfig;
        public ConfigFile[] FishConfigJson;
        public FishPack[] FishPackRes;
        public FishPack[] FishBossPackRes;
        public Fish[] FishRes;
        public Fish[] FishBossRes;
        public Bullet[] BulletRes;
        public Net[] NetRes;
        public Player[] PlayerRes;
        public PanelConfig GamePanelRes;
        public Gold[] GoldRes;
        public Score[] ScoreRes;
        public PlusTips[] PlusTipsRes;
        public SpecialDeclare[] SpecialDeclareRes;
        public Lightning[] LightningRes;
        public Effect[] EffectRes;
        public Skill[] SkillRes;
        public Audio[] AudioRes;
        public Dictionary<string, string> GameSetRes;
        public BGTexture[] BGRes;
        public int[] LockFishList = {
            43,42,41,40,39,
            38,37,36,35,34,
            33,32,31,30,29,
            28,27,26,25,24,
            23,22,21,20 };
        public TipsContent[] TipsContentRes;

        public FishGameConfig()
        {
            Init();
        }
        public void Init()
        {
            InitData();
        }

        private void InitData()
        {
            InitProtoConfig();
            InitFishConfig();
            InitBulletConfig();
            InitPlayerConfig();
            InitLockFishConfig();
            InitNetConfig();
            InitPanelConfig();
            InitGameAtlasConfig();
            InitGoldConfig();
            InitScoreConfig();
            InitPlusTipsConfig();
            InitSpecialDeclareConfig();
            InitEffectConfig();
            InitAudioConfig();
            InitGameBGConfig();
            InitLightningConfig();
            InitSkillConfig();
            InitTipsContentConfig();
            InitFishBossConfig();
        }


        private void InitProtoConfig()
        {
            FishConfig = new string[] {
                "Fish_Msg",
                "FishConfig",
                "CoinEffectConfig",
                "DieEffectConfig",
                "GunConfig",
                "RoomConfig",
                "ScoreEffectConfig",
                "SoundConfig",
                "PlusTipsEffectConfig",
                "SpecialDeclareConfig",
            };


            FishConfigJson = new ConfigFile[]
            {
                new ConfigFile {name = "FishConfigList", path = "/FishConfig/FishConfigData.json"},
                new ConfigFile {name = "CoinEffectConfigList", path = "/FishConfig/CoinEffectConfigData.json"},
                new ConfigFile {name = "DieEffectConfigList", path = "/FishConfig/DieEffectConfigData.json"},
                new ConfigFile {name = "GunConfigList", path = "/FishConfig/GunConfigData.json"},
                new ConfigFile {name = "RoomConfigList", path = "/FishConfig/RoomConfigData.json"},
                new ConfigFile {name = "ScoreEffectConfigList", path = "/FishConfig/ScoreEffectConfigData.json"},
                new ConfigFile {name = "SoundConfigList", path = "/FishConfig/SoundConfigData.json"},
                new ConfigFile {name = "PlusTipsEffectConfigList", path = "/FishConfig/PlusTipsEffectConfigData.json"},
                new ConfigFile {name = "SpecialDeclareConfigList", path = "/FishConfig/SpecialDeclareConfigData.json"},
            };
        }

        private void InitFishConfig()
        {
            FishPackRes = new FishPack[]
            {
                new FishPack {name = "Fish_Pack_01", path = "Prefabs/FishPack/Fish_Pack_01.prefab"},
                new FishPack {name = "Fish_Pack_02", path = "Prefabs/FishPack/Fish_Pack_02.prefab"},
                new FishPack {name = "Fish_Pack_03", path = "Prefabs/FishPack/Fish_Pack_03.prefab"},
            };

            FishRes = new Fish[]
            {
                new Fish {name = "Fish_01", ParentName = "Fish_Pack_01", amout = 15, count = 1},
                new Fish {name = "Fish_02", ParentName = "Fish_Pack_01", amout = 15, count = 1},
                new Fish {name = "Fish_03", ParentName = "Fish_Pack_01", amout = 15, count = 1},
                new Fish {name = "Fish_04", ParentName = "Fish_Pack_01", amout = 15, count = 1},
                new Fish {name = "Fish_05", ParentName = "Fish_Pack_01", amout = 15, count = 1},
                new Fish {name = "Fish_06", ParentName = "Fish_Pack_01", amout = 15, count = 1},
                new Fish {name = "Fish_07", ParentName = "Fish_Pack_01", amout = 15, count = 1},
                new Fish {name = "Fish_08", ParentName = "Fish_Pack_01", amout = 15, count = 1},
                new Fish {name = "Fish_09", ParentName = "Fish_Pack_02", amout = 15, count = 1},
                new Fish {name = "Fish_10", ParentName = "Fish_Pack_02", amout = 15, count = 1},
                new Fish {name = "Fish_11", ParentName = "Fish_Pack_02", amout = 15, count = 1},
                new Fish {name = "Fish_12", ParentName = "Fish_Pack_02", amout = 15, count = 1},
                new Fish {name = "Fish_13", ParentName = "Fish_Pack_02", amout = 15, count = 1},
                new Fish {name = "Fish_14", ParentName = "Fish_Pack_02", amout = 15, count = 1},
                new Fish {name = "Fish_15", ParentName = "Fish_Pack_02", amout = 15, count = 1},
                new Fish {name = "Fish_17", ParentName = "Fish_Pack_02", amout = 15, count = 1},
                new Fish {name = "Fish_31", ParentName = "Fish_Pack_03", amout = 15, count = 1},
                new Fish {name = "Fish_34", ParentName = "Fish_Pack_03", amout = 15, count = 1},
                new Fish {name = "Fish_37", ParentName = "Fish_Pack_03", amout = 15, count = 1},
                new Fish {name = "Fish_38", ParentName = "Fish_Pack_03", amout = 15, count = 1},
                new Fish {name = "Fish_44", ParentName = "Fish_Pack_03", amout = 15, count = 1},
            };
        }


        private void InitFishBossConfig()
        {
            FishBossPackRes = new FishPack[]
            {
                new FishPack {name = "fish_boss_01", path = "Prefabs/FishPack/fish_boss_01.prefab"},
                new FishPack {name = "fish_boss_02", path = "Prefabs/FishPack/fish_boss_02.prefab"},
                new FishPack {name = "fish_boss_03", path = "Prefabs/FishPack/fish_boss_03.prefab"},
                new FishPack {name = "fish_boss_04", path = "Prefabs/FishPack/fish_boss_04.prefab"},
            };

            FishBossRes = new Fish[]
            {
                new Fish {name = "Fish_25", ParentName = "fish_boss_01", amout = 15, count = 1},
                new Fish {name = "Fish_26", ParentName = "fish_boss_01", amout = 15, count = 1},
                new Fish {name = "Fish_28", ParentName = "fish_boss_01", amout = 15, count = 1},
                new Fish {name = "Fish_32", ParentName = "fish_boss_01", amout = 15, count = 1},
                new Fish {name = "Fish_18", ParentName = "fish_boss_02", amout = 15, count = 1},
                new Fish {name = "Fish_19", ParentName = "fish_boss_02", amout = 15, count = 1},
                new Fish {name = "Fish_40", ParentName = "fish_boss_02", amout = 15, count = 1},
                new Fish {name = "Fish_16", ParentName = "fish_boss_03", amout = 15, count = 1},
                new Fish {name = "Fish_20", ParentName = "fish_boss_03", amout = 15, count = 1},
                new Fish {name = "Fish_21", ParentName = "fish_boss_03", amout = 15, count = 1},
                new Fish {name = "Fish_45", ParentName = "fish_boss_03", amout = 15, count = 1},
                new Fish {name = "Fish_22", ParentName = "fish_boss_04", amout = 15, count = 1},
                new Fish {name = "Fish_23", ParentName = "fish_boss_04", amout = 15, count = 1},
                new Fish {name = "Fish_24", ParentName = "fish_boss_04", amout = 15, count = 1},
                new Fish {name = "Fish_46", ParentName = "fish_boss_04", amout = 15, count = 1},
            };
        }

        private void InitBulletConfig()
        {
            BulletRes = new Bullet[]
            {
                new Bullet {name = "bullet_1", ParentName = "Fish_Pack_01", amout = 45, count = 1},
            };
        }

        private void InitNetConfig()
        {
            NetRes = new Net[]
            {
            new Net {name = "Net", path = "Prefabs/Net/Net.prefab", amout = 20, count = 1},
            };
        }

        public void InitPlayerConfig()
        {
            PlayerRes = new Player[]
            {
                new Player {name = "PlayerPanelDown1", path = "Prefabs/Panel/PlayerPanelDown1.prefab", amout = 1, count = 1},
                new Player {name = "PlayerPanelDown2", path = "Prefabs/Panel/PlayerPanelDown2.prefab", amout = 1, count = 1},
                new Player {name = "PlayerPanelUp1", path = "Prefabs/Panel/PlayerPanelUp1.prefab", amout = 1, count = 1},
                new Player {name = "PlayerPanelUp2", path = "Prefabs/Panel/PlayerPanelUp2.prefab", amout = 1, count = 1},
            };
        }

        public void InitPanelConfig()
        {
            GamePanelRes = new PanelConfig
            {
                GameSetPanel = "Prefabs/Panel/GameSetPanel.prefab"
            };
        }

        public void InitGoldConfig()
        {
            GoldRes = new Gold[]
            {
            new Gold {name = "Coin01", path = "Prefabs/Gold/Coin01.prefab", amout = 40, count = 1},
            };
        }

        public void InitScoreConfig()
        {
            ScoreRes = new Score[]
            {
            new Score {name = "Score01", path = "Prefabs/Score/Score01.prefab", amout = 30, count = 1},
            new Score {name = "Score02", path = "Prefabs/Score/Score02.prefab", amout = 30, count = 1},
            };
        }

        public void InitPlusTipsConfig()
        {
            PlusTipsRes = new PlusTips[]
            {
                new PlusTips {name = "PlusTips", path = "Prefabs/PlusTips/PlusTips.prefab", amout = 20, count = 1},
            };
        }

        public void InitSpecialDeclareConfig()
        {
            SpecialDeclareRes = new SpecialDeclare[]
            {
                new SpecialDeclare {name = "SpecialDeclare_Bomb", path = "Prefabs/SpecialDeclare/SpecialDeclare_Bomb.prefab", amout = 1, count = 1},
                new SpecialDeclare {name = "SpecialDeclare_Drill", path = "Prefabs/SpecialDeclare/SpecialDeclare_Drill.prefab", amout = 1, count = 1},
                new SpecialDeclare {name = "SpecialDeclare_Electric", path = "Prefabs/SpecialDeclare/SpecialDeclare_Electric.prefab", amout = 1, count = 1},
                new SpecialDeclare {name = "SpecialDeclare_Flash", path = "Prefabs/SpecialDeclare/SpecialDeclare_Flash.prefab", amout = 1, count = 1},
                new SpecialDeclare {name = "SpecialDeclare_MultBomb", path = "Prefabs/SpecialDeclare/SpecialDeclare_MultBomb.prefab", amout = 1, count = 1},
                new SpecialDeclare {name = "SpecialDeclare_Vortex", path = "Prefabs/SpecialDeclare/SpecialDeclare_Vortex.prefab", amout = 1, count = 1},
                new SpecialDeclare {name = "SpecialDeclare_FireStorm", path = "Prefabs/SpecialDeclare/SpecialDeclare_FireStorm.prefab", amout = 1, count = 1},
                new SpecialDeclare {name = "FireStormYouWin", path = "Prefabs/SpecialDeclare/FireStormYouWin.prefab", amout = 1, count = 1},
                new SpecialDeclare {name = "Bison_CutIn", path = "Prefabs/SpecialDeclare/Bison_CutIn.prefab", amout = 1, count = 1},
                new SpecialDeclare {name = "SpecialDeclare_Bison", path = "Prefabs/SpecialDeclare/SpecialDeclare_Bison.prefab", amout = 1, count = 1},
                new SpecialDeclare {name = "SpecialDeclare_BisonsOther", path = "Prefabs/SpecialDeclare/SpecialDeclare_BisonsOther.prefab", amout = 1, count = 1},
                new SpecialDeclare {name = "SpecialDeclare_Hammer", path = "Prefabs/SpecialDeclare/SpecialDeclare_Hammer.prefab", amout = 1, count = 1},
                new SpecialDeclare {name = "DeclareLight", path = "Prefabs/SpecialDeclare/Boss/DeclareLight.prefab", amout = 1, count = 1},
                new SpecialDeclare {name = "GhostShipDeclare", path = "Prefabs/SpecialDeclare/Boss/GhostShipDeclare.prefab", amout = 1, count = 1},
                new SpecialDeclare {name = "LanternFishDeclare", path = "Prefabs/SpecialDeclare/Boss/LanternFishDeclare.prefab", amout = 1, count = 1},
                //new SpecialDeclare {name = "DragonDeclare", path = "Prefabs/SpecialDeclare/Boss/DragonDeclare.prefab", amout = 1, count = 1},

            };
        }

        public void InitLightningConfig()
        {
            LightningRes = new Lightning[]
            {
            new Lightning {name = "Effect_FlashLine", path = "Prefabs/LightningEffect/Effect_FlashLine.prefab", amout = 30, count = 1},
            };
        }

        public void InitEffectConfig()
        {
            EffectRes = new Effect[]
            {
                new Effect {name = "Effect_Swirl", path = "Prefabs/Effect/Effect_Swirl.prefab", amout = 1, count = 1},
                new Effect {name = "Effect_Flash", path = "Prefabs/Effect/Effect_Flash.prefab", amout = 1, count = 1},
                new Effect {name = "Effect_FlashBall", path = "Prefabs/Effect/Effect_FlashBall.prefab", amout = 1, count = 1},
                new Effect {name = "burst_coin_small", path = "Prefabs/Effect/burst_coin_small.prefab", amout = 30, count = 1},
                new Effect {name = "Effect_Bomb", path = "Prefabs/Effect/Effect_Bomb.prefab", amout = 1, count = 1},
                new Effect {name = "burst_coin_bison", path = "Prefabs/Effect/burst_coin_bison.prefab", amout = 30, count = 1},
                new Effect {name = "burst_coin_large_luckyCat", path = "Prefabs/Effect/burst_coin_large_luckyCat.prefab", amout = 1, count = 1},
                new Effect {name = "burst_coin_long", path = "Prefabs/Effect/burst_coin_long.prefab", amout = 1, count = 5},
                new Effect {name = "burst_coin_long1", path = "Prefabs/Effect/burst_coin_long1.prefab", amout = 1, count = 5},
                new Effect {name = "Effect_Warning", path = "Prefabs/Effect/Effect_Warning.prefab", amout = 1, count = 5},
            };
        }

        public void InitSkillConfig()
        {
            SkillRes = new Skill[]
            {
                new Skill {name = "Skill_Electric", path = "Prefabs/Skill/Skill_Electric.prefab", amout = 1, count = 1},
                new Skill {name = "Gun_Electric", path = "Prefabs/Skill/Gun_Electric.prefab", amout = 1, count = 1},
                new Skill {name = "Skill_FireStorm", path = "Prefabs/Skill/Skill_FireStorm.prefab", amout = 1, count = 1},
                new Skill {name = "Skill_Drill", path = "Prefabs/Skill/Skill_Drill.prefab", amout = 1, count = 1},
                new Skill {name = "Gun_Drill", path = "Prefabs/Skill/Gun_Drill.prefab", amout = 1, count = 1},
                new Skill {name = "Bullet_Drill", path = "Prefabs/Skill/Bullet_Drill.prefab", amout = 1, count = 1},
                new Skill {name = "Skill_CountdownTimer", path = "Prefabs/Skill/Skill_CountdownTimer.prefab", amout = 1, count = 1},
                new Skill {name = "Skill_Bomb", path = "Prefabs/Skill/Skill_Bomb.prefab", amout = 1, count = 1},
                new Skill {name = "Skill_MultiBomb", path = "Prefabs/Skill/Skill_MultiBomb.prefab", amout = 1, count = 1},
                new Skill {name = "Skill_Bison", path = "Prefabs/Skill/Skill_Bison.prefab", amout = 1, count = 1},
                new Skill {name = "ThunderHammer", path = "Prefabs/Skill/ThunderHammer.prefab", amout = 1, count = 1},
                new Skill {name = "Skill_GhostShip", path = "Prefabs/Skill/Skill_GhostShip.prefab", amout = 1, count = 1},
                new Skill {name = "Skill_LaternFish", path = "Prefabs/Skill/Skill_LaternFish.prefab", amout = 1, count = 1},
                //new Skill {name = "Skill_DragonTurtle", path = "Prefabs/Skill/Skill_DragonTurtle.prefab", amout = 1, count = 1},
                //new Skill {name = "Skill_ThunderDragon", path = "Prefabs/Skill/Skill_ThunderDragon.prefab", amout = 1, count = 1},
            };
        }

        public void InitAudioConfig()
        {
            AudioRes = new Audio[]
            {
                new Audio {name = "AudioPanel", path = "Prefabs/Audio/AudioPanel.prefab", amout = 1, count = 1},
            };
        }

        public void InitGameAtlasConfig()
        {
            GameSetRes = new Dictionary<string, string>
            {
                {"GameSetSpriteAtlas", "Common/Atlas/GameSet/GameSetSpriteAtlas.spriteatlas"},
                {"Bullet_NetSpriteAtlas", "Common/Atlas/Bullet_Net/Bullet_NetSpriteAtlas.spriteatlas"},
                {"Fish1SpriteAtlas", "Common/Atlas/Fish/Fish1SpriteAtlas.spriteatlas"},
                {"Fish2SpriteAtlas", "Common/Atlas/Fish/Fish2SpriteAtlas.spriteatlas"},
                {"Fish3SpriteAtlas", "Common/Atlas/Fish/Fish3SpriteAtlas.spriteatlas"},
                {"Fish4SpriteAtlas", "Common/Atlas/Fish/Fish4SpriteAtlas.spriteatlas"},
                {"Fish5SpriteAtlas", "Common/Atlas/Fish/Fish5SpriteAtlas.spriteatlas"},
                {"Fish6SpriteAtlas", "Common/Atlas/Fish/Fish6SpriteAtlas.spriteatlas"},
                {"Fish7SpriteAtlas", "Common/Atlas/Fish/Fish7SpriteAtlas.spriteatlas"},
                {"Fish8SpriteAtlas", "Common/Atlas/Fish/Fish8SpriteAtlas.spriteatlas"},
                {"Fish9SpriteAtlas", "Common/Atlas/Fish/Fish9SpriteAtlas.spriteatlas"},
                {"Fish10SpriteAtlas", "Common/Atlas/Fish/Fish10SpriteAtlas.spriteatlas"},
                {"GoldSpriteAtlas", "Common/Atlas/Gold/GoldSpriteAtlas.spriteatlas"},
                {"PlayerSpriteAtlas", "Common/Atlas/Player/PlayerSpriteAtlas.spriteatlas"},
                {"FontSpriteAtlas", "Common/Atlas/Font/FontSpriteAtlas.spriteatlas"},
                {"DeclareSpriteAtlas", "Common/Atlas/Declare/DeclareSpriteAtlas.spriteatlas"},
                {"SkillSpriteAtlas", "Common/Atlas/Skill/SkillSpriteAtlas.spriteatlas"},
            };
        }

        public void InitGameBGConfig()
        {
            BGRes = new BGTexture[]
            {
                new BGTexture {name = "t_bg_1", path = "Texture/BG/t_bg_1.png", amout = 1, count = 1},
                new BGTexture {name = "t_bg_2", path = "Texture/BG/t_bg_2.png", amout = 1, count = 1},
                new BGTexture {name = "t_bg_3", path = "Texture/BG/t_bg_3.png", amout = 1, count = 1},
                new BGTexture {name = "t_bg_4", path = "Texture/BG/t_bg_4.jpg", amout = 1, count = 1},
                new BGTexture {name = "t_bg_5", path = "Texture/BG/t_bg_5.jpg", amout = 1, count = 1},
            };
        }

        public void InitTipsContentConfig()
        {
            TipsContentRes = new TipsContent[]
            {
                    new TipsContent {name = "TipsContent", path = "Prefabs/Tips/TipsContent.prefab", amout = 10},
            };
        }

        public void InitLockFishConfig()
        {
            LockFishList = new int[]
            {
            43,42,41,40,39,
            38,37,36,35,34,
            33,32,31,30,29,
            28,27,26,25,24,
            23,22,21,20
            };
        }
    }

    public enum FishGameState
    {
        Normal = 0,
        SanYu = 1,
        JieSuan = 2,
    }

    public enum FishType
    {
        Normal = 1,
        Combo = 2,
        Spine = 3,
        Tips = 4,
        Part = 5,
        Dragon = 6,
    }

    public enum BombFishType
    {

    }

    public class FishOutTips
    {
        public string name;
        public string path;
        public int amout;
        public int count;
    }
    public class Skill
    {
        public string name;
        public string path;
        public int amout;
        public int count;
    }

    public class SpecialDeclare
    {
        public string name;
        public string path;
        public int amout;
        public int count;
    }

    public class PlusTips
    {
        public string name;
        public string path;
        public int amout;
        public int count;
    }

    public class Score
    {
        public string name;
        public string path;
        public int amout;
        public int count;
    }
    public class Net
    {
        public string name;
        public string path;
        public int amout;
        public int count;
    }

    public class Bullet
    {
        public string name;
        public string ParentName;
        public int amout;
        public int count;
    }

    public class ConfigFile
    {
        public string name;
        public string path;
        //public string data;
    }

    public class FishPack
    {
        public string name;
        public string path;
    }

    public class Fish
    {
        public string name;
        public string ParentName;
        public int amout;
        public int count;
    }

    public class Player
    {
        public string name;
        public string path;
        public int amout;
        public int count;
    }

    public class PanelConfig
    {
        public string GameSetPanel;
    }

    public class Gold
    {
        public string name;
        public string path;
        public int amout;
        public int count;
    }

    public class Lightning
    {
        public string name;
        public string path;
        public int amout;
        public int count;
    }

    public class Effect
    {
        public string name;
        public string path;
        public int amout;
        public int count;
    }
    public class Audio
    {
        public string name;
        public string path;
        public int amout;
        public int count;
    }

    public class BGTexture
    {
        public string name;
        public string path;
        public int amout;
        public int count;
    }

    public class TipsContent
    {
        public string name;
        public string path;
        public int amout;
    }
}
