using UnityEngine;

namespace SlotMaker
{
    [CreateAssetMenu(fileName = "New PayLineEditorConfig", menuName = "SlotMaker/ScriptableObject/PayLineConfig/PayLineEditorConfig")]
    public class PayLineEditorConfig : BasePayLineEditorConfig
    {
        public float symbolWidth;
        public float symbolHeight;
    }
}
