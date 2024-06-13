using NodeCanvas;
using NodeCanvas.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SlotMaker
{
    public class VerticalReelMovement : ReelMovement
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
        public bool lockedOutOfBound;
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

        // TargetIndex is index of reel.symbols[0] when reel is completely stopped.
        // TODO: Replace targetIndex to nextIndex
        private int GetTargetIndex()
        {
            int totalRow = ContentCustomData.GetSlotData(reel.slotMachine.slotIndex).row;
            return reel.strip.CalcIndex((reel.nextIndex + reel.beginRow - reel.topBuffer) - totalRow);
        }

        public void UpdateBackStopDisplacement()
        {
            int targetIndex = GetTargetIndex();

            var backSymbol = reel.GetSymbols()[reel.symbols.Count - 1];
            var srcSymbolInfo = backSymbol.symbolInfo;

            int dstIndex = reel.strip.CalcIndex(targetIndex + reel.topBuffer);
            var dstSymbolInfo = SlotUtils.GetSymbol(reel.slotMachine.slotIndex, reel.reelIndex, reel.strip, dstIndex);
            int dstPatchCount = dstSymbolInfo.link.rowCount - (dstSymbolInfo.link.rowOffset + 1);
            dstIndex = reel.strip.CalcIndex(dstIndex - dstPatchCount);
            dstSymbolInfo = SlotUtils.GetSymbol(reel.slotMachine.slotIndex, reel.reelIndex, reel.strip, dstIndex);

            if (blankSolver && (SymbolMask.HasBlank(srcSymbolInfo) == SymbolMask.HasBlank(dstSymbolInfo)))
            {
                dstPatchCount += 1;
                dstIndex = reel.strip.CalcIndex(dstIndex - 1);
            }

            displacement = Vector3.zero;
            UpdateDisplacement(reel.RowCount, -(backSymbol.rectTransform.anchoredPosition.y + reel.offset.y - ((float)reel.expandTopCount * reel.offset.w)));
            UpdateDisplacement(dstPatchCount, 0);
            reel.backPatch.SetPatch(reel.backIndex, dstIndex);
        }

        public void UpdateFrontStopDisplacement()
        {
            int targetIndex = GetTargetIndex();

            var frontSymbol = reel.GetSymbols()[0];
            int srcPatchCount = (frontSymbol.symbolInfo.link.rowCount - frontSymbol.symbolInfo.link.rowOffset) - 1;
            int srcIndex = reel.strip.CalcIndex(reel.frontIndex - srcPatchCount);
            var srcSymbolInfo = SlotUtils.GetSymbol(reel.slotMachine.slotIndex, reel.reelIndex, reel.strip, srcIndex);

            int dstIndex = reel.strip.CalcIndex(targetIndex + reel.RowCount + (reel.topBuffer - 1));
            var dstSymbolInfo = SlotUtils.GetSymbol(reel.slotMachine.slotIndex, reel.reelIndex, reel.strip, dstIndex);
            int dstPatchCount = dstSymbolInfo.link.rowOffset;
            dstIndex = reel.strip.CalcIndex(dstIndex + dstPatchCount);
            dstSymbolInfo = SlotUtils.GetSymbol(reel.slotMachine.slotIndex, reel.reelIndex, reel.strip, dstIndex);

            if (blankSolver && (SymbolMask.HasBlank(srcSymbolInfo) == SymbolMask.HasBlank(dstSymbolInfo)))
            {
                dstPatchCount += 1;
                dstIndex = reel.strip.CalcIndex(dstIndex + 1);
            }

            displacement = Vector3.zero;
            UpdateDisplacement(-reel.RowCount, reel.offset.y - frontSymbol.rectTransform.anchoredPosition.y);
            UpdateDisplacement(-(srcPatchCount + dstPatchCount), 0);
            reel.frontPatch.SetPatch(srcIndex, dstIndex);
        }

        public void UpdateDisplacement(int symbolCount, float offset)
        {
            float symbolHeight = reel.cellSize.y + reel.spacing.y;
            displacement.y += symbolHeight * symbolCount + offset;
        }

        public void ForceUpdateDisplacement(int symbolCount, float offset)
        {
            displacement = Vector3.zero;
            UpdateDisplacement(symbolCount, offset);
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

            // execute action
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

            // change transform
            if (!IsStopped())
            {
                velocity += acceleration * Time.fixedDeltaTime;
                movement = velocity * Time.fixedDeltaTime;
                displacement -= movement;

                reel.TranslateSymbols(movement);
                if (!lockedOutOfBound)
                    reel.UpdateSymbolsBounds();
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
