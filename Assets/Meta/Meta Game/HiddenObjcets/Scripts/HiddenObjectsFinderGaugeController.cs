using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using System.Collections;

namespace BagelCode.HiddenObjects
{
    public class HiddenObjectsFinderGaugeController : EventMonoBehaviour
    {
        private const float FINDER_GAUGE_MIN = 0.12f;

        private ContextElement root;
        private Blackboard bb;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;

        private void Start()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();

            root.UpdateContext(false);

            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
            Register(MetaEventDefine.ON_META_UI_EVENT, HiddenObjects.Events.ON_UPDATE_FINDER_COUNT, UpdateFinderCount);

            StartCoroutine(LateStartCoroutine());
        }

        protected override void OnEnable() { }
        protected override void OnDisable() { }

        private void OnDestroy()
        {
            base.OnDisable();
        }

        private IEnumerator LateStartCoroutine()
        {
            yield return new WaitForEndOfFrame();
            LateStart();
        }

        private void LateStart()
        {
            var hideShopButton = bb.GetVariable<bool>("hideShopButton")?.value ?? false;
            var mainScene = HiddenObjects.Utils.MainScene;

            bool isFinderShopEnabled = BlackboardQueryUtils.IsFinderShopEnabled();
            bool isBundleShopEnabled = BlackboardQueryUtils.IsFinderBundleShopEnabled(out _);
            bool showShopButton = (isFinderShopEnabled || isBundleShopEnabled) && !hideShopButton;
            if (showShopButton)
            {
                MetaContextElementUtils.SetClickable(root, () => EventSender.SendEvent(
                    mainScene, HiddenObjects.Events.ON_CLICK_FINDER_SHOP));
            }

            MetaContextElementUtils.SimpleSetActive(root, "Plus Icon", showShopButton, CHILDREN);

            UpdateFinderCount();
        }

        private void UpdateFinderCount()
        {
            int finder = HiddenObjects.Utils.Finder;
            int maxFinder = HiddenObjects.Utils.MaxFinder;
            float ratio = (float)finder / maxFinder;
            ratio = Mathf.Lerp(FINDER_GAUGE_MIN, 1f, ratio);
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Text", "A_PER_B", CHILDREN, finder, maxFinder);
            MetaContextElementUtils.SimpleSetSliderValue(root, "Progress Bar", ratio, CHILDREN);
        }
    }
}
