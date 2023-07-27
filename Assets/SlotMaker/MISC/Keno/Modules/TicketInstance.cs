using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace SlotMaker.Keno
{
    public class TicketInstance : MonoBehaviour
    {
        public int column;
        public int row;
        public int minPickCount;
        public int maxPickCount;

        private int _pickCount;
        public int pickCount
        {
            get
            {
                _pickCount = 0;
                foreach (var spot in spots)
                {
                    if (spot.markState == MarkState.Mark)
                        _pickCount++;
                }
                return _pickCount;
            }
        }
        private int _hitCount;
        public int hitCount
        {
            get
            {
                _hitCount = 0;
                foreach (var spot in spots)
                {
                    if (spot.IsHit)
                        _hitCount++;
                }
                return _hitCount;
            }            
        }

        // [HideInInspector]
        public TicketInfo ticketInfo;
        public List<SpotInstance> spots;

        [InlineEditor]        
        public KenoMediator mediator;

        public SpotInstance GetSpot(int index)
        {
            return spots[index];
        }

        public void Initialize()
        {
            foreach (var spot in spots)
                ticketInfo.AddSpotInfo(spot.spotInfo);
        }

        public void Refresh()
        {
            for (int i = 0; i < ticketInfo.spotCount; ++i)
            {
                spots[i].spotInfo = ticketInfo.GetSpotInfo(i);
                spots[i].SendEvent("Apply");
            }
        }

        public void Win(List<SpotInstance> spots)
        {
            foreach (SpotInstance spot in spots)
                spot.SendEvent("Win");
        }

        public void SkipWin()
        {
            BroadCastSpotEvent("SkipWin");
        }

        public void BroadCastSpotEvent(string eventName)
        {
            foreach (SpotInstance spot in spots)
                spot.SendEvent(eventName);
        }

        public void SendSpotEvent(int index, string eventName)
        {
            spots[index].SendEvent(eventName);
        }

        public TicketInstance Save()
        {
            var go = new GameObject();
    		go.name = "Ticket Board";
    		var snapshot = go.AddComponent<TicketInstance>();
            snapshot.ticketInfo = ticketInfo.Clone();

    		return snapshot;
        }

        public void Load(TicketInstance snapshot)
    	{
            ticketInfo.spotInfos = new List<SpotInfo>();
            for (var i = 0; i < spots.Count; i++) {
                spots[i].spotInfo = (SpotInfo)snapshot.ticketInfo.GetSpotInfo(i).Clone();
                ticketInfo.spotInfos.Add(spots[i].spotInfo);
            }

    	}
    }
}
