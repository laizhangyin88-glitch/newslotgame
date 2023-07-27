using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

namespace SlotMaker
{
    public class ContextImage : ContextCompositor, IContextImage
    {
    	public Image image;
    	public UnityEvent onChangedSprite;

        public string imageHash = "";

        public void SetHash(string hashCode)
        {
            imageHash = hashCode;
        }

        public bool CheckHash(string hashCode)
        {
            if (imageHash != hashCode) return false;

            return true;
        }

    	public void SetSprite(Sprite sprite)
    	{
            if (image == null) return;

        	image.sprite = sprite;

    		if (onChangedSprite != null)
    		{
    			onChangedSprite.Invoke();
    		}
    	}

        public void AddListenerOnChangedSprite(UnityAction<ContextElement> action)
        {
            onChangedSprite.AddListener(() => { action(this); });
        }

        public void SetColor(Color color)
        {
            if(image != null)
            {
                image.color = color;
            }
        }
    }
}
