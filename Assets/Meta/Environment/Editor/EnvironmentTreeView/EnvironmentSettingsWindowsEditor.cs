using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace BagelCode
{
    public class EnvironmentSettingsWindowsEditor : EditorWindow
    {
        private EnvironmentSettingsMultiTreeView treeView;
        private TreeModel<CopyAssetTreeViewItem> treeModel;
        [SerializeField] private TreeViewState treeViewState;
        [SerializeField] private MultiColumnHeaderState m_MultiColumnHeaderState;

        private static List<CheckAssetItem> checkAssetItemList = new List<CheckAssetItem>();
        private static EnvironmentSettings environmentSettings = null;
        private static List<EnvironmentAssets> whiteListCopyAssets = null;

        public enum RootState
        {
            ROOT = 0,
            MATCH,
            DEST_ONLY,
            SOURCE_ONLY,
            WHITE_LIST,
            ETC
        }

        private Rect multiColumnTreeViewRect
        {
            get { return new Rect(20, 30, position.width - 40, position.height - 60); }
        }

        private Rect bottomToolbarRect
        {
            get { return new Rect(20f, position.height - 18f, position.width - 40f, 16f); }
        }

        public static void ShowWindow(EnvironmentSettings _environmentSettings, List<CheckAssetItem> _checkAssetItemList, List<EnvironmentAssets> _whiteListCopyAssets)
        {
            environmentSettings = _environmentSettings;
            checkAssetItemList = _checkAssetItemList;
            whiteListCopyAssets = _whiteListCopyAssets;
            EnvironmentSettingsWindowsEditor window = EditorWindow.GetWindow<EnvironmentSettingsWindowsEditor>("Environment Copy Window");
            window.Focus();
            window.Repaint();
        }

        private void OnGUI()
        {
            GUILayout.Label("Title : " + environmentSettings?.uniqueName ?? "Unknown");
            CheckTreeView();
            BottomToolBar(bottomToolbarRect);
        }

        private void CheckTreeView()
        {
            if (treeViewState == null)
                treeViewState = new TreeViewState();

            if (treeView == null && checkAssetItemList != null)
            {
                var headerState = EnvironmentSettingsMultiTreeView.CreateDefaultMultiColumnHeaderState(multiColumnTreeViewRect.width);
                if (MultiColumnHeaderState.CanOverwriteSerializedFields(m_MultiColumnHeaderState, headerState))
                    MultiColumnHeaderState.OverwriteSerializedFields(m_MultiColumnHeaderState, headerState);
                m_MultiColumnHeaderState = headerState;

                var multiColumnHeader = new EnvironmentSettingsMultiColumnHeader(headerState);
                multiColumnHeader.ResizeToFit();

                treeModel = new TreeModel<CopyAssetTreeViewItem>(GetInitData());
                UpdateTreeView();
                treeView = new EnvironmentSettingsMultiTreeView(treeViewState, multiColumnHeader, treeModel);
            }
            else
                treeView.OnGUI(multiColumnTreeViewRect);
        }

        private void BottomToolBar(Rect rect)
        {
            GUILayout.BeginArea(rect);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Apply"))
                {
                    if (environmentSettings != null)
                    {
                        UpdateCheckAssetItemList(treeModel.ListData);
                        environmentSettings.ApplyCopyAssetWithEditorWindow(checkAssetItemList, whiteListCopyAssets);
                        OnCloseWindow();
                    }
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Item Add"))
                    CreateItem();

                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Item Delete"))
                    DeleteItem();

                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Close"))
                    OnCloseWindow();
            }

            GUILayout.EndArea();
        }

        private void UpdateCheckAssetItemList(IList<CopyAssetTreeViewItem> listData)
        {
            if (whiteListCopyAssets == null)
                whiteListCopyAssets = new List<EnvironmentAssets>();

            if (listData != null)
            {
                checkAssetItemList.Clear();
                whiteListCopyAssets.Clear();
                for (int i = 0; i < listData.Count; ++i)
                {
                    if (listData[i].enabled)
                        checkAssetItemList.Add(new CheckAssetItem(listData[i].enabled, listData[i].sourceElement, listData[i].destElement));
                    else if (listData[i].checkWhiteList)
                        whiteListCopyAssets.Add(new EnvironmentAssets() { destObj = listData[i].destElement.obj, sourceObj = listData[i].sourceElement.obj });
                }
            }
        }

        private void OnCloseWindow()
        {
            EnvironmentSettingsWindowsEditor window = EditorWindow.GetWindow<EnvironmentSettingsWindowsEditor>("Environment Copy Window");
            window.Close();
        }

        private IList<CopyAssetTreeViewItem> GetInitData()
        {
            List<CopyAssetTreeViewItem> itemList = new List<CopyAssetTreeViewItem>();

            itemList.Add(new CopyAssetTreeViewItem("Root", -1, (int)RootState.ROOT, new CheckAssetItem(false, null, null)));

            itemList.Add(new CopyAssetTreeViewItem("Match", 0, (int)RootState.MATCH, new CheckAssetItem(false, null, null)));
            itemList.Add(new CopyAssetTreeViewItem("Dest Only", 0, (int)RootState.DEST_ONLY, new CheckAssetItem(false, null, null)));
            itemList.Add(new CopyAssetTreeViewItem("Source Only", 0, (int)RootState.SOURCE_ONLY, new CheckAssetItem(false, null, null)));
            itemList.Add(new CopyAssetTreeViewItem("White List", 0, (int)RootState.WHITE_LIST, new CheckAssetItem(false, null, null)));
            itemList.Add(new CopyAssetTreeViewItem("Etc", 0, (int)RootState.ETC, new CheckAssetItem(false, null, null)));

            return itemList;
        }

        private void UpdateTreeView()
        {
            if (treeModel == null) return;

            CopyAssetTreeViewItem matchRoot = treeModel.Find((int)RootState.MATCH);
            CopyAssetTreeViewItem destRoot = treeModel.Find((int)RootState.DEST_ONLY);
            CopyAssetTreeViewItem sourceRoot = treeModel.Find((int)RootState.SOURCE_ONLY);
            CopyAssetTreeViewItem whiteRoot = treeModel.Find((int)RootState.WHITE_LIST);
            CopyAssetTreeViewItem etcRoot = treeModel.Find((int)RootState.ETC);

            List<CopyAssetTreeViewItem> matchList = new List<CopyAssetTreeViewItem>();
            List<CopyAssetTreeViewItem> destList = new List<CopyAssetTreeViewItem>();
            List<CopyAssetTreeViewItem> sourceList = new List<CopyAssetTreeViewItem>();
            List<CopyAssetTreeViewItem> whiteList = new List<CopyAssetTreeViewItem>();
            List<CopyAssetTreeViewItem> etcList = new List<CopyAssetTreeViewItem>();

            if (checkAssetItemList != null)
            {
                for (int i = 0; i < checkAssetItemList.Count; ++i)
                {
                    int idCount = treeModel.GenerateUniqueID();
                    string fileName = checkAssetItemList[i].destElement.obj != null ? checkAssetItemList[i].destElement.GetFileName() : checkAssetItemList[i].sourceElement.GetFileName();
                    CopyAssetTreeViewItem item = new CopyAssetTreeViewItem(fileName, 1, idCount, checkAssetItemList[i]);

                    if (checkAssetItemList[i].destElement.obj != null && checkAssetItemList[i].sourceElement.obj != null)
                        matchList.Add(item);
                    else
                    {
                        var checkWhiteListItem = whiteListCopyAssets.Find(x =>
                            x.destObj == checkAssetItemList[i].destElement.obj &&
                            x.sourceObj == checkAssetItemList[i].sourceElement.obj);
                        if (checkWhiteListItem != null)
                        {
                            item.checkWhiteList = true;
                            whiteList.Add(item);
                        }
                        else if (checkAssetItemList[i].destElement.obj != null)
                            destList.Add(item);
                        else if (checkAssetItemList[i].sourceElement.obj != null)
                            sourceList.Add(item);
                        else
                            etcList.Add(item);
                    }
                }
            }
            if (matchList.Count > 0)
                treeModel.AddElements(matchList, matchRoot, 0);
            if (destList.Count > 0)
                treeModel.AddElements(destList, destRoot, 0);
            if (sourceList.Count > 0)
                treeModel.AddElements(sourceList, sourceRoot, 0);
            if (whiteList.Count > 0)
                treeModel.AddElements(whiteList, whiteRoot, 0);
            if (etcList.Count > 0)
                treeModel.AddElements(etcList, etcRoot, 0);
        }

        private void CreateItem()
        {
            CopyAssetTreeViewItem etcRoot = treeModel.Find((int)RootState.ETC);
            int newId = treeModel.GenerateUniqueID();
            treeModel.AddElement(new CopyAssetTreeViewItem("New Item " + newId, 1, newId, new CheckAssetItem(false, null, null)), etcRoot, etcRoot.children?.Count ?? 0);
        }

        private void DeleteItem()
        {
            IList<int> selectList = treeView.GetSelection();
            if (selectList != null && selectList.Count > 0)
            {
                List<int> newSelectionList = new List<int>(selectList);
                for (int i = newSelectionList.Count - 1; i >= 0; --i)
                {
                    if (newSelectionList[i] <= 4)
                        newSelectionList.RemoveAt(i);
                }

                if (newSelectionList.Count > 0)
                    treeModel.RemoveElements(newSelectionList);
            }
        }
    }

    internal class EnvironmentSettingsMultiColumnHeader : MultiColumnHeader
    {
        public enum Mode
        {
            LargeHeader,
            DefaultHeader,
            MinimumHeaderWithoutSorting
        }

        Mode m_Mode;

        public EnvironmentSettingsMultiColumnHeader(MultiColumnHeaderState state)
            : base(state)
        {
            mode = Mode.DefaultHeader;
        }

        public Mode mode
        {
            get
            {
                return m_Mode;
            }
            set
            {
                m_Mode = value;
                switch (m_Mode)
                {
                    case Mode.LargeHeader:
                        canSort = true;
                        height = 37f;
                        break;
                    case Mode.DefaultHeader:
                        canSort = true;
                        height = DefaultGUI.defaultHeight;
                        break;
                    case Mode.MinimumHeaderWithoutSorting:
                        canSort = false;
                        height = DefaultGUI.minimumHeight;
                        break;
                }
            }
        }

        protected override void ColumnHeaderGUI(MultiColumnHeaderState.Column column, Rect headerRect, int columnIndex)
        {
            // Default column header gui
            base.ColumnHeaderGUI(column, headerRect, columnIndex);

            // Add additional info for large header
            if (mode == Mode.LargeHeader)
            {
                // Show example overlay stuff on some of the columns
                if (columnIndex > 2)
                {
                    headerRect.xMax -= 3f;
                    var oldAlignment = EditorStyles.largeLabel.alignment;
                    EditorStyles.largeLabel.alignment = TextAnchor.UpperRight;
                    GUI.Label(headerRect, 36 + columnIndex + "%", EditorStyles.largeLabel);
                    EditorStyles.largeLabel.alignment = oldAlignment;
                }
            }
        }
    }
}