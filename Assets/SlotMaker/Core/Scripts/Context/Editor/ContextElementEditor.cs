using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SlotMaker
{
    [CustomEditor(typeof(ContextElement), true)]
    public class ContextElementEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var element = (ContextElement)target;
            Print(element);
            ShowPath(element);
            ShowChildren(element, "");
        }

        private void Print(ContextElement element)
        {
            if (GUILayout.Button("Print"))
            {
                element.UpdateContext(true);
                Debug.Log(element.ToString());
            }
        }

        private void ShowPath(ContextElement element)
        {
            string parentHierarchy = element.ContextName;
            Transform parent = element.transform.parent;
            while (parent != null)
            {
                var parentElement = parent.GetComponent<ContextElement>();
                if (parentElement != null)
                {
                    parentHierarchy = string.Format("{0}/{1}", parentElement.ContextName, parentHierarchy);

                    if (parentElement.IsRoot)
                        break;
                }
                parent = parent.parent;
            }

            if (!string.IsNullOrEmpty(parentHierarchy))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Path");
                EditorGUILayout.TextField(parentHierarchy);
                EditorGUILayout.EndHorizontal();
            }
        }

        private void ShowChildren(ContextElement element, string path)
        {
            var enumerator = element.GetEnumerator();
            if (enumerator == null) return;
            
            while (enumerator.MoveNext())
            {
                ContextElement item = enumerator.Current as ContextElement;
                if (item != null)
                {
                    var newPath = string.Format("{0}/{1}", path, item.ContextName);

                    GUILayout.BeginHorizontal();
                    ShowCopyChildrenButton(newPath);
                    ShowChildButton(item.transform, newPath);
                    GUILayout.EndHorizontal();
                    
                    ShowChildren(item, newPath);
                }
                else 
                {
                    var pair = (KeyValuePair<string, ContextElement>)enumerator.Current;
                    var newPath = string.Format("{0}/{1}", path, pair.Key);

                    if(pair.Value == null)
                        continue;
                    
                    GUILayout.BeginHorizontal();
                    ShowCopyChildrenButton(newPath);
                    ShowChildButton(pair.Value.transform, newPath);
                    GUILayout.EndHorizontal();
                    
                    ShowChildren(pair.Value, newPath);
                }
            }
        }

        private void ShowChildButton(Transform child, string text)
        {
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.alignment = TextAnchor.MiddleLeft;

            if (GUILayout.Button(text, buttonStyle))
            {
                Selection.activeGameObject = child.gameObject;
            }
        }
        
        private void ShowCopyChildrenButton(string path)
        {
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.alignment = TextAnchor.MiddleLeft;
            buttonStyle.stretchWidth = false;
            
            if (GUILayout.Button("COPY", buttonStyle))
            {
                GUIUtility.systemCopyBuffer = path.Substring(1);
                Debug.Log("Copy \"" + path.Substring(1) + "\" to clipboard");
            }
        }
    }
}
