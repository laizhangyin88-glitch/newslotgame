using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace SlotMaker
{
	[AddComponentMenu("SlotMaker/GameObject/Object Pool/Scene Base Object Pool")]
	public class SceneBaseObjectPool : ObjectPool 
	{
		public SceneInfoObject sceneInfoObject;

		public override void CreatePool()
		{
			prefab = SceneManager.LoadScene(transform, sceneInfoObject.GetSceneInfo());
			prefab.SetActive(false);

			base.CreatePool();
		}
	}
}