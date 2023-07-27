using UnityEngine;
using System.Collections;

namespace SlotMaker
{
    public class UnityShaderBugFix : MonoBehaviour
    {
        private void Awake()
        {
            Shader.EnableKeyword("UNITY_UI_CLIP_RECT");
            Destroy(this);
        }
    }
}