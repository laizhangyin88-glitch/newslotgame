using UnityEngine;

namespace BagelCode
{
    public class SharedDataAsset<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    var founds = FindObjectsOfType(typeof(T));

                    if (founds.Length == 0)
                    {
                        return null;
                    }
                    
                    _instance = (T)founds[0];
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
