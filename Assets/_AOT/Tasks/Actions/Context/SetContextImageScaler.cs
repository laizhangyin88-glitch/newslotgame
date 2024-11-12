using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SetContextImageScaler : ActionTask<ContextElement>
{
    public BBParameter<ScreenUtils.ScaleType> scaleType;
    public BBParameter<float> scaleFactor;

    protected override string info
    {
        get { return string.Format("{0} Change Image Scale", agentInfo); }
    }

    protected override void OnExecute()
    {
        IContextImage imageContext = agent as IContextImage;
        if (imageContext == null)
        {
            Debug.LogError("[Context] " + agent.ContextName + " is not IContextImage");
            EndAction(false);
        }
        else
        {
            Image image = agent.GetComponent<Image>();

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
