using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.IoC
{
    [CreateAssetMenu(fileName="New Command Set", menuName="SlotMaker2/Slot/Command/Command Set")]
    public class CommandSet : ScriptableObject
    {
        [Serializable]
        public class NamedCommand
        {
            public string name;
            [InlineEditor]
            public VariableInt selector;
            public List<CommandList> commandList;
        }
        public List<NamedCommand> commandSet;
    }
}