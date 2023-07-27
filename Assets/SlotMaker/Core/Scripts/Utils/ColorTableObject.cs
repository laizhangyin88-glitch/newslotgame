using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
	[CreateAssetMenu(fileName="New ColorTableObject", menuName="SlotMaker/ScriptableObject/ColorTableObject")]
	public class ColorTableObject : ScriptableObject
	{
		public List<Color> colors;

		public Color GetColor(int index)
		{
			while (index >= colors.Count)
				index -= colors.Count;

			return colors[index];
		}
	}
}
