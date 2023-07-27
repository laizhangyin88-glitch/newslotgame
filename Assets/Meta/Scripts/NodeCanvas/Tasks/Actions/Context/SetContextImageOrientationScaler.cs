using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SetContextImageOrientationScaler : ActionTask<ContextElement>
{
    public BBParameter<float> scaleFactorWidth;
    public BBParameter<float> scaleFactorHeight;

    protected override string info
    {
        get { return string.Format("{0} Change Image Scale({1}x{2})", agentInfo, scaleFactorWidth, scaleFactorHeight); }
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

            if(image != null && image.sprite != null && image.sprite.texture != null)
            {
                Vector2 origSize = new Vector2(image.sprite.texture.width, image.sprite.texture.height);
                Vector2 factorSize = new Vector2(scaleFactorWidth.value, scaleFactorHeight.value);

                image.rectTransform.sizeDelta = ScreenUtils.GetScaleSize(origSize, factorSize);
            }

            EndAction();
        }
    }
}

}
