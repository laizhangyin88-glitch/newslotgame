using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NodeCanvas;

namespace SlotMaker
{
    public class CascadeReelMovement : ReelMovement
    {
        public Reel reel;
        public List<float> stopDelays;
        public List<float> boostStopDelays;
        public bool enableForceSkip;
        private List<List<float>> stopDelaysList = new List<List<float>>();

        [Serializable]
        public class ReelEvent : UnityEvent<BaseReel> {}

        public ReelEvent onPrepareStopped;
    	public ReelEvent onStopped;
        public UnityIntEvent onStoppedSymbol;

        private int spinningCount;

        private int lastBeginRow;
        private bool skip;
        private Coroutine coroutine;

        public override float GetVelocity() { return 0f; }

        public int Floor { get; protected set; }

        private void Awake()
        {
            stopDelaysList.Add(stopDelays);
            stopDelaysList.Add(boostStopDelays);
        }

        public override void Spin() 
        {
            spinState = SpinState.Spinning;
            reel.slotMachine.movement.OnSpinReel();

            lastBeginRow = reel.beginRow;
            reel.SwapIndex();
            reel.PushFrontSymbols(lastBeginRow);

            Floor = reel.endRow;
            for (int i = 0; i < reel.endRow; ++i)
            {
                var symbol = reel.GetSymbol(reel.beginColumn, i);
                var symbolMovement = symbol.GetComponent<CascadeSymbolMovement>();
                symbolMovement.UpdateSymbolPosition();
                if (symbolMovement.CompleteMovement())
                    Floor = Mathf.Min(i, Floor);
                else 
                    symbolMovement.Spin();
            }

            skip = false;
        }

        public override void Stop() 
        {
            spinState = SpinState.Stopping;
            coroutine = StartCoroutine(StopCo());
        }
        private IEnumerator StopCo()
        {
            for (int i = lastBeginRow; i < reel.endRow; ++i)
            {
                var symbolMovement = reel.GetSymbol(reel.beginColumn, i).GetComponent<CascadeSymbolMovement>();
                if (symbolMovement.IsSpinning())
                    symbolMovement.Stop();
            }

            for (int i = 0; i < lastBeginRow; ++i)
            {
                reel.GetSymbol(reel.beginColumn, lastBeginRow - 1 - i).GetComponent<CascadeSymbolMovement>().Stop();

                float elapsedTime = 0f;
                while (!skip && elapsedTime < stopDelaysList[reel.slotMachine.movement.movementType][i])
                {
                    yield return new WaitForEndOfFrame();
                    elapsedTime += Time.deltaTime;
                }
            }
        }

        public override void ForceStop() 
        {
            if (enableForceSkip)
            {
                if (coroutine != null)
                    StopCoroutine(coroutine);

                for (int i = 0; i < reel.endRow; ++i)
                {
                    var symbolMovement = reel.GetSymbol(reel.beginColumn, i).GetComponent<CascadeSymbolMovement>();
                    if (!symbolMovement.IsStopped())
                        symbolMovement.ForceStop();
                }
            }
            skip = true;
        }

        public override void Play(int actionIndex) {}

        public override void Play(string actionName) {}

        public override void OnSpinSymbol() 
        {
            ++spinningCount;
        }

        public override void OnPrepareStoppedSymbol(int row)
        {
            if (row == (Floor - 1))
                OnPrepareStopped();
        }

        public override void OnStoppedSymbol(int row)
        {
            --spinningCount;

            onStoppedSymbol.Invoke(row);

            if (spinningCount == 0)
            {
                spinState = SpinState.Stopped;
                OnStopped();
            }
        }

        public void OnPrepareStopped()
        {
            onPrepareStopped.Invoke(reel);
            reel.slotMachine.movement.OnPrepareStoppedReel(reel.reelIndex);
        }

        public void OnStopped()
    	{
    		onStopped.Invoke(reel);
    		reel.slotMachine.movement.OnStoppedReel(reel.reelIndex);
    	}
    }
}
