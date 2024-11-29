using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.Rendering
{
	[CreateAssetMenu(fileName="New SortingLayerNameIDs", menuName="SlotMaker2/Slot/Strategy/Utils/SortingLayerNameIDs")]
	public class SortingLayerNameIDs : ScriptableObject
	{
	    public List<string> sortingLayerNames = new List<string>();
	    
	    private int[] sortingLayerIDs;

	    private void OnEnable()
	    {
	    	UpdateIDs();
	    }

	    private void UpdateIDs()
	    {
	    	sortingLayerIDs = new int[sortingLayerNames.Count];
	    	for (int i = 0; i < sortingLayerIDs.Length; ++i)
	    	{
	    		sortingLayerIDs[i] = SortingLayer.NameToID(sortingLayerNames[i]);
	    	}
	    }

	    public int ValidateIndex(int index)
	    {
	    	return Mathf.Clamp(index, 0, sortingLayerIDs.Length - 1);
	    }

	    public int GetID(int index)
	    {
			return sortingLayerIDs[index];
	    }

		//////////////////////////////////////////////////////////////////////////////////////////
        /// EDITOR
        //////////////////////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
        private void OnValidate()
        {
        	UpdateIDs();
        }
#endif
	}
}