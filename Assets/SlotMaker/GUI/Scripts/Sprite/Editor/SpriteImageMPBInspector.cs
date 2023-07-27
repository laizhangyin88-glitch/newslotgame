using UnityEngine;
using UnityEditor;
using System.Collections;

namespace SlotMaker
{
    [CustomEditor(typeof(SpriteImageMPB))]
    public class SpriteImageMPBInspector : Editor
    {
        private SpriteImageMPB spriteImageMPB;
        private SpritePanel panel;

        private void OnEnable()
        {
            spriteImageMPB = target as SpriteImageMPB;
            panel = spriteImageMPB.GetComponentInParent<SpritePanel>();
        }

        public override void OnInspectorGUI()
        {
            if (panel == null) return;

            if (panel.clipping != SpritePanel.Clipping.None)
            {
                Vector4 args = spriteImageMPB.args;
                if (panel.clipping == SpritePanel.Clipping.Distortion)
                {
                    GUILayout.BeginHorizontal();
                    float distortionCenterOffset = EditorGUILayout.FloatField("Distortion Center Offset", args.x);
                    GUILayout.EndHorizontal();

                    args.x = distortionCenterOffset;

                    if (spriteImageMPB.args != args)
                    {
                        spriteImageMPB.args = args;
                    }
                }
            }
        }
    }
}
