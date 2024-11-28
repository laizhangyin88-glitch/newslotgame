using UnityEngine;
using System;
using System.Collections;
using NodeCanvas.Framework;
using Sirenix.OdinInspector;

namespace SlotMaker
{
	[Serializable]
	public class StringStringPairVariable
	{
		public string key;
        [TextArea]
		public string value;
	}
    
    [Serializable]
    public class StringGraphOwnerPairVariable
    {
        public string key;
        public GraphOwner value;
    }
}