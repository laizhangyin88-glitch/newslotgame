using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

namespace SlotMaker
{
    public class GlobalSymbolAssets : SymbolAssets
    {
        private static SymbolAssets _instance;

        public static SymbolAssets Instance
        {
            get
            {
                if (_instance == null)
                {
                    var founds = FindObjectsOfType(typeof(GlobalSymbolAssets));
                    if (founds.Length > 1)
                    {
                        Debug.LogError("[Weak Singleton] Singlton '" + typeof(GlobalSymbolAssets) +
                            "' should never be more than 1!");
                        return null;
                    }
                    else if (founds.Length > 0)
                    {
                        _instance = (SymbolAssets)founds[0];

                        if (ApplicationSettings.LogSystem())
                            Debug.Log("[Weak Singleton] Singleton '" + typeof(GlobalSymbolAssets) +
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

        // For Easy Development
        private static Dictionary<int, string> aniamtorStateHashs = new Dictionary<int, string>
        {
            { Animator.StringToHash("Idle"), "Idle" },
            { Animator.StringToHash("Win"), "Win" }
        };

        public static string _AnimatorHashToString(int stateNameHash)
        {
            string str;
            if (aniamtorStateHashs.TryGetValue(stateNameHash, out str))
                return str;

            return stateNameHash.ToString();
        }
    }
}
