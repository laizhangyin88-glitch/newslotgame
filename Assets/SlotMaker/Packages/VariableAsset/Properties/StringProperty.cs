using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
	public class StringProperty : VariableProperty
	{
	    public UnityStringEvent onValueChanged;

	    public string asString 
	    { 
	    	get { return (string)variable.value; } 
	    	set { variable.value = value; }
	    }

	    protected override void OnValueChanged(VariableAsset val)
	    {
	    	onValueChanged.Invoke((string)val.value);
	    }
	}
}