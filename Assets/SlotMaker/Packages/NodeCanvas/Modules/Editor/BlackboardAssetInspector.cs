using UnityEngine;
using UnityEditor;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using NodeCanvas.Editor;

namespace SlotMaker
{
    [CustomEditor(typeof(BlackboardAsset))]
    public class BlackboardAssetInspector : UnityEditor.Editor
    {
        private BlackboardAsset bb {
            get { return (BlackboardAsset)target; }
        }

        public override void OnInspectorGUI() {
            BlackboardEditor.ShowVariables(bb, bb);
            EditorUtils.EndOfInspector();
            if ( Application.isPlaying || Event.current.isMouse ) {
                Repaint();
            }
        }
    }
}