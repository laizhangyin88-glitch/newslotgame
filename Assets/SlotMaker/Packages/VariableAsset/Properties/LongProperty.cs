using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
	public class LongProperty : VariableProperty
	{
	    public UnityLongEvent onValueChanged;

	    public long asLong 
	    { 
	    	get { return (long)variable.value; } 
	    	set { variable.value = value; }
	    }

	    protected override void OnValueChanged(VariableAsset val)
	    {
	    	onValueChanged.Invoke((long)val.value);
	    }
	}
}