using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Keno
{
    [Serializable]
    public class TicketInfo
    {
        public List<SpotInfo> spotInfos = new List<SpotInfo>();
        public int spotCount { get { return spotInfos.Count; } }

        public SpotInfo GetSpotInfo(int index)
        {
            return spotInfos[index];
        }

        public void AddSpotInfo(SpotInfo spotInfo)
        {
            spotInfos.Add(spotInfo);
        }

        public TicketInfo Clone()
        {
            var newTicket = new TicketInfo();
            newTicket.spotInfos = SpotInfo.CloneList1(spotInfos);

            return newTicket;
        }
    }
}
