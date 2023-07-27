using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace SlotMaker
{
	public class ContextList : ContextElement, IContextList
	{
		private const char SPLIT_CHARACTER = '/';

		public bool isRoot;
		public override bool IsRoot { get { return isRoot; } }

		public override bool IsLeaf { get { return false; } }

		protected List<ContextElement> elementList = new List<ContextElement>();

		public override void AddContextElement(ContextElement element)
		{
			element.Parent = this;

			elementList.Add(element);
		}

		public override void RemoveContextElement(ContextElement element)
		{
			element.Parent = null;

			elementList.Remove(element);
			element.DestroyElement();
		}

		public override ContextElement Find(string name, bool deepSearch = false)
	    {
			int count = elementList.Count;
			for (int i = 0; i < count; ++i)
	        {
				var childElement = elementList[i];
				if (childElement.name.Equals(name, StringComparison.Ordinal))
	                return childElement;

	            if (deepSearch)
	            {
	                ContextElement tempElement = childElement.Find(name, deepSearch);
	                if (tempElement != null) return tempElement;
	            }
	        }

	        return null;
	    }

	    public override ContextElement FindWithFullName(string fullName)
	    {
	        var tokens = fullName.Split(SPLIT_CHARACTER);
	        if (tokens == null || tokens.Length == 0)
	            return null;

	        ContextElement tempElement = this;
			int count = tokens.Length;
	        for (int i = 0; i < count; ++i)
	        {
	            tempElement = tempElement.Find(tokens[i]);
	            if (tempElement == null) return null;
	        }
	        return tempElement;
	    }

		public override int ChildCount { get { return elementList.Count; } }

		public override ContextElement GetChildElement(int index)
		{
	        return elementList[index];
		}

		public override IEnumerator GetEnumerator()
		{
			return (elementList as IEnumerable).GetEnumerator();
		}

		public override void UpdateContext(bool forceUpdate = false)
		{
			if (IsDirty() || forceUpdate)
			{
				if (forceUpdate)
					elementList.Clear();

				UpdateContext(this, transform, forceUpdate);
				dirty = false;
			}
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("[");
			for (int i = 0; i < elementList.Count; ++i)
			{
				sb.Append(string.Format("{{\"{0}\":{1}}}{2}", elementList[i].ContextName, elementList[i].ToString(), (i < elementList.Count - 1) ? "," : ""));
			}
			sb.Append("]");
			return sb.ToString();
		}
	}
}
