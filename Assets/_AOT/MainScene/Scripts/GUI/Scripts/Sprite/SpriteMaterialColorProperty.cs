using UnityEngine;
using System.Collections;

namespace SlotMaker
{
	[ExecuteInEditMode]
	[AddComponentMenu("SlotMaker/UI/Sprite/Property/Color")]
	[RequireComponent(typeof(SpritePanel))]
	public class SpriteMaterialColorProperty : MonoBehaviour
	{
		public Color value;

		SpritePanel _panel;
		SpritePanel panel { get { return _panel ?? (_panel = GetComponent<SpritePanel>()); } }
		
		void Update()
		{
	        panel.color = value;
		}
	}
}
