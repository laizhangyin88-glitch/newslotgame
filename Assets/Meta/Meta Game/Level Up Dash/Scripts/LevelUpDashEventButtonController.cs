using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using ParadoxNotion;

namespace BagelCode.LevelUpDash
{
    public class LevelUpDashEventButtonController : EventMonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;
        private Animator anim;

        private ContextElement metaGameAreaElement;

        private bool isInGame;

        private bool isActive;

        [Sirenix.OdinInspector.Button]
        private void TestActiveLevelUpDash()
        {
            EventSender.SendEvent(gameObject, MetaEventDefine.ON_META_UI_EVENT, LevelUpDash.Events.ACTIVE_LEVEL_UP_DASH);
        }

        public void Init()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            isInGame = BlackboardQueryUtils.IsIngame();

            metaGameAreaElement = ContextUtils.FindElement(root, "State Meta Game", ContextSearchingType.ChildrenSearch);

            UpdateLevelUpDashState();

            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
            Register(MetaEventDefine.ON_META_UI_EVENT, LevelUpDash.Events.ON_UPDATE_LEVEL_UP_DASH, UpdateLevelUpDashState);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (anim != null)
            {
                anim.SetBool("Active", isActive);
            }
        }

        public void MakeLevelUpDashButton()
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = isInGame ?
                "Level Up Dash In Game Button Scene" :
                "Level Up Dash Lobby Button Scene";
            Transform parent = metaGameAreaElement.transform;

            var buttonObj = MetaObjectUtils.MakeScene(bundle, asset, parent, "", false);
            MetaObjectUtils.SetCalleeCaller(buttonObj, gameObject);

            var eventData = new EventData<GameObject>("OnMakeEventButton", buttonObj);
            EventSender.SendCalleeCallback(gameObject, eventData);
        }

        public void ShowButton()
        {
            isActive = true;

            if (anim != null)
                anim.SetBool("Active", isActive);
        }

        public void HideButton()
        {
            isActive = false;

            if (anim != null)
                anim.SetBool("Active", isActive);
        }

        private void UpdateLevelUpDashState()
        {
            bool isActive = LevelUpDash.Utils.IsActiveLevelUpDash();
            BlackboardUtils.SetOrCreateValue(bb, "isActiveLevelUpDash", isActive);
        }
    }
}
