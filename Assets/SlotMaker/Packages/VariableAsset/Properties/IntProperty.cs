using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
	public class IntProperty : VariableProperty
	{
	    public UnityIntEvent onValueChanged;

	    public int asInt 
	    { 
	    	get { return (int)variable.value; } 
	    	set { variable.value = value; }
	    }

	    protected override void OnValueChanged(VariableAsset val)
	    {
	    	onValueChanged.Invoke((int)val.value);
	    }
	}
}