using UnityEngine;

namespace SlotMaker
{
    public enum SpinState
    {
        Spinning,
        Stopping,
        PrepareStopped,
        Stopped
    };

    public abstract class ReelMovement : MonoBehaviour, IReelMovement
    {
        public SpinState spinState = SpinState.Stopped;

        public bool IsSpinning()
        {
            return spinState == SpinState.Spinning;
        }

        public bool IsStopping()
        {
            return spinState == SpinState.Stopping;
        }

        public bool IsPrepareStopped()
        {
            return spinState == SpinState.PrepareStopped;
        }

        public bool IsStopped()
        {
            return spinState == SpinState.Stopped;
        }

        public abstract float GetVelocity();

        public abstract void Spin();

        public abstract void Stop();

        public abstract void ForceStop();

        public abstract void Play(int actionIndex);

        public abstract void Play(string actionName);

        public virtual void OnSpinSymbol()
        { }

        public virtual void OnPrepareStoppedSymbol(int row)
        { }

        public virtual void OnStoppedSymbol(int row)
        { }
    }
}
