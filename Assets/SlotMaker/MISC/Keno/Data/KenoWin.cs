using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SlotMaker.Keno
{
    [System.Serializable]
    public class KenoWin 
    {
        public long earnCredit;
        public long multiplier;
        public int hitCount;
        public List<SpotInstance> spots = new List<SpotInstance>();

        public KenoWin()
        {
            earnCredit = 0L;
            multiplier = 1L;
            spots       = new List<SpotInstance>();
            hitCount    = 0;
        }
    }
}
