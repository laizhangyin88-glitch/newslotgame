using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace SlotMaker.UI
{
	[RequireComponent(typeof(Toggle))]
	public class ToggleExtensions : MonoBehaviour
	{
	    public Toggle toggle;
	    public enum ToggleTransition
	    {
	    	None,
	    	SpriteSwipe,
	    	Animation
	    }
	    public ToggleTransition toggleTransition = ToggleTransition.SpriteSwipe;

	    [ShowIf("toggleTransition", ToggleTransition.SpriteSwipe)]
	    public Image image;
	    [ShowIf("toggleTransition", ToggleTransition.SpriteSwipe)]
	    public Sprite spriteOn;
	    [ShowIf("toggleTransition", ToggleTransition.SpriteSwipe)]
	    public Sprite spriteOff;
	    [ShowIf("toggleTransition", ToggleTransition.Animation)]
	    public Animator animator;
	    [ShowIf("toggleTransition", ToggleTransition.Animation)]
	    public string isOn;

	    private void Start()
	    {
	    	PlayEffect();
	    }

	    public void OnValueChanged(bool changeSet)
	    {
	    	PlayEffect();
	    }

	    protected virtual void PlayEffect()
	    {
	    	switch (toggleTransition)
	    	{
	    	case ToggleTransition.SpriteSwipe:
	    		if (image != null)
	    			image.sprite = toggle.isOn ? spriteOn : spriteOff;
	    		break;
	    	case ToggleTransition.Animation:
	    		if (animator != null)
	    			animator.SetBool(isOn, toggle.isOn);
	    		break;
	    	}
	    }

#if UNITY_EDITOR
	    //////////////////////////////////////////////////////////////////////////////////////////
        /// EDITOR
        //////////////////////////////////////////////////////////////////////////////////////////
        
        private void OnValidate()
        {
            if (toggle == null)
            	toggle = GetComponent<Toggle>();

            if (image == null)
            	image = GetComponent<Image>();

        	if (animator == null)
        		animator = GetComponent<Animator>();
        }
#endif
	}
}