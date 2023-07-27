using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.IoC
{
    [CreateAssetMenu(fileName="New Command List", menuName="SlotMaker2/Slot/Command/Command List")]
    public class CommandList : VariableList<CommandAsset> 
    {
        public List<Command> Create(Component newAgent)
        {
            var list = new List<Command>();
            for (int i = 0, count = Count; i < count; ++i)
            {
                var newCommand = value[i].Create();
                newCommand.SetAgent(newAgent);
                list.Add(newCommand);
            }
            return list;
        }
    }
}