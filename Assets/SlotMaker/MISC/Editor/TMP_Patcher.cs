using System.Collections.Generic;
using System.Linq;
using SlotMaker.Json;
using UnityEditor;
using UnityEngine;
using TMPro;

namespace SlotMaker
{
    public static class TMP_Patcher
    {
        [MenuItem("Assets/Patch/TextMeshPro/Bold Spacing (FontAsset)", false, 800)]
        public static void BoldSpacing()
        {
            var patchResult = new Dictionary<string, object>();

            string[] paths = Selection.assetGUIDs.Select(x => AssetDatabase.GUIDToAssetPath(x)).ToArray();
            string[] foundGUIDs = AssetDatabase.FindAssets("t:TMP_FontAsset", paths);
            foreach (var guid in foundGUIDs)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var assetImporter = AssetImporter.GetAtPath(path);

                if (!string.IsNullOrEmpty(assetImporter.userData))
                {
                    patchResult = SlotSimpleJson.DeserializeObject<Dictionary<string, object>>(assetImporter.userData);
                    if (patchResult.ContainsKey("fixedBoldSpacing"))
                    {
                        Debug.LogWarning("Already patched: " + path);
                        continue;
                    }
                }

                var fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
                fontAsset.boldSpacing = GetNewBoldSpacing(fontAsset);
                EditorUtility.SetDirty(fontAsset);

                patchResult["fixedBoldSpacing"] = true;
                assetImporter.userData = SlotSimpleJson.SerializeObject(patchResult);
                assetImporter.SaveAndReimport();

                Debug.Log(path);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static float GetNewBoldSpacing(TMP_FontAsset fontAsset, float constantOffset = 0)
        {
            // Getting median of entire glyph's horizontal advance
            float median;
            {
                List<float> list = new List<float>(from glyph in fontAsset.glyphTable select glyph.metrics.horizontalAdvance);
                list.Sort();
                median = list[list.Count / 2];
            }

            return (median * (fontAsset.boldSpacing) + fontAsset.normalSpacingOffset * 100) / fontAsset.faceInfo.pointSize - fontAsset.normalSpacingOffset + constantOffset;
        }

        [MenuItem("Assets/Patch/TextMeshPro/Default Space(32) Character (FontAsset)", false, 801)]
        public static void DefaultSpaceChar()
        {
            var patchResult = new Dictionary<string, object>();

            string[] paths = Selection.assetGUIDs.Select(x => AssetDatabase.GUIDToAssetPath(x)).ToArray();
            string[] foundGUIDs = AssetDatabase.FindAssets("t:TMP_FontAsset", paths);
            foreach (var guid in foundGUIDs)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var assetImporter = AssetImporter.GetAtPath(path);

                if (!string.IsNullOrEmpty(assetImporter.userData))
                {
                    patchResult = SlotSimpleJson.DeserializeObject<Dictionary<string, object>>(assetImporter.userData);
                    if (patchResult.ContainsKey("fixedDefaultSpaceChar"))
                    {
                        Debug.LogWarning("Already patched: " + path);
                        continue;
                    }
                }

                var fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
                var lastCharacter = fontAsset.characterTable[fontAsset.characterTable.Count - 1];
                if (lastCharacter.unicode != 32)
                    continue;

                var lastGlyph = fontAsset.glyphTable[fontAsset.glyphTable.Count - 1];
                if (lastCharacter.glyphIndex != lastGlyph.index)
                {
                    Debug.Log("Not Matched: " + path);
                    continue;
                }

                var newMetrics = lastGlyph.metrics;
                newMetrics.horizontalAdvance = fontAsset.faceInfo.pointSize / 4f;
                lastGlyph.metrics = newMetrics;

                EditorUtility.SetDirty(fontAsset);

                patchResult["fixedDefaultSpaceChar"] = true;
                assetImporter.userData = SlotSimpleJson.SerializeObject(patchResult);
                assetImporter.SaveAndReimport();

                Debug.Log(path);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/Patch/TextMeshPro/Spacing Options (Prefab)", false, 802)]
        public static void SpacingOptions()
        {
            var patchResult = new Dictionary<string, object>();

            string[] paths = Selection.assetGUIDs.Select(x => AssetDatabase.GUIDToAssetPath(x)).ToArray();
            string[] foundGUIDs = AssetDatabase.FindAssets("t:prefab", paths);
            foreach (var guid in foundGUIDs)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var assetImporter = AssetImporter.GetAtPath(path);

                if (!string.IsNullOrEmpty(assetImporter.userData))
                {
                    patchResult = SlotSimpleJson.DeserializeObject<Dictionary<string, object>>(assetImporter.userData);
                    if (patchResult.ContainsKey("fixedSpacingOptions"))
                    {
                        Debug.LogWarning("Already patched: " + path);
                        continue;
                    }
                }

                GameObject prefab = PrefabUtility.LoadPrefabContents(path);

                TMP_Text[] texts = prefab.GetComponentsInChildren<TMP_Text>(true);
                foreach (var text in texts)
                {
                    if (text.font == null)
                    {
                        Debug.LogError(path + ", " + text.gameObject.name);
                    }
                    else
                    {
                        float px2em = 100f / text.font.faceInfo.pointSize;
                        text.characterSpacing *= px2em;
                        text.wordSpacing *= px2em;
                        text.lineSpacing *= px2em;
                        text.paragraphSpacing *= px2em;
                    }
                }

                var isSuccess = false;
                PrefabUtility.SaveAsPrefabAsset(prefab, path, out isSuccess);
                PrefabUtility.UnloadPrefabContents(prefab);

                patchResult["fixedSpacingOptions"] = true;
                assetImporter.userData = SlotSimpleJson.SerializeObject(patchResult);
                assetImporter.SaveAndReimport();

                Debug.Log(path);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
