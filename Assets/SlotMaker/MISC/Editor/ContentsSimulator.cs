using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SlotMaker
{
	public class ContentsSimulator : EditorWindowBase<ContentsSimulator>
	{
		private string gameTitle;

	    public override string GetEditorName()
        {
            return "Contents Simulator";
        }

        [MenuItem("SlotMaker/Contents Simulator", false, 11)]
        static void Initialize()
        {
            CreateWindow();
        }

        void OnGUI()
        {
        	gameTitle = EditorPrefs.GetString("gameTitle", "");

            Space();
        	LabelField(string.Format("{0} - {1}", gameTitle, EditorPrefs.GetString("gameTitleName")));

            Space();
        	var paytable = GameObject.Find("Paytable");
        	bool isPaytable = paytable != null;
        	BeginCheck();
        	isPaytable = EditorGUILayout.ToggleLeft("Paytable", isPaytable);
        	if (EndCheck())
        	{
        		if (isPaytable)
                    LoadPopup("Paytable");
                else
                    DestroyGameObject(paytable);
        	}
        }

        GameObject LoadGameObject(string assetName, Transform parent)
        {
            var prefab = AssetBundleManager.LoadAsset<GameObject>(gameTitle, assetName);
            GameObject go = (Application.isPlaying ? GameObject.Instantiate(prefab) : PrefabUtility.InstantiatePrefab(prefab)) as GameObject;
            go.name = assetName;
            go.transform.SetParent(parent, false);
            Selection.activeGameObject = go;
            return go;
        }

        GameObject LoadPopup(string assetName)
        {
        	var go = LoadGameObject(assetName, PopupManager.Instance.contents);
        	if (Application.isPlaying) PopupManager.Instance.Open(go);
        	return go;
        }

        void DestroyGameObject(GameObject go)
        {
        	if (Application.isPlaying)
        		GameObject.Destroy(go);
    		else
    			GameObject.DestroyImmediate(go);
        }
	}
}