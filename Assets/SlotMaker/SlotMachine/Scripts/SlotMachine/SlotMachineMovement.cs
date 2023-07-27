using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using ParadoxNotion;

namespace SlotMaker
{
    public class SlotMachineMovement : MonoBehaviour, ISlotMachineMovement
    {
        public SpinState spinState = SpinState.Stopped;
        public float spinTime;
        public int spinningCount { get; protected set; }
        public int movementType { get; set; }

        public UnityIntEvent onPrepareStoppedReel;
        public UnityIntEvent onStoppedReel;
    	public UnityEvent onStoppedSlotMachine;

        private BaseSlotMachine _slotMachine;
        private BaseSlotMachine slotMachine 
        {
            get 
            {
                if (_slotMachine == null)
                    _slotMachine = GetComponent<BaseSlotMachine>();
                return _slotMachine;
            }
        }

        private const string ON_SLOT_EVENT = "OnSlotEvent";
        private const string ON_SPIN_SLOT_EVENT = "SpinSlotMachine";
        private const string ON_RESPIN_SLOT_EVENT = "ReSpinSlotMachine";
        private const string ON_STOP_SLOT_EVENT = "StopSlotMachine";
        private const string ON_STOPPED_SLOT_EVENT = "StoppedSlotMachine";

        private void OnEnable()
        {
            MessageDispatcher.Register(ON_SLOT_EVENT, OnSlotEvent);
        }

        private void OnDisable()
        {
            MessageDispatcher.UnRegister(ON_SLOT_EVENT, OnSlotEvent);
        }

        private void OnSlotEvent(EventData eventData)
        {
            if (eventData.id != slotMachine.slotIndex) return;

            if (eventData.name.Equals(ON_SPIN_SLOT_EVENT, StringComparison.Ordinal))
            {
                spinTime = Time.time;
                spinState = SpinState.Spinning;
            }
            else if (eventData.name.Equals(ON_RESPIN_SLOT_EVENT, StringComparison.Ordinal))
            {
                spinTime = Time.time;
                spinState = SpinState.Spinning;
            }
            else if (eventData.name.Equals(ON_STOP_SLOT_EVENT, StringComparison.Ordinal))
            {
                spinState = SpinState.Stopping;
            }
        }

    	public bool IsSpinning()
    	{
    		return spinningCount > 0;
    	}

    	public bool IsStopped()
    	{
    		return spinningCount == 0;
    	}

    	public virtual void OnSpinReel()
    	{
    		++spinningCount;
    	}

        public virtual void OnPrepareStoppedReel(int reelIndex)
        {
            onPrepareStoppedReel.Invoke(reelIndex);
        }

    	public virtual void OnStoppedReel(int reelIndex)
    	{
    		--spinningCount;

            onStoppedReel.Invoke(reelIndex);

    		if (spinningCount == 0)
    			spinState = SpinState.Stopped;
    	}

        public void OnStoppedSlotMachine()
        {
            onStoppedSlotMachine.Invoke();
        }
    }
}
