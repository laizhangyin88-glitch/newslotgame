using fishMsg;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishDragonTurtleSkillManager : FishSkillManagerBase<FishDragonTurtleSkillManager>
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
                var tempSkillItem = skillItem as FishDragonTurtleSkillItem;
                tempSkillItem.OnBomBMsg();
            }
        }

        private void ResponesSkillDestroyMsg(byte[] bytes)
        {
            SerialBombCrabBombRsp data = WebSocketTool.Deserialize<SerialBombCrabBombRsp>(bytes);
            var skillItem = curUseSkillInsList[data.usSerialBombCrabId];
            skillItem?.DelaySkillDestroy();
        }

        public void EnterDragonTurtleSkillMode(CreateSerialBombCrabRsp data)
        {
            FishPlayerInfo playerIns = FishPlayerManager.Instance.GetPlayerInsByChairId(data.usChairId);
            int UID = data.usSerialBombCrabId;
            int skillStatus = data.usStatus;
            int skillTime = data.usStatusTime;
            int bombCount = data.usBombCount;
            int killFishUID = data.usKilledFishId;
            if (!curUseSkillInsList.ContainsKey(data.usSerialBombCrabId) || curUseSkillInsList[data.usSerialBombCrabId] == null)
            {
                if (data.usNextBombPosX == 0 && data.usNextBombPosy == 0)
                    return;
                else
                {
                    Vector3 beginPos = FishGameObjectPoolManager.Instance.GetPoolParent(PoolType.EffectPool).transform.TransformPoint(FishCsharpManager.ScreenPointToRealPoint(data.usBombPosX, data.usBombPosY));
                    Vector3 nextPos = FishGameObjectPoolManager.Instance.GetPoolParent(PoolType.EffectPool).transform.TransformPoint(FishCsharpManager.ScreenPointToRealPoint(data.usNextBombPosX, data.usNextBombPosy));
                    var skillIns = GetSkill(UID, beginPos, nextPos, bombCount, playerIns, skillStatus, skillTime, killFishUID, 47);
                    if (skillIns != null)
                    {
                        skillIns.objResName = "Skill_DragonTurtle";
                        var skillTemp = skillIns as FishDragonTurtleSkillItem;
                        skillTemp.ResetSkillState(beginPos, skillTime);
                        skillTemp.ShowSkill();
                    }
                }
            }
            else
                Debug.LogError("DragonTurtleSkill is currently in the exploding state");
        }
    }
}
