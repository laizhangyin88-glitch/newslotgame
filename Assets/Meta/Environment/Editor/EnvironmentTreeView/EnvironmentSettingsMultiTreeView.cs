using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;

namespace BagelCode
{
	internal class EnvironmentSettingsMultiTreeView : EnvironmentSettingsTreeModel<CopyAssetTreeViewItem>
    {
		const float kRowHeights = 20f;
        const float kToggleWidth = 18f;

        // All columns
        public enum TreeColumns
        {
            Toggle,
            FileName,
			SourceObj,
			DestObj,
        }

		public EnvironmentSettingsMultiTreeView(TreeViewState state, MultiColumnHeader multicolumnHeader, TreeModel<CopyAssetTreeViewItem> model) : base(state, multicolumnHeader, model)
        {
			// Custom setup
			rowHeight = kRowHeights;
			columnIndexForTreeFoldouts = 1;
			showAlternatingRowBackgrounds = true;
			showBorder = true;
			customFoldoutYOffset = (kRowHeights - EditorGUIUtility.singleLineHeight) * 0.5f; // center foldout in the row since we also center content. See RowGUI
			extraSpaceBeforeIconAndLabel = kToggleWidth;

			Reload();
		}


		// Note we We only build the visible rows, only the backend has the full tree information. 
		// The treeview only creates info for the row list.
		protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
		{
			var rows = base.BuildRows(root);
			return rows;
		}

		protected override void RowGUI(RowGUIArgs args)
		{
			var item = (EnvironmentSettingsTreeViewItem<CopyAssetTreeViewItem>)args.item;

			for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
			{
				CellGUI(args.GetCellRect(i), item, (TreeColumns)args.GetColumn(i), ref args);
			}
		}

		void CellGUI(Rect cellRect, EnvironmentSettingsTreeViewItem<CopyAssetTreeViewItem> item, TreeColumns column, ref RowGUIArgs args)
		{
			// Center cell rect vertically (makes it easier to place controls, icons etc in the cells)
			CenterRectUsingSingleLineHeight(ref cellRect);

			switch (column)
			{
				case TreeColumns.Toggle:
					{
						Rect toggleRect = cellRect;
						toggleRect.x += GetContentIndent(item);
						toggleRect.width = kToggleWidth;
						if (toggleRect.xMax < cellRect.xMax && item.data.depth > 0)
						{
							bool toggleValue = EditorGUI.Toggle(toggleRect, item.data.enabled); // hide when outside cell rect
							if (toggleValue == true)
								toggleValue = item.data.destElement.obj != null && item.data.sourceElement.obj != null;
							item.data.enabled = toggleValue;
                        }
					}
					break;

				case TreeColumns.FileName:
					{
						float toggleSpace = 18.0f;
						if (item.data.depth > 0 && (item.data.destElement.obj == null || item.data.sourceElement.obj == null))
						{
							Rect toggleRect = cellRect;
							toggleRect.x += GetContentIndent(item);
							toggleRect.width = kToggleWidth;
							if (toggleRect.xMax < cellRect.xMax && item.data.depth > 0)
							{
								CheckWhiteListItemRoot(item);
								bool toggleValue = EditorGUI.Toggle(toggleRect, item.data.checkWhiteList);
                                item.data.checkWhiteList = toggleValue;
								toggleSpace = 0.0f;
							}
						}
						cellRect.xMin -= toggleSpace;	// toggle space delete
						args.rowRect = cellRect;
						base.RowGUI(args);
					}
					break;
				case TreeColumns.SourceObj:
				case TreeColumns.DestObj:
					{
						if (item.data.depth > 0)
						{
							cellRect.xMin += 5f;    // spacing

							if (column == TreeColumns.SourceObj)
								item.data.sourceElement.obj = item.data.sourceElement != null ? EditorGUI.ObjectField(cellRect, GUIContent.none, item.data.sourceElement.obj, typeof(UnityEngine.Object), false) : null;
							if (column == TreeColumns.DestObj)
								item.data.destElement.obj = item.data.destElement != null ? EditorGUI.ObjectField(cellRect, GUIContent.none, item.data.destElement.obj, typeof(UnityEngine.Object), false) : null;
						}
					}
					break;
			}
		}

		private void CheckWhiteListItemRoot(EnvironmentSettingsTreeViewItem<CopyAssetTreeViewItem> item)
        {
			switch (item.parent.id)
            {
				case (int)EnvironmentSettingsWindowsEditor.RootState.DEST_ONLY:
				case (int)EnvironmentSettingsWindowsEditor.RootState.SOURCE_ONLY:
                    {
						if (item.data.checkWhiteList)
                        {
							CopyAssetTreeViewItem whiteListRoot = treeModel.Find((int)EnvironmentSettingsWindowsEditor.RootState.WHITE_LIST);
							treeModel.MoveElements(whiteListRoot, whiteListRoot.children == null ? 0 : whiteListRoot.children.Count, new List<EnvironmentSettingsTreeElement>() { item.data });
                        }
                    }
					break;
				case (int)EnvironmentSettingsWindowsEditor.RootState.WHITE_LIST:
                    {
						if (!item.data.checkWhiteList)
                        {
							int parentIndex = (int)EnvironmentSettingsWindowsEditor.RootState.DEST_ONLY;
							if (item.data.destElement == null || item.data.destElement.obj == null)
								parentIndex = (int)EnvironmentSettingsWindowsEditor.RootState.SOURCE_ONLY;

							CopyAssetTreeViewItem parentRoot = treeModel.Find(parentIndex);
							treeModel.MoveElements(parentRoot, parentRoot.children == null ? 0 : parentRoot.children.Count, new List<EnvironmentSettingsTreeElement>() { item.data });
                        }
					}
					break;
			}
		}

		public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState(float treeViewWidth)
		{
			var columns = new[]
			{
				new MultiColumnHeaderState.Column
				{
					headerContent = new GUIContent("Check"),
					contextMenuText = "Asset",
					headerTextAlignment = TextAlignment.Center,
					sortedAscending = false,
					sortingArrowAlignment = TextAlignment.Right,
					width = 60,
					minWidth = 60,
					maxWidth = 90,
					autoResize = false,
					allowToggleVisibility = false
				},
				new MultiColumnHeaderState.Column
				{
					headerContent = new GUIContent("Name(White List Check)"),
					headerTextAlignment = TextAlignment.Left,
					sortedAscending = true,
					sortingArrowAlignment = TextAlignment.Center,
					width = 150,
					minWidth = 60,
					autoResize = false,
					allowToggleVisibility = true
				},
				new MultiColumnHeaderState.Column 
				{
					headerContent = new GUIContent("Source Obj"),
					headerTextAlignment = TextAlignment.Left,
					sortedAscending = true,
					sortingArrowAlignment = TextAlignment.Center,
					width = 95,
					minWidth = 60,
					autoResize = true,
					allowToggleVisibility = false
				},
				new MultiColumnHeaderState.Column
				{
					headerContent = new GUIContent("Dest Obj"),
					headerTextAlignment = TextAlignment.Left,
					sortedAscending = true,
					sortingArrowAlignment = TextAlignment.Center,
					width = 95,
					minWidth = 60,
					autoResize = true,
					allowToggleVisibility = false
				}
			};

			Assert.AreEqual(columns.Length, Enum.GetValues(typeof(TreeColumns)).Length, "Number of columns should match number of enum values: You probably forgot to update one of them.");

			var state = new MultiColumnHeaderState(columns);
			return state;
		}
	}

    [System.Serializable]
	internal class CopyAssetTreeViewItem : EnvironmentSettingsTreeElement
    {
        public bool enabled;
		public bool checkWhiteList = false;
		public CopyAssetElement sourceElement;
		public CopyAssetElement destElement;

		public CopyAssetTreeViewItem(string name, int depth, int id, CheckAssetItem checkAssetItem) : base(name, depth, id)
        {
            enabled = checkAssetItem?.enabled ?? false;
			sourceElement = checkAssetItem?.sourceElement;
			destElement = checkAssetItem?.destElement;
        }
    }
}