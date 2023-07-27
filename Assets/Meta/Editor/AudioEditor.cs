using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.IMGUI.Controls;
using SlotMaker;
using SlotMaker.Json;
using NodeCanvas.Framework;

namespace BagelCode
{
    public class AudioEditor : EditorWindowBase<AudioEditor>
    {
        private bool isPlaying;
        [SerializeField] TreeViewState treeViewState; // Serialized in the window layout file so it survives assembly reloading

        private string gameId;
        private GameObject targetGo;
        private Blackboard targetBB;
        private Dictionary<string, GSHandler> changeSet;
        private string query;
        private List<Regex> regs = new List<Regex>();
        private bool modifyFilter;
        private Vector2 scrollPosition;

        private Dictionary<string, Dictionary<string, GSHandler>> originalSets = new Dictionary<string, Dictionary<string, GSHandler>>();
        private Dictionary<string, Dictionary<string, GSHandler>> changeSets = new Dictionary<string, Dictionary<string, GSHandler>>();

        public override string GetEditorName()
        {
            return "Audio Editor";
        }

        [MenuItem("BagelCode/Audio Editor", false, 101)]
        private static void Initialize()
        {
            CreateWindow();
        }

        private void OnGUI()
        {
            if (EditorApplication.isPlaying)
            {
                bool isFirst = !isPlaying;
                isPlaying = true;

                if (isFirst)
                    OnEnterPlaying();
            }
            else 
            {
                bool isFirst = isPlaying;
                isPlaying = false;

                if (isFirst)
                    OnLeavePlaying();
            }
            
            BeginHorizontal();
            if (Button("Common"))
                LoadCommon();
            if (Button("Content"))
                LoadContent();
            EndHorizontal();

            Space();

            BeginHorizontal(EditorStyles.helpBox);
            LabelField(gameId, GUILayout.Width(80f));
            BeginCheck();
            query = TextField(query);
            if (EndCheck())
            {
                var tokens = query.Split(' ');
                regs.Clear();
                foreach (var token in tokens)
                {
                    regs.Add(new Regex(token, RegexOptions.IgnoreCase | RegexOptions.Compiled));
                }
            }
            modifyFilter = Toggle(modifyFilter, GUILayout.Width(20f));
            if (Button("Clear", GUILayout.Width(80f)))
                Clear();
            if (Button(string.Format("Revert({0})", changeSet != null ? changeSet.Count : 0), GUILayout.Width(80f)) && changeSet != null)
            {
                Revert();
            }
            if (Button("Apply", GUILayout.Width(80f)))
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            
            EndHorizontal();

            Space();

            if (targetGo != null && targetBB != null)
            {
                using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition))
                {
                    scrollPosition = scrollView.scrollPosition;

                    var variables = targetBB.variables;
                    foreach (var pair in variables)
                    {
                        var handler = (GSHandler)pair.Value.value;
                        GSHandler currentChange = null;
                        if (changeSet != null)
                            changeSet.TryGetValue(pair.Key, out currentChange);

                        if (modifyFilter && currentChange == null)
                            continue;
                        
                        bool passed = true;
                        foreach (var reg in regs)
                        {
                            if (!reg.IsMatch(pair.Key))
                            {
                                passed = false;
                                break;
                            }
                        }
                        if (!passed)
                            continue;
                        
                        BeginHorizontal(EditorStyles.helpBox);

                        bool revert = !Toggle(currentChange != null, GUILayout.Width(20f));
                        if (revert && currentChange != null)
                        {
                            CopyGSHandler(originalSets[gameId][pair.Key], handler);
                            currentChange = null;
                            changeSet.Remove(pair.Key);
                            if (changeSet.Count == 0)
                            {
                                changeSet = null;
                                changeSets.Remove(gameId);
                            }        
                        }

                        BeginCheck();
                        LabelField(pair.Key, GUILayout.Width(180f));

                        handler.output = (GSMixerGroup)EnumPopup(handler.output, GUILayout.Width(75f));
                        handler.loop = Toggle(handler.loop, GUILayout.Width(16f));
                        handler.volume = Slider(handler.volume, 0f, 1f, GUILayout.Width(120f));
                        handler.playingType = (GSPlayingType)EnumPopup(handler.playingType, GUILayout.Width(75f));
                        handler.countLimit = IntField(handler.countLimit, GUILayout.Width(20f));
                        handler.fadeIn = DrawFadeInOut(handler.fadeIn);
                        handler.fadeOut = DrawFadeInOut(handler.fadeOut);
                        if (EndCheck())
                        {
                            if (changeSet == null)
                            {
                                changeSet = new Dictionary<string, GSHandler>();
                                changeSets[gameId] = changeSet;
                            }

                            if (currentChange == null)
                            {
                                currentChange = new GSHandler();
                                changeSet[pair.Key] = currentChange;
                            }

                            CopyGSHandler(handler, currentChange);

                            if (!isPlaying)
                            {
                                EditorUtility.SetDirty(targetGo);
                            }
                        }
                        EndHorizontal();
                    }
                }
            }
        }

        private GSFadeInOut DrawFadeInOut(GSFadeInOut fadeInOut)
        {
            if (fadeInOut != null)
            {
                fadeInOut.easeType = (GSEaseType)EnumPopup(fadeInOut.easeType, GUILayout.Width(75f));
                fadeInOut.time = FloatField(fadeInOut.time, GUILayout.Width(30f));
                if (fadeInOut.time == 0f && fadeInOut.easeType == GSEaseType.None)
                    return null;
            }
            else
            {
                var easeType = (GSEaseType)EnumPopup(GSEaseType.None, GUILayout.Width(75f));
                float time = FloatField(0f, GUILayout.Width(30f));
                if (time != 0f || easeType != GSEaseType.None)
                    return new GSFadeInOut { time = time, easeType = easeType };
            }

            return fadeInOut;
        }

        private void LoadCommon()
        {
            gameId = "common";
            if (!isPlaying)
            {
                targetGo = AssetBundleManager.LoadAsset<GameObject>("login" + ApplicationSettings.Instance.applicationType, "GS Manager");
                targetBB = targetGo.transform.GetChild(3).GetComponent<Blackboard>();
            }
            else 
            {
                targetBB = GSManager.Instance.handlers[0];
                targetGo = targetBB.gameObject;
            }

            LoadOriginalSet();
        }

        private void LoadContent()
        {
            if (!isPlaying)
            {
                gameId = EditorPrefs.GetString("gameId");
                targetGo = AssetBundleManager.LoadAsset<GameObject>(gameId, "customData");
                if (targetGo != null)
                    targetBB = targetGo.transform.GetChild(0).GetComponent<Blackboard>();
            }
            else 
            {
                gameId = BlackboardUtils.FindVariable<string>(null, "./game/gameTitle").value;
                targetBB = GSManager.Instance.handlers[1];
                targetGo = targetBB.gameObject;
            }

            LoadOriginalSet();
        }

        private void OnEnterPlaying()
        {
            Reset();
        }

        private void OnLeavePlaying()
        {
            ApplyChangeSet();
            Reset();
        }

        private void Reset()
        {
            gameId = null;
            targetGo = null;
            targetBB = null;
            query = null;
            regs.Clear();
            scrollPosition = Vector2.zero;
        }

        private void LoadOriginalSet()
        {
            if (targetGo != null && targetBB != null)
            {
                if (!originalSets.ContainsKey(gameId))
                {
                    var originalSet = new Dictionary<string, GSHandler>();
                    var variables = targetBB.variables;
                    foreach (var pair in variables)
                    {
                        var temp = new GSHandler();
                        CopyGSHandler((GSHandler)pair.Value.value, temp);
                        originalSet[pair.Key] = temp;
                    }
                    originalSets[gameId] = originalSet;    
                }

                changeSets.TryGetValue(gameId, out changeSet);
            }
        }

        private void Clear()
        {
            originalSets = new Dictionary<string, Dictionary<string, GSHandler>>();
            changeSets = new Dictionary<string, Dictionary<string, GSHandler>>();
            changeSet = null;

            LoadOriginalSet();
        }

        private void Revert()
        {
            if (targetGo != null && targetBB != null)
            {
                var variables = targetBB.variables;
                foreach (var pair in changeSet)
                {
                    CopyGSHandler(originalSets[gameId][pair.Key], (GSHandler)variables[pair.Key].value);
                }
            }

            changeSets.Remove(gameId);
            changeSet = null;      
        }

        private void ApplyChangeSet()
        {
            foreach (var pair1 in changeSets)
            {
                if (pair1.Key.Equals("common"))
                {
                    targetGo = AssetBundleManager.LoadAsset<GameObject>("login" + ApplicationSettings.Instance.applicationType, "GS Manager");
                    targetBB = targetGo.transform.GetChild(3).GetComponent<Blackboard>();
                }
                else 
                {
                    targetGo = AssetBundleManager.LoadAsset<GameObject>(pair1.Key, "customData");
                    if (targetGo != null)
                        targetBB = targetGo.transform.GetChild(0).GetComponent<Blackboard>();
                }

                var variables = targetBB.variables;
                foreach (var pair2 in pair1.Value)
                {
                    CopyGSHandler(pair2.Value, (GSHandler)variables[pair2.Key].value);
                }

                EditorUtility.SetDirty(targetGo);
            }
        }

        private void Apply()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void CopyGSHandler(GSHandler src, GSHandler dst)
        {
            dst.output = src.output;
            dst.loop = src.loop;
            dst.volume = src.volume;
            dst.delay = src.delay;
            dst.playingType = src.playingType;
            dst.countLimit = src.countLimit;
            dst.fadeIn = (src.fadeIn == null) ? null : new GSFadeInOut { time = src.fadeIn.time, easeType = src.fadeIn.easeType };
            dst.fadeOut = (src.fadeOut == null) ? null : new GSFadeInOut { time = src.fadeOut.time, easeType = src.fadeOut.easeType };
            dst.autoRelease = src.autoRelease;
        }
    }
}

