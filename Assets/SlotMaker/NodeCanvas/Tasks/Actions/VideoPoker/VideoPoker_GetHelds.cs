using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SimpleJSON;
using static GameUtil.Timer;

namespace SlotMaker.Cards.Tasks.Actions
{
    [Category("★ SlotMaker/Cards")]
    public class VideoPoker_GetHelds : ActionTask
    {
        public BBParameter<GameObject> videoPoker;

        public BBParameter<List<bool>> saveAs;

        protected override string info { get { return string.Format("{0} = VideoPoker.GetHelds()", saveAs); } }

        protected override void OnExecute()
        {
            if (LastFreeGameManager.Instance.isLastGameSpin)
            {
                if(LastFreeGameManager.Instance.historyRes.Count > 0)
                {
                    string response = LastFreeGameManager.Instance.historyRes[0];
                    if (response != null)
                    {
                        JSONNode node = JSONNode.Parse(response);
                        JSONNode clientData = node["client_data"];
                        List<bool> helds = new List<bool> ();
                        for (int i = 0; i < clientData["helds"].Count; i++)
                        {
                            bool temp = clientData["helds"][i].AsBool;
                            helds.Add (temp);
                        }
                        var handInstance = videoPoker.value.GetComponent<VideoPoker>().GetHand();
                        for (int i = 0; i < helds.Count; i++)
                        {
                            if (helds[i] && !handInstance.cards[i].held)
                            {
                                handInstance.cards[i].OnClick();
                            }
                        }
                        saveAs.value = helds;
                        var timer = TimerExtensions.DelayAction(handInstance, 1f, () =>
                        {
                            EndAction();
                        });
                        timer.Restart(UpdateMode.RealTime);
                    }
                }
            }
            else
            {
                saveAs.value = videoPoker.value.GetComponent<VideoPoker>().GetHelds();
                EndAction();
            }
        }
    }
}
