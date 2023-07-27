using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker.Json;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Sirenix.OdinInspector;

namespace SlotMaker
{
    public class PayTableRuntimeBuilder : MonoBehaviour
    {
        public TextAsset json;
        public GameObject prefab;
        public Color color;

        [Serializable]
        public class Page
        {
            public int beginIndex;
            public int endIndex;
            public Transform target;
        }
        public List<Page> pages;

        private void Start()
        {
            StartCoroutine(Build());
        }

        private IEnumerator Build()
        {
            yield return new WaitForSeconds(0.5f);
            
            var payLines = SlotSimpleJson.DeserializeObject<List<List<int>>>(json.text);
            for (int i = 0; i < pages.Count; ++i)
            {
                var page = pages[i];
                for (int lineNumber = page.beginIndex; lineNumber <= page.endIndex; ++lineNumber)
                {
                    var go = Instantiate(prefab) as GameObject;
                    go.name = string.Format("Line {0}", lineNumber);
                    go.transform.SetParent(page.target, false);

                    go.GetComponent<IPayTableLineSegments>().SetPayLine(lineNumber, payLines[lineNumber - 1], in color);
                }

                yield return null;
            }
        }

#if UNITY_EDITOR
        [Button]
        public void StaticBuild()
        {
            Clear();
            var payLines = SlotSimpleJson.DeserializeObject<List<List<int>>>(json.text);
            for (int i = 0; i < pages.Count; ++i)
            {
                var page = pages[i];
                for (int lineNumber = page.beginIndex; lineNumber <= page.endIndex; ++lineNumber)
                {
                    GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                    go.name = string.Format("Line {0}", lineNumber);
                    go.transform.SetParent(page.target, false);
                    go.GetComponent<IPayTableLineSegments>().SetPayLine(lineNumber, payLines[lineNumber - 1], in color);
                }
            }
        }

        [Button]
        public void Clear()
        {
            for (int i = 0; i < pages.Count; ++i)
            {
                var page = pages[i];
                page.target.gameObject.DestroyImmediateChildren();
            }
        }
#endif
    }
}
