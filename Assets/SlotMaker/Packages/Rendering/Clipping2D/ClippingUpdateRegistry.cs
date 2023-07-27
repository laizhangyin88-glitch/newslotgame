using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlotMaker.Rendering.Clipping2D
{
	// See UnityEngine.UI.Collections.IndexedSet<T> (internal class)
	// 
	//This is a container that gives:
    //  - Unique items
    //  - Fast random removal
    //  - Fast unique inclusion to the end
    //  - Sequential access
    //Downsides:
    //  - Uses more memory
    //  - Ordering is not persistent
    //  - Not Serialization Friendly.

    //We use a Dictionary to speed up list lookup, this makes it cheaper to guarantee no duplicates (set)
    //When removing we move the last item to the removed item position, this way we only need to update the index cache of a single item. (fast removal)
    //Order of the elements is not guaranteed. A removal will change the order of the items.
	internal class IndexedSet<T> : IList<T>
	{
		readonly List<T> list = new List<T>();
		Dictionary<T, int> dictionary = new Dictionary<T, int>();

		public void Add(T item)
        {
            list.Add(item);
            dictionary.Add(item, list.Count - 1);
        }

        public bool AddUnique(T item)
        {
            if (Contains(item))
                return false;

            Add(item);
            return true;
        }

        public bool Remove(T item)
        {
            int index = -1;
            if (!dictionary.TryGetValue(item, out index))
                return false;

            RemoveAt(index);
            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Clear()
        {
            list.Clear();
            dictionary.Clear();
        }

        public bool Contains(T item)
        {
            return dictionary.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public int Count { get { return list.Count; } }
        public bool IsReadOnly { get { return false; } }
        public int IndexOf(T item)
        {
            int index = -1;
            if (dictionary.TryGetValue(item, out index))
                return index;
            return -1;
        }

        public void Insert(int index, T item)
        {
            //We could support this, but the semantics would be weird. Order is not guaranteed..
            throw new NotSupportedException("Random Insertion is semantically invalid, since this structure does not guarantee ordering.");
        }

        public void RemoveAt(int index)
        {
            T item = list[index];
            dictionary.Remove(item);
            if (index == list.Count - 1)
                list.RemoveAt(index);
            else
            {
                int replaceItemIndex = list.Count - 1;
                T replaceItem = list[replaceItemIndex];
                list[index] = replaceItem;
                dictionary[replaceItem] = index;
                list.RemoveAt(replaceItemIndex);
            }
        }

        public T this[int index]
        {
            get { return list[index]; }
            set
            {
                T item = list[index];
                dictionary.Remove(item);
                list[index] = value;
                dictionary.Add(item, index);
            }
        }

        public void RemoveAll(Predicate<T> match)
        {
            //I guess this could be optmized by instead of removing the items from the list immediatly,
            //We move them to the end, and then remove all in one go.
            //But I don't think this is going to be the bottleneck, so leaving as is for now.
            int i = 0;
            while (i < list.Count)
            {
                T item = list[i];
                if (match(item))
                    Remove(item);
                else
                    i++;
            }
        }

        //Sorts the internal list, this makes the exposed index accessor sorted as well.
        //But note that any insertion or deletion, can unorder the collection again.
        public void Sort(Comparison<T> sortLayoutFunction)
        {
            //There might be better ways to sort and keep the dictionary index up to date.
            list.Sort(sortLayoutFunction);
            //Rebuild the dictionary index.
            for (int i = 0; i < list.Count; ++i)
            {
                T item = list[i];
                dictionary[item] = i;
            }
        }
	}

	public class ClippingUpdateRegistry 
	{
		private static ClippingUpdateRegistry instance;

		private readonly IndexedSet<ClippingNode> clippingNodes = new IndexedSet<ClippingNode>();
		private readonly IndexedSet<ClippingElement> clippingElements = new IndexedSet<ClippingElement>();

	    protected ClippingUpdateRegistry()
	    {
	    	Canvas.willRenderCanvases += PerformUpdate;
	    }

	    public static ClippingUpdateRegistry Instance
	    {
	    	get
	    	{
	    		return instance ?? (instance = new ClippingUpdateRegistry());
	    	}
	    }

	    private void CleanInvalidItems()
	    {
	    	for (int i = clippingNodes.Count - 1; i >= 0; --i)
	    	{
	    		var item = clippingNodes[i];
	    		if (item == null || item.IsDestroyed())
	    			clippingNodes.RemoveAt(i);
	    	}

	    	for (int i = clippingElements.Count - 1; i >= 0; --i)
	    	{
	    		var item = clippingElements[i];
	    		if (item == null || item.IsDestroyed())
	    			clippingElements.RemoveAt(i);
	    	}
	    }

	    ///
	    /// 1. ClippingNodes    PreBuild - Detect parent dirty and re-calculate mask datas
	    /// 2. ClippingElement  PreBuild - Re-target node with clippingInteraction
	    /// 3. ClippingElement  Build    - Populate sharedMaterial
	    /// 4. ClippingNodes    Build    - Update sharedMaterial properties
	    private void PerformUpdate()
	    {
	    	CleanInvalidItems();

	    	clippingNodes.Sort(SortNodes);
	    	for (int i = 0, nodesCount = clippingNodes.Count; i < nodesCount; ++i)
	    	{
	    		var rebuild = clippingNodes[i];
	    		rebuild.PreBuild();
	    	}

	    	for (int i = 0, elementsCount = clippingElements.Count; i < elementsCount; ++i)
	    	{
	    		var rebuild = clippingElements[i];
	    	    rebuild.PreBuild();
	    	}

	    	for (int i = 0, elementsCount = clippingElements.Count; i < elementsCount; ++i)
	    	{
	    		var rebuild = clippingElements[i];
	    	    rebuild.Build();
	    	}

	    	for (int i = 0, nodesCount = clippingNodes.Count; i < nodesCount; ++i)
	    	{
	    		var rebuild = clippingNodes[i];
	    		rebuild.Build();
	    	}
	    }

	    private static int SortNodes(ClippingNode x, ClippingNode y)
	    {
	    	return ParentCount(x) - ParentCount(y);
	    }

	    private static int ParentCount(ClippingNode node)
	    {
	    	int count = 0;
	    	var parent = node.parent;
	    	while (parent != null)
	    	{
	    		++count;
	    		parent = parent.parent;
	    	}
	    	return count;
	    }

	    public static void RegisterClippingNode(ClippingNode clippingNode)
	    {
	    	Instance.InternalRegisterClippingNode(clippingNode);
	    }

	    private bool InternalRegisterClippingNode(ClippingNode clippingNode)
	    {
	    	return clippingNodes.AddUnique(clippingNode);
	    }

	    public static void UnRegisterClippingNode(ClippingNode clippingNode)
	    {
	    	Instance.InternalUnRegisterClippingNode(clippingNode);
	    }

	    private void InternalUnRegisterClippingNode(ClippingNode clippingNode)
	    {
	    	clippingNodes.Remove(clippingNode);
	    }

	    public static void RegisterClippingElement(ClippingElement clippingElement)
	    {
	    	Instance.InternalRegisterClippingElement(clippingElement);
	    }

	    private bool InternalRegisterClippingElement(ClippingElement clippingElement)
	    {
	    	return clippingElements.AddUnique(clippingElement);
	    }

	    public static void UnRegisterClippingElement(ClippingElement clippingElement)
	    {
	    	Instance.InternalUnRegisterClippingElement(clippingElement);
	    }

	    private void InternalUnRegisterClippingElement(ClippingElement clippingElement)
	    {
	    	clippingElements.Remove(clippingElement);
	    }
	}
}