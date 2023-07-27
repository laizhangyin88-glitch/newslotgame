using UnityEngine;
using UnityEditor;
using SlotMaker.IoC;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using NodeCanvas.Editor;

namespace SlotMaker.Extentions
{
    [CustomEditor(typeof(ActionListPlayer))]
    public class ActionListPlayerInspector : Editor
    {
        private ActionListPlayer list {
            get { return (ActionListPlayer)target; }
        }

        public override void OnInspectorGUI() {

            GUI.skin.label.richText = true;
            GUILayout.Space(10);

            list.blackboard = (Blackboard)EditorGUILayout.ObjectField("Target Blackboard", (Blackboard)list.blackboard, typeof(Blackboard), true);
            list.enableAction = (EnableAction)EditorGUILayout.EnumPopup("Enable Action", list.enableAction);
            TaskEditor.TaskFieldSingle(list.actionList, null, false);
            EditorUtils.EndOfInspector();

            if ( Event.current.isMouse ) {
                Repaint();
            }
        }
    }
}
