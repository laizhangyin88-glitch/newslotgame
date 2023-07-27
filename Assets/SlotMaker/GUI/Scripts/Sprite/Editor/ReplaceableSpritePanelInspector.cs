using UnityEditor;
using UnityEngine;

namespace SlotMaker
{
    [CustomEditor(typeof(ReplaceableSpritePanel))]
    public class ReplaceableSpritePanelInspector : Editor
    {
        private ReplaceableSpritePanel panel;

        private void OnEnable()
        {
            panel = target as ReplaceableSpritePanel;
        }

        public override void OnInspectorGUI()
        {
            Material mat = (Material)EditorGUILayout.ObjectField("Material", panel.material, typeof(Material), false);
            if (panel.material != mat)
            {
                panel.material = mat;
            }

            panel.color = EditorGUILayout.ColorField("Color", panel.color);

            ReplaceableSpritePanel.Clipping clipping = (ReplaceableSpritePanel.Clipping)EditorGUILayout.EnumPopup("Clipping", panel.clipping);

            if (panel.clipping != clipping)
            {
                panel.clipping = clipping;
            }

            if (panel.clipping != ReplaceableSpritePanel.Clipping.None)
            {
                Vector4 range = panel.clipRange;

                GUILayout.BeginHorizontal();
                GUILayout.Space(80f);
                Vector2 pos =
                    EditorGUILayout.Vector2Field("Center", new Vector2(range.x, range.y), GUILayout.MinWidth(20f));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(80f);
                Vector2 size =
                    EditorGUILayout.Vector2Field("Size", new Vector2(range.z, range.w), GUILayout.MinWidth(20f));
                GUILayout.EndHorizontal();

                range.x = pos.x;
                range.y = pos.y;
                range.z = size.x;
                range.w = size.y;

                if (panel.clipRange != range)
                {
                    panel.clipRange = range;
                }

                if (panel.clipping == ReplaceableSpritePanel.Clipping.SoftClip)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(80f);
                    Vector2 soft =
                        EditorGUILayout.Vector2Field("Softness", panel.clipSoftness, GUILayout.MinWidth(20f));
                    GUILayout.EndHorizontal();

                    if (panel.clipSoftness != soft)
                    {
                        panel.clipSoftness = soft;
                    }
                }
                else if (panel.clipping == ReplaceableSpritePanel.Clipping.TextureMask)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(80f);
                    float cutoff = EditorGUILayout.FloatField("Cutoff", panel.cutoff);
                    cutoff = Mathf.Clamp(cutoff, -1f, 1f);
                    GUILayout.EndHorizontal();

                    if (panel.cutoff != cutoff)
                    {
                        panel.cutoff = cutoff;
                    }

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(80f);
                    float soft = EditorGUILayout.FloatField("Cutoff Softness", panel.cutoffSoftness);
                    soft = Mathf.Clamp(soft, 0.001f, 1f);
                    GUILayout.EndHorizontal();

                    if (panel.cutoffSoftness != soft)
                    {
                        panel.cutoffSoftness = soft;
                    }

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(80f);
                    EditorGUIUtility.labelWidth = 0;
                    EditorGUIUtility.fieldWidth = 15;
                    Texture2D tex = (Texture2D)EditorGUILayout.ObjectField("Clip Texture", panel.clipTexture,
                        typeof(Texture2D), false);
                    GUILayout.EndHorizontal();

                    if (panel.clipTexture != tex)
                    {
                        panel.clipTexture = tex;
                    }
                }
                else if (panel.clipping == ReplaceableSpritePanel.Clipping.Distortion)
                {
                    Vector4 args = panel.clipArgs;

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(80f);
                    float softness = EditorGUILayout.FloatField("Softness", panel.clipArgs.x);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(80f);
                    float deepness = EditorGUILayout.FloatField("Deepness", panel.clipArgs.y);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(80f);
                    float curveRange = EditorGUILayout.FloatField("Range", panel.clipArgs.z);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(80f);
                    float smoothness = EditorGUILayout.FloatField("Smoothness", panel.clipArgs.w);
                    GUILayout.EndHorizontal();

                    args.x = softness;
                    args.y = deepness;
                    args.z = curveRange;
                    args.w = smoothness;

                    if (panel.clipArgs != args)
                    {
                        panel.clipArgs = args;
                    }

                    Vector4 scale = panel.scale;

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(80f);
                    Vector2 vectorScale = EditorGUILayout.Vector2Field("Vector Scale",
                        new Vector2(panel.scale.x, panel.scale.y), GUILayout.MinWidth(20f));
                    vectorScale = Vector2.Max(vectorScale, Vector2.one);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(80f);
                    Vector2 clipHeight = EditorGUILayout.Vector2Field("Clip Height",
                        new Vector2(panel.scale.z, panel.scale.w), GUILayout.MinWidth(20f));
                    GUILayout.EndHorizontal();

                    scale.x = vectorScale.x;
                    scale.y = vectorScale.y;
                    scale.z = clipHeight.x;
                    scale.w = clipHeight.y;

                    if (panel.scale != scale)
                    {
                        panel.scale = scale;
                    }
                }

                if (GUILayout.Button("Fit"))
                {
                    size = panel.GetComponent<RectTransform>().sizeDelta;
                    panel.clipRange = new Vector4(0f, 0f, size.x, size.y);
                }
            }

            var spriteImagesProperty = serializedObject.FindProperty("spriteImages");
            serializedObject.Update();
            EditorGUILayout.PropertyField(spriteImagesProperty, true);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
