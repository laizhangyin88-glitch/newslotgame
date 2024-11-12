using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SimpleSetContextImageScaler : ActionTask<ContextElement>
{
    public BBParameter<string> elementName;    
    public ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch;

    public BBParameter<ScreenUtils.ScaleType> scaleType;
    public BBParameter<float> scaleFactor;

    protected override string info
    {
        get { return string.Format("{0} Change Image Scale", elementName); }
    }

    protected override void OnExecute()
    {
        ContextElement element = ContextUtils.FindElement(agent.GetComponent<ContextElement>(), elementName.value, searchingType);
        if (element == null)
        {
            Debug.LogError("[Context] " + elementName.value + " is not found");
            EndAction(false);
        }
        else 
        {
            IContextImage imageContext = element as IContextImage;
            if (imageContext == null)
            {
                Debug.LogError("[Context] " + elementName.value + " is not IContextImage");
                EndAction(false);
            }
            else
            {
                Image image = element.GetComponent<Image>();

                if(image.sprite != null && image.sprite.texture != null)
                {
                    Vector2 origSize = new Vector2(image.sprite.texture.width, image.sprite.texture.height);
                    Rect origRect = image.rectTransform.rect;
                    image.rectTransform.sizeDelta = ScreenUtils.GetScaleSize(origSize, scaleType.value, scaleFactor.value);
                }

                EndAction();
            }
        }

        
    }
}

}
