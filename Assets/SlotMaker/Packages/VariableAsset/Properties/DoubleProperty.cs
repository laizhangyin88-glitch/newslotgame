using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
	public class DoubleProperty : VariableProperty
	{
	    public UnityDoubleEvent onValueChanged;

	    public double asDouble 
	    { 
	    	get { return (double)variable.value; } 
	    	set { variable.value = value; }
	    }

	    protected override void OnValueChanged(VariableAsset val)
	    {
	    	onValueChanged.Invoke((double)val.value);
	    }
	}
}