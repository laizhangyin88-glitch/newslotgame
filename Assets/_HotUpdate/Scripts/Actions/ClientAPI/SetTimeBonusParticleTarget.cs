using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{
	[Category("★ BagelCode/ClientAPI")]
	public class SetTimeBonusParticleTarget : ActionTask <Blackboard> 
	{
		protected override string info
		{ 
			get 
			{ 
				return string.Format("Set Time Bonus Coin Particle Target");
			} 
		}
		protected override void OnExecute()
		{
			var root = agent.GetComponent<ContextElement>();
			var particleElement = ContextUtils.FindElement(root, "Anchor/Particle Coin Up", ContextSearchingType.FullNameSearch);
			var particle = particleElement?.GetComponent<ParticleSystem>();

			var targetTransform = MetaGameAppearTransformManager.Instance.GetTransform("NavigationUserCoinParticleTarget");
			var targetAttractor = targetTransform?.GetComponent<Coffee.UIExtensions.UIParticleAttractor>();
			targetAttractor?.RegistParticle(particle);
			EndAction(true);
		}
	}
}
