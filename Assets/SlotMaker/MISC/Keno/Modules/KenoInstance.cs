using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SlotMaker.Keno.Events;
using Sirenix.OdinInspector;

namespace SlotMaker.Keno
{
    [Serializable]
    public enum KenoState
    {
        Ready = 0,
        Play = 1,
        Wait = 2,
    };

    public class KenoInstance : MonoBehaviour
    {
        public int column { get { return ticket.column; } }
        public int row { get { return ticket.row; } }
        public int minPickCount { get { return ticket.minPickCount; } }
        public int maxPickCount { get { return ticket.maxPickCount; } }
        public int pickCount { get { return ticket.pickCount; } }
        public int hitCount { get { return ticket.hitCount; } }

        public TicketInstance ticket;
        public BallGeneratorInstance ballGenerator;

        [InlineEditor]
        public KenoMediator mediator;

        private KenoState _kenoState = KenoState.Ready;
        public KenoState kenoState
        {
            get { return _kenoState; }
            set
            {
                if (_kenoState != value)
                {
                    _kenoState = value;
                    switch (_kenoState)
                    {
                        case KenoState.Ready:
                            OnReady();
                            break;
                        case KenoState.Play:
                            OnPlay();
                            break;
                        case KenoState.Wait:
                            OnWait();
                            break;
                    }
                }
            }
        }

        public KenoEvent onReady;
        public KenoEvent onPlay;
        public KenoEvent onWait;

        public void OnReady()
        {
            onReady.Invoke(this, gameObject);
            if (mediator) mediator.OnReady(this);
        }

        public void OnPlay()
        {
            onPlay.Invoke(this, gameObject);
            if (mediator) mediator.OnPlay(this);
        }

        public void OnWait()
        {
            onWait.Invoke(this, gameObject);
            if (mediator) mediator.OnWait(this);
        }

        public bool IsDrawnAll { get { return ballGenerator.IsDrawnAll; } }

        protected virtual void Awake()
        {
            mediator.KenoInstance = this;
        }

        protected virtual void OnDestroy()
        {
            mediator.KenoInstance = null;
        }

        public virtual void Initialize()
        {
            ticket.Initialize();
            ballGenerator.Initialize();
        }

        public void Play(List<int> numbers)
        {
            ballGenerator.ChargeNumbers(numbers);
            ballGenerator.Draw();
        }

        public void Stop()
        {
            ballGenerator.Stop();
        }

        public void Win(KenoWin win)
        {
            ticket.Win(win.spots);
        }

        public void SkipWin()
        {
            ticket.SkipWin();
        }

        public SpotInstance GetSpot(int index)
        {
            return ticket.GetSpot(index);
        }

        public void BroadCastSpotEvent(string eventName)
        {
            ticket.BroadCastSpotEvent(eventName);
        }

        public void SendSpotEvent(int index, string eventName)
        {
            ticket.SendSpotEvent(index, eventName);
        }

        public KenoInstance Save()
    	{
    		var go = new GameObject();
    		go.name = "Keno Instance";
    		var snapshot = go.AddComponent<KenoInstance>();

    		var to = new GameObject();
            to.name = "Ticket Board";
            to.transform.SetParent(go.transform, false);

            snapshot.ticket = ticket.Save();
            // int count = reels.Count;
    		// for (int i = 0; i < count; ++i)
    		// {
    		// 	snapshot.CreateReel(reels[i]).transform.SetParent(ro.transform, false);
    		// }

    		return snapshot;
    	}
    }
}
