using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SetContextFileImage : ActionTask<ContextElement>
{
    public BBParameter<string> path;

    // public BBParameter<bool> useScaler;

    // public BBParameter<ScreenUtils.ScaleType> scaleType;
    // public BBParameter<float> scaleFactor;

    public BBParameter<bool> useSaveBytes;
    public BBParameter<byte[]> saveBytes;

    protected override string info
    {
        get { return string.Format("{0}.sprite = LoadFile", agentInfo); }
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
            if (path != null && !string.IsNullOrEmpty(path.value))
            {
                byte[] bytes = FileUtils.Read(path.value);

                if (bytes != null && bytes.Length > 0)
                {
                    // Vector2 scaleSize = Vector2.zero;

                    imageContext.SetSprite(FileImageLoader.Instance.LoadTextureToSprite(bytes));

                    // if(useScaler.value == true)
                    // {
                    //     Image image = agent.GetComponent<Image>();
                    //     Rect origRect = image.rectTransform.rect;
                    //     image.rectTransform.sizeDelta = scaleSize;
                    // }

                    if(useSaveBytes.value == true)
                    {
                        saveBytes.value = bytes;
                    }
                }
            }

            EndAction();
        }
    }
}

}
