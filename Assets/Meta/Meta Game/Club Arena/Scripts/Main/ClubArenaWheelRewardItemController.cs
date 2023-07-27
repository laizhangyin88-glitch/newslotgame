using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

namespace BagelCode.ClubArena
{
    public class ClubArenaWheelRewardItemController : MonoBehaviour
    {
        private ClubArenaSpinResultType type;

        public ClubArenaSpinResultType Type { get { return type; } }

        public void OnInit(ClubArenaSpinResultType _type)
        {
            type = _type;

            switch (type)
            {
                case ClubArenaSpinResultType.POINT_10:
                case ClubArenaSpinResultType.POINT_50:
                case ClubArenaSpinResultType.ATTACK_10:
                case ClubArenaSpinResultType.ATTACK_20:
                case ClubArenaSpinResultType.ATTACK_50:
                case ClubArenaSpinResultType.ATTACK_100:
                case ClubArenaSpinResultType.STEAL:
                    GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_POINT_FLY).Play();
                    break;
                case ClubArenaSpinResultType.SHIELD:
                    GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_SHIELD_FLY).Play();
                    break;
            }
        }

        public void OnClose()
        {

        }
    }
}