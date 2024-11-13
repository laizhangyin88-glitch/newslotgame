using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SetContextImage : ActionTask<ContextElement>
{
    public BBParameter<string> bundleName;
    public BBParameter<string> assetName;
    public BBParameter<bool> combineApplicationType;

    protected override string info
    {
        get { return string.Format("{0}.sprite = {1}", agentInfo, assetName); }
    }

    protected override void OnExecute()
    {
        IContextImage image = agent as IContextImage;
        if (image == null)
        {
            Debug.LogError("[Context] " + agent.ContextName + " is not IContextImage");
            EndAction(false);
        }
        else 
        {
            var sprite = AssetBundleManager.LoadAsset<Sprite>(GetBundleName(), assetName.value);
            image.SetSprite(sprite);
            EndAction();
        }
    }

    protected string GetBundleName()
    {
        return combineApplicationType.value ? ApplicationSettings.MakeApplicationBundleName(bundleName.value) : bundleName.value;
    }
}

}