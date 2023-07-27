using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.Animations;
using NodeCanvas.Framework;
using TMPro;
using SlotMaker.IoC;
using SlotMaker.IoC.Strategy;
using SlotMaker.IoC.UI;

namespace SlotMaker
{
	public static class DeepCopyAssets
	{
		[MenuItem("Assets/Deep Copy", false, 701)]
		public static void DeepCopy()
		{
			DeepCopy(Selection.assetGUIDs);
		}

		public static void DeepCopy(string[] assetGUIDs)
		{
			if (assetGUIDs == null)
				return;

			ClearDatabase();

			string[] srcPaths = assetGUIDs.Select(x => AssetDatabase.GUIDToAssetPath(x)).ToArray();
			string[] dstPaths = srcPaths.Select(x => x + " (Clone)").ToArray();

			AddPathsToDatabase(srcPaths, dstPaths);
			UpdateDatabase();
			CopyDatabase();
			DeepLinkDatabase();
		}

		private class Database
		{
            public Dictionary<string, string> referenceFolders = new Dictionary<string, string>();
            public Dictionary<string, string> referenceAssets = new Dictionary<string, string>();
            public Dictionary<string, string> animators = new Dictionary<string, string>();
            public Dictionary<string, string> animations = new Dictionary<string, string>();
            public Dictionary<string, string> graphs = new Dictionary<string, string>();
            public Dictionary<string, string> sprites = new Dictionary<string, string>();
            public Dictionary<string, string> textures = new Dictionary<string, string>();
            public Dictionary<string, string> fonts = new Dictionary<string, string>();
            public Dictionary<string, string> prefabs = new Dictionary<string, string>();
            public Dictionary<string, string> meshes = new Dictionary<string, string>();
            public Dictionary<string, string> materials = new Dictionary<string, string>();
            public Dictionary<string, string> variableAssets = new Dictionary<string, string>();
            public Dictionary<string, string> stringSources = new Dictionary<string, string>();
            public Dictionary<string, string> symbolAssets = new Dictionary<string, string>();
		}
		private static Database database = new Database();

		public static void ClearDatabase()
		{
            database.referenceFolders.Clear();
            database.referenceAssets.Clear();
            database.animators.Clear();
            database.animations.Clear();
            database.graphs.Clear();
            database.sprites.Clear();
            database.textures.Clear();
            database.fonts.Clear();
            database.prefabs.Clear();
            database.meshes.Clear();
            database.materials.Clear();
            database.variableAssets.Clear();
            database.stringSources.Clear();
            database.symbolAssets.Clear();
		}

		public static void AddPathsToDatabase(string[] srcPaths, string[] dstPaths)
		{
			for (int i = 0; i < srcPaths.Length; ++i)
			{
				if (AssetDatabase.IsValidFolder(srcPaths[i]))
					database.referenceFolders[srcPaths[i]] = dstPaths[i];
				else
					database.referenceAssets[srcPaths[i]] = dstPaths[i];
			}
		}

		public static void UpdateDatabase()
		{
			foreach (var pair in database.referenceFolders)
			{
                UpdateDatabaseAssetPaths(database.animators, pair.Key, pair.Value, "t:AnimatorController");
                UpdateDatabaseAssetPaths(database.animations, pair.Key, pair.Value, "t:Animation");
                UpdateDatabaseAssetPaths(database.graphs, pair.Key, pair.Value, "t:Graph");
                UpdateDatabaseAssetPaths(database.sprites, pair.Key, pair.Value, "t:Sprite");
                UpdateDatabaseAssetPaths(database.textures, pair.Key, pair.Value, "t:Texture");
                UpdateDatabaseAssetPaths(database.fonts, pair.Key, pair.Value, "t:TMP_FontAsset");
                UpdateDatabaseAssetPaths(database.prefabs, pair.Key, pair.Value, "t:prefab");
                UpdateDatabaseAssetPaths(database.meshes, pair.Key, pair.Value, "t:mesh");
                UpdateDatabaseAssetPaths(database.materials, pair.Key, pair.Value, "t:material");
                UpdateDatabaseAssetPaths(database.variableAssets, pair.Key, pair.Value, "t:VariableAsset");
                UpdateDatabaseAssetPaths(database.stringSources, pair.Key, pair.Value, "t:StringSources");
                UpdateDatabaseAssetPaths(database.symbolAssets, pair.Key, pair.Value, "t:SymbolAsset");
			}
		}

		public static void CopyDatabase()
		{
			foreach (var pair in database.referenceFolders)
			{
				FileUtil.CopyFileOrDirectory(pair.Key, pair.Value);
			}

			foreach (var pair in database.referenceAssets)
			{
				FileUtil.CopyFileOrDirectory(pair.Key, pair.Value);
			}

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		public static void DeepLinkDatabase()
		{
			DeepLinkAnimatorController();
			DeepLinkGraph();
			DeepLinkPrefab();
			DeepLinkSymbolAsset();
			DeepLinkFontMaterial();

	        AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		private static void DeepLinkAnimatorController()
		{
			foreach (var pair in database.animators)
			{
				AnimatorController animator = AssetDatabase.LoadAssetAtPath(pair.Value, typeof(AnimatorController)) as AnimatorController;
				foreach (var layer in animator.layers)
				{
					foreach (var state in layer.stateMachine.states)
					{
						string srcClipPath = AssetDatabase.GetAssetPath(state.state.motion);
						string dstClipPath;
						if (database.animations.TryGetValue(srcClipPath, out dstClipPath))
						{
							AnimationClip dstClip = AssetDatabase.LoadAssetAtPath(dstClipPath, typeof(AnimationClip)) as AnimationClip;
							state.state.motion = dstClip;
						}
						else
							Debug.Log(string.Format("[DeepCopy] Out bounds animation({0}) founded in {1}", srcClipPath, pair.Key));
					}
				}
	            EditorUtility.SetDirty(animator);
			}
		}

		private static void DeepLinkGraph()
		{
			foreach (var pair in database.graphs)
			{
				Graph graph = AssetDatabase.LoadAssetAtPath(pair.Value, typeof(Graph)) as Graph;
				foreach (var node in graph.allNodes.OfType<IGraphAssignable>())
				{
					if (node.nestedGraph == null)
						continue;

					string srcGraphPath = AssetDatabase.GetAssetPath(node.nestedGraph);
					string dstGraphPath;
					if (database.graphs.TryGetValue(srcGraphPath, out dstGraphPath))
					{
						Graph dstGraph = AssetDatabase.LoadAssetAtPath(dstGraphPath, typeof(Graph)) as Graph;
						node.nestedGraph = dstGraph;
					}
					else
						Debug.Log(string.Format("[DeepCopy] Out bounds graph({0}) founded in {1}", srcGraphPath, pair.Key));
				}
	            EditorUtility.SetDirty(graph);
			}
		}

		private static void DeepLinkPrefab()
		{
			foreach (var pair in database.prefabs)
			{
				GameObject prefab = PrefabUtility.LoadPrefabContents(pair.Value);

                DeepLinkPrefabAnimator(prefab);
                DeepLinkPrefabGraph(prefab);
                DeepLinkPrefabImage(prefab);
                DeepLinkPrefabTMP_Text(prefab);
                DeepLinkPrefabMeshRenderer(prefab);
                DeepLinkPrefabParticleSystem(prefab);
                DeepLinkPrefabAnimatorMediator(prefab);
                DeepLinkPrefabTextMediator(prefab);
                DeepLinkPrefabObjectPool(prefab);

				var isSuccess = false;
                PrefabUtility.SaveAsPrefabAsset(prefab, pair.Value, out isSuccess);
				DeepLinkPrefabNestedPrefab(prefab, pair.Value);
                PrefabUtility.UnloadPrefabContents(prefab);

				AssetDatabase.ImportAsset(pair.Value, ImportAssetOptions.ImportRecursive);
			}
		}

		private static void DeepLinkSymbolAsset()
		{
			foreach (var pair in database.symbolAssets)
			{
				SymbolAsset symbolAsset = AssetDatabase.LoadAssetAtPath(pair.Value, typeof(SymbolAsset)) as SymbolAsset;

				// link sprites
				for (int i = 0; i < symbolAsset.Sprites.Count; ++i)
				{
					string srcPath = AssetDatabase.GetAssetPath(symbolAsset.Sprites[i]);
					string dstPath;
					if (database.sprites.TryGetValue(srcPath, out dstPath))
					{
						Sprite sprite = AssetDatabase.LoadAssetAtPath(dstPath, typeof(Sprite)) as Sprite;
						symbolAsset.Sprites[i] = sprite;
					}
					else
						Debug.Log(string.Format("[DeepCopy] Out bounds sprite({0}) founded in {1}", srcPath, pair.Key));
				}

				// link prefabs
				for (int i = 0; i < symbolAsset.Prefabs.Count; ++i)
				{
					string srcPath = AssetDatabase.GetAssetPath(symbolAsset.Prefabs[i]);
					string dstPath;
					if (database.prefabs.TryGetValue(srcPath, out dstPath))
					{
						GameObject go = AssetDatabase.LoadAssetAtPath(dstPath, typeof(GameObject)) as GameObject;
						symbolAsset.Prefabs[i] = go;
					}
					else
						Debug.Log(string.Format("[DeepCopy] Out bounds prefab({0}) founded in {1}", srcPath, pair.Key));
				}

				// link graph
				{
					string srcPath = AssetDatabase.GetAssetPath(symbolAsset.Graph);
					string dstPath;
					if (database.graphs.TryGetValue(srcPath, out dstPath))
					{
						Graph graph = AssetDatabase.LoadAssetAtPath(dstPath, typeof(Graph)) as Graph;
						symbolAsset.Graph = graph;
					}
					else
						Debug.Log(string.Format("[DeepCopy] Out bounds graph({0}) founded in {1}", srcPath, pair.Key));
				}
	            EditorUtility.SetDirty(symbolAsset);
			}
		}

		private static void DeepLinkFontMaterial()
		{
			foreach (var pair in database.materials)
			{
				Material material = AssetDatabase.LoadAssetAtPath(pair.Value, typeof(Material)) as Material;

				{
					string srcPath = AssetDatabase.GetAssetPath(material.mainTexture);
					string dstPath;
					if (database.fonts.TryGetValue(srcPath, out dstPath))
					{
						TMP_FontAsset fontAsset = AssetDatabase.LoadAssetAtPath(dstPath, typeof(TMP_FontAsset)) as TMP_FontAsset;
						material.mainTexture = fontAsset.atlasTexture;
					}
					else
						Debug.Log(string.Format("[DeepCopy] Out bounds font material({0}) founded in {1}", srcPath, pair.Key));					
				}

				if (material.HasProperty("_FaceTex"))
				{
					string srcPath = AssetDatabase.GetAssetPath(material.GetTexture("_FaceTex"));
					string dstPath;
					if (database.textures.TryGetValue(srcPath, out dstPath))
					{
						Texture texture = AssetDatabase.LoadAssetAtPath(dstPath, typeof(Texture)) as Texture;
						material.SetTexture("_FaceTex", texture);
					}
					else
						Debug.Log(string.Format("[DeepCopy] Out bounds font material({0}) founded in {1}", srcPath, pair.Key));					
				}

				if (material.HasProperty("_OutlineTex"))
				{
					string srcPath = AssetDatabase.GetAssetPath(material.GetTexture("_OutlineTex"));
					string dstPath;
					if (database.textures.TryGetValue(srcPath, out dstPath))
					{
						Texture texture = AssetDatabase.LoadAssetAtPath(dstPath, typeof(Texture)) as Texture;
						material.SetTexture("_OutlineTex", texture);
					}
					else
						Debug.Log(string.Format("[DeepCopy] Out bounds font material({0}) founded in {1}", srcPath, pair.Key));					
				}

	            EditorUtility.SetDirty(material);
			}
		}

		private static void DeepLinkPrefabAnimator(GameObject prefab)
		{
			Animator[] animators = prefab.GetComponentsInChildren<Animator>(true);
			foreach (var animator in animators)
			{
				string srcPath = AssetDatabase.GetAssetPath(animator.runtimeAnimatorController);
				string dstPath;
				if (database.animators.TryGetValue(srcPath, out dstPath))
				{
					AnimatorController dstAnimator = AssetDatabase.LoadAssetAtPath(dstPath, typeof(AnimatorController)) as AnimatorController;
					animator.runtimeAnimatorController = dstAnimator;
				}
				else
					Debug.Log(string.Format("[DeepCopy] Out bounds animator({0}) founded in {1} ({2})", srcPath, animator.gameObject.name, prefab.name));
			}
		}

		private static void DeepLinkPrefabGraph(GameObject prefab)
		{
			GraphOwner[] owners = prefab.GetComponentsInChildren<GraphOwner>(true);
			foreach (var owner in owners)
			{
				string srcPath = AssetDatabase.GetAssetPath(owner.graph);
				string dstPath;
				if (database.graphs.TryGetValue(srcPath, out dstPath))
				{
					Graph dstGraph = AssetDatabase.LoadAssetAtPath(dstPath, typeof(Graph)) as Graph;
					owner.graph = dstGraph;
				}
				else
					Debug.Log(string.Format("[DeepCopy] Out bounds graph({0}) founded in {1} ({2})", srcPath, owner.gameObject.name, prefab.name));
			}
		}

		private static void DeepLinkPrefabImage(GameObject prefab)
		{
			Image[] images = prefab.GetComponentsInChildren<Image>(true);
			foreach (var image in images)
			{
				string srcPath = AssetDatabase.GetAssetPath(image.sprite);
				string dstPath;
				if (database.sprites.TryGetValue(srcPath, out dstPath))
				{
					Sprite dstSprite = AssetDatabase.LoadAssetAtPath(dstPath, typeof(Sprite)) as Sprite;
					image.sprite = dstSprite;
				}
				else 
					Debug.Log(string.Format("[DeepCopy] Out bounds sprite({0}) founded in {1} ({2})", srcPath, image.gameObject.name, prefab.name));
			
                srcPath = AssetDatabase.GetAssetPath(image.material);
                if (database.materials.TryGetValue(srcPath, out dstPath))
                {
                    Material dstMaterial = AssetDatabase.LoadAssetAtPath(dstPath, typeof(Material)) as Material;
                    image.material = dstMaterial;
                }
                else
                    Debug.Log(string.Format("[DeepCopy] Out bounds material({0}) founded in {1} ({2})", srcPath, image.gameObject.name, prefab.name));
            }
		}

		private static void DeepLinkPrefabTMP_Text(GameObject prefab)
		{
			TMP_Text[] texts = prefab.GetComponentsInChildren<TMP_Text>(true);
			foreach (var text in texts)
			{
				string srcPath = AssetDatabase.GetAssetPath(text.font);
				string srcMatPath = AssetDatabase.GetAssetPath(text.fontSharedMaterial);

				string dstPath;
				if (database.fonts.TryGetValue(srcPath, out dstPath))
				{
					TMP_FontAsset dstFont = AssetDatabase.LoadAssetAtPath(dstPath, typeof(TMP_FontAsset)) as TMP_FontAsset;
					text.font = dstFont;
				}
				else
				{
					Debug.Log(string.Format("[DeepCopy] Out bounds font({0}) founded in {1} ({2})", srcPath, text.gameObject.name, prefab.name));
				}

				string dstMatPath;
				if (database.materials.TryGetValue(srcMatPath, out dstMatPath))
				{
					Material material = AssetDatabase.LoadAssetAtPath(dstMatPath, typeof(Material)) as Material;
					text.fontSharedMaterial = material;
				}
				else
				{
					Debug.Log(string.Format("[DeepCopy] Out bounds font material({0}) founded in {1} ({2})", srcPath, text.gameObject.name, prefab.name));
				}

				text.SetMaterialDirty();
			}
		}

        private static void DeepLinkPrefabMeshRenderer(GameObject prefab)
        {
            SkinnedMeshRenderer[] SMRs = prefab.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            foreach (var smr in SMRs)
            {
                string srcPath = AssetDatabase.GetAssetPath(smr.sharedMesh);
                string dstPath;
                if (database.meshes.TryGetValue(srcPath, out dstPath))
                {
                    Mesh dstMesh = AssetDatabase.LoadAssetAtPath(dstPath, typeof(Mesh)) as Mesh;
                    smr.sharedMesh = dstMesh;
                }
                else
                    Debug.Log(string.Format("[DeepCopy] Out bounds font({0}) founded in {1} ({2})", srcPath, smr.gameObject.name, prefab.name));
            }
        }

        private static void DeepLinkPrefabParticleSystem(GameObject prefab)
        {
            ParticleSystemRenderer[] particleSystemes = prefab.GetComponentsInChildren<ParticleSystemRenderer>(true);
            foreach (var particleSystem in particleSystemes)
            {
                string srcPath = AssetDatabase.GetAssetPath(particleSystem.sharedMaterial);
                string dstPath;
                if (database.materials.TryGetValue(srcPath, out dstPath))
                {
                    Material dstMaterial = AssetDatabase.LoadAssetAtPath(dstPath, typeof(Material)) as Material;
                    particleSystem.sharedMaterial = dstMaterial;
                }
                else
                    Debug.Log(string.Format("[DeepCopy] Out bounds font({0}) founded in {1} ({2})", srcPath, particleSystem.gameObject.name, prefab.name));
            }
        }

        private static void DeepLinkPrefabAnimatorMediator(GameObject prefab)
        {
            AnimatorMediator[] mediators = prefab.GetComponentsInChildren<AnimatorMediator>(true);
            foreach (var mediator in mediators)
            {
				for (int i = 0, count = mediator.listeners.Count; i < count; ++i)
				{
					string srcPath = AssetDatabase.GetAssetPath(mediator.listeners[i].value);
					string dstPath;
					if (database.variableAssets.TryGetValue(srcPath, out dstPath))
					{
						VariableAsset dstAsset = AssetDatabase.LoadAssetAtPath(dstPath, typeof(VariableAsset)) as VariableAsset;
                        mediator.listeners[i].value = dstAsset;
					}
					else
						Debug.Log(string.Format("[DeepCopy] Out bounds VariableAsset({0}) founded in {1} ({2})", srcPath, mediator.gameObject.name, prefab.name));
				}
            }
        }

        private static void DeepLinkPrefabTextMediator(GameObject prefab)
        {
            TextMediator[] mediators = prefab.GetComponentsInChildren<TextMediator>(true);
            foreach (var mediator in mediators)
            {
				string srcPath = AssetDatabase.GetAssetPath(mediator.sourcesInjector);
				string dstPath;
				if (database.stringSources.TryGetValue(srcPath, out dstPath))
				{
					StringSources dstAsset = AssetDatabase.LoadAssetAtPath(dstPath, typeof(StringSources)) as StringSources;
					mediator.sourcesInjector = dstAsset;
				}
				else
					Debug.Log(string.Format("[DeepCopy] Out bounds VariableAsset({0}) founded in {1} ({2})", srcPath, mediator.gameObject.name, prefab.name));
            }
        }

		private static void DeepLinkPrefabNestedPrefab(GameObject prefab, string prefabPath) {
			List<string> nonNestedInfoList = new List<string>();
			List<string> nestedInfoList = new List<string>();
			int nonNestedInfoListCount = 0;
			int nestedInfoListCount = 0;
			bool isNestedInfo = false;
			StreamReader streamReader = new StreamReader(prefabPath);
			if (streamReader != null) {
				string line = "";
				while ((line = streamReader.ReadLine()) != null)
				{
					if (line == "PrefabInstance:") {
						isNestedInfo = true;
					}

					if (isNestedInfo) {
						nestedInfoList.Add(line);
					} else {
						nonNestedInfoList.Add(line);
					}
				}
				nonNestedInfoListCount = nonNestedInfoList.Count;
				nestedInfoListCount = nestedInfoList.Count;
			} else {
				Debug.LogError("streamReader is null");
			}
			streamReader.Close();

			Transform[] children = prefab.GetComponentsInChildren<Transform>(true);
			List<GameObject> childrenObjList = new List<GameObject>();
			for (int i = 0; i < children.Length; i++) {
				GameObject childObj = children[i].gameObject;
				PrefabInstanceStatus childStatus = PrefabUtility.GetPrefabInstanceStatus(childObj);
				if (childStatus == PrefabInstanceStatus.Connected) {
					childrenObjList.Add(childObj);
					string srcPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(childObj);
					string dstPath;
					if (database.prefabs.TryGetValue(srcPath, out dstPath)) {
						string srcGuid = AssetDatabase.AssetPathToGUID(srcPath);
						string dstGuid = AssetDatabase.AssetPathToGUID(dstPath);
						for (int j = 0; j < nestedInfoListCount; j++) {
							nestedInfoList[j] = nestedInfoList[j].Replace(srcGuid, dstGuid);
						}
					}
				}
			}

			StreamWriter streamWriter = new StreamWriter(prefabPath);
			if (streamWriter != null) {
				for (int i = 0; i < nonNestedInfoListCount; i++) {
					streamWriter.WriteLine(nonNestedInfoList[i]);
				}
				for (int i = 0; i < nestedInfoListCount; i++) {
					streamWriter.WriteLine(nestedInfoList[i]);
				}
			} else {
				Debug.LogError("streamWriter is null");
			}
			streamWriter.Close();
		}

		private static void DeepLinkPrefabObjectPool(GameObject prefab)
		{
			ObjectPool[] objectPools = prefab.GetComponentsInChildren<ObjectPool>(true);
			foreach (var objectPool in objectPools)
			{
				if (objectPool.prefab == null)
					continue;

				string srcPath = AssetDatabase.GetAssetPath(objectPool.prefab);
				string dstPath;
				if (database.prefabs.TryGetValue(srcPath, out dstPath))
				{
					GameObject dstPrefab = AssetDatabase.LoadAssetAtPath(dstPath, typeof(GameObject)) as GameObject;
					objectPool.prefab = dstPrefab;
					for (int i = 0; i < objectPool.availiableObjects.Count; ++i)
						objectPool.availiableObjects[i] = dstPrefab.GetComponent<PooledObject>();
				}
				else
					Debug.Log(string.Format("[DeepCopy] Out bounds prefab({0}) founded in {1} ({2})", srcPath, objectPool.gameObject.name, prefab.name));
			}
		}

		private static void UpdateDatabaseAssetPaths(Dictionary<string, string> database, string srcFolderPath, string dstFolderPath, string filter)
		{
			string[] srcAssetGUIDs = AssetDatabase.FindAssets(filter, new string[]{ srcFolderPath });
			foreach (string srcAssetGUID in srcAssetGUIDs)
			{
				var srcAssetPath = AssetDatabase.GUIDToAssetPath(srcAssetGUID);
				database[srcAssetPath] = srcAssetPath.Replace(srcFolderPath, dstFolderPath);
			}
		}
	}
}
