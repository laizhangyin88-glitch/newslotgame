using System.Collections;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;

using static BagelCode.KudoEventManager;

namespace BagelCode
{
    public class KudoDataBossRaidersRoundComplete : KudoData
    {
        private ContextElement iconAreaElement;

        protected override string GetKudoSceneName()
        {
            return "Kudo Boss Raiders Round Complete Scene";
        }

        protected override void InitProperty()
        {
            base.InitProperty();

            iconAreaElement = ContextUtils.FindElement(root, "Boss Raiders Icon Area", ContextSearchingType.ChildrenSearch);

            // Set Flexible Text
            SetFlexibleText("BOSS_RAIDERS_KUDO_ROUND");
        }

        protected override IEnumerator UpdateKudoData(Blackboard info)
        {
            // Check Meta Scene
            var currentSceneState = BlackboardUtils.GetOrCreateVariable<SceneState>("/currentSceneState");
            bool isMetaScene = currentSceneState.value == SceneState.EVENT_META_GAME;

            if (isMetaScene) yield break;

            Blackboard clubInfoBB = BlackboardUtils.FindVariable<Blackboard>(null, "/clubInfo")?.value ?? null;
            if (clubInfoBB == null) yield break;

            BlackboardUtils.SetOrCreateValue<string>(info, "clubSymbol", BlackboardUtils.FindValue<string>(clubInfoBB, "symbol"));

            MakeKudoIconPrefab(iconAreaElement, GetBossRaidersIconName(info.GetVariable<int>("themeId")?.value ?? -1));

            // Active Animator
            ActiveAnimator();

            // Send Bi
            SendBiKudo("trigger", "kudo_boss_raiders_round_complete");

            // Make Club Icon
            MakeClubIcon(info);

            // Wait | Skip
            var timerTrigger = new TimerTrigger(KUDO_DISPLAY_TIME);
            var onSkipTrigger = new EventTrigger(controller, ON_SKIP);
            yield return new WaitUntilTrigger(timerTrigger, onSkipTrigger);

            // Disappear
            yield return controller.StartCoroutine(DisappearCoroutine());
        }
    }
}
