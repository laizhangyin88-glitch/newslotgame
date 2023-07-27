using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SlotMaker
{
    public class HashTools : EditorWindowBase<HashTools>
    {
        private string sortingLayerName = "Base";
        private string animatorString = "Idle";

        private GUILayoutOption DEFAULT_WIDTH = GUILayout.Width(80f);

        public override string GetEditorName()
        {
            return "Hash Tools";
        }

        [MenuItem("SlotMaker/Tools/Hash Tools", false, 199)]
        private static void Initialize()
        {
            CreateWindow();
        }

        private void OnGUI()
        {
            BeginHorizontal();
                LabelField("Sorting Layer", DEFAULT_WIDTH);
                sortingLayerName = TextField(sortingLayerName, DEFAULT_WIDTH);
                TextField(SortingLayer.NameToID(sortingLayerName).ToString());
            EndHorizontal();

            BeginHorizontal();
                LabelField("Animator", DEFAULT_WIDTH);
                animatorString = TextField(animatorString, DEFAULT_WIDTH);
                TextField(Animator.StringToHash(animatorString).ToString());
            EndHorizontal();
        }
    }
}
