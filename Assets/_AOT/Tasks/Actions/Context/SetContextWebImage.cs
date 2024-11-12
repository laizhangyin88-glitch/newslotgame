using UnityEngine;
using System;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SetContextWebImage : ActionTask<ContextElement>
{
    public BBParameter<string> imageUrl;
    public BBParameter<CacheType> cachingType;
    public BBParameter<bool> priority;

    [BlackboardOnly]
    public BBParameter<bool> isLoaded = false;

    protected override string info
    {
        get { return string.Format("{0}.sprite = {1}", agentInfo, imageUrl); }
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
            if(!string.IsNullOrEmpty(imageUrl.value))
            {
                string downloadURL = imageUrl.value;

                image.SetHash(downloadURL.GetHashCode().ToString());

                WebImageDownloader.Instance.LoadWebImage(
                    downloadURL,
                    cachingType.value,
                    priority.value,
                    null,
                    delegate(Sprite img)
                    {
                        if (agent != null && image != null && isLoaded != null)
                        {
                            isLoaded.value = true;
                            if (image.CheckHash(downloadURL.GetHashCode().ToString()))
                            {
                                image.SetSprite(img);
                            }
                        }
                    }
                );
            }
            
            EndAction();
        }
    }
}

}
