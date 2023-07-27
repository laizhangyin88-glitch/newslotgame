using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{

public class WeightProgress
{
	private class ProgressItem
	{
		public float progress;
		public float weight;
	}
	private Dictionary<string, ProgressItem> weightDict = new Dictionary<string, ProgressItem>();

	public void Clear()
	{
		weightDict.Clear();
	}

	public float GetTotalProgress()
	{
		if (weightDict.Count == 0)
			return 0f;

		float totalProgress = 0f;
		float totalWeight = 0f;
		foreach (var pair in weightDict)
		{
			var item = pair.Value;
			totalProgress += item.progress * item.weight;
			totalWeight += item.weight;
		}
		return totalProgress / totalWeight;
	}

	public void AddProgress(string key, float weight)
	{
		weightDict[key] = new ProgressItem { weight = weight };
	}

	public void UpdateProgress(string key, float progress)
	{
		weightDict[key].progress = progress;
	}
}

}
