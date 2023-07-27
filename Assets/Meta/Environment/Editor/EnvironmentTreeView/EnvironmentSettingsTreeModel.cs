using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace BagelCode
{
	internal class EnvironmentSettingsTreeViewItem<T> : TreeViewItem where T : EnvironmentSettingsTreeElement
	{
		public T data { get; set; }

		public EnvironmentSettingsTreeViewItem(int id, int depth, string displayName, T data) : base(id, depth, displayName)
		{
			this.data = data;
		}
	}

	internal class EnvironmentSettingsTreeModel<T> : TreeView where T : EnvironmentSettingsTreeElement
	{
		TreeModel<T> m_TreeModel;
		readonly List<TreeViewItem> m_Rows = new List<TreeViewItem>(100);
		public event Action treeChanged;

		public TreeModel<T> treeModel { get { return m_TreeModel; } }
		public event Action<IList<TreeViewItem>> beforeDroppingDraggedItems;


		public EnvironmentSettingsTreeModel(TreeViewState state, TreeModel<T> model) : base(state)
		{
			Init(model);
		}

		public EnvironmentSettingsTreeModel(TreeViewState state, MultiColumnHeader multiColumnHeader, TreeModel<T> model)
			: base(state, multiColumnHeader)
		{
			Init(model);
		}

		void Init(TreeModel<T> model)
		{
			m_TreeModel = model;
			m_TreeModel.modelChanged += ModelChanged;
		}

		void ModelChanged()
		{
			if (treeChanged != null)
				treeChanged();

			Reload();
		}

		protected override TreeViewItem BuildRoot()
		{
			int depthForHiddenRoot = -1;
			return new EnvironmentSettingsTreeViewItem<T>(m_TreeModel.root.id, depthForHiddenRoot, m_TreeModel.root.name, m_TreeModel.root);
		}

		protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
		{
			if (m_TreeModel.root == null)
			{
				Debug.LogError("tree model root is null. did you call SetData()?");
			}

			m_Rows.Clear();

			if (m_TreeModel.root.hasChildren)
				AddChildrenRecursive(m_TreeModel.root, 0, m_Rows);

			// We still need to setup the child parent information for the rows since this 
			// information is used by the TreeView internal logic (navigation, dragging etc)
			SetupParentsAndChildrenFromDepths(root, m_Rows);

			return m_Rows;
		}

		void AddChildrenRecursive(T parent, int depth, IList<TreeViewItem> newRows)
		{
			foreach (T child in parent.children)
			{
				var item = new EnvironmentSettingsTreeViewItem<T>(child.id, depth, child.name, child);
				newRows.Add(item);

				if (child.hasChildren)
				{
					if (IsExpanded(child.id))
					{
						AddChildrenRecursive(child, depth + 1, newRows);
					}
					else
					{
						item.children = CreateChildListForCollapsedParent();
					}
				}
			}
		}

		protected override IList<int> GetAncestors(int id)
		{
			return m_TreeModel.GetAncestors(id);
		}

		protected override IList<int> GetDescendantsThatHaveChildren(int id)
		{
			return m_TreeModel.GetDescendantsThatHaveChildren(id);
		}


		// Dragging
		//-----------

		const string k_GenericDragID = "GenericDragColumnDragging";

		protected override bool CanStartDrag(CanStartDragArgs args)
		{
			return true;
		}

		protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
		{
			if (hasSearch)
				return;

			DragAndDrop.PrepareStartDrag();
			var draggedRows = GetRows().Where(item => args.draggedItemIDs.Contains(item.id)).ToList();
			DragAndDrop.SetGenericData(k_GenericDragID, draggedRows);
			DragAndDrop.objectReferences = new UnityEngine.Object[] { }; // this IS required for dragging to work
			string title = draggedRows.Count == 1 ? draggedRows[0].displayName : "< Multiple >";
			DragAndDrop.StartDrag(title);
		}

		protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
		{
			// Check if we can handle the current drag data (could be dragged in from other areas/windows in the editor)
			var draggedRows = DragAndDrop.GetGenericData(k_GenericDragID) as List<TreeViewItem>;
			if (draggedRows == null)
				return DragAndDropVisualMode.None;

			// Parent item is null when dragging outside any tree view items.
			switch (args.dragAndDropPosition)
			{
				case DragAndDropPosition.UponItem:
				case DragAndDropPosition.BetweenItems:
					{
						bool validDrag = ValidDrag(args.parentItem, draggedRows);
						if (args.performDrop && validDrag)
						{
							T parentData = ((EnvironmentSettingsTreeViewItem<T>)args.parentItem).data;
							OnDropDraggedElementsAtIndex(draggedRows, parentData, args.insertAtIndex == -1 ? 0 : args.insertAtIndex);
						}
						return validDrag ? DragAndDropVisualMode.Move : DragAndDropVisualMode.None;
					}

				case DragAndDropPosition.OutsideItems:
					{
						if (args.performDrop)
							OnDropDraggedElementsAtIndex(draggedRows, m_TreeModel.root, m_TreeModel.root.children.Count);

						return DragAndDropVisualMode.Move;
					}
				default:
					Debug.LogError("Unhandled enum " + args.dragAndDropPosition);
					return DragAndDropVisualMode.None;
			}
		}

		public virtual void OnDropDraggedElementsAtIndex(List<TreeViewItem> draggedRows, T parent, int insertIndex)
		{
			if (beforeDroppingDraggedItems != null)
				beforeDroppingDraggedItems(draggedRows);

			var draggedElements = new List<EnvironmentSettingsTreeElement>();
			foreach (var x in draggedRows)
				draggedElements.Add(((EnvironmentSettingsTreeViewItem<T>)x).data);

			var selectedIDs = draggedElements.Select(x => x.id).ToArray();
			m_TreeModel.MoveElements(parent, insertIndex, draggedElements);
			SetSelection(selectedIDs, TreeViewSelectionOptions.RevealAndFrame);
		}


		bool ValidDrag(TreeViewItem parent, List<TreeViewItem> draggedItems)
		{
			TreeViewItem currentParent = parent;
			while (currentParent != null)
			{
				if (draggedItems.Contains(currentParent))
					return false;
				currentParent = currentParent.parent;
			}
			return true;
		}
	}

	public class TreeModel<T> where T : EnvironmentSettingsTreeElement
	{
		IList<T> m_Data;
		T m_Root;
		int m_MaxID;

		public T root { get { return m_Root; } set { m_Root = value; } }
		public event Action modelChanged;
		public int numberOfDataElements
		{
			get { return m_Data.Count; }
		}

		public IList<T> ListData
        {
			get { return m_Data; }
        }

		public TreeModel(IList<T> data)
		{
			SetData(data);
		}

		public T Find(int id)
		{
			return m_Data.FirstOrDefault(element => element.id == id);
		}

		public void SetData(IList<T> data)
		{
			Init(data);
		}

		void Init(IList<T> data)
		{
			if (data == null)
				throw new ArgumentNullException("data", "Input data is null. Ensure input is a non-null list.");

			m_Data = data;
			if (m_Data.Count > 0)
				m_Root = EnvironmentSettingsTreeElementUtils.ListToTree(data);

			m_MaxID = m_Data.Max(e => e.id);
		}

		public int GenerateUniqueID()
		{
			return ++m_MaxID;
		}

		public IList<int> GetAncestors(int id)
		{
			var parents = new List<int>();
			EnvironmentSettingsTreeElement T = Find(id);
			if (T != null)
			{
				while (T.parent != null)
				{
					parents.Add(T.parent.id);
					T = T.parent;
				}
			}
			return parents;
		}

		public IList<int> GetDescendantsThatHaveChildren(int id)
		{
			T searchFromThis = Find(id);
			if (searchFromThis != null)
			{
				return GetParentsBelowStackBased(searchFromThis);
			}
			return new List<int>();
		}

		IList<int> GetParentsBelowStackBased(EnvironmentSettingsTreeElement searchFromThis)
		{
			Stack<EnvironmentSettingsTreeElement> stack = new Stack<EnvironmentSettingsTreeElement>();
			stack.Push(searchFromThis);

			var parentsBelow = new List<int>();
			while (stack.Count > 0)
			{
				EnvironmentSettingsTreeElement current = stack.Pop();
				if (current.hasChildren)
				{
					parentsBelow.Add(current.id);
					foreach (var T in current.children)
					{
						stack.Push(T);
					}
				}
			}

			return parentsBelow;
		}

		public void RemoveElements(IList<int> elementIDs)
		{
			IList<T> elements = m_Data.Where(element => elementIDs.Contains(element.id)).ToArray();
			RemoveElements(elements);
		}

		public void RemoveElements(IList<T> elements)
		{
			foreach (var element in elements)
				if (element == m_Root)
					throw new ArgumentException("It is not allowed to remove the root element");

			var commonAncestors = EnvironmentSettingsTreeElementUtils.FindCommonAncestorsWithinList(elements);

			foreach (var element in commonAncestors)
			{
				element.parent.children.Remove(element);
				element.parent = null;
			}

			EnvironmentSettingsTreeElementUtils.TreeToList(m_Root, m_Data);

			Changed();
		}

		public void AddElements(IList<T> elements, EnvironmentSettingsTreeElement parent, int insertPosition)
		{
			if (elements == null)
				throw new ArgumentNullException("elements", "elements is null");
			if (elements.Count == 0)
				throw new ArgumentNullException("elements", "elements Count is 0: nothing to add");
			if (parent == null)
				throw new ArgumentNullException("parent", "parent is null");

			if (parent.children == null)
				parent.children = new List<EnvironmentSettingsTreeElement>();

			parent.children.InsertRange(insertPosition, elements.Cast<EnvironmentSettingsTreeElement>());
			foreach (var element in elements)
			{
				element.parent = parent;
				element.depth = parent.depth + 1;
				EnvironmentSettingsTreeElementUtils.UpdateDepthValues(element);
			}

			EnvironmentSettingsTreeElementUtils.TreeToList(m_Root, m_Data);

			Changed();
		}

		public void AddRoot(T root)
		{
			if (root == null)
				throw new ArgumentNullException("root", "root is null");

			if (m_Data == null)
				throw new InvalidOperationException("Internal Error: data list is null");

			if (m_Data.Count != 0)
				throw new InvalidOperationException("AddRoot is only allowed on empty data list");

			root.id = GenerateUniqueID();
			root.depth = -1;
			m_Data.Add(root);
		}

		public void AddElement(T element, EnvironmentSettingsTreeElement parent, int insertPosition)
		{
			if (element == null)
				throw new ArgumentNullException("element", "element is null");
			if (parent == null)
				throw new ArgumentNullException("parent", "parent is null");

			if (parent.children == null)
				parent.children = new List<EnvironmentSettingsTreeElement>();

			parent.children.Insert(insertPosition, element);
			element.parent = parent;

			EnvironmentSettingsTreeElementUtils.UpdateDepthValues(parent);
			EnvironmentSettingsTreeElementUtils.TreeToList(m_Root, m_Data);

			Changed();
		}

		public void MoveElements(EnvironmentSettingsTreeElement parentElement, int insertionIndex, List<EnvironmentSettingsTreeElement> elements)
		{
			if (insertionIndex < 0)
				throw new ArgumentException("Invalid input: insertionIndex is -1, client needs to decide what index elements should be reparented at");

			// Invalid reparenting input
			if (parentElement == null)
				return;

			// We are moving items so we adjust the insertion index to accomodate that any items above the insertion index is removed before inserting
			if (insertionIndex > 0)
				insertionIndex -= parentElement.children.GetRange(0, insertionIndex).Count(elements.Contains);

			// Remove draggedItems from their parents
			foreach (var draggedItem in elements)
			{
				draggedItem.parent.children.Remove(draggedItem);    // remove from old parent
				draggedItem.parent = parentElement;                 // set new parent
			}

			if (parentElement.children == null)
				parentElement.children = new List<EnvironmentSettingsTreeElement>();

			// Insert dragged items under new parent
			parentElement.children.InsertRange(insertionIndex, elements);

			EnvironmentSettingsTreeElementUtils.UpdateDepthValues(root);
			EnvironmentSettingsTreeElementUtils.TreeToList(m_Root, m_Data);

			Changed();
		}

		void Changed()
		{
			if (modelChanged != null)
				modelChanged();
		}
	}
}