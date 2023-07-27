using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ParadoxNotion;
using Sirenix.OdinInspector;

namespace SlotMaker
{
	public class IntCompareProperty : IntProperty
	{
		public CompareMethod checkType = CompareMethod.EqualTo;
	    public int compare;

	    public UnityBoolEvent onConditionChanged;
		public UnityEvent onTrue;
		public UnityEvent onFalse;

	    protected override void OnValueChanged(VariableAsset val)
	    {
	    	onValueChanged.Invoke((int)val.value);

            bool condition = OperationUtils.Compare((int)val.value, compare, checkType);
            onConditionChanged.Invoke(condition);
            if (condition) onTrue.Invoke();
            else onFalse.Invoke();
	    }
	}
}