using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/ClientAPI")]
    public class RequestChallengeInfo : ActionTask<Blackboard>
    {
        protected override string info { get { return "Request Challenge Info"; } }

        protected override void OnExecute()
        {
            agent.StartCoroutine(RequestCoroutine());
        }

        private IEnumerator RequestCoroutine()
        {
            yield return agent.StartCoroutine(ChallengeUtils.RequestChallengeInfo(agent, agent.gameObject));

            bool isSuccess = agent.GetVariable<bool>("isChallengeInfoRequestSuccess")?.value ?? false;
            EndAction(isSuccess);
        }
    }
}