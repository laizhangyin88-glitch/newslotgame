using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using NodeCanvas.Framework;
using SlotMaker.Json;

namespace SlotMaker
{
	public class PayLines : MonoBehaviour
	{
		public List<LineRenderer> lines;

		public void Play(int lineIndex, string animationName)
		{
			var line = lines[lineIndex];
			line.gameObject.SetActive(true);
			line.GetComponent<Animator>().SetTrigger(animationName);
		}

		public void Stop()
		{
			int count = lines.Count;
			for (int i = 0; i < count; ++i)
			{
				lines[i].gameObject.SetActive(false);
			}
		}
	}
}
