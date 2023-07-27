using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    public class CascadeSymbolMovement : ReelMovement
    {
        protected const string STOP_EFFECT_ANIMATION_NAME = "Stop Effect";
        protected const string ON_SLOT_DETAIL_EVENT = "OnSlotDetailEvent";
        protected const string PREPARE_STOPPED_SPECIAL_SYMBOL = "PrepareStoppedSpecialSymbol";
        protected const string PREPARE_STOPPED_SYMBOL = "PrepareStoppedSymbol";

        public BaseSymbol symbol;
        public Rigidbody _rigidbody;
        public float gravityScale;
        public float sleepThreshold;
        public float timeToSleep;

        private float sleepTime;
        private int spinCount;
        private Vector3 targetPosition;

        public override float GetVelocity() { return _rigidbody.velocity.magnitude; }

        public void UpdateSymbolPosition()
        {
            targetPosition = symbol.reel.CalcSymbolPosition(symbol);
            symbol.rectTransform.anchoredPosition3D = new Vector3(targetPosition.x, symbol.rectTransform.anchoredPosition3D.y, targetPosition.z);
        }

        public bool CompleteMovement()
        {
            return targetPosition.y == symbol.rectTransform.anchoredPosition3D.y;
        }

        public override void Spin()
        {
            spinState = SpinState.Spinning;
            ++spinCount;
            
            symbol.reel.movement.OnSpinSymbol();
        }

        public override void Stop()
        {
            spinState = SpinState.Stopping;
            _rigidbody.isKinematic = false;
        }

        public override void ForceStop()
        {
            if (!IsPrepareStopped())
                OnPrepareStopped();

            spinState = SpinState.Stopped;
            symbol.rectTransform.anchoredPosition3D = targetPosition;
            sleepTime = 0f;
            _rigidbody.isKinematic = true;

            symbol.reel.movement.OnStoppedSymbol(symbol.row);
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (collision.relativeVelocity.y != 0f && (IsSpinning() || IsStopping()))
            {
                OnPrepareStopped();
            }
        }

        private void OnPrepareStopped()
        {
            spinState = SpinState.PrepareStopped;
            
            if (spinCount == 1)
            {
                var spots = ContentCustomData.GetSlotData(symbol.slotMachine.slotIndex).expectation.expectationSpots[symbol.column];
                foreach (var cell in spots)
                {
                    if (cell.row == symbol.row)
                    {
                        symbol.Play(STOP_EFFECT_ANIMATION_NAME);
                        MessageDispatcher.Dispatch(ON_SLOT_DETAIL_EVENT, new ParadoxNotion.EventData<BaseSymbol>(PREPARE_STOPPED_SPECIAL_SYMBOL, symbol.slotMachine.slotIndex, symbol));
                        break;
                    }
                }
            }

            MessageDispatcher.Dispatch(ON_SLOT_DETAIL_EVENT, new ParadoxNotion.EventData<BaseSymbol>(PREPARE_STOPPED_SYMBOL, symbol.slotMachine.slotIndex, symbol));

            symbol.reel.movement.OnPrepareStoppedSymbol(symbol.row);
        }

        public void Clear(BaseSymbol symbol)
        {
            spinCount = 0;
        }

        public override void Play(int actionIndex) {}
        public override void Play(string actionName) {}

        void FixedUpdate()
        {
            if (!_rigidbody.isKinematic)
            {
                _rigidbody.AddForce(Physics.gravity * gravityScale, ForceMode.Acceleration);

                if (_rigidbody.velocity.sqrMagnitude < sleepThreshold)
                    sleepTime += Time.fixedDeltaTime;
                else 
                    sleepTime = 0f;
                    
                if (sleepTime > timeToSleep)
                    ForceStop();
            }
        }
    }
}
