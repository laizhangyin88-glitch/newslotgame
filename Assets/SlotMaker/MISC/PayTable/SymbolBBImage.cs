using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlotMaker
{
	[ExecuteInEditMode]
	public class SymbolBBImage : MonoBehaviour 
	{
	    public int symbol;
	    public string blackBoardKey;
	    public ContextImage image;

	    private void Awake()
	    {
	        var spriteList = BlackboardUtils.FindVariable<List<Sprite>>(null, "./customData/" + blackBoardKey).value;
	        image.SetSprite( spriteList[symbol] );
	    }
	}
}
