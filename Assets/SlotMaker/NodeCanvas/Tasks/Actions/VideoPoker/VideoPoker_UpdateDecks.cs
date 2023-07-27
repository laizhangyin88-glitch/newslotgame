using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Cards.Tasks.Actions
{
    [Category("★ SlotMaker/Cards")]
    public class VideoPoker_UpdateDecks : ActionTask
    {
        public BBParameter<GameObject> videoPoker;
        public BBParameter<List<Blackboard>> decks;

        protected override string info { get { return string.Format("VideoPoker.UpdateDecks({0})", decks); } }

        protected override void OnExecute()
        {
            var newDecks = new List<List<int>>();
            int deckCount = decks.value.Count;
            for (int i = 0; i < deckCount; ++i)
            {
                newDecks.Add(decks.value[i].GetValue<List<int>>("cardList"));
            }
            videoPoker.value.GetComponent<VideoPoker>().UpdateDecks(newDecks);

            EndAction();
        }
    }
}