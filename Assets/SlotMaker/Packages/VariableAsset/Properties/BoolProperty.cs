using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
	public class BoolProperty : VariableProperty
	{
	    public UnityBoolEvent onValueChanged;

	    public bool asBool 
	    { 
	    	get { return (bool)variable.value; } 
	    	set { variable.value = value; }
	    }

	    protected override void OnValueChanged(VariableAsset val)
	    {
	    	onValueChanged.Invoke((bool)val.value);
	    }
	}
}