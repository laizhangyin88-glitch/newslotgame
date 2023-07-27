using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace SlotMaker
{
	[InitializeOnLoad]
	static class ManagedPrefabHelper
	{
		static ManagedPrefabHelper()
		{
			EditorApplication.hierarchyWindowItemOnGUI += DragAndDropScene;
		}

		private static void DragAndDropScene(int instanceID, Rect selectionRect)
		{
			if (Event.current.type == EventType.DragPerform)
			{
				if (!selectionRect.Contains(Event.current.mousePosition))
					return;

				GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
				if (go == null)
					return;

				DragAndDrop.AcceptDrag();

				SceneInfo sceneInfo = null;
				for (int i = 0; i < DragAndDrop.objectReferences.Length; ++i)
	            {
	                var sceneInfoObj = DragAndDrop.objectReferences[i] as SceneInfoObject;
	                if (sceneInfoObj != null)
	                {
	                    sceneInfo = sceneInfoObj.GetSceneInfo();
	                    EditorPrefs.SetString("SlotMaker.lastDirectory", Path.GetDirectoryName(DragAndDrop.paths[i]));
	                    break;
	                }
	            }
				if (sceneInfo == null) return;

				SceneManager.LoadSceneInEditor(go.transform, sceneInfo);

				Event.current.Use();
			}
		}

		[MenuItem("GameObject/♞ Apply", false, -100)]
		private static void Apply()
		{
			if (!IsManagedPrefab())
			{
				EditorUtility.DisplayDialog("♞ Apply", "You have to select ManagedPrefab!", "Okay");
				return;
			}

			var go = Selection.activeGameObject;
			var root = go.transform.parent;
			List<GameObject> prefabs = new List<GameObject>();

			SceneInfo sceneInfo = BuildSceneInfo(root, go, "", null, prefabs, false);

			var cacheObj = CreateCacheObject(prefabs);

			Object.DestroyImmediate(go.GetComponent<ManagedPrefab>());
			var prefab = PrefabUtility.GetCorrespondingObjectFromSource(go);
			var prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefab);
			PrefabUtility.SaveAsPrefabAssetAndConnect(go, prefabPath, InteractionMode.AutomatedAction);
			go.AddComponent<ManagedPrefab>();

			RestoreHierarchy(root, sceneInfo, cacheObj);

			Object.DestroyImmediate(cacheObj);
		}

		[MenuItem("GameObject/♞ Apply All", false, -99)]
		private static void ApplyAll()
		{
			if (!IsManagedPrefab())
			{
				EditorUtility.DisplayDialog("♞ Apply All", "You have to select ManagedPrefab!", "Okay");
				return;
			}

			var go = Selection.activeGameObject;
			var root = go.transform.parent;
			List<GameObject> prefabs = new List<GameObject>();

			SceneInfo sceneInfo = BuildSceneInfo(root, go, "", null, prefabs, false);

			var cacheObj = CreateCacheObject(prefabs);

			for (int i = 0; i < cacheObj.transform.childCount; ++i)
			{
				go = cacheObj.transform.GetChild(i).gameObject;
				Object.DestroyImmediate(go.GetComponent<ManagedPrefab>());
                var prefab = PrefabUtility.GetCorrespondingObjectFromSource(go);
                var prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefab);
                PrefabUtility.SaveAsPrefabAssetAndConnect(go, prefabPath, InteractionMode.AutomatedAction);
				go.AddComponent<ManagedPrefab>();
			}

			RestoreHierarchy(root, sceneInfo, cacheObj);

			Object.DestroyImmediate(cacheObj);
		}

	    [MenuItem("GameObject/♞ Remove All", false, -98)]
	    private static void RemoveAll()
	    {
	        var go = Selection.activeGameObject;
	        ManagedPrefab[] children = go.GetComponentsInChildren<ManagedPrefab>(true);
	        for (int i = 0; i < children.Length; ++i)
	        {
	            if (children[i] == null)
	            {
	                // Already destroyed
	            }
	            else if (children[i].gameObject == go)
	            {
	                // Exclude selected object
	            }
	            else
	            {
	                GameObject.DestroyImmediate(children[i].gameObject);
	            }
	        }
	    }

		[MenuItem("GameObject/♞ Save to Scene", false, -97)]
		private static void SaveToScene()
		{
			Mark();

			if (!IsManagedPrefab())
			{
				EditorUtility.DisplayDialog("♞ Save to Scene", "You have to select ManagedPrefab!", "Okay");
				return;
			}

			string dir = EditorPrefs.GetString("SlotMaker.lastDirectory", "Assets");
			string path = EditorUtility.SaveFilePanel("Save scene", dir, Selection.activeGameObject.name + ".asset", "asset");
			if (!string.IsNullOrEmpty(path))
			{
				var go = Selection.activeGameObject;
				bool replace = true;
	            string assetPath = path.Remove(0, path.IndexOf("Assets"));
	            SceneInfoObject asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(SceneInfoObject)) as SceneInfoObject;
	            if (asset == null)
	            {
	                asset = ScriptableObject.CreateInstance<SceneInfoObject>();
	                replace = false;
	            }
	            asset.SetSceneInfo(BuildSceneInfo(null, go, "", null, null, true));
	            if (replace)
	                EditorUtility.SetDirty(asset);
	            else
	                AssetDatabase.CreateAsset(asset, path.Remove(0, path.IndexOf("Assets")));

	            AssetDatabase.SaveAssets();
	            AssetDatabase.Refresh();

	            EditorPrefs.SetString("SlotMaker.lastDirectory", Path.GetDirectoryName(path));
				EditorUtility.FocusProjectWindow();

	        	Selection.activeObject = asset;
			}
		}

		private static void Mark()
		{
			AddPrefabInstanceToManagedPrefab(Selection.activeGameObject);
		}

		private static void AddPrefabInstanceToManagedPrefab(GameObject go)
		{
			if (go == null ||
				PrefabUtility.GetPrefabAssetType(go) != PrefabAssetType.Regular)
			{
				return;
			}

			if (go == PrefabUtility.GetOutermostPrefabInstanceRoot(go) &&
				go.GetComponent<ManagedPrefab>() == null)
			{
				go.AddComponent<ManagedPrefab>();
			}

			for (int i = 0; i < go.transform.childCount; ++i)
			{
				AddPrefabInstanceToManagedPrefab(go.transform.GetChild(i).gameObject);
			}
		}

		private static bool IsManagedPrefab()
		{
			return Selection.activeGameObject != null &&
				PrefabUtility.GetPrefabAssetType(Selection.activeGameObject) == PrefabAssetType.Regular &&
				Selection.activeGameObject.GetComponent<ManagedPrefab>() != null;
		}

		private static SceneInfo BuildSceneInfo(Transform root, GameObject go, string parentName, SceneInfo sceneInfo, List<GameObject> prefabs, bool needBundleInfo)
		{
			if (go.GetComponent<ManagedPrefab>() != null)
			{
				if (sceneInfo == null)
				{
					sceneInfo = new SceneInfo();
				}
				else
				{
					var newSceneInfo = new SceneInfo();
					if (sceneInfo.children == null)
						sceneInfo.children = new List<SceneInfo>();
					sceneInfo.children.Add(newSceneInfo);
					sceneInfo = newSceneInfo;
				}

				if (needBundleInfo)
				{
					var prefab = PrefabUtility.GetCorrespondingObjectFromSource(go);
					var path = AssetDatabase.GetAssetPath(prefab);
					var importer = AssetImporter.GetAtPath(path);

					sceneInfo.bundleName = importer.assetBundleName;
					sceneInfo.assetName = Path.GetFileNameWithoutExtension(path);
				}

				sceneInfo.uniqueName = go.name;
				sceneInfo.parentName = parentName;
				sceneInfo.activate = go.activeSelf;

				root = go.transform;
				parentName = "";

				if (prefabs != null)
					prefabs.Add(go);
			}
			else
			{
				if (string.IsNullOrEmpty(parentName))
					parentName = go.name;
				else
					parentName = parentName + "/" + go.name;
			}

			for (int i = 0; i < go.transform.childCount; ++i)
			{
				BuildSceneInfo(root, go.transform.GetChild(i).gameObject, parentName, sceneInfo, prefabs, needBundleInfo);
			}

			return sceneInfo;
		}

		private static void RestoreHierarchy(Transform root, SceneInfo sceneInfo, GameObject cacheObj)
		{
			var go = cacheObj.transform.GetChild(0).gameObject;

			Transform parent = root;
			if (!string.IsNullOrEmpty(sceneInfo.parentName))
			{
				if (parent == null)
					parent = GameObject.Find(sceneInfo.parentName).transform;
				else
					parent = parent.Find(sceneInfo.parentName);
			}
			go.transform.SetParent(parent, false);
			go.SetActive(sceneInfo.activate);

			if (sceneInfo.children != null)
			{
				for (int i = 0; i < sceneInfo.children.Count; ++i)
				{
					RestoreHierarchy(go.transform, sceneInfo.children[i], cacheObj);
				}
			}
		}

		private static GameObject CreateCacheObject(List<GameObject> prefabs)
		{
			var cacheObj = new GameObject();
			cacheObj.name = "_Cache";
			for (int i = 0; i < prefabs.Count; ++i)
				prefabs[i].transform.SetParent(cacheObj.transform, false);
			return cacheObj;
		}
	}
}
