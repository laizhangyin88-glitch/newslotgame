using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SlotMaker.IoC;
using SlotMaker.Slots.Strategy;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
    public class Slot2D : SlotInstance
    {
        [TabGroup("SlotInstance", "Setup")]
        [InlineEditor]
        public CommandSet commandAsset;

        private Dictionary<string, SelectorableCommand> commands = new Dictionary<string, SelectorableCommand>();
        [ShowIf("debug")]
        [ShowInInspector]
        [PropertyOrder(1100)]
        private CommandQueue commandQueue = new CommandQueue();

        protected override void Awake()
        {
            base.Awake();
            
            commands.Clear();
            foreach (var namedCommand in commandAsset.commandSet)
            {
                SelectorableCommand selectorableCommand = new SelectorableCommand();
                selectorableCommand.selector = namedCommand.selector;
                foreach (var commandList in namedCommand.commandList)
                {
                    selectorableCommand.commandList.Add(commandList.Create(this));
                }
                commands[namedCommand.name] = selectorableCommand;
            }
        }

        public override void SendEvent(string eventName)
        {
            AddCommand(eventName);
        }

        public void AddCommand(string command)
        {
            SelectorableCommand selectorableCommand = null;
            if (commands.TryGetValue(command, out selectorableCommand))
                commandQueue.Enqueue(selectorableCommand.Get());
        }

        public void AddCommand(List<Command> commandList)
        {
            commandQueue.Enqueue(commandList);
        }

        public override void Skip()
        {
            while (commandQueue.Count > 0)
            {
                var status = commandQueue.Skip();
                if (status == Command.Status.Resting || status == Command.Status.Running)
                    break;

                commandQueue.Dequeue();
            }
        }

        private void ExecuteCommands()
        {
            while (commandQueue.Count > 0)
            {
                var status = commandQueue.Execute(deltaTime);
                if (status == Command.Status.Resting || status == Command.Status.Running)
                    break;

                commandQueue.Dequeue();
            }
        }

        private void LateUpdate()
        {
            ExecuteCommands();
        }
    }
}