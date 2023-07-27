using UnityEngine;
using SlotMaker;
using System.Collections;

namespace BagelCode
{
    public class LobbyVipPayDealController : EventMonoBehaviour
    {
        private ContextElement root;
        private EventTagController eventTagController;

        private ContextElement stateDownloadingElement;
        private ContextElement stateButtonElement;

        private ContextElement progressElement;
        private ContextElement progressTextElement;

        private bool isInit = false;
        private bool isLoaded = false;
        private bool setSprite = false;
        private Sprite loadedSprite = null;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;

        private SimpleReserveTimer _reserveTimer;
        private SimpleReserveTimer reserveTimer
        {
            get
            {
                if (_reserveTimer == null)
                {
                    _reserveTimer = GetComponent<SimpleReserveTimer>();
                    if (_reserveTimer == null)
                        _reserveTimer = gameObject.AddComponent<SimpleReserveTimer>();
                }
                return _reserveTimer;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateVariables();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            reserveTimer.Stop();
        }

        private void Update()
        {
            if (isLoaded && !setSprite)
            {
                setSprite = true;
                UpdateProgress(1f);

                var webImageElement = ContextUtils.FindElement(stateButtonElement, "Multiplier Web Image", CHILDREN);
                MetaContextElementUtils.SetSprite(webImageElement, loadedSprite);
                MetaContextElementUtils.SetActive(stateDownloadingElement, false);
                MetaContextElementUtils.SetActive(stateButtonElement, true);
            }
        }

        private void InitProperty()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();
            root.UpdateContext(true);

            stateDownloadingElement = ContextUtils.FindElement(root, "State Downloading", CHILDREN);
            stateButtonElement = ContextUtils.FindElement(root, "State Button", CHILDREN);

            var eventTimerArea = ContextUtils.FindElement(stateButtonElement, "Event Timer Area", CHILDREN);
            var eventTagObject = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Event Tag Without Text", eventTimerArea.transform);
            eventTagController = eventTagObject.GetComponent<EventTagController>();
            
            MetaContextElementUtils.SetClickable(root, OnClick);

            DownloadWebImage();

            isInit = true;
        }

        private void DownloadWebImage()
        {
            progressElement = ContextUtils.FindElement(stateDownloadingElement, "Gauge", CHILDREN);
            progressTextElement = ContextUtils.FindElement(stateDownloadingElement, "Text Gauge", CHILDREN);

            UpdateProgress(0f);

            string webImageUrl = VipDealV2.Utils.GetIconWebImageUrl();

            // Load Sprite
            WebImageDownloader.Instance.LoadWebImage(
                webImageUrl, CacheType.FileCache, false,
                null,
                (Sprite img) =>
                {
                    isLoaded = true;
                    loadedSprite = img;
                },
                UpdateProgress,
                null
            );
        }

        private void UpdateProgress(float progress)
        {
            MetaContextElementUtils.SetTextGlobal(progressTextElement, "TEXT_PERCENTAGE", progress);
            MetaContextElementUtils.SetFloatProperty(progressElement, progress);
        }

        private void OnClick()
        {
            MetaContextElementUtils.SetBooleanProperty(root, false); // disable click

            AEUtils.SendAE("client_click_vip_deal", ("type", "Lobby"));

            GSManager.Instance.GetHandler(VipDealV2.Defines.SOUND_LOBBY_DEAL_BUTTON_CLICK);
            EventSender.SendGlobalMetaEvent(VipDealV2.Events.ON_ENTER_VIP_DEAL);
        }

        public void UpdateVariables()
        {
            InitProperty();

            var vipDealInfo = VipDealV2.Utils.GetActiveInfo();
            if (vipDealInfo != null)
            {
                var endTimestamp = vipDealInfo.GetValue<long>("endTimestamp");

                eventTagController.Initialize(endTimestamp,
                                                "TIME_FORMAT_HHMMSS_TOTALHOUR",
                                                null,
                                                "Ended",
                                                true,
                                                null,
                                                null
                );

                reserveTimer.SetReserveCallback(endTimestamp,
                    () =>
                    {
                        gameObject.SetActive(false);
                    }
                );
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
