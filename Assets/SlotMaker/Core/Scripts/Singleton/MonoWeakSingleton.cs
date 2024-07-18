using UnityEngine;

namespace SlotMaker
{
	public class MonoWeakSingleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static T _instance;

		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					var founds = FindObjectsOfType(typeof(T));
					if (founds.Length > 1)
					{
						Debug.LogError("[Weak Singleton] Singlton '" + typeof(T) +
							"' should never be more than 1!");
						return null;
					}
					else if (founds.Length > 0)
					{
						_instance = (T)founds[0];

						if (ApplicationSettings.LogSystem())
							Debug.Log("[Weak Singleton] Singleton '" + typeof(T) +
								"' already created in this scene!");
					}
				}
				return _instance;
			}
		}

		protected virtual void OnDestroy()
		{
			_instance = null;
		}
	}
}
