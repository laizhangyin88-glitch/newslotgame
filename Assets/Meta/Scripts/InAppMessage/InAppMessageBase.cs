using UnityEngine;
using NodeCanvas.Framework;
using SlotMaker;
using BagelCode.ClientModels;
using System.Collections.Generic;

namespace BagelCode.InAppMessage
{
    public class InAppMessageBase : EventMonoBehaviour
    {
        //
        public bool isPreview = false;
        public Blackboard iamInfo = null;
        public string bundleName = "";
        public long endTimestamp = 0L;

        [System.NonSerialized] public Transform parent = null;
        [System.NonSerialized] public Transform scrollAnchor = null;
        [System.NonSerialized] public Transform loadingArea = null;
        [System.NonSerialized] public GameObject loadingObj = null;
        //

        protected ContextElement root;
        protected Blackboard bb;
        protected Animator anim;

        // component object list
        protected List<GameObject> componentObjList = null;

        private bool isInit = false;

        protected const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        protected const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        protected const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

        public void InitProperty()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext(true);

            parent  = transform.Find("Anchor");
            scrollAnchor = transform.Find("Scroll View Anchor");
            loadingArea = transform.Find("Loading Area");

            isInit = true;
        }

        public virtual void LoadIAM(string _bundleName, Blackboard _iamInfo, long _endTimestamp)
        {
            InitProperty();

            bundleName = _bundleName;
            iamInfo = _iamInfo;
            endTimestamp = _endTimestamp;

            componentObjList = IAMUtils.MakeComponents(this);

            // BackButton Lock.
            MetaSystem.SubscribeBackButton(gameObject.GetHashCode(), () => { });

            bool useCloseButton = true;
            var iamType = iamInfo.GetValue<InAppMessageType>("type");
            if (iamType == InAppMessageType.TERMS_OF_USE_POPUP)
                useCloseButton = false;

            if (useCloseButton) MakeCloseButton(bundleName);

            anim.SetTrigger("Active");
        }

        public void MakeCloseButton(string bundleName)
        {
            Transform parentTransform = transform.Find("Button Close Anchor").transform;
            parentTransform.SetAsLastSibling();

            var closeButton = MetaObjectUtils.MakePrefab(bundleName, "Button Close Timer", parentTransform);
            var closeButtonElement = closeButton.GetComponent<ContextElement>();
            var closeButtonTimer = closeButton.GetComponent<CloseButtonTimer>();

            var locktimeSec = BlackboardUtils.FindVariable<int>(iamInfo, "locktimeSec").value;
            if (locktimeSec > 0)
            {
                closeButtonTimer.StartTimer((float)locktimeSec);
                closeButtonTimer.onTimerState.AddListener((bool isActiveTimer) =>
                {
                    if (!isActiveTimer)
                        UpdateCloseButtonEvent(closeButtonElement);
                });
            }
            else
            {
                closeButtonTimer.StopTimer();
                UpdateCloseButtonEvent(closeButtonElement);
            }
        }

        private void UpdateCloseButtonEvent(ContextElement closeButtonElement)
        {
            // Android BackButton Unlock.
            MetaSystem.UnSubscribeBackButton(gameObject.GetHashCode());

            // Close Button Event
            MetaSystem.SubscribeBackButton(gameObject.GetHashCode(),
                () => EventSender.SendEvent(gameObject, "OnBackButtonClose"));

            MetaContextElementUtils.SetClickable(
                closeButtonElement,
                "OnClickClose",
                root,
                null
            );
        }

        private void OnDestroy()
        {
            if (MetaSystem.Instance != null)
                MetaSystem.UnSubscribeBackButton(gameObject.GetHashCode());
        }
    }
}
