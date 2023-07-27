using UnityEngine;
using System;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SimpleSetContextWebImage : ActionTask
{
	public BBParameter<string> elementName;
	public ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch;
    public BBParameter<string> imageUrl;
    public BBParameter<CacheType> cachingType;
    public BBParameter<bool> priority;

    [BlackboardOnly]
    public BBParameter<bool> isLoaded = false;

    [BlackboardOnly]
    public BBParameter<bool> isLoadFailed = false;

    protected override string info
    {
        get 
        { 
            if (searchingType == ContextSearchingType.SelfContext)
                return string.Format("{0}.sprite = {1}", agentInfo, imageUrl); 
            else
                return string.Format("{0}.sprite = {1}", elementName, imageUrl); 
        }
    }

    protected override void OnExecute()
    {
    	ContextElement element = ContextUtils.FindElement(agent.GetComponent<ContextElement>(), elementName.value, searchingType);
    	IContextImage imageElement = element as IContextImage;
    	if (imageElement == null)
    	{
            Debug.LogError("[Context] " + element.ContextName + " is not IContextImage");
            isLoadFailed.value = true;
            EndAction(false);
    	}
    	else 
    	{
    		Blackboard bb = agent.GetComponent<Blackboard>();
    		var variable = BlackboardUtils.FindVariable<string>(bb, imageUrl.value);
    		if (variable == null)
    		{
    			Debug.LogError("[Blackboard](" + agent.name + ") Null variable founded in " + imageUrl);
                isLoadFailed.value = true;
    			EndAction(false);
    		}
    		else 
    		{
                if(!string.IsNullOrEmpty(variable.value))
                {
                    string downloadURL = variable.value;

                    imageElement.SetHash(downloadURL.GetHashCode().ToString());

                    WebImageDownloader.Instance.LoadWebImage( 
                        downloadURL,
                        cachingType.value,
                        priority.value,
                        null,
                        delegate(Sprite img) 
                        { 
                            if (agent != null && imageElement != null && isLoaded != null)
                            {
                                isLoaded.value = true;
                            
                                if (imageElement.CheckHash(downloadURL.GetHashCode().ToString()))
                                {
                                    imageElement.SetSprite(img); 
                                }
                            }
                            
                        },
                        null,
                        delegate(WebImageDownloader.WebImageDownloadError error)
                        { 
                            if (agent != null && imageElement != null && variable != null && isLoadFailed != null)
                            {
                                isLoadFailed.value = true;
                            }
                            
                        }
                    );
                }
	            
	            EndAction();
    		}
    	}
    }
}

}
