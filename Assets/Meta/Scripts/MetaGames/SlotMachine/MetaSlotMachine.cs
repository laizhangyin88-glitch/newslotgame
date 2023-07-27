using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using SlotMaker;

namespace BagelCode
{
    public class MetaSlotMachine : SlotMachine
    {
        public MetaSlotMachineMovement metaMovement;

        public override BaseReel CreateReel()
        {
            var po = reelPool.GetObject();
            return po.GetComponent<MetaSlotMachineReel>();
        }

        public BaseReel CreateMetaSlotMachineReel(int beginColumn, int beginRow, int endColumn, int endRow, int expandTopCount)
        {
            var reel = CreateReel();
            reel.transform.SetParent(reelsTransform, false);
            reel.Initialize(this, reels.Count, beginColumn, beginRow, endColumn, endRow, expandTopCount);
            reels.Add(reel);
            return reel;
        }
    }
}