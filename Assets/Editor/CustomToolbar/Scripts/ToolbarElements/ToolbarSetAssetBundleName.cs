using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityToolbarExtender;
using System.IO;
using UnityEditor.UIElements;

[Serializable]
internal class ToolbarSetAssetBundleName : BaseToolbarElement
{
    private static GUIContent titleGUiContent;

    public override string NameInList => "[Input] Set AssetBundleName";

    public override void Init()
    {
        titleGUiContent = new GUIContent("abName");
        titleGUiContent.tooltip = "设置当前选择资源的ab名，有些没解决的问题：1.鼠标放在Toolbar上才会更新显示 2.有些资源是不可以设置ab名的，代码中未做严谨的类型判断";
    }

    protected override void OnDrawInList(Rect position)
    {

    }

    protected override void OnDrawInToolbar()
    {
        UnityEngine.Object selectObj = UnityEditor.Selection.activeObject;
        string assetPath = AssetDatabase.GetAssetPath(selectObj);
        string extension = Path.GetExtension(assetPath);
        if (selectObj == null || extension == ".cs")
            return;
        AssetImporter ai = AssetImporter.GetAtPath(assetPath);

        
        try
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(titleGUiContent, GUILayout.Width(60f));
            ai.assetBundleName = GUILayout.TextField(ai.assetBundleName, GUILayout.Width(180f));
            GUILayout.EndHorizontal();
        }
        catch (Exception)
        {
        }
    }
}
