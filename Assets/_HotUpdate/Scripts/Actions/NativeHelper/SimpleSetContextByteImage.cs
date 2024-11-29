using UnityEngine;
using System;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SimpleSetContextByteImage : ActionTask
{
    public BBParameter<string> elementName;
    public ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch;
    public BBParameter<byte[]> imageBytes;
    public BBParameter<CacheType> cachingType;
    public BBParameter<bool> priority;

    [BlackboardOnly]
    public BBParameter<bool> isLoaded = false;

    protected override string info
    {
        get
        {
            if (searchingType == ContextSearchingType.SelfContext)
                return string.Format("{0}.sprite = {1}", agentInfo, imageBytes);
            else
                return string.Format("{0}.sprite = {1}", elementName, imageBytes);
        }
    }

    protected override void OnExecute()
    {
        ContextElement element = ContextUtils.FindElement(agent.GetComponent<ContextElement>(), elementName.value, searchingType);
        IContextImage imageElement = element as IContextImage;
        if (imageElement == null)
        {
            Debug.LogError("[Context] " + element.ContextName + " is not IContextImage");
            EndAction();
        }
        else
        {
            if (imageBytes.value != null)
            {
                Texture2D tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                tex.filterMode = FilterMode.Trilinear;
                tex.anisoLevel = 4;
                tex.wrapMode = TextureWrapMode.Clamp;

                tex.LoadImage(imageBytes.value);

                var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
                imageElement.SetSprite(sprite);
            }

            EndAction();
        }
    }
}

}
