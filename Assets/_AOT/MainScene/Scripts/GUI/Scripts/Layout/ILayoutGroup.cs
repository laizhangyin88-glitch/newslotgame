using UnityEngine;
using System.Collections;

namespace SlotMaker.Layout
{
	public interface ILayoutController
	{
	    void SetLayoutHorizontal();
	    void SetLayoutVertical();
	}

	public interface ILayoutGroup : ILayoutController
	{
	}
}
