using UnityEngine;
using System;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using BagelCode;

namespace SlotMaker.Tasks.Actions
{

[Category("★ BagelCode/Context")]
public class SetContextWebImageProfile : ActionTask<ContextElement>
{
    public BBParameter<string> imageUrl;
    public BBParameter<CacheType> cachingType;
    public BBParameter<bool> priority;
    public BBParameter<int> reportCount;
    public BBParameter<string> targetUserId;

    [BlackboardOnly]
    public BBParameter<bool> isLoaded = false;
    [BlackboardOnly]
    public BBParameter<bool> isIgnored = false;

    protected override string info
    {
        get 
        { 
            return string.Format("{0}.sprite = {1}", agentInfo, imageUrl); 
        }
    }

    protected override void OnExecute()
    {
        bool ignored = false;

        IContextImage image = agent as IContextImage;

        if (image == null)
        {
            Debug.LogError("[Context] " + agent.ContextName + " is not IContextImage");
            EndAction(false);
            return;
        }
        else
        {
            //Check Report
            if(targetUserId != null)
            {
                if(reportCount != null)
                {
                    ignored = MetaSystem.IsIgnoredUser(targetUserId.value, reportCount.value);
                }
                else
                {
                    ignored = MetaSystem.IsIgnoredUser(targetUserId.value, 0);
                }
            }
            else
            {
                Debug.LogError("[Blackbaord]Null variable Found in " + targetUserId.value);
                EndAction(false);
                return;
            }

            //Load Image
            if(ignored || string.Equals(imageUrl.value, ""))
            {
                if(agent != null)
                {
                    agent.gameObject.SetActive(false);
                }
            }
            else
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
                                agent.gameObject.SetActive(true);
                            }
                        }
                    },
                    null,
                    delegate(WebImageDownloader.WebImageDownloadError error)
                    {
                        if(agent != null)
                        {
                            agent.gameObject.SetActive(false);
                        }
                    }
                );
            }

            isIgnored.value = ignored;

            EndAction();
        }
    }
}

}
