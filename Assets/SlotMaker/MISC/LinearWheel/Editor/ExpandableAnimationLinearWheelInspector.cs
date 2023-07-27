using UnityEditor;
using UnityEngine;

namespace SlotMaker
{
    [CustomEditor(typeof(ExpandableAnimationLinearWheel))]
    public class ExpandableAnimationLinearWheelInspector : Editor
    {
        private ExpandableAnimationLinearWheel _expandableWheel;
        
        private void OnEnable()
        {
            _expandableWheel = target as ExpandableAnimationLinearWheel;
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (_expandableWheel == null)
            {
                _expandableWheel = target as ExpandableAnimationLinearWheel;
            }

            if (_expandableWheel != null)
            {
                EditorGUILayout.Space();
            
                if (GUILayout.Button("Start Spin"))
                {
                    _expandableWheel.StartSpin();
                }
            
                EditorGUILayout.Space();
            
                if (GUILayout.Button("Stop Spin"))
                {
                    _expandableWheel.StopSpin();
                }
            
                EditorGUILayout.Space();
            
                if (GUILayout.Button("Expand jackpot width"))
                {
                    _expandableWheel.ExpandJackpotWidth();
                }
                
                EditorGUILayout.Space();
                
                if (GUILayout.Button("Increase sector values"))
                {
                    _expandableWheel.IncreaseSectorValues();
                }
                
                EditorGUILayout.Space();
            }
        }
    }
}