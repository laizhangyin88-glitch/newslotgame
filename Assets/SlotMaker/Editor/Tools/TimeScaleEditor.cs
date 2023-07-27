using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SlotMaker
{
    public class TimeScaleEditor : EditorWindowBase<TimeScaleEditor>
    {
        private bool initialized = false;
        private bool isPlaying = false;
        private float timeScale = 1f;

        public override string GetEditorName()
        {
            return "Time Scale";
        }

        [MenuItem("SlotMaker/Tools/Time Scale", false, 204)]
        private static void Initialize()
        {
            CreateWindow();

            _editor.minSize = new Vector2(200f, 50f);
        }

        private void OnGUI()
        {
            if (!initialized)
                Reset();

            BeginCheck();
            timeScale = Time.timeScale;
            timeScale = Slider("Time Scale", timeScale, 0f, 16f);
            BeginHorizontal();
                if (Button("0x"))
                    timeScale = 0f;
                if (Button("0.5x"))
                    timeScale = 0.5f;
                if (Button("1x"))
                    timeScale = 1f;
                if (Button("2x"))
                    timeScale = 2f;
                if (Button("4x"))
                    timeScale = 4f;
                if (Button("8x"))
                    timeScale = 8f;
                if (Button("16x"))
                    timeScale = 16f;
            EndHorizontal();
            if (EndCheck() && EditorApplication.isPlaying)
            {
                Time.timeScale = timeScale;
            }

            if (!isPlaying && EditorApplication.isPlaying)
                Time.timeScale = timeScale;
            isPlaying = EditorApplication.isPlaying;
        }

        private void Reset()
        {
            initialized = true;
            isPlaying = EditorApplication.isPlaying;
        }
    }
}
