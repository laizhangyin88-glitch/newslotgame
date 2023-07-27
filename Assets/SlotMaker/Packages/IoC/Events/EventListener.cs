using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace SlotMaker.IoC
{
	public class EventListener : MonoBehaviour
	{
	    [Serializable]
	    public class Listener
	    {
	    	public EnableAction enableAction = EnableAction.DoNothing;
	    	public enum ParameterType
	    	{
	    		Void,
	    		Bool,
	    		Int,
	    		Float,
	    		String
	    	};
	    	public ParameterType parameterType = ParameterType.Void;
	    	[InlineEditor]
	    	public VariableAsset value;
	    	public ValueType valueType = ValueType.ByAsset;

	    	[ShowIf("ActiveBoolValue")]
	    	public bool boolValue;
	    	[ShowIf("ActiveIntValue")]
	    	public int intValue;
	    	[ShowIf("ActiveFloatValue")]
	    	public float floatValue;
	    	[ShowIf("ActiveStringValue")]
	    	public string stringValue;

	    	[ShowIf("parameterType", ParameterType.Void)]
	    	public UnityEvent onValueChanged;
	    	[ShowIf("parameterType", ParameterType.Bool)]
	    	public UnityBoolEvent onBoolValueChanged;
	    	[ShowIf("parameterType", ParameterType.Int)]
	    	public UnityIntEvent onIntValueChanged;
	    	[ShowIf("parameterType", ParameterType.Float)]
	    	public UnityFloatEvent onFloatValueChanged;
	    	[ShowIf("parameterType", ParameterType.String)]
	    	public UnityStringEvent onStringValueChanged;

	    	public void Register()
	    	{
	    	    if (enableAction == EnableAction.EnableBehaviour)
	    	        OnValueChanged(value);

	    	    value.onValueChanged += OnValueChanged;
	    	}

	    	public void UnRegister()
	    	{
	    	    value.onValueChanged -= OnValueChanged;
	    	}

	    	public void OnValueChanged(VariableAsset val)
	    	{
	    	    switch (parameterType)
	    	    {
	    	    case ParameterType.Void:
	    	    	if (onValueChanged != null) onValueChanged.Invoke();
	    	        break;
	    	    case ParameterType.Bool:
	    	    	if (onBoolValueChanged != null) onBoolValueChanged.Invoke(valueType == ValueType.ByAsset ? (bool)value.value : boolValue);
	    	        break;
	    	    case ParameterType.Int:
	    	    	if (onIntValueChanged != null) onIntValueChanged.Invoke(valueType == ValueType.ByAsset ? (int)value.value : intValue);
	    	        break;
	    	    case ParameterType.Float:
	    	    	if (onFloatValueChanged != null) onFloatValueChanged.Invoke(valueType == ValueType.ByAsset ? (float)value.value : floatValue);
	    	        break;
    	        case ParameterType.String:
    	        	if (onStringValueChanged != null) onStringValueChanged.Invoke(valueType == ValueType.ByAsset ? (string)value.value : stringValue);
	    	        break;
	    	    }
	    	}

	    	private bool ActiveBoolValue() { return valueType == ValueType.ByValue && parameterType == ParameterType.Bool; }
	    	private bool ActiveIntValue() { return valueType == ValueType.ByValue && parameterType == ParameterType.Int; }
	    	private bool ActiveFloatValue() { return valueType == ValueType.ByValue && parameterType == ParameterType.Float; }
	    	private bool ActiveStringValue() { return valueType == ValueType.ByValue && parameterType == ParameterType.String; }
	    }
	    public List<Listener> listeners = new List<Listener>();

	    private void OnEnable()
        {
            foreach (var listener in listeners)
            {
                listener.Register();
            }
        }

        private void OnDisable()
        {
            foreach (var listener in listeners)
            {
                listener.UnRegister();
            }
        }
	}
}