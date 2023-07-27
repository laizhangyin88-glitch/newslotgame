using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace BagelCode
{
	[Category("★ BagelCode/Utils")]
	public class RefreshCanvasGroupInteractable : ActionTask
	{
		[BlackboardOnly]
		public BBParameter<bool> interactable;

		protected override void OnExecute()
		{
			UnityEngine.CanvasGroup canvasGroup = agent.gameObject.GetComponent<UnityEngine.CanvasGroup>();

			bool interactableValue = interactable.value;

			canvasGroup.interactable = !interactableValue;
			canvasGroup.interactable = interactableValue;

			EndAction();
		}
	}
}