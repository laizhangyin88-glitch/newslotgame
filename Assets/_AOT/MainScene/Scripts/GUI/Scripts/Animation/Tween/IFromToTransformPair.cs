using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
	public interface IFromToTransformPair
	{
		Transform From { get; }
		Transform To { get; }
	}
}