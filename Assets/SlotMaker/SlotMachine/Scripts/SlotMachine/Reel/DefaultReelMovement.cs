using NodeCanvas;
using NodeCanvas.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SlotMaker
{
    public class DefaultReelMovement : ReelMovement
    {
        public List<ActionListPlayerCollection> actions;

        private List<ActionListPlayer> pendingActions = new List<ActionListPlayer>();
        private ActionListPlayer stopAction;

        public Vector3 acceleration;
        public Vector3 velocity;
        public Vector3 displacement;

        public Vector3 position
        { get { return -displacement; } }

        public Vector3 forceStopVelocity = new Vector3(0f, -3500f, 0f);
        public bool locked;
        public bool lockedOutOfBound;
        public bool freeMove;
        public int srcPatchCount;
        public int dstPatchCount;
        public bool blankSolver;

        [Serializable]
        public class ReelEvent : UnityEvent<BaseReel>
        { }

        public ReelEvent onPrepareStopped;
        public ReelEvent onTargetPosition;
        public ReelEvent onStopped;

        private Vector3 movement;
        private float updateTime;

        private Reel _reel = null;

        protected Reel reel
        { get { return _reel ?? (_reel = GetComponent<Reel>()); } }

        public override float GetVelocity()
        {
            return velocity.y;
        }

        public override void Spin()
        {
            PlayAction(GetAction(0));
        }

        public override void Stop()
        {
            stopAction = GetAction(1);
        }

        public override void ForceStop()
        {
            if (IsPrepareStopped() || IsStopped())
                return;

            ClearActions();

            if (pendingActions.Count == 0)
                velocity = forceStopVelocity;

            if (stopAction == null)
                Stop();
        }

        public override void Play(int actionIndex)
        {
            PlayAction(GetAction(actionIndex));
        }

        public override void Play(string actionName)
        {
            PlayAction(FindAction(actionName));
        }

        public void OnSpin()
        {
            reel.slotMachine.movement.OnSpinReel();
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

        private ActionListPlayer GetAction(int actionIndex)
        {
            return actions[reel.slotMachine.movement.movementType].GetActionListPlayer(actionIndex);
        }

        private ActionListPlayer FindAction(string actionName)
        {
            return actions[reel.slotMachine.movement.movementType].FindActionListPlayer(actionName);
        }

        private void ClearActions()
        {
            int count = pendingActions.Count;
            int actionIndex = 0;
            for (int i = 0; i < count; ++i)
            {
                pendingActions[actionIndex].Skip();
                if (pendingActions[actionIndex].actionList.isRunning)
                    ++actionIndex;
                else
                    pendingActions.RemoveAt(actionIndex);
            }
        }

        private void PlayAction(ActionListPlayer action)
        {
            pendingActions.Add(action);
        }

        public void UpdateStopDisplacement()
        {
            displacement = Vector3.zero;
            var frontSymbol = reel.GetSymbols()[0];
            float symbolHeight = reel.cellSize.y + reel.spacing.y;
            float reelHeight = symbolHeight * reel.RowCount;

            srcPatchCount = (frontSymbol.symbolInfo.link.rowCount - frontSymbol.symbolInfo.link.rowOffset) - 1;

            int dstIndex = reel.strip.CalcIndex(reel.nextIndex - 1);
            var dstSymbol = SlotUtils.GetSymbol(reel.slotMachine.slotIndex, reel.reelIndex, reel.strip, dstIndex);
            dstPatchCount = dstSymbol.link.rowOffset;

            if (blankSolver)
            {
                int srcIndex = reel.strip.CalcIndex(reel.index - srcPatchCount);
                var srcSymbol = SlotUtils.GetSymbol(reel.slotMachine.slotIndex, reel.reelIndex, reel.strip, srcIndex);
                bool srcBlank = SymbolMask.HasBlank(srcSymbol);

                if (dstPatchCount > 0)
                {
                    dstIndex = reel.strip.CalcIndex(dstIndex + dstPatchCount);
                    dstSymbol = SlotUtils.GetSymbol(reel.slotMachine.slotIndex, reel.reelIndex, reel.strip, dstIndex);
                }

                bool dstBlank = SymbolMask.HasBlank(dstSymbol);
                if (srcBlank == dstBlank)
                    dstPatchCount += 1;
            }

            displacement.y += -reelHeight;
            displacement.y += reel.offset.y - frontSymbol.rectTransform.anchoredPosition.y;
            displacement.y += -symbolHeight * srcPatchCount;
            displacement.y += -symbolHeight * dstPatchCount;
        }

        public void UpdateManualDisplacement(int count)
        {
            displacement = Vector3.zero;
            float symbolHeight = reel.cellSize.y + reel.spacing.y;
            displacement.y = symbolHeight * count;
        }

        private int fixedCount;

        private void FixedUpdate()
        {
            if (++fixedCount > 2)
            {
                updateTime = Time.time;
                acceleration = Vector3.zero;
                return;
            }

            if (pendingActions.Count > 0)
            {
                if (pendingActions[0].ExecuteAction() != Status.Running)
                    pendingActions.RemoveAt(0);
            }
            else if (stopAction != null)
            {
                if (stopAction.ExecuteAction() != Status.Running)
                    stopAction = null;
            }

            if (!IsStopped())
            {
                velocity += acceleration * Time.fixedDeltaTime;
                movement = velocity * Time.fixedDeltaTime;
                displacement -= movement;

                reel.TranslateSymbols(movement);

                if (!lockedOutOfBound && reel.IsOutOfBound())
                {
                    do
                    {
                        if (locked)
                        {
                            if (srcPatchCount > 0)
                            {
                                --srcPatchCount;
                            }
                            else if (reel.nextIndex >= 0)
                            {
                                reel.nextIndex += dstPatchCount;
                                dstPatchCount = 0;
                                reel.SwapIndex();
                                reel.isFinish = true;
                            }
                        }

                        reel.PushFrontSymbol();
                    } while (reel.IsOutOfBound());

                    reel.UpdateSymbolsZOrder();
                    reel.RemoveOutBoundSymbols();
                }
            }
            else if (freeMove)
            {
                velocity += acceleration * Time.fixedDeltaTime;
                movement = velocity * Time.fixedDeltaTime;
                displacement -= movement;

                reel.TranslateSymbols(movement);
            }
            else
            {
                movement = Vector3.zero;
            }

            updateTime = Time.time;
            acceleration = Vector3.zero;
        }

        private void Update()
        {
            fixedCount = 0;
            float dt = (updateTime - (Time.time - Time.fixedDeltaTime)) / Time.fixedDeltaTime;
            reel.symbolsTransform.anchoredPosition3D = -movement * dt;
        }
    }
}
