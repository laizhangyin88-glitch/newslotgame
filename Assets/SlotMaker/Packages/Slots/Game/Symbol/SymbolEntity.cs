using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
    [CreateAssetMenu(fileName="New Symbol", menuName="SlotMaker2/Math/Symbol")]
	public class SymbolEntity : VariableAsset<int>
	{
        [Flags]
        public enum SymbolAttribute
        {
            Wild = (1 << 0),
            Bar = (1 << 1),
            Seven = (1 << 2),
            Blank = (1 << 3),
            Scatter1 = (1 << 4),
            Scatter2 = (1 << 5),
            Scatter3 = (1 << 6),
            Scatter4 = (1 << 7),
            Scatter5 = (1 << 8),
            Scatter6 = (1 << 9),
            Scatter7 = (1 << 10),
            Scatter8 = (1 << 11),
            Scatter9 = (1 << 12),
            Scatter10 = (1 << 13),
            Scatter11 = (1 << 14),
            Scatter12 = (1 << 15),
        }

        public event Action<SymbolEntity> onAttributeChanged;

        protected void OnAttributeChanged()
        {
            if (onAttributeChanged != null) onAttributeChanged(this);
        }

        [HideInInspector]
        [SerializeField]
        protected SymbolAttribute overridenAttribute;

        protected SymbolAttribute runtimeAttribute;

        [ShowInInspector]
        [DisableIf("isProtected")]
        public SymbolAttribute attribute
        {
            get { return _attribute; }
            set
            {
                if (!isProtected && !object.Equals(_attribute, value))
                {
                    _attribute = value;
                    OnAttributeChanged();
                }
            }
        }

        protected SymbolAttribute _attribute
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying || runtimeEditMode)
                    return overridenAttribute;
#endif
                return runtimeAttribute;
            }

            set
            {
#if UNITY_EDITOR
                if (!Application.isPlaying || runtimeEditMode)
                    overridenAttribute = value;
#endif
                runtimeAttribute = value;
            }
        }

        public event Action<SymbolEntity> onMultiplierChanged;

        protected void OnMultiplierChanged()
        {
            if (onMultiplierChanged != null) onMultiplierChanged(this);
        }

        [HideInInspector]
        [SerializeField]
        protected long overridenMultiplier = 1L;

        protected long runtimeMultiplier = 1L;

        [ShowInInspector]
        [DisableIf("isProtected")]
        public long multiplier
        {
            get { return _multiplier; }
            set
            {
                if (!isProtected && !object.Equals(_multiplier, value))
                {
                    _multiplier = value;
                    OnMultiplierChanged();
                }
            }
        }

        protected long _multiplier
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying || runtimeEditMode)
                    return overridenMultiplier;
#endif
                return runtimeMultiplier;
            }

            set
            {
#if UNITY_EDITOR
                if (!Application.isPlaying || runtimeEditMode)
                    overridenMultiplier = value;
#endif
                runtimeMultiplier = value;
            }
        }

        [HideInInspector]
        [SerializeField]
        protected int overridenColumnCount = 1;

        protected int runtimeColumnCount = 1;

        [ShowInInspector]
        [DisableIf("isProtected")]
        public int columnCount
        {
            get { return _columnCount; }
            set
            {
                if (!isProtected && !object.Equals(_columnCount, value))
                    _columnCount = value;
            }
        }

        protected int _columnCount
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying || runtimeEditMode)
                    return overridenColumnCount;
#endif
                return runtimeColumnCount;
            }

            set
            {
#if UNITY_EDITOR
                if (!Application.isPlaying || runtimeEditMode)
                    overridenColumnCount = value;
#endif
                runtimeColumnCount = value;

                volume = columnCount * rowCount;
            }
        }

        [HideInInspector]
        [SerializeField]
        protected int overridenRowCount = 1;

        protected int runtimeRowCount = 1;

        [ShowInInspector]
        [DisableIf("isProtected")]
        public int rowCount
        {
            get { return _rowCount; }
            set
            {
                if (!isProtected && !object.Equals(_rowCount, value))
                    _rowCount = value;
            }
        }

        protected int _rowCount
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying || runtimeEditMode)
                    return overridenRowCount;
#endif
                return runtimeRowCount;
            }

            set
            {
#if UNITY_EDITOR
                if (!Application.isPlaying || runtimeEditMode)
                    overridenRowCount = value;
#endif
                runtimeRowCount = value;

                volume = columnCount * rowCount;
            }
        }

        [HideInInspector]
        [SerializeField]
        protected int overridenVolume = 1;

        protected int runtimeVolume = 1;

        [ShowInInspector]
        [DisableIf("isProtected")]
        public int volume
        {
            get { return _volume; }
            set
            {
                if (!isProtected && !object.Equals(_volume, value))
                    _volume = value;
            }
        }

        protected int _volume
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying || runtimeEditMode)
                    return overridenVolume;
#endif
                return runtimeVolume;
            }

            set
            {
#if UNITY_EDITOR
                if (!Application.isPlaying || runtimeEditMode)
                    overridenVolume = value;
#endif
                runtimeVolume = value;
            }
        }

        public bool HasAttribute(SymbolAttribute mask)
        {
            return (attribute & mask) == attribute;
        }

        public bool HasAnyAttribute(SymbolAttribute mask)
        {
            return (int)(attribute & mask) != 0;
        }

        public bool IsWild()
        {
            return HasAttribute(SymbolAttribute.Wild);
        }

        public bool IsSeven()
        {
            return HasAttribute(SymbolAttribute.Seven);
        }

        public bool IsBar()
        {
            return HasAttribute(SymbolAttribute.Bar);
        }

        public bool IsBlank()
        {
            return HasAttribute(SymbolAttribute.Blank);
        }

        protected override void Deserialize()
        {
            base.Deserialize();

            runtimeAttribute   = overridenAttribute;
            runtimeMultiplier  = overridenMultiplier;
            runtimeColumnCount = overridenColumnCount;
            runtimeRowCount    = overridenRowCount;
            runtimeVolume      = overridenVolume;
        }

        public void Copy(SymbolEntity entity_)
        {
            this.value       = entity_.value;
            this.attribute   = entity_.attribute;
            this.multiplier  = entity_.multiplier;
            this.columnCount = entity_.columnCount;
            this.rowCount    = entity_.rowCount;
            this.volume      = entity_.volume;
        }
	}
}