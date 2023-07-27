using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlotMaker
{
	[ExecuteInEditMode]
	public class SymbolStaticImage : MonoBehaviour 
	{
	    public int symbol;
	    public ContextImage image;

	    private void Awake()
	    {
	        image.SetSprite( ContentCustomData.Instance.symbolSprite.sprites[symbol] );
	    }
	}
}
