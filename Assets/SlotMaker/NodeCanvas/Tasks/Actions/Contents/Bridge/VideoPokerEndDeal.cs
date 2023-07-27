using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode;
using System.Collections.Generic;

namespace BagelCode.Tasks.Actions.ClientAPI
{
	[Category("★ BagelCode/ClientAPI")]
	public class VideoPokerEndDeal : ActionTask
	{
	    private const string BLOCK_SEQ = "./turn/spin/response/common/blockseq";

	    protected override void OnExecute()
	    {
	    	MetaSystem.VideoPokerEndDeal();
	        EndAction();
	    }
	}
}
