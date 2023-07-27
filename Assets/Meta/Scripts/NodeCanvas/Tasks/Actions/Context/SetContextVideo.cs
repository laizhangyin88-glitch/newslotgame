using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEngine.Video;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;
using BagelCode;

namespace BagelCode.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SetContextVideo : ActionTask<ContextElement>
{
    public BBParameter<string> url;
    public BBParameter<Vector2> textureSize;
    public BBParameter<VideoAspectRatio> aspectRatio;

    protected override string info
    {
        get { return string.Format("Play Video : {0}({1}x{2})", url, (int)textureSize.value.x, (int)textureSize.value.y); }
    }

    protected override void OnExecute()
    {
        var videoManager = agent.gameObject.GetComponent<VideoPlayerManager>();
        if (videoManager == null)
        {
            Debug.LogError("[Context] " + agent.ContextName + " is not VideoPlayerManager");
            EndAction(false);
        }
        else
        {
            if (url != null && !string.IsNullOrEmpty(url.value))
            {
                videoManager.Play(url.value, textureSize.value, aspectRatio.value);
            }

            EndAction();
        }
    }
}

}
