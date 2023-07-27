using UnityEngine;
using System.Collections;

namespace SlotMaker.Layout
{
	public interface ILayoutElement
	{
		bool  ignoreLayout    { get; }
		float minWidth        { get; }
		float minHeight       { get; }
		float preferredWidth  { get; }
		float preferredHeight { get; }
		float flexibleWidth   { get; }
		float flexibleHeight  { get; }
	}

	[RequireComponent(typeof(RectTransform))]
	public class LayoutElement : MonoBehaviour, ILayoutElement
	{
		public bool _ignoreLayout;
		public bool ignoreLayout { get { return _ignoreLayout; } }
		public float _minWidth;
		public float minWidth { get { return _minWidth; } }
		public float _minHeight;
		public float minHeight { get { return _minHeight; } }
		public float _preferredWidth;
		public float preferredWidth { get { return preferredWidth; } }
		public float _preferredHeight;
		public float preferredHeight { get { return _preferredHeight; } }
		public float _flexibleWidth;
		public float flexibleWidth { get { return _flexibleWidth; } }
		public float _flexibleHeight;
		public float flexibleHeight { get { return _flexibleHeight; } }	
	}
}