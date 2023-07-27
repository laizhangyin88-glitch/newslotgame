using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.IoC.Tween
{
	[RequireComponent(typeof(TweenMediator))]
	public class TweenManualStepController : MonoBehaviour
	{
		public TweenMediator mediator;
		[InlineEditor]
		public VariableInt step;
		public EnableAction enableAction = EnableAction.DoNothing;
		public bool allowZeroStep;

		protected bool startCalled;

		protected virtual void Start()
		{
			startCalled = true;
			if (enableAction == EnableAction.EnableBehaviour)
				Apply();
		}

		protected virtual void OnEnable()
		{
			if (startCalled && enableAction == EnableAction.EnableBehaviour)
				Apply();

			step.onValueChanged += OnValueChanged;
		}

		protected virtual void OnDisable()
		{
			step.onValueChanged -= OnValueChanged;
		}

		protected virtual void OnValueChanged(VariableAsset val)
		{
			Apply();
		}

		public void Apply()
		{
			if (step.value == 0 && !allowZeroStep)
			{
				mediator.Reset();
				return;
			}

			mediator.step = step.value - 1;
			mediator.nextStep = step.value;
			mediator.Play();
		}

		//////////////////////////////////////////////////////////////////////////////////////////
        /// EDITOR
        //////////////////////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (mediator == null) mediator = GetComponent<TweenMediator>();
        }
#endif
	}
}