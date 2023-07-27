using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.Keno
{
    public class KenoHelper : MonoBehaviour
    {
        [BoxGroup("Spot Number Helper")]
        public TicketInstance ticket;

        [BoxGroup("Spot Number Helper")]
        public int startNum;

        [BoxGroup("Spot Number Helper")]
        [Button]
        private void SpotNumberHelper()
        {
            foreach(SpotInstance spot in ticket.spots)
            {
                spot.number = startNum++;
            }
        }

        [BoxGroup("Spot Text Helper")]
        public List<ContextTextMeshProUGUI> texts;

        [BoxGroup("Spot Text Helper")]
        public int textStartNum;

        [BoxGroup("Spot Text Helper")]
        [Button]
        private void SpotTextHelper()
        {
            foreach(ContextTextMeshProUGUI text in texts)
            {
                text.SetText(textStartNum.ToString());
                textStartNum++;
            }
        }
    }
}
