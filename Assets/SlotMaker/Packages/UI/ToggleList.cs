using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlotMaker.UI
{
	public class ToggleList : MonoBehaviour
	{
	    public VariableInt toggleValue;
	    public ToggleGroup toggleGroup;
	    public List<Toggle> toggles;

	    public enum InteractionMode
	    {
	    	Manually,
	    	Immediately
	    }
	    public InteractionMode interactionMode = InteractionMode.Manually;

	    protected virtual void OnEnable()
	    {
	    	if (toggleValue != null) toggleValue.onValueChanged += OnSourceChanged;
	    }

	    protected virtual void OnDisable()
	    {
	    	if (toggleValue != null) toggleValue.onValueChanged -= OnSourceChanged;
	    }

	    protected virtual void OnSourceChanged(VariableAsset src)
	    {
	    	if (interactionMode == InteractionMode.Immediately)
	    		Apply();
	    }

	    public void Apply()
	    {
	    	if (toggleValue.value == 0 || toggleValue.value > toggles.Count)
	    	{
	    		if (toggleGroup != null) toggleGroup.SetAllTogglesOff();
	    	}
	    	else
	    	{
	    		toggles[toggleValue.value - 1].isOn = true;
	    	}
	    }
	}
}