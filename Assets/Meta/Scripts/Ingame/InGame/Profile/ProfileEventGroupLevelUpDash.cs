using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    public class ProfileEventGroupLevelUpDash : ProfileEventGroup
    {
        protected override float DisplayTime => GetDisplayTime();

        private LevelUpDashEventTagController tagController;

        public ProfileEventGroupLevelUpDash(ContextElement _root) : base(_root)
        {

        }

        private float GetDisplayTime()
        {
            float time = 0f;

            if (LevelUpDash.LevelUpDash.Utils.IsActiveLevelUpDash())
            {
                time += 8f;

                int limit = LevelUpDash.LevelUpDash.Defines.MAX_REMAINING_LEVEL_FOR_DISPLAYING_LEFT_LEVEL;
                if (LevelUpDash.LevelUpDash.Utils.IsLevelRemainingLessThan(limit + 1))
                    time += 4f;
            }

            if (LevelUpDash.LevelUpDash.Utils.IsActivePurchaseBooster()) time += 4f;

            return time;
        }

        public override bool IsAvailable()
        {
            return LevelUpDash.LevelUpDash.Utils.IsActiveLevelUpDash() ||
                (LevelUpDash.LevelUpDash.Utils.IsActiveLevelUpDashCampaign() &&
                LevelUpDash.LevelUpDash.Utils.IsActivePurchaseBooster());
        }

        protected override void Initialize()
        {
            if (isInit) return;
            isInit = true;

            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Event Tag Level Up Dash";
            Transform parent = eventTagAreaElement.transform;

            tagObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);
            anim = tagObj.GetComponent<Animator>();
            anim.SetBool("Appear", false);

            tagController = tagObj.GetComponent<LevelUpDashEventTagController>();
            tagController.InitTag();
        }

        protected override void OnAppearTag()
        {
            tagController.UpdateTag();
        }
    }
}
