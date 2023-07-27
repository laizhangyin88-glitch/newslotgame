using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Cards.Tasks.Actions
{
    [Category("★ SlotMaker/Cards")]
    public class CreateRandomCard : ActionTask 
    {
        public BBParameter<List<bool>> includeSuits = new List<bool>(new bool[4]);
        public BBParameter<int> saveAsCard;

        protected override string info{
            get { return string.Format("Create Random Card"); }
        }

        protected override void OnExecute()
        {
            int cardNumber = Random.Range(1, 14); // 1 ~ 13
            int randomPick = Random.Range(0, 4); // 0 ~ 3
            int suit = 0;

            for (int i = 0; i < 4; ++i)
            {
                int pick = (i + randomPick) % 4;
                if (includeSuits.value[ pick ]) 
                {
                    suit = pick;
                    break;
                }
            }

            saveAsCard.value = cardNumber * 4 + suit;

            EndAction();
        }
    }

}