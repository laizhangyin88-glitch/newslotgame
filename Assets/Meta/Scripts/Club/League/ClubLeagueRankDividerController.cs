using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode;
using ParadoxNotion;
using NodeCanvas.Framework;
using BagelCode.OSA_Scroll;

namespace BagelCode
{
    public class ClubLeagueRankDividerController : MonoBehaviour
    {
        private ContextElement rootElement;

        private ContextElement textElement;

        private bool isInit = false;

        private const string PROMOTE_TEXT = "CLUB_LEAGUE_CELL_DIVIDER_PROMOTE_TEXT";
        private const string DEMOTE_TEXT = "CLUB_LEAGUE_CELL_DIVIDER_DEMOTE_TEXT";

        private void InitProperty()
        {
            if(isInit) return;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            textElement = ContextUtils.FindElement(rootElement, "Text Club League Divider Context", ContextSearchingType.ChildrenSearch);

            isInit = true;
        }

        public void UpdateVariables(OSA_Scroll.ClubLeagueRankModel_Divider model)
        {
            InitProperty();

            if(model.cellType == ClubLeagueRankCellType.Promote)
            {
                MetaContextElementUtils.SetText(textElement, StringTableUtils.GetString(StringTable.StringTableType.Global, PROMOTE_TEXT, model.index + 1));
            }
            else
            {
                MetaContextElementUtils.SetText(textElement, StringTableUtils.GetString(StringTable.StringTableType.Global, DEMOTE_TEXT));
            }
        }
    }
}
