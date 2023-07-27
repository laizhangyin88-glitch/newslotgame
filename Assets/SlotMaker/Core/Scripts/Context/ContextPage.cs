using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
	public class ContextPage : ContextCompositor, IContextIntProperty
	{
	    public PageScrollRect page;

		public void SetIntProperty(int value)
	    {
	        page.pageIndex = value;
	    }

	    public int GetIntProperty()
	    {
	        return page.pageIndex;
	    }
	}
}
