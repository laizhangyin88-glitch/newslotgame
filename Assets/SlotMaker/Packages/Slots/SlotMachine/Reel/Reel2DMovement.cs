using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker.IoC;
using SlotMaker.Slots.Strategy;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
    public partial class Reel2D : ReelInstance, IBody
    {
        [TabGroup("ReelInstance", "Physics")]
        [PropertyOrder(201)]
        public float damping;
        [TabGroup("ReelInstance", "Physics")]
        [PropertyOrder(202)]
        public float drag;
        [TabGroup("ReelInstance", "Physics")]
        [PropertyOrder(203)]
        public float mass = 1f;
        [TabGroup("ReelInstance", "Physics")]
        [PropertyOrder(204)]
        public Vector3 velocity = Vector3.zero;
        [TabGroup("ReelInstance", "Physics")]
        [PropertyOrder(205)]
        public Vector3 position = Vector3.zero;
        [TabGroup("ReelInstance", "Physics")]
        [PropertyOrder(206)]
        public Vector3 desiredPosition = Vector3.zero;
        [TabGroup("ReelInstance", "Physics")]
        [PropertyOrder(207)]
        public Vector3 renderPosition = Vector3.zero;

        private Dictionary<string, SelectorableCommand> commands = new Dictionary<string, SelectorableCommand>();
        [ShowIf("debug")]
        [ShowInInspector]
        [PropertyOrder(1100)]
        private CommandQueue commandQueue = new CommandQueue();

        private void Awake()
        {
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

        private class InterpolationValue
        {
            public float oldTime;
            public Vector3 oldPosition;

            public float time;
            public Vector3 position;

            public void Reset()
            {
                oldTime = time = 0f;
                oldPosition = position = Vector3.zero;
            }

            public void Swap()
            {
                oldTime = time;
                oldPosition = position;
            }

            public Vector3 GetPosition()
            {
                float lerpValue = (Time.time - oldTime - Time.fixedDeltaTime) / Time.fixedDeltaTime;
                return Vector3.Lerp(oldPosition, position, Mathf.Clamp01(lerpValue));
            }
        }
        private InterpolationValue interpolation = new InterpolationValue();
        private Vector3 finalForce = Vector3.zero;

        // https://answers.unity.com/questions/802181/trying-to-understand-rigidbody-forcemode-derivatio.html
        public void AddForce(Vector3 force, ForceMode forceMode)
        {
            switch (forceMode)
            {
            case ForceMode.Force:
                finalForce += force * fixedDeltaTime / mass;
                break;
            case ForceMode.Acceleration:
                finalForce += force * fixedDeltaTime;
                break;
            case ForceMode.Impulse:
                finalForce += force / mass;
                break;
            case ForceMode.VelocityChange:
                finalForce += force;
                break;
            }
        }

        public Vector3 GetVelocity()
        {
            return this.velocity;
        }

        public void SetVelocity(Vector3 velocity)
        {
            this.velocity = velocity;
        }

        public float GetMass()
        {
            return 1f;
        }

        public override void SendEvent(string eventName)
        {
            EnqueueCommand(eventName);
        }

        public void EnqueueCommand(string command)
        {
            SelectorableCommand selectorableCommand = null;
            if (commands.TryGetValue(command, out selectorableCommand))
                commandQueue.Enqueue(selectorableCommand.Get());
        }

        public void EnqueueCommand(List<Command> commandList)
        {
            commandQueue.Enqueue(commandList);
        }

        public void AddFirstCommand(string command)
        {
            SelectorableCommand selectorableCommand = null;
            if (commands.TryGetValue(command, out selectorableCommand))
                commandQueue.AddFirst(selectorableCommand.Get());
        }

        public void AddFirstCommand(List<Command> commandList)
        {
            commandQueue.AddFirst(commandList);
        }

        [ShowIf("debug")]
        [Button]
        [ButtonGroup("MovementControl")]
        [PropertyOrder(1015)]
        public override void Skip()
        {
            while (commandQueue.Count > 0)
            {
                var status = commandQueue.Skip();
                if (status == Command.Status.Resting || status == Command.Status.Running)
                    break;

                commandQueue.Dequeue();
            }

            if ((int)_spinState < (int)SpinState.PrepareStopped)
                AddFirstCommand(SKIP);
        }

        private void ExecuteCommands()
        {
            while (commandQueue.Count > 0)
            {
                var status = commandQueue.Execute(fixedDeltaTime);
                if (status == Command.Status.Resting || status == Command.Status.Running)
                    break;

                commandQueue.Dequeue();
            }
        }

        private void PostExecuteCommands()
        {
            if (commandQueue.Count > 0)
            {
                var status = commandQueue.PostExecute(deltaTime);
                if (status == Command.Status.Resting || status == Command.Status.Running)
                    return;

                commandQueue.Dequeue();
                ExecuteCommands();
            }
        }

        private void FixedUpdate()
        {
            finalForce = Vector3.zero;
            ExecuteCommands();

            velocity += finalForce;
            velocity -= velocity * damping * fixedDeltaTime;
            var velocityMag = velocity.magnitude;
            var direction = velocity.normalized;
            velocity -= direction * Mathf.Min(drag * fixedDeltaTime, velocityMag);
            var movement = velocity * fixedDeltaTime;
            position += movement;

            PostExecuteCommands();

            interpolation.Swap();
            interpolation.time = Time.fixedTime;
            interpolation.position = position;
        }

        private void Update()
        {
            var newRenderPosition = interpolation.GetPosition();
            var movement = newRenderPosition - renderPosition;
            for (int i = 0, count = content.childCount; i < count; ++i)
            {
                var symbolTransform = content.GetChild(i) as RectTransform;
                var newPosition = symbolTransform.anchoredPosition;
                newPosition.y += movement.y;
                symbolTransform.anchoredPosition = newPosition;
            }
            renderPosition = newRenderPosition;

            UpdateBounds();
        }
    }
}
