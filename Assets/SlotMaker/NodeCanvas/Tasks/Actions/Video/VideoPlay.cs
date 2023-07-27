using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ SlotMaker/Video")]
	public class VideoPlay : ActionTask<Transform> 
	{
		public bool waitPrepared = true;

		protected override void OnExecute()
		{
			var videoPlayer = agent.GetComponent<VideoPlayer>();

			if (waitPrepared)
			{
				videoPlayer.prepareCompleted += OnPrepared;
				videoPlayer.Prepare();
			}
			else
			{
				videoPlayer.Play();
				EndAction();
			}
		}

		void OnPrepared(VideoPlayer videoPlayer)
		{
			videoPlayer.Play();

			videoPlayer.prepareCompleted -= OnPrepared;
			EndAction();
		}
	}
}
