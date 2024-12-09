using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SimpleJSON;

namespace SlotMaker.Cards.Tasks.Actions
{
    [Category("★ SlotMaker/Cards")]
    public class VideoPoker_GetMultipliers : ActionTask 
    {
        public BBParameter<long> betCredit;
        public BBParameter<List<long>> saveAs;

        protected override void OnExecute()
        {
            List<long> multipliers = new List<long>();
            if (LastFreeGameManager.Instance.isLastGameSpin)
            {
                List<List<long>> multiplierList = new List<List<long>>();
                for (int i = 0; i < LastFreeGameManager.Instance.historyRes.Count; i++)
                {
                    string response = LastFreeGameManager.Instance.historyRes[i];
                    if (response.Contains("jacks_draw") && response.Contains("multiplierList"))
                    {
                        JSONNode node = JSONNode.Parse(response);
                        JSONNode clientData = node["client_data"];
                        for (int j = 0; j < clientData["multiplierList"].Count; j++)
                        {
                            JSONNode temp = clientData["multiplierList"][j];
                            List<long> tempList = new List<long>();
                            for (int k = 0; k < temp.Count; k++)
                            {
                                long value = temp[k].AsLong;
                                tempList.Add(value);
                            }
                            multiplierList.Add(tempList);
                        }
                    }
                }
                if (multiplierList.Count > 0)
                {
                    List<Blackboard> handMetaInfoListNew = BlackboardUtils.FindVariable<List<Blackboard>>("./game/handMetaInfoPerBet").value;
                    for (int i = 0; i < handMetaInfoListNew.Count; ++i)
                    {
                        Blackboard bb = handMetaInfoListNew[i];

                        List<Blackboard> metaInfoListPerHandList = bb.GetValue<List<Blackboard>>("handMetaInfoPerHand");
                        for (int j = 0; j < metaInfoListPerHandList.Count; ++j)
                        {
                            metaInfoListPerHandList[j].SetValue("multiplier", multiplierList[i][j]);
                        }
                    }
                }
            }

            List<Blackboard> handMetaInfoList = BlackboardUtils.FindVariable<List<Blackboard>>("./game/handMetaInfoPerBet").value;
            for (int i = 0; i < handMetaInfoList.Count; ++i)
            {
                Blackboard bb = handMetaInfoList[i];
                long betPerHand = bb.GetValue<long>("betPerHand");

                if (betPerHand == betCredit.value)
                {
                    List<Blackboard> metaInfoListPerHandList = bb.GetValue<List<Blackboard>>("handMetaInfoPerHand");
                    for (int j = 0; j < metaInfoListPerHandList.Count; ++j)
                    {
                        long temp = metaInfoListPerHandList[j].GetValue<long>("multiplier");
                        multipliers.Add(temp);
                    }
                }
            }
            saveAs.value = multipliers;

            EndAction();
        }
    }
}
