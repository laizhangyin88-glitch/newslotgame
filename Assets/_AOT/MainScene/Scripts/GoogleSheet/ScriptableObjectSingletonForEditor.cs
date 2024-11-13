#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace SlotMaker
{
	public class ScriptableObjectSingletonForEditor<T> : ScriptableObject where T : ScriptableObject
	{
		private static T _instance;
		private static object _mutex = new object();

		public static string assetPath = "Assets/SlotMaker/Editor/";

		public static T Instance
		{
			get 
			{
				lock(_mutex)
				{
					if (_instance == null)
					{
						string path = assetPath + typeof(T).Name + ".asset";

						_instance = (T)AssetDatabase.LoadAssetAtPath<T>(path);
						if (_instance == null)
						{
							_instance = ScriptableObject.CreateInstance<T>();
							AssetDatabase.CreateAsset(_instance, AssetDatabase.GenerateUniqueAssetPath(path));
						}
					}

					return _instance;
				}
			}
		}
	}
}
#endif