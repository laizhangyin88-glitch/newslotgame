using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ SlotMaker/Video")]
	public class VideoSetUrl : ActionTask<Transform> 
	{
		public BBParameter<string> url;

		protected override void OnExecute()
		{
			var videoPlayer = agent.GetComponent<VideoPlayer>();
			videoPlayer.url = url.value;

			EndAction();
		}
	}
}
