using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
	[Serializable]
	public class OverridenSymbolEntity : IEquatable<OverridenSymbolEntity>, ICloneable
	{
        [InlineEditor]
	    public SymbolEntity master;

        public bool isNull;
        public int index;

		[HideInInspector]
		[SerializeField]
	    protected bool isOverridenValue;
        [HideInInspector]
        [SerializeField]
        protected bool isOverridenAttribute;
        [HideInInspector]
        [SerializeField]
        protected bool isOverridenMultiplier;
        [HideInInspector]
        [SerializeField]
        protected bool isOverridenColumnCount;
        [HideInInspector]
        [SerializeField]
        protected bool isOverridenRowCount;

	    [HideInInspector]
        [SerializeField]
	    private int overridenValue;

	    [ShowInInspector]
	    public int value
	    {
	    	get 
	    	{
	    		if (master == null || isOverridenValue) return overridenValue;
	    		return master.value;
	    	}

	    	set 
	    	{
    			isOverridenValue = !(master != null && object.Equals(master.value, value));
	    		overridenValue = value;
	    	}
	    }

	    [HideInInspector]
        [SerializeField]
	    private SymbolEntity.SymbolAttribute overridenAttribute;

	    [ShowInInspector]
	    public SymbolEntity.SymbolAttribute attribute
	    {
	    	get 
	    	{
	    		if (master == null || isOverridenAttribute) return overridenAttribute;
	    		return master.attribute;
	    	}

	    	set 
	    	{
	    		isOverridenAttribute = !(master != null && object.Equals(master.attribute, value));
	    		overridenAttribute = value;
	    	}
	    }

        [HideInInspector]
        [SerializeField]
        private long overridenMultiplier;

        [ShowInInspector]
        public long multiplier
        {
            get
            {
                if (master == null || isOverridenMultiplier) return overridenMultiplier;
                return master.multiplier;
            }

            set
            {
                isOverridenMultiplier = !(master != null && object.Equals(master.multiplier, value));
                overridenMultiplier = value;
            }
        }
	    
	    [HideInInspector]
	    [SerializeField]
	    private int overridenColumnCount;

	    [ShowInInspector]
	    public int columnCount
	    {
	    	get
	    	{
	    		if (master == null || isOverridenColumnCount) return overridenColumnCount;
	    		return master.columnCount;
	    	}

	    	set
	    	{
	    		isOverridenColumnCount = !(master != null && object.Equals(master.value, value));
	    		overridenColumnCount = value;
	    	}
	    }

	    [HideInInspector]
	    [SerializeField]
	    private int overridenRowCount;

	    [ShowInInspector]
	    public int rowCount
	    {
	    	get
	    	{
	    		if (master == null || isOverridenRowCount) return overridenRowCount;
	    		return master.rowCount;
	    	}

	    	set
	    	{
	    		isOverridenRowCount = !(master != null && object.Equals(master.value, value));
	    		overridenRowCount = value;
	    	}
	    }

	    [PropertyOrder(100)]
	    public int columnOffset;
	    [PropertyOrder(101)]
	    public int rowOffset;

        public static OverridenSymbolEntity zero = new OverridenSymbolEntity{ isNull = true };

        public void MarkOverriden()
        {
            isOverridenValue = true;
            isOverridenAttribute = true;
            isOverridenColumnCount = true;
            isOverridenRowCount = true;
        }

        [Button]
		[PropertyOrder(1000)]
	    public void ClearOverriden()
	    {
	    	isOverridenValue = false;
	    	isOverridenAttribute = false;
	    	isOverridenColumnCount = false;
	    	isOverridenRowCount = false;
	    }

	    public bool HasAttribute(SymbolEntity.SymbolAttribute mask)
        {
            return (attribute & mask) == mask;
        }

        public bool HasAnyAttribute(SymbolEntity.SymbolAttribute mask)
        {
            return (int)(attribute & mask) != 0;
        }

        public bool IsWild()
        {
        	return HasAttribute(SymbolEntity.SymbolAttribute.Wild);
        }

        public bool IsSeven()
        {
        	return HasAttribute(SymbolEntity.SymbolAttribute.Seven);
        }

        public bool IsBar()
        {
        	return HasAttribute(SymbolEntity.SymbolAttribute.Bar);
        }

        public bool IsBlank()
        {
        	return HasAttribute(SymbolEntity.SymbolAttribute.Blank);
        }

        public OverridenSymbolEntity() {}

        public bool Equals(OverridenSymbolEntity other)
        {
            return value == other.value;
        }

        public object Clone()
        {
            var newEntity = GetObject();
            newEntity.master        = master;
			newEntity.isNull        = isNull;
            newEntity.index         = index;
            newEntity.value         = value;
            newEntity.attribute     = attribute;
            newEntity.multiplier    = multiplier;
            newEntity.columnCount   = columnCount;
            newEntity.rowCount      = rowCount;
            newEntity.columnOffset  = columnOffset;
            newEntity.rowOffset     = rowOffset;
            return newEntity;
        }

		public void ReturnToPool()
		{
			AddObject(this);
		}

		private static List<OverridenSymbolEntity> cachedPools = new List<OverridenSymbolEntity>();

		public static OverridenSymbolEntity GetObject()
		{
			OverridenSymbolEntity overridenSymbolEntity;
			int lastAvaliiableIndex = cachedPools.Count - 1;
			if (lastAvaliiableIndex >= 0)
			{
				overridenSymbolEntity = cachedPools[lastAvaliiableIndex];
				cachedPools.RemoveAt(lastAvaliiableIndex);
			}
			else
			{
				overridenSymbolEntity = new OverridenSymbolEntity();
			}
			return overridenSymbolEntity;
		}

		public static void AddObject(OverridenSymbolEntity overridenSymbolEntity)
		{
			cachedPools.Add(overridenSymbolEntity);
		}
	}
}
