using System.Collections.Generic;
using UnityEngine;

namespace BagelCode.HiddenObjects
{
    // 현재 프로젝트에 포함된 챕터 번들 목록
    [CreateAssetMenu(fileName = "HiddenObjectsChapterData", menuName = "Meta/ScriptableObject/HiddenObjectsChapterData")]
    public class HiddenObjectsChapterData : ScriptableObject
    {
        public List<string> chapterSymbolList;
    }
}
