using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using Sirenix.OdinInspector;

namespace SlotMaker
{
	[CreateAssetMenu(fileName="New Aspect Ratio Checker", menuName="SlotMaker2/VariableAsset/Utils/Aspect Ratio")]
	public class VariableAspectRatioChecker : VariableBool
	{
	    public CompareMethod checkType = CompareMethod.EqualTo;
	    public float ratio;

	    protected override object objectValue
	    {
	    	get { return OperationUtils.Compare((float)Screen.width / (float)Screen.height, ratio, checkType, Mathf.Epsilon); }
	    	set {}
	    }
	}
}