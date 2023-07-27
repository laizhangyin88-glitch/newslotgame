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
    public class ClubArenaMyChestController : ClubArenaChestBaseController
    {
        protected ContextElement pointAppearTextElement;
        protected ContextElement shieldAppearTextElement;

        protected override void InitProperty()
        {
            base.InitProperty();

            pointAppearTextElement = ContextUtils.FindElement(rootElement, "Text Appear Point", ContextSearchingType.ChildrenSearch);
            shieldAppearTextElement = ContextUtils.FindElement(rootElement, "Text Appear Shield", ContextSearchingType.ChildrenSearch);
        }

        protected override void InitData()
        {
            base.InitData();

            ClubArenaPersonal myStateClass = ClubArenaUtils.MyStateClass;
            if (myStateClass == null)
                return;
        }
    }
}