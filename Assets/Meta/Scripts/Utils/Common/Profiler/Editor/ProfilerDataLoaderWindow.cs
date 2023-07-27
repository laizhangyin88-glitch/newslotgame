using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.Profiling;

namespace BagelCode
{
    public class ProfilerDataLoaderWindow : EditorWindow
    {
        private static List<string> cachedFilePathList = new List<string>();
        private static int dataIndex = -1;

        [MenuItem("Tools/ProfilerDataLoader")]
        private static void Init()
        {
            var window = (ProfilerDataLoaderWindow)GetWindow(typeof(ProfilerDataLoaderWindow));
            window.Show();

            ReadProfilerDataFiles();
        }

        private static void ReadProfilerDataFiles()
        {
            var filePaths = Directory.GetFiles(Application.persistentDataPath, "profilerLog*");
            var test = new Regex(".data$");

            foreach (var p in filePaths)
            {
                Match match = test.Match(p);
                if (!match.Success)
                {
                    // 이진 데이터 파일이 아니면 리스트에 바로 추가
                    Debug.Log("Found file: " + p);
                    cachedFilePathList.Add(p);
                }
            }

            dataIndex = -1;
        }

        private void OnGUI()
        {
            if(GUILayout.Button("Find Files"))
            {
                ReadProfilerDataFiles();
            }

            if (cachedFilePathList == null) return;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Files");
            EditorGUILayout.BeginHorizontal();

            var defaultStyle = new GUIStyle(GUI.skin.button);
            defaultStyle.fixedWidth = 40f;

            var highlightStyle = new GUIStyle(defaultStyle);
            highlightStyle.normal.textColor = Color.red;

            for(int i = 0; i < cachedFilePathList.Count; ++i)
            {
                if(i % 5 == 0)
                {
                    // Split 5
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }

                GUIStyle style = dataIndex == i ? highlightStyle : defaultStyle;

                if (GUILayout.Button(i.ToString(), style))
                {
                    Profiler.AddFramesFromFile(cachedFilePathList[i]);
                    dataIndex = i;
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}