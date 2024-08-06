using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityToolbarExtender;

[Serializable]
internal class ToolbarButtonShowInspector : BaseToolbarElement
{
    [SerializeField] public string targetPath;
    private static GUIContent btn;

    public override string NameInList => "[Button] ShowInspector";

    public override void Init()
    {
        //(Texture2D)AssetDatabase.LoadAssetAtPath($"{GetPackageRootPath}/Editor/CustomToolbar/Icons/LookDevResetEnv@2x.png"
        
        btn = new GUIContent(EditorGUIUtility.FindTexture("Settings"), "打开Application Settings");
    }

    protected override void OnDrawInList(Rect position)
    {
        position.width = 200.0f;
        targetPath = EditorGUI.TextField(position, targetPath);
    }

    protected override void OnDrawInToolbar()
    {
        EditorGUIUtility.SetIconSize(new Vector2(17, 17));
        if (GUILayout.Button(btn, ToolbarStyles.commandButtonStyle))
        {
            //if (EditorApplication.isPlaying)
            //{
            //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            //}
            //加载想要选中的文件/文件夹
            UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(targetPath);
            //在Project面板标记高亮显示
            UnityEditor.EditorGUIUtility.PingObject(obj);
            //在Project面板自动选中，并在Inspector面板显示详情
            UnityEditor.Selection.activeObject = obj;
        }
    }
}
