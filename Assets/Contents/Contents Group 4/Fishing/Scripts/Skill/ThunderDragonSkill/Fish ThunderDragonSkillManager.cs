using fishMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishThunderDragonSkillManager : FishSkillManagerBase<FishThunderDragonSkillManager>
    {
        protected override void AddEventListener()
        {
            //WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_SERIALBOMBCRAB_BOMB_RSP.ToString(), ResponesBombMsg);
            //WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_DESTORYSERIALBOMBCRAB_RSP.ToString(), ResponesSkillDestroyMsg);
        }

        protected override void RemoveEventListener()
        {
            //WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_SERIALBOMBCRAB_BOMB_RSP.ToString(), ResponesBombMsg);
            //WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_DESTORYSERIALBOMBCRAB_RSP.ToString(), ResponesSkillDestroyMsg);
        }

        private void ResponesBombMsg(byte[] bytes)
        {
            SerialBombCrabBombRsp data = WebSocketTool.Deserialize<SerialBombCrabBombRsp>(bytes);
            var skillItem = curUseSkillInsList[data.usSerialBombCrabId];
            if (skillItem != null)
            {
                skillItem.skillVo.BombCount = data.usBombCount;
                if (data.usNextBombPosX == 0 && data.usNextBombPosy == 0)
                {
                    skillItem.skillVo.haveNextPos = false;
                }
                else
                {
                    skillItem.skillVo.haveNextPos = true;
                    skillItem.skillVo.NextPos = FishGameObjectPoolManager.Instance.GetPoolParent(PoolType.EffectPool).transform.TransformPoint(FishCsharpManager.ScreenPointToRealPoint(data.usNextBombPosX, data.usNextBombPosy));
                }
                var tempSkillItem = skillItem as FishThunderDragonSkillItem;
                tempSkillItem.OnBombMsg();
            }
        }

        private void ResponesSkillDestroyMsg(byte[] bytes)
        {
            SerialBombCrabBombRsp data = WebSocketTool.Deserialize<SerialBombCrabBombRsp>(bytes);
            var skillItem = curUseSkillInsList[data.usSerialBombCrabId];
            skillItem?.DelaySkillDestroy();
        }

        ///测试用
        public void EnterSkillMode(CreateThunderHammerRsp data)
        {
            Debug.LogError("EnterTestSkill");
            var playerIns = FishPlayerManager.Instance.GetPlayerInsByChairId(data.usChairId);
            int uid = data.usThunderHammerId;
            int skillStatus = data.usStatus;
            int skillTime = data.usStatusTime;
            int fishId = data.bombFishId;
            int killFishUid = data.usKilledFishId;
            if (!curUseSkillInsList.ContainsKey(uid) || curUseSkillInsList[uid] == null)
            {
                Vector3 beginPos = Vector3.zero;
                Vector3 nextPos = Vector3.zero;
                var skillIns = GetSkill(uid, beginPos, nextPos, 3, playerIns, skillStatus, skillTime, killFishUid, fishId);
                if (skillIns != null)
                {
                    skillIns.objResName = "Skill_ThunderDragon";
                    var skillTemp = skillIns as FishThunderDragonSkillItem;
                    skillTemp.ResetSkillState(beginPos, skillTime);
                    skillTemp.ShowSkill();
                }
            }
            else
                Debug.LogError("ThunderDragonSkill is currently in the exploding state");
        }

        public void EnterSkillMode(CreateSerialBombCrabRsp data)
        {
            Debug.LogError("EnterTestSkill");
            FishPlayerInfo playerIns = FishPlayerManager.Instance.GetPlayerInsByChairId(data.usChairId);
            int uid = data.usSerialBombCrabId;
            int skillStatus = data.usStatus;
            int skillTime = data.usStatusTime;
            int bombCount = data.usBombCount;
            int fishId = data.bombFishId;
            int killFishUid = data.usKilledFishId;
            if (!curUseSkillInsList.ContainsKey(uid) || curUseSkillInsList[uid] == null)
            {
                if (data.usNextBombPosX == 0 && data.usNextBombPosy == 0)
                    return;
                else
                {
                    Vector3 beginPos = FishGameObjectPoolManager.Instance.GetPoolParent(PoolType.EffectPool).transform.TransformPoint(FishCsharpManager.ScreenPointToRealPoint(data.usBombPosX, data.usBombPosY));
                    Vector3 nextPos = FishGameObjectPoolManager.Instance.GetPoolParent(PoolType.EffectPool).transform.TransformPoint(FishCsharpManager.ScreenPointToRealPoint(data.usNextBombPosX, data.usNextBombPosy));
                    var skillIns = GetSkill(uid, beginPos, nextPos, bombCount, playerIns, skillStatus, skillTime, killFishUid, fishId);
                    if (skillIns != null)
                    {
                        skillIns.objResName = "Skill_ThunderDragon";
                        var skillTemp = skillIns as FishThunderDragonSkillItem;
                        skillTemp.ResetSkillState(beginPos, skillTime);
                        skillTemp.ShowSkill();
                    }
                }
            }
            else
                Debug.LogError("ThunderDragonSkill is currently in the exploding state");
        }

        public void EnterSkillMode(CreateAnglerFishRsp data)
        {
            Debug.LogError("EnterTestSkill");
            FishPlayerInfo playerIns = FishPlayerManager.Instance.GetPlayerInsByChairId(data.usChairId);
            int uid = data.usAnglerFishId;
            int skillStatus = data.usStatus;
            int skillTime = data.usStatusTime;
            int bombCount = data.usBombCount;
            int fishId = data.bombFishId;
            int killFishUid = data.usKilledFishId;
            if (!curUseSkillInsList.ContainsKey(uid) || curUseSkillInsList[uid] == null)
            {
                if (data.usNextBombPosX == 0 && data.usNextBombPosy == 0)
                    return;
                else
                {
                    Vector3 beginPos = FishGameObjectPoolManager.Instance.GetPoolParent(PoolType.EffectPool).transform.TransformPoint(FishCsharpManager.ScreenPointToRealPoint(data.usBombPosX, data.usBombPosY));
                    Vector3 nextPos = FishGameObjectPoolManager.Instance.GetPoolParent(PoolType.EffectPool).transform.TransformPoint(FishCsharpManager.ScreenPointToRealPoint(data.usNextBombPosX, data.usNextBombPosy));
                    var skillIns = GetSkill(uid, beginPos, nextPos, bombCount, playerIns, skillStatus, skillTime, killFishUid, fishId);
                    if (skillIns != null)
                    {
                        skillIns.objResName = "Skill_ThunderDragon";
                        var skillTemp = skillIns as FishThunderDragonSkillItem;
                        skillTemp.ResetSkillState(beginPos, skillTime);
                        skillTemp.ShowSkill();
                    }
                }
            }
            else
                Debug.LogError("ThunderDragonSkill is currently in the exploding state");
        }
    }
}
