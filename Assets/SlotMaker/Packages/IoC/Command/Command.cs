using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.IoC
{
    [Serializable]
    public class Command
    {
        [SerializeField]
        protected Component _agent;
        public Component agent { get { return _agent; } }
        public virtual Type agentType { get { return null; } }

        [SerializeField]
        [InlineEditor]
        protected CommandAsset _sharedCommand;
        public CommandAsset sharedCommand { get { return _sharedCommand; } set { _sharedCommand = value; } }

        public enum Status
        {
            Failure = 0,
            Success = 1,
            Running = 2,
            Resting = 3
        }
        [PropertyOrder(-100)]
        public Status status = Status.Resting;

        public void SetAgent(Component newAgent)
        {
            _agent = TransformAgent(newAgent, agentType);
            OnInit();
        }

        public void Reset()
        {
            status = Status.Resting;
        }

        public Status Execute(float deltaTime)
        {
            if (status == Status.Running)
            {
                OnUpdate(deltaTime);
                return status;
            }

            status = Status.Running;
            OnExecute(deltaTime);
            if (status == Status.Running)
                OnUpdate(deltaTime);
            return status;
        }

        public Status PostExecute(float deltaTime)
        {
            if (status == Status.Running)
                OnLateUpdate(deltaTime);
            return status;
        }

        public Status Skip()
        {
            OnSkip();
            return status;
        }

        public void EndCommand() { EndCommand(true); }
        public void EndCommand(bool success)
        {
            status = success ? Status.Success : Status.Failure;
            OnStop();
        }

        protected virtual void OnInit() {}
        protected virtual void OnExecute(float deltaTime) {}
        protected virtual void OnUpdate(float deltaTime) {}
        protected virtual void OnLateUpdate(float deltaTime) {}
        protected virtual void OnStop() {}
        protected virtual void OnSkip() 
        { 
            if (sharedCommand.skippable)
                EndCommand(); 
        }

        static Component TransformAgent(Component input, Type type)
        {
            if (input != null && type != null && !type.IsAssignableFrom(input.GetType()))
            {
                if (type.IsSubclassOf(typeof(Component)) || type.IsInterface)
                {
                    input = input.GetComponent(type);
                }
            }
            return input;
        }
    }

    [Serializable]
    public class Command<T, U> : Command 
        where T : class
        where U : CommandAsset
    {
        new public T agent { get { return _agent as T; } }
        public override Type agentType { get { return typeof(T); } }
        new public U sharedCommand { get { return (U)_sharedCommand; } set { _sharedCommand = value; } }
    }

    public class SelectorableCommand
    {
        public VariableInt selector;
        public List<List<Command>> commandList = new List<List<Command>>();

        public List<Command> Get()
        {
            return commandList[Mathf.Clamp(selector.value, 0, commandList.Count - 1)];
        }
    }

    [Serializable]
    public class CommandQueue
    {
        public List<Command> commands = new List<Command>();

        public int Count { get { return commands.Count; } }

        public Command.Status Execute(float deltaTime)
        {
            return commands[0].Execute(deltaTime);
        }

        public Command.Status PostExecute(float deltaTime)
        {
            return commands[0].PostExecute(deltaTime);
        }

        public Command.Status Skip()
        {
            return commands[0].Skip();
        }
        
        public void Dequeue()
        {
            commands[0].Reset();
            commands.RemoveAt(0);
        }

        public void Enqueue(List<Command> commandList)
        {
            commands.AddRange(commandList);
        }

        public void AddFirst(List<Command> commandList)
        {
            commands.InsertRange(0, commandList);
        }
    }
}