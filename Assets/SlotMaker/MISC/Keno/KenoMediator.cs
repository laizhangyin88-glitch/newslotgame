using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace SlotMaker.Keno
{
    [CreateAssetMenu(fileName = "New Keno Mediator", menuName = "SlotMaker/KenoMediator")]
    public class KenoMediator : ScriptableObject
    {
        [ShowInInspector]
        protected KenoInstance kenoInstance;
        public KenoInstance KenoInstance
        {
            get { return kenoInstance; }
            set { kenoInstance = value; }
        }

        [ShowInInspector]
        public KenoState kenoState
        {
            get { return kenoInstance != null ? kenoInstance.kenoState : KenoState.Ready; }
            set { if (kenoInstance) KenoInstance.kenoState = value; }
        }

        public int maxPickCount { get { return kenoInstance != null ? kenoInstance.maxPickCount : 0; } }
 
        public int minPickCount { get { return kenoInstance != null ? kenoInstance.minPickCount : 0; } }
 
        public int pickCount { get { return kenoInstance != null ? kenoInstance.pickCount : 0; } }

        public int hitCount { get { return kenoInstance != null ? kenoInstance.hitCount : 0; } }

        public int row { get { return kenoInstance != null ? kenoInstance.row : 0; } }

        public int column { get { return kenoInstance != null ? kenoInstance.column : 0; } }

        public void Initialize()
        {
            if (kenoInstance) kenoInstance.Initialize();
        }

        public void Play(List<int> numbers)
        {
            if (kenoInstance) kenoInstance.Play(numbers);
        }

        public void Stop()
        {
            if (kenoInstance) kenoInstance.Stop();
        }

        public void Win(KenoWin win)
        {
            if (kenoInstance) kenoInstance.Win(win);
        }

        public void SkipWin()
        {
            if (kenoInstance) kenoInstance.SkipWin();
        }

        public SpotInstance GetSpot(int index)
        {
            if (!kenoInstance) return null;
            return kenoInstance.GetSpot(index);
        }

        public void BroadCastSpotEvent(string eventName)
        {
            if (kenoInstance) kenoInstance.BroadCastSpotEvent(eventName);
        }

        ////////////////////////////////////////////////////////////////////////////
        /// Keno Events
        ////////////////////////////////////////////////////////////////////////////
        public event Action<KenoInstance> onReady;
        public event Action<KenoInstance> onPlay;
        public event Action<KenoInstance> onStop;
        public event Action<KenoInstance> onWait;

        public void OnReady(KenoInstance kenoInstance)
        {
            if (onReady != null) onReady(kenoInstance);
        }

        public void OnPlay(KenoInstance kenoInstance)
        {
            if (onPlay != null) onPlay(kenoInstance);
        }

        public void OnStop(KenoInstance kenoInstance)
        {
            if (onStop != null) onStop(kenoInstance);
        }

        public void OnWait(KenoInstance kenoInstance)
        {
            if (onWait != null) onWait(kenoInstance);
        }

        ////////////////////////////////////////////////////////////////////////////
        /// Spot Events
        ////////////////////////////////////////////////////////////////////////////
        public event Action<SpotInstance> onPick;
        public event Action<SpotInstance> onUnpick;
        public event Action<SpotInstance> onPickFail;
        public event Action<SpotInstance> onCatch;
        public event Action<SpotInstance> onRelease;

        public void OnPick(SpotInstance spot)
        {
            if (onPick != null) onPick(spot);
        }

        public void OnUnpick(SpotInstance spot)
        {
            if (onUnpick != null) onUnpick(spot);
        }

        public void OnPickFail(SpotInstance spot)
        {
            if (onPickFail != null) onPickFail(spot);
        }

        public void OnCatch(SpotInstance spot)
        {
            if (onCatch != null) onCatch(spot);
        }

        public void OnRelease(SpotInstance spot)
        {
            if (onRelease != null) onRelease(spot);
        }

        ////////////////////////////////////////////////////////////////////////////
        /// Ball Events 
        ////////////////////////////////////////////////////////////////////////////
        public event Action<BallInstance> onDrawn;
        public event Action<BallInstance> onDrawing;
        public event Action<BallInstance> onCatchBall;
        public event Action<BallInstance> onReleaseBall;

        public void OnDrawn(BallInstance ball)
        {
            if (onDrawn != null) onDrawn(ball);
        }

        public void OnDrawing(BallInstance ball)
        {
            if (onDrawing != null) onDrawing(ball);
        }

        public void OnCatchBall(BallInstance ball)
        {
            if (onCatchBall != null) onCatchBall(ball);
        }

        public void OnReleaseBall(BallInstance ball)
        {
            if (onReleaseBall != null) onReleaseBall(ball);
        }
    }
}
