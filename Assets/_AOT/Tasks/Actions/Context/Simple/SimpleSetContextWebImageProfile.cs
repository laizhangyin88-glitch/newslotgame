using UnityEngine;
using System;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using BagelCode;

namespace SlotMaker.Tasks.Actions
{

[Category("★ BagelCode/Context")]
public class SimpleSetContextWebImageProfile : ActionTask<Blackboard>
{
    public BBParameter<string> elementName;
    public ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch;
    public BBParameter<string> imageUrl;
    public BBParameter<CacheType> cachingType;
    public BBParameter<bool> priority;
    public BBParameter<string> reportCount;
    public BBParameter<string> targetUserId;

	[BlackboardOnly]
    public BBParameter<bool> isLoaded = false;
	[BlackboardOnly]
    public BBParameter<bool> isIgnored = false;

    protected override string info
    {
        get 
        { 
	        if (searchingType == ContextSearchingType.SelfContext)
	            return string.Format("{0}.sprite = {1}", agentInfo, imageUrl.value); 
	        else
	            return string.Format("{0}.sprite = {1}", elementName, imageUrl.value); 
        }
    }

    protected override void OnExecute()
    {
    	bool ignored = false;

        ContextElement element = ContextUtils.FindElement(agent.gameObject.GetComponent<ContextElement>(), elementName.value, searchingType);
        IContextImage imageContext = element as IContextImage;

        if (imageContext == null)
        {
            Debug.LogError("[Context] " + element.ContextName + " is not IContextImage");
            EndAction(false);
            return;
        }
        else
        {
        	//Check Report
			var report = BlackboardUtils.FindVariable<int>(agent, reportCount.value);
			var userId = BlackboardUtils.FindVariable<string>(agent, targetUserId.value);
			if(userId != null)
			{
				if(report != null)
				{
					ignored = MetaSystem.IsIgnoredUser(userId.value, report.value);
				}
				else
				{
					ignored = MetaSystem.IsIgnoredUser(userId.value, 0);
				}
			}
			else
			{
	            Debug.LogError("[Blackbaord]Null variable Found in " + userId.value);
	            EndAction(false);
	            return;
			}

			//Load Image
	    	var url = BlackboardUtils.FindVariable<string>(agent, imageUrl.value);
			if(url == null)
			{
	            Debug.LogError("[Blackbaord]Null variable Found in " + imageUrl.value);
	            EndAction(false);
	            return;
			}
			else
			{
				if(ignored || string.Equals(url.value, ""))
				{
                	if(element != null)
                	{
						element.gameObject.SetActive(false);
                	}
				}
				else
				{
                    string downloadURL = url.value;
		            imageContext.SetHash(downloadURL.GetHashCode().ToString());

		            WebImageDownloader.Instance.LoadWebImage(
		                downloadURL,
		                cachingType.value,
		                priority.value,
		                null,
		                delegate(Sprite img)
		                {
		                    if (element != null && imageContext != null && isLoaded != null)
		                    {
		                        isLoaded.value = true;
		                        if (imageContext.CheckHash(downloadURL.GetHashCode().ToString()))
		                        {
		                            imageContext.SetSprite(img);
									element.gameObject.SetActive(true);
		                        }
		                    }
		                },
		                null,
		                delegate(WebImageDownloader.WebImageDownloadError error)
		                {
		                	if(element != null)
		                	{
								element.gameObject.SetActive(false);
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

}
