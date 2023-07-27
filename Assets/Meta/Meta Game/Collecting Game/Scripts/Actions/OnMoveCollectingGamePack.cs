using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Scratcher.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Collecting Game")]
    public class OnMoveCollectingGamePack : ActionTask<ContextElement>
    {
        private GameObject pack;
        public BBParameter<string> bundle;
        public BBParameter<Transform> button;

        protected override string info
        {
            get { return "On Move Collecting Game Pack"; }
        }

        protected override void OnExecute()
        {
            Blackboard inGameInfo = BlackboardQueryUtils.GetMetaGameEnterInfo();

            int packId = inGameInfo.GetVariable<int>("packId")?.value ?? -1;
            if (packId == -1)
            {
                if (ApplicationSettings.LogTest())
                    Debug.LogError("OnMoveCollectingGamePack.OnExecute failure. /metaGameEnterInfo/packId is empty.");

                BlackboardQueryUtils.CollectingGameMovePackComplete();
                EndAction();
                return;
            }

            pack = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, string.Format("Collecting Game Chest 0{0}", (packId - 1) % 4 + 1), agent.transform);
            pack.SetActive(false);

            Transform target = button.value;

            if (target != null)
            {
                pack.GetComponent<DirectionalWeightPositionController>().@from = target;
                pack.GetComponent<DirectionalWeightPositionController>().@to = agent.transform;
                pack.SetActive(true);
            }
            else
            {
                EndAction();
            }
        }

        protected override void OnUpdate()
        {
            if (elapsedTime >= 0.5f && Vector3.SqrMagnitude(pack.transform.position - agent.transform.position) < 0.05f * 0.05f)
            {
                BlackboardQueryUtils.CollectingGameMovePackComplete();
                EndAction();
            }

        }

        protected override void OnStop()
        {
            if(pack != null)
            {
                pack.DestroyThis();
            }
        }
    }
}
