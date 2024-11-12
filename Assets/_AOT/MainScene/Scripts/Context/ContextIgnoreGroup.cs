using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SlotMaker
{
	public class ContextIgnoreGroup : ContextElement, IContextIgnoreGroup 
	{
		public override bool IsRoot { get { return false; } }
		
		public override bool IsLeaf { get { return false; } }
		
		public override void AddContextElement(ContextElement element) {}
		public override void RemoveContextElement(ContextElement element) {}
		public override void DestroyElement() {}
		
		public override ContextElement Find(string name, bool deepSearch = false) { return null; }

		public override ContextElement FindWithFullName(string fullName) { return null; }

		public override int ChildCount { get { return 0; } }

		public override ContextElement GetChildElement(int index) { return null; }

		public override IEnumerator GetEnumerator() { return null; }

		public override void UpdateContext(bool forceUpdate = false) {}
	}
}