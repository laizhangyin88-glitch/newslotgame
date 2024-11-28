using UnityEngine;
using System.Collections;

namespace SlotMaker.Layout
{
	public enum Axis
	{
		Horizontal,
		Vertical
	};

	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	public abstract class LayoutGroup : MonoBehaviour, ILayoutGroup
	{
		public RectOffset _padding = new RectOffset();
		public RectOffset padding 
		{ 
			get { return _padding; }
			set { SetProperty(ref _padding, value); } 
		}
		public TextAnchor _childAlignment = TextAnchor.UpperLeft;
		public TextAnchor childAlignment 
		{ 
			get { return _childAlignment; } 
			set { SetProperty(ref _childAlignment, value); }
		}
		public Vector4 _spacing = Vector3.zero;
		public Vector4 spacing 
		{ 
			get { return _spacing; } 
			set { SetProperty(ref _spacing, value); }
		}

		private RectTransform _rectTransform;
		protected RectTransform rectTransform 
		{
			get 
			{
				if (_rectTransform == null)
					_rectTransform = GetComponent<RectTransform>();
				return _rectTransform;
			}
		}

		private Vector2 totalMinSize = Vector2.zero;
		private Vector2 totalPreferredSize = Vector2.zero;
		private Vector2 totalFlexibleSize = Vector2.zero;

		protected float GetTotalMinSize(int axis)
		{
			return totalMinSize[axis];
		}

		protected float GetTotalPreferredSize(int axis)
		{
			return totalPreferredSize[axis];
		}

		protected float GetTotalFlexibleSize(int axis)
		{
			return totalFlexibleSize[axis];
		}

		protected float GetStartOffset(int axis, float requiredSpaceWithoutPadding)
		{
			float requiredSpace = requiredSpaceWithoutPadding + (axis == 0 ? padding.horizontal : padding.vertical);
			float availableSpace = rectTransform.rect.size[axis];
			float surplusSpace = availableSpace - requiredSpace;
			float alignmentOnAxis = 0;
			if (axis == 0)
				alignmentOnAxis = ((int)childAlignment % 3) * 0.5f;
			else
				alignmentOnAxis = ((int)childAlignment / 3) * 0.5f;
			return (axis == 0 ? padding.left : padding.top) + surplusSpace * alignmentOnAxis;
		}

		protected void SetLayoutInputForAxis(float totalMin, float totalPreferred, float totalFlexible, int axis)
		{
			totalMinSize[axis] = totalMin;
			totalPreferredSize[axis] = totalPreferred;
			totalFlexibleSize[axis] = totalFlexible;
		}

		protected virtual void OnEnable()
		{
			SetDirty();
		}

		protected virtual void OnDisable()
		{

		}

		protected virtual void OnTransformChildrenChanged()
	    {
	        SetDirty();
	    }

		protected bool dirty = false;
		public virtual void SetDirty()
		{
			if (!isActiveAndEnabled)
				return;

			dirty = true;
		}

		public abstract void CalculateLayoutInputHorizontal();
		public abstract void CalculateLayoutInputVertical();
		public abstract void SetLayoutHorizontal();
	    public abstract void SetLayoutVertical();

		protected void SetProperty<T>(ref T currentValue, T newValue)
		{
			if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
	                return;
	        currentValue = newValue;
	        SetDirty();
		}
	}
}