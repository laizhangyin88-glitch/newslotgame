using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
	public class FloatProperty : VariableProperty
	{
	    public UnityFloatEvent onValueChanged;

	    public float asFloat 
	    { 
	    	get { return (float)variable.value; } 
	    	set { variable.value = value; }
	    }

	    protected override void OnValueChanged(VariableAsset val)
	    {
	    	onValueChanged.Invoke((float)val.value);
	    }
	}
}