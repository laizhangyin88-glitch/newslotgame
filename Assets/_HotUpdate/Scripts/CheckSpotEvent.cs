using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Keno.Tasks.Conditions
{
    public enum SpotEventType
    {
        Pick,
        Unpick,
        PickFail,
        Catch,
        Release,
    }

    [Category("★ Keno/Events")]
    public class CheckSpotEvent : ConditionTask<GraphOwner>
    {
        [RequiredField]
        public BBParameter<KenoMediator> mediator;
        public BBParameter<SpotEventType> spotEventType;

        protected virtual void OnSpotEvent(SpotInstance spot)
        {
            if (isActive)
                YieldReturn(true);
        }

        private void OnPickSpotEvent(SpotInstance spot)
        {
            if (spotEventType.value == SpotEventType.Pick)
                OnSpotEvent(spot);
        }

        private void OnUnpickSpotEvent(SpotInstance spot)
        {
            if (spotEventType.value == SpotEventType.Unpick)
                OnSpotEvent(spot);
        }

        private void OnPickFailSpotEvent(SpotInstance spot)
        {
            if (spotEventType.value == SpotEventType.PickFail)
                OnSpotEvent(spot);
        }

        private void OnCatchSpotEvent(SpotInstance spot)
        {
            if (spotEventType.value == SpotEventType.Catch)
                OnSpotEvent(spot);
        }

        private void OnReleaseSpotEvent(SpotInstance spot)
        {
            if (spotEventType.value == SpotEventType.Release)
                OnSpotEvent(spot);
        }

        protected override void OnEnable()
        {
            mediator.value.onPick += OnPickSpotEvent;
            mediator.value.onUnpick += OnUnpickSpotEvent;
            mediator.value.onPickFail += OnPickFailSpotEvent;
            mediator.value.onCatch += OnCatchSpotEvent;
            mediator.value.onRelease += OnReleaseSpotEvent;
        }

        protected override void OnDisable()
        {
            mediator.value.onPick -= OnPickSpotEvent;
            mediator.value.onUnpick -= OnUnpickSpotEvent;
            mediator.value.onPickFail -= OnPickFailSpotEvent;
            mediator.value.onCatch -= OnCatchSpotEvent;
            mediator.value.onRelease -= OnReleaseSpotEvent;
        }

        protected override string info{ get {return $"★ Spot Event [{spotEventType}]"; } }
        protected override bool OnCheck(){ return false; }
    }

    [Category("★ Keno/Events")]
    public class CheckSpotEventValue : CheckSpotEvent
    {
        [BlackboardOnly]
        public BBParameter<SpotInstance> saveEventValue;

        protected override void OnSpotEvent(SpotInstance spot)
        {
            if (isActive)
            {
                saveEventValue.value = spot;
                YieldReturn(true);
            }
        }

        protected override string info{ get {return $"★ Spot Event [{spotEventType}]\n{saveEventValue} = EventValue"; } }
    }
}
