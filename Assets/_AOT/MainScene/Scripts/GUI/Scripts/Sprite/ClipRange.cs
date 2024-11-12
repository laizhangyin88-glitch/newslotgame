using UnityEngine;
using System.Collections;

namespace SlotMaker
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(SpritePanel))]
	public class ClipRange : MonoBehaviour 
	{
		private SpritePanel _panel;
		private SpritePanel panel
		{
			get 
			{
				if (_panel == null)
					_panel = GetComponent<SpritePanel>();
				return _panel;
			}
		}

		public void UpdateClipRange(Vector4 clipRange)
		{
			panel.clipRange = clipRange;
		}
	}
}