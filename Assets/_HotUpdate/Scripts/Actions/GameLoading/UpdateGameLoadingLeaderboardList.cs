using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.EpicAlbum
{
    [Category("★ BagelCode/Game Loading")]
    public class UpdateGameLoadingLeaderboardList : ActionTask<ContextElement>
    {
        public BBParameter<int> gameID;

        private List<Blackboard> rankBBList;

        private List<ContextElement> userCellList;

        protected override string info
        {
            get { return "Update Game Loading Leaderboard"; }
        }

        protected override void OnExecute()
        {
            var bigWinRankBBList = BlackboardUtils.GetOrCreateVariable<List<Blackboard>>(MainBlackboard.Get(), "bigwinRankList").value;

            rankBBList = null;
            for(int i=0; i < bigWinRankBBList.Count; ++i)
            {
                var id = bigWinRankBBList[i].GetValue<int>("gameId");

                if(id == gameID.value)
                {
                    rankBBList = bigWinRankBBList[i].GetValue<List<Blackboard>>("rankList");
                    break;
                }
            }

            for(int i=0; i < 6; ++i)
            {
                var userCellElement = ContextUtils.FindElement(agent, string.Format("Leaderboard Cell {0}", i), ContextSearchingType.ChildrenSearch);

                if(userCellElement != null)
                {
                    MetaContextElementUtils.SetBlackboardValue<int>(userCellElement, "index", i+1);
                    if(rankBBList != null && rankBBList.Count > i)
                    {
                        MetaContextElementUtils.SetBlackboardValue<Blackboard>(userCellElement, "rank", rankBBList[i]);
                    }

                    MetaContextElementUtils.SetBlackboardValue<bool>(userCellElement, "update", true);
                    userCellElement.gameObject.SetActive(true);
                }
            }

            EndAction();
        }
    }
}