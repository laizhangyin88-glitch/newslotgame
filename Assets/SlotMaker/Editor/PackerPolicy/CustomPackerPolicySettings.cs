using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
	[CreateAssetMenu(fileName="CustomPackerPolicySettings", menuName="SlotMaker/ScriptableObject/CustomPackerPolicyObject")]
	public class CustomPackerPolicySettings : ScriptableObjectSingleton<CustomPackerPolicySettings>
	{
		public List<string> packingTags;
	}
}