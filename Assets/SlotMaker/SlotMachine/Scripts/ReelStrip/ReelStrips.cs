using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SlotMaker
{
	public class ReelStrips : MonoBehaviour, IReelStrips
	{
		public bool singleStrip;
		public int reelCount { get { return reelStrips.Count; } }

	    public List<BaseReelStrip> reelStrips;

		public BaseReelStrip GetReelStrip(int reelIndex)
		{
            int rI;
            if(reelIndex < 0 || reelIndex > reelStrips.Count)
            {
                rI = 0;
            }
            else
            {
                rI = reelIndex;
            }
			return reelStrips[singleStrip ? 0 : rI];
		}

		public void SetReelStrip(int reelIndex, IReelStrip reelStrip)
		{
			GetReelStrip(reelIndex).CopyFrom(reelStrip);
		}
	}
}
