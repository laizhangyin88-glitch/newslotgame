using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker
{
	public class ProbabilityReelStrip : BaseReelStrip
	{
		public int _stripIndex;
	    public override int stripIndex { get { return _stripIndex; } set { _stripIndex = value; } }

		public List<SymbolInfo> strip;
		public List<float> probabilities;

	    public override int stripCount { get { return strip.Count; } }

		protected int _stripSubSymbolOffset = 0;
	    public override int stripSubSymbolOffset { get { return _stripSubSymbolOffset; } set { _stripSubSymbolOffset = value; } }

	    public override int GetRandomIndex() { return 0; }

		public override int CalcIndex(int idx)
	    {
	        if (idx < 0)
	            idx += strip.Count;
	        else if (idx > (strip.Count - 1))
	            idx -= strip.Count;

	        return idx;
	    }

	    public override SymbolInfo GetSymbol(int index)
		{
			float rnd = Random.value;
			for (int i = 0; i < strip.Count - 1; ++i)
			{
				if (rnd < probabilities[i])
					return strip[i];
			}
			return strip[strip.Count - 1];
		}

		[Button]
		public void SetByDefault()
		{
			var symbolMask = ContentCustomData.GetSlotData(0).symbolMask;
			strip = new List<SymbolInfo>();
			for (int i = 0; i < ContentCustomData.Instance.symbolCount; ++i)
				strip.Add(SlotUtils.CreateSymbolInfo(i, symbolMask));

			NormalizeProbabilities();
		}

		[Button]
		public void NormalizeProbabilities()
		{
			if (strip.Count != probabilities.Count)
			{
				probabilities = new List<float>();
				for (int i = 0; i < strip.Count; ++i)
					probabilities.Add(1f);
			}

			float totalWeight = 0f;
			for (int i = 0; i < probabilities.Count; ++i)
				totalWeight += probabilities[i];

			float acc = 0f;
			for (int i = 0; i < probabilities.Count; ++i)
			{
				acc += probabilities[i];
				probabilities[i] = acc / totalWeight;
			}
		}
	}
}
