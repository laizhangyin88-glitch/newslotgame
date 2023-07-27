using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SlotMaker;
using UnityEngine;
using Sirenix.OdinInspector;

namespace BagelCode
{
    public class MetaGameAppearTransformManager : SlotMaker.MonoWeakSingleton<MetaGameAppearTransformManager>
    {
        private Dictionary<string, List<Transform>> transformDict;

        public void Regist(string id, Transform targetTransform)
        {
            if(transformDict == null) transformDict = new Dictionary<string, List<Transform>>();

            if(transformDict.ContainsKey(id))
            {
                if( !transformDict[id].Contains(targetTransform) )
                    transformDict[id].Add(targetTransform);
            }
            else
            {
                transformDict.Add( id, new List<Transform>(){targetTransform} );
            }

        }

        public void UnRegist(string id, Transform targetTransform)
        {
            if(transformDict == null) return;

            if(transformDict.ContainsKey(id))
            {
                transformDict[id].Remove(targetTransform);
                if(transformDict[id].Count <= 0)
                    transformDict.Remove(id);
            }
        }

        public Transform GetTransform(string id, int index = -1)
        {
            if(transformDict == null) return null;

            if(transformDict.ContainsKey(id))
            {
                if( index < 0 )
                {
                    if(transformDict[id].Count > 0)
                        return transformDict[id][transformDict[id].Count - 1];
                }
                else if( transformDict[id].Count <= index )
                {
                    return transformDict[id][index];
                }
            }

            return null;
        }
#if UNITY_EDITOR
        [Button]
        public void PrintDetails()
        {
            if(transformDict == null) return;

            foreach(var info in transformDict)
            {
                Debug.Log( string.Format("{0} : {1}", info.Key, info.Value == null ? "null" : info.Value.Count.ToString()) );
            }
        }

        public string testKey;
        [Button]
        public void PrintLastItem()
        {
            var transform = GetTransform(testKey);

            Debug.LogError( string.Format("{0} is {1}", testKey, transform == null ? "Null" : transform.name ) );
        }
#endif

    }
}
