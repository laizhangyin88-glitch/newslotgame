using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    [CreateAssetMenu(fileName = "New MultiPayLineEditorConfig", menuName = "SlotMaker/ScriptableObject/PayLineConfig/MultiPayLineEditorConfig")]
    public class NonUniformPayLineEditorConfig : BasePayLineEditorConfig
    {
        [SerializeField] public List<ColumnPref> columnsPref;

        [System.Serializable]
        public class ColumnPref
        {
            public float Width;
            public float Height;
            public bool subPointsMask;
            public SubPointSize subPointsSize;
        }

        [System.Serializable]
        public class SubPointSize
        {
            public float Width;
            public float Height;
        }
    }

}
