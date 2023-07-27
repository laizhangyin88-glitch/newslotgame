using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker
{
    [CreateAssetMenu(fileName="New Audio Sources", menuName="SlotMaker2/Audio/Audio Sources")]
    public class AudioSources : ScriptableObject
    {
        [TableList(IsReadOnly = false, DrawScrollView = true, ShowPaging = false)]
        public List<GSHandler> sheet;

        public string[] GetNames()
        {
            return sheet.Select(handler => handler.handlerId).ToArray();
        }
    }
}