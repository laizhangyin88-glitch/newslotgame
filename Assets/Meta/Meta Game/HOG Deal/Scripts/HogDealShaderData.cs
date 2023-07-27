using System.Collections.Generic;
using UnityEngine;

namespace BagelCode.HiddenObjects
{
    [System.Serializable]
    public class ChapterStageMaterial
    {
        public string chapterSymbol;
        public int stageNumber;
        public Material targetMaterial;
        public Component attachComponent;
    }

    // 각 스테이지의 타겟 매터리얼
    [CreateAssetMenu(fileName = "HogDealShaderData", menuName = "Meta/ScriptableObject/HogDealShaderData")]
    public class HogDealShaderData : ScriptableObject
    {
        public Material defaultMaterial;
        public List<ChapterStageMaterial> materialList;
    }
}
