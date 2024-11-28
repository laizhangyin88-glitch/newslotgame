using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker
{
	public class ContentBlackboard : MonoWeakSingleton<ContentBlackboard>
	{
		public Blackboard blackboard;

		public enum EntityType
		{
			Turn,
			Spin,
			Bonus
		};

		private static readonly string TURN = "turn";
        private static readonly string SPIN = "spin";
        private static readonly string BONUS = "bonus";

		public static Blackboard Get()
		{
			return Instance.blackboard;
		}

        public static Blackboard Get(EntityType entityType)
        {
			switch (entityType)
			{
			case EntityType.Turn:
				return Turn();
			case EntityType.Spin:
				return Spin();
			case EntityType.Bonus:
				return Bonus();
			}
			return null;
        }

		public static Blackboard Turn()
		{
			return Get().GetVariable<Blackboard>(TURN).value;
		}

        public static Blackboard Spin()
        {
            return Get().GetVariable<Blackboard>(SPIN).value;
        }

        public static Blackboard Bonus()
        {
            return Get().GetVariable<Blackboard>(BONUS).value;
        }
	}
}
