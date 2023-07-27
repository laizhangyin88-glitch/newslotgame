using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NodeCanvas;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using SlotMaker;

namespace BagelCode
{
    [ExecuteInEditMode]
    public class MetaSlotMachineGlobalReelStrips : MonoWeakSingleton<MetaSlotMachineGlobalReelStrips>, IGlobalReelStrips
	{
		public int _index;
		public int index { get { return _index; } set { _index = value; } }

		public int stripsCount { get { return stripsList.Count; } }

		public List<ReelStrips> stripsList;

		public ReelStrips GetReelStrips()
		{
			return stripsList[index];
		}
	}
}