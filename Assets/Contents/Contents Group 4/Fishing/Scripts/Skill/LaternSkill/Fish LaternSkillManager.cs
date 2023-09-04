using fishMsg;
using SlotMaker;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishLaternSkillManager : MonoSingleton<FishLaternSkillManager>
    {
        FishGameData gameData;

        Dictionary<int, FishLaternFishSkillItem> curUseSkillInsList = new Dictionary<int, FishLaternFishSkillItem> ();
        List<FishLaternFishSkillItem> allUseSkillInsList = new List<FishLaternFishSkillItem>();

        private void Awake()
        {
            gameData = FishGameUIManager.Instance.gameData;
            AddEventListener();
        }

        private void AddEventListener()
        {
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_ANGLERFISH_BOMB_RSP.ToString(), ResponesAnglerFishBombMsg);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_DESTORYANGLERFISH_RSP.ToString(), ResponesAnglerFishSkillDestroyMsg);
        }

        private void RemoveEventListener()
        {
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_ANGLERFISH_BOMB_RSP.ToString(), ResponesAnglerFishBombMsg);
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_DESTORYANGLERFISH_RSP.ToString(), ResponesAnglerFishSkillDestroyMsg);
        }

        private void ResponesAnglerFishBombMsg(byte[] bytes)
        {
            Debug.LogError("call ResponesAnglerFishBombMsg");
            AnglerFishBombRsp data = WebSocketTool.Deserialize<AnglerFishBombRsp>(bytes);
            curUseSkillInsList.TryGetValue(data.usAnglerFishId,out var laternFishSkillItem);
            if (laternFishSkillItem != null)
            {
                laternFishSkillItem.skillVo.BombCount = data.usBombCount;
                if (data.usNextBombPosX == 0 && data.usNextBombPosy == 0)
                {
                    laternFishSkillItem.skillVo.haveNextPos = false;
                }
                else
                {
                    laternFishSkillItem.skillVo.haveNextPos = true;
                    laternFishSkillItem.skillVo.NextPos = FishGameObjectPoolManager.Instance.GetPoolParent(PoolType.EffectPool).transform.TransformPoint(FishCsharpManager.ScreenPointToRealPoint(data.usNextBombPosX, data.usNextBombPosy));
                }
                laternFishSkillItem.OnBombMsg();
            }
        }

        private void ResponesAnglerFishSkillDestroyMsg(byte[] bytes)
        {
            DestoryAnglerFishRsp data = WebSocketTool.Deserialize<DestoryAnglerFishRsp>(bytes);
            curUseSkillInsList.TryGetValue(data.usAnglerFishId, out var laternFishSkillItem);
            if (laternFishSkillItem != null)
            {
                laternFishSkillItem.DelayLaternFishSkillDestroy();
            }
        }

        public void EnterLaternFishSkillMode(CreateAnglerFishRsp data)
        {
            FishPlayerInfo playerIns = FishPlayerManager.Instance.GetPlayerInsByChairId(data.usChairId);
            int UID = data.usAnglerFishId;
            int skillStatus = data.usStatus;
            int skillTime = data.usStatusTime;
            int bombCount = data.usBombCount;
            int killFishUID = data.usKilledFishId;
            if (!curUseSkillInsList.ContainsKey(data.usAnglerFishId) || curUseSkillInsList[data.usAnglerFishId] == null)
            {
                if (data.usNextBombPosX == 0 && data.usNextBombPosy == 0)
                {
                    return;
                }
                else
                {
                    Vector3 beginPos = FishGameObjectPoolManager.Instance.GetPoolParent(PoolType.EffectPool).transform.TransformPoint(FishCsharpManager.ScreenPointToRealPoint(data.usBombPosX, data.usBombPosY));
                    Vector3 nextPos = FishGameObjectPoolManager.Instance.GetPoolParent(PoolType.EffectPool).transform.TransformPoint(FishCsharpManager.ScreenPointToRealPoint(data.usNextBombPosX, data.usNextBombPosy));
                    CreateLaternFishSkill(UID, beginPos, nextPos, bombCount, playerIns, skillStatus, skillTime, killFishUID);
                }
            }
            else
            {
                Debug.LogError("LaternFishSkill is currently in the exploding state");
            }
        }

        private void CreateLaternFishSkill(int UID, Vector3 beginPos, Vector3 nextPos, int bombCount, FishPlayerInfo playerIns, int skillStatus, float skillTime, int killFishUID)
        {
            SkillVo laternFishSkillVo = GetLaternFishSkillVo(UID, nextPos, bombCount, playerIns, killFishUID);
            var tempLaternFishSkillIns = GetLaternFishSkill(laternFishSkillVo);
            if (tempLaternFishSkillIns != null)
            {
                tempLaternFishSkillIns.ResetSkillState(beginPos, skillStatus, skillTime);
            }
            else
            {
                Debug.LogError("Failed to get LaternFishSkill instance");
            }
        }

        public SkillVo GetLaternFishSkillVo(int UID, Vector3 nextPos, int bombCount, FishPlayerInfo playerIns, int killFishUID)
        {
            SkillVo vo = new SkillVo
            {
                UID = UID,
                NextPos = nextPos,
                BombCount = bombCount,
                PlayerIns = playerIns,
                chairId = playerIns.GetPlayerChairId(),
                IsMe = (playerIns.GetPlayerChairId() == gameData.playerChairId),
                killFishUID = killFishUID
            };
            return vo;
        }

        public FishLaternFishSkillItem GetLaternFishSkill(SkillVo laternFishSkillVo)
        {
            if (allUseSkillInsList != null && allUseSkillInsList.Count > 0)
            {
                var tempLaternFishSkillIns = allUseSkillInsList[0];
                allUseSkillInsList.RemoveAt(0);
                if (tempLaternFishSkillIns != null)
                {
                    tempLaternFishSkillIns.ResetSkillVo(laternFishSkillVo);
                    curUseSkillInsList[laternFishSkillVo.UID] = tempLaternFishSkillIns;
                    return tempLaternFishSkillIns;
                }
            }
            else
            {
                var tempLaternFishSkillIns = new FishLaternFishSkillItem();
                if (tempLaternFishSkillIns != null)
                {
                    tempLaternFishSkillIns.ResetSkillVo(laternFishSkillVo);
                    curUseSkillInsList[laternFishSkillVo.UID] = tempLaternFishSkillIns;
                    return tempLaternFishSkillIns;
                }
            }
            return null;
        }

        public void ClearAllLaternFishSkill()
        {
            if (curUseSkillInsList != null)
            {
                foreach (var kvp in curUseSkillInsList)
                {
                    kvp.Value.isCanDestroy = true;
                }
                UpdateRemoveLaternFishSkill();
            }
            curUseSkillInsList.Clear();
        }

        public void ClearOtherPlayerLaternFishSkill(int charidID)
        {
            if (curUseSkillInsList != null)
            {
                foreach (var kvp in curUseSkillInsList)
                {
                    if (kvp.Value.skillVo.chairId == charidID)
                    {
                        kvp.Value.isCanDestroy = true;
                    }
                }
                UpdateRemoveLaternFishSkill();
            }
        }

        private void UpdateRemoveLaternFishSkill()
        {
            if (curUseSkillInsList != null)
            {
                List<int> removeKeyCatch = new List<int>();
                foreach (var item in curUseSkillInsList)
                {
                    if (item.Value.isCanDestroy)
                    {
                        item.Value.Destroy();
                        removeKeyCatch.Add(item.Key);
                    }
                }
                for (int i = 0; i < removeKeyCatch.Count; i++)
                {
                    RecycleLaternFishSkill(curUseSkillInsList[removeKeyCatch[i]].skillVo);
                }
            }
        }

        private void Update()
        {
            UpdateRemoveLaternFishSkill();
        }

        public void RecycleLaternFishSkill(SkillVo laternFishSkillVo)
        {
            var tempLaternFishSkillIns = curUseSkillInsList[laternFishSkillVo.UID];
            if (tempLaternFishSkillIns != null)
            {
                allUseSkillInsList.Add(tempLaternFishSkillIns);
                curUseSkillInsList.Remove(laternFishSkillVo.UID);
            }
            else
            {
                Debug.LogError("Failed to recycle LaternFishSkillItem => " + laternFishSkillVo.UID);
            }
        }

        protected override void OnDestroy()
        {
            RemoveEventListener();
        }
    }
}
