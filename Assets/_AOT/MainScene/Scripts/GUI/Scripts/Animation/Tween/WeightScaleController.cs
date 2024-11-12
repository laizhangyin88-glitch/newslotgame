using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
	[ExecuteInEditMode]
	public class WeightScaleController : MonoBehaviour
	{
	    public Vector3 from;
	    public Vector3 to;

	    public float weight;

	    private void LateUpdate()
	    {
	        Vector3 direction  = to - from;
	        transform.position = from + direction * weight;
	    }
	}
}
