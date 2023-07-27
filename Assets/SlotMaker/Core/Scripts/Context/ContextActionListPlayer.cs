using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace SlotMaker
{
	public class ContextActionListPlayer : ContextCompositor, IContextPlayer
	{
	    public NodeCanvas.ActionListPlayer actionListPlayer;

	    public void Play()
	    {
	        if (actionListPlayer != null)
	            actionListPlayer.Play();
	    }
	}
}
