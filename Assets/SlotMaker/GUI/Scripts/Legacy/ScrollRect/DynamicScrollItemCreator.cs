using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using Sirenix.OdinInspector;

namespace SlotMaker
{
	public abstract class DynamicScrollItemCreator : MonoBehaviour 
	{
		public DynamicScrollRect dynamicScrollRect;

		protected RectTransform content
		{
			get 
			{
				return dynamicScrollRect.content;
			}
		}

	    [ReadOnly]
	    public int beginIndex;
	    private int _frontIndex;
		public int frontIndex
	    {
	        get { return _frontIndex; }
	        set 
	        { 
	            _frontIndex = value;
	            beginIndex = Mathf.Min(beginIndex, _frontIndex);
	        }
	    }

	    [ReadOnly]
	    public int endIndex;
	    private int _backIndex;
		public int backIndex
	    {
	        get { return _backIndex; }
	        set 
	        {
	            _backIndex = value;
	            endIndex = Mathf.Max(endIndex, _backIndex);
	        }
	    }

		public int itemTotalCount { get { return endIndex - beginIndex; } }
	    public int itemCount { get { return backIndex - frontIndex; } }

		public Layout.Axis axis;
		public float bufferingEdge = 0.05f;
		public int bufferingCount = 3;
		public int maxBufferingCount = 10;

		public void OnValueChanged(Vector2 normalizedPosition)
		{
			if (axis == Layout.Axis.Horizontal)
			{
				if (normalizedPosition.x < 0f + bufferingEdge)
				{
					if (PushFrontBuffers() > 0)
						RebuildContentBounds();

					PopBackBuffers();
				}
				else if (normalizedPosition.x > 1f - bufferingEdge)
				{
					if (PopFrontBuffers() > 0)
						RebuildContentBounds();

					PushBackBuffers();
				}
			}
			else if (axis == Layout.Axis.Vertical)
			{
				if (normalizedPosition.y < 0f + bufferingEdge)
				{
					if (PopFrontBuffers() > 0)
						RebuildContentBounds();

					PushBackBuffers();
				}
				else if (normalizedPosition.y > 1f - bufferingEdge)
				{
					if (PushFrontBuffers() > 0)
						RebuildContentBounds();

					PopBackBuffers();
				}
			}
		}

		public abstract void OnInitialize();

		protected abstract bool PushFront();

		protected abstract bool PushBack();

		protected abstract bool PopFront();

		protected abstract bool PopBack();

		protected void RebuildContentBounds()
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(content);
			dynamicScrollRect.RebuildContentBounds();
		}

		protected int PushFrontBuffers()
		{
			int count = 0;
			for (; count < bufferingCount; ++count)
			{
				if (!PushFront())
					break;
			}
			return count;
		}

		protected int PushBackBuffers()
		{
			int count = 0;
			for (; count < bufferingCount; ++count)
			{
				if (!PushBack())
					break;
			}
			return count;
		}

		protected int PopFrontBuffers()
		{
			int count = 0;
			for (; content.childCount > maxBufferingCount; ++count)
			{
				if (!PopFront())
					break;
			}
			return count;
		}

		protected int PopBackBuffers()
		{
			int count = 0;
			for (; content.childCount > maxBufferingCount; ++count)
			{
				if (!PopBack())
					break;
			}
			return count;
		}
	}
}
