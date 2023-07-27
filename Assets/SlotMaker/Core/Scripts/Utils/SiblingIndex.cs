using UnityEngine;

namespace SlotMaker
{
	public class SiblingIndex : MonoBehaviour
	{
	    public int siblingIndex 
	    {
	        get 
	        {
	            return transform.GetSiblingIndex();
	        }
	    }
	}
}
