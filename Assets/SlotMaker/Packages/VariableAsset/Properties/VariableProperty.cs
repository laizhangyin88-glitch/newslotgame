using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker.IoC;
using Sirenix.OdinInspector;

namespace SlotMaker
{
	public abstract class VariableProperty : MonoBehaviour
	{
		[InlineEditor]
		public VariableAsset variable;
		public EnableAction enableAction = EnableAction.DoNothing;

		protected bool startCalled;

		protected virtual void Start()
		{
			startCalled = true;
			if (enableAction == EnableAction.EnableBehaviour)
				OnValueChanged(variable);
		}

		protected virtual void OnEnable()
		{
			if (startCalled && enableAction == EnableAction.EnableBehaviour)
				OnValueChanged(variable);

			variable.onValueChanged += OnValueChanged;
		}

		protected virtual void OnDisable()
		{
			variable.onValueChanged -= OnValueChanged;
		}

		protected abstract void OnValueChanged(VariableAsset val);

		public override string ToString() { return variable.value.ToString(); }
	}
}