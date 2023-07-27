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
    public class ClubArenaOpponentChestController : ClubArenaChestBaseController
    {
        protected override void InitProperty()
        {
            base.InitProperty();
        }

        protected override void InitData()
        {
            base.InitData();

            ClubArenaPersonalSimpleWithProfile opponentStateClass = ClubArenaUtils.OpponentStateClass;
            if (opponentStateClass == null)
                return;
        }
    }
}