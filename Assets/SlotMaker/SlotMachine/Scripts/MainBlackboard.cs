using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker
{
	public class MainBlackboard : MonoWeakSingleton<MainBlackboard>
	{
		public Blackboard blackboard;

		public static Blackboard Get()
		{
			return Instance.blackboard;
		}
	}
}
