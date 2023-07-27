using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
    [CreateAssetMenu(fileName = "New Slot Control", menuName = "SlotMaker2/Slot/Slot Control")]
    public class SlotMediator : ScriptableObject
    {
        [ShowInInspector]
        protected SlotInstance instance;
        public SlotInstance Instance
        {
            get { return instance; }
            set { instance = value; }
        }

        public event Action<SlotInstance> onSpinning;
        public event Action<SlotInstance> onStopping;
        public event Action<SlotInstance> onStopped;

        public event Action<ReelInstance> onSpinningReel;
        public event Action<ReelInstance> onStoppingReel;
        public event Action<ReelInstance> onPrepareStoppedReel;
        public event Action<ReelInstance> onStoppedReel;

        [ShowInInspector]
        public SpinState spinState
        {
            get { return instance != null ? instance.spinState : SpinState.Stopped; }
            set { if (instance) instance.spinState = value; }
        }

        [ShowInInspector]
        public int columnCount { get { return instance != null ? instance.columnCount : 0; } }
        [ShowInInspector]
        public int rowCount { get { return instance != null ? instance.rowCount : 0; } }
        [ShowInInspector]
        public int layerCount { get { return instance != null ? instance.layerCount : 0; } }

        public void Initialize()
        {
            if (instance) instance.Initialize();
        }

        public void SendEvent(string eventName)
        {
            if (instance) instance.SendEvent(eventName);
        }

        public void SendSymbolEvent(string eventName, int layer)
        {
            if (instance) instance.SendSymbolEvent(eventName, layer);
        }

        public void Skip()
        {
            if (instance) instance.Skip();
        }

        public Vector3 GetSlotPosition()
        {
            if (!instance) return Vector3.zero;
            return instance.transform.position;
        }

        public void ClearLayer(int layer)
        {
            if (instance) instance.Clear(layer);
        }

        public int GetReelIndex(ReelInstance reelInstance)
        {
            if (!instance) return -1;
            return instance.GetReelIndex(reelInstance);
        }

        public ReelInstance GetReelInstanceByIndex(int index, int layer)
        {
            if (!instance) return null;
            return instance.GetReelByIndex(index, layer);
        }

        public Vector3 GetReelPositionByIndex(int index, int layer)
        {
            if (!instance) return Vector3.zero;
            return instance.GetReelPositionByIndex(index, layer);
        }

        public ReelInstance GetReelInstance(int x, int y, int z)
        {
            if (!instance) return null;
            return instance.GetReel(x, y, z);
        }

        public Vector3 GetReelPosition(int x, int y, int z)
        {
            if (!instance) return Vector3.zero;
            return instance.GetReelPosition(x, y, z);
        }

        public SymbolInstance GetSymbolInstance(int x, int y, int z)
        {
            if (!instance) return null;
            return instance.GetSymbol(x, y, z);
        }

        public void SetSymbol(int x, int y, int z, SymbolInstance symbolInstance)
        {
            if (instance) instance.SetSymbol(x, y, z, symbolInstance);
        }

        public void AddSymbol(int x, int y, int z, SymbolInstance symbolInstance)
        {
            if (instance) instance.AddSymbol(x, y, z, symbolInstance);
        }

        public void MoveSymbolLayer(int x, int y, int z, int targetLayer)
        {
            if (instance) instance.MoveSymbolLayer(x, y, z, targetLayer);
        }

        public Vector3 GetSymbolPosition(int x, int y, int z)
        {
            if (!instance) return Vector3.zero;
            return instance.GetSymbolPosition(x, y, z);
        }

        ////////////////////////////////////////////////////////////////////////////
        /// SlotInstance Events
        ////////////////////////////////////////////////////////////////////////////
        public void OnSpinning(SlotInstance slotInstance)
        {
            if (onSpinning != null) onSpinning(slotInstance);
        }

        public void OnStopping(SlotInstance slotInstance)
        {
            if (onStopping != null) onStopping(slotInstance);
        }

        public void OnStopped(SlotInstance slotInstance)
        {
            if (onStopped != null) onStopped(slotInstance);
        }

        ////////////////////////////////////////////////////////////////////////////
        /// ReelInstance Events
        ////////////////////////////////////////////////////////////////////////////
        public void OnSpinningReel(ReelInstance reelInstance)
        {
            if (onSpinningReel != null) onSpinningReel(reelInstance);
        }

        public void OnStoppingReel(ReelInstance reelInstance)
        {
            if (onStoppingReel != null) onStoppingReel(reelInstance);
        }

        public void OnPrepareStoppedReel(ReelInstance reelInstance)
        {
            if (onPrepareStoppedReel != null) onPrepareStoppedReel(reelInstance);
        }

        public void OnStoppedReel(ReelInstance reelInstance)
        {
            if (onStoppedReel != null) onStoppedReel(reelInstance);
        }
    }
}