using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;
using System.Linq;

using static BagelCode.KudoEventManager;

namespace BagelCode
{
    public abstract class KudoData
    {
        public bool isUsable = false;

        public KudoData()
        {
            isUsable = false;
            sceneName = GetKudoSceneName();

            if(MakeKudoScene())
            {
                InitProperty();
                isUsable = true;
            }
        }

        public KudoController controller;

        protected Blackboard currentInfo;

        protected ContextElement root;
        protected Blackboard bb;
        protected Animator anim;

        private string sceneName;

        public List<Blackboard> feedInfoList;

        private List<GameObject> willDestroyIconList = new List<GameObject>();

        protected ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        protected ContextSearchingType FULL = ContextSearchingType.FullNameSearch;
        protected StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

        //

        protected abstract string GetKudoSceneName();
        protected abstract IEnumerator UpdateKudoData(Blackboard info);

        public virtual IEnumerator DisplayKudoCoroutine()
        {
            while (feedInfoList.Count > 0)
            {
                var feed = feedInfoList.First();
                if (feed != null)
                {
                    yield return controller.StartCoroutine(UpdateKudoData(feed));
                    OnDisappearKudo();
                }
            }
        }

        protected virtual void InitProperty()
        {
            root = controller.GetComponent<ContextElement>();
            bb = controller.GetComponent<Blackboard>();
            anim = controller.GetComponent<Animator>();

            root.UpdateContext(false);

            MetaContextElementUtils.SimpleSetClickable(
                root,
                "Button Accept",
                () => EventSender.SendEvent(controller.gameObject, ON_ACCEPT));

            MetaContextElementUtils.SimpleSetClickable(
                root,
                "Button Close",
                () => EventSender.SendEvent(controller.gameObject, ON_SKIP));
        }

        protected virtual void OnDisappearKudo(bool removeAll = false)
        {
            willDestroyIconList.ForEach(o => GameObject.Destroy(o));
            if (removeAll)
            {
                while (feedInfoList.Count > 0)
                {
                    BlackboardUtils.RemoveAtBlackboardList(bb, "feedInfoList", 0);
                }
            }
            else
            {
                BlackboardUtils.RemoveAtBlackboardList(bb, "feedInfoList", 0);
            }
        }

        //

        public void AddFeedToList(Blackboard feed)
        {
            var newFeedBB = (Blackboard)BlackboardUtils.CreateBlackboard("feed");
            BlackboardUtils.CopyBlackboard(feed, newFeedBB);
            BlackboardUtils.AddToBlackboardList(bb, "feedInfoList", newFeedBB);

            if (feedInfoList == null)
                feedInfoList = bb.GetValue<List<Blackboard>>("feedInfoList");
        }

        protected void ActiveAnimator()
        {
            anim.SetBool("IsActive", true);
        }

        protected IEnumerator DisappearCoroutine()
        {
            anim.SetBool("IsActive", false);
            var onDisappearTrigger = new EventTrigger(controller, ON_DISAPPEAR);
            yield return new WaitUntilTrigger(onDisappearTrigger);

            if(anim.GetBool("IsAccepted"))
            {
                anim.SetBool("IsAccepted", false);
                yield return new WaitForSeconds(KUDO_ACCEPTED_DISPLAY_TIME);
            }
        }

        protected void DeactiveBanner()
        {
            MetaContextElementUtils.SimpleSetActive(root, "Banner", false, CHILDREN);
        }

        protected void ActiveJackpotImageArea()
        {
            MetaContextElementUtils.SimpleSetActive(root, "Jackpot Info Area/Image Area", true, FULL);
        }

        protected void MakeClubIcon(Blackboard info)
        {
            string symbolName = info.GetValue<string>("clubSymbol");
            Transform parent = controller.transform;
            string parentName = "Anchor/Layout/Club Info Area/Anchor/Club Symbol Area";
            GameObject iconObj = MetaIconUtils.MakeClubSymbolIconObject(symbolName, parent, parentName);
            willDestroyIconList.Add(iconObj);
        }

        protected void MakeTierIcon(Blackboard info, bool destroyOnDisappear = true)
        {
            int clubTier = info.GetValue<int>("clubLeagueTier");
            Transform parent = controller.transform;
            string parentName = "Anchor/Layout/Club Info Area/Anchor/Club Tier Area";
            GameObject iconObj = MetaIconUtils.MakeClubTierIconObject(clubTier, parent, parentName);

            if (destroyOnDisappear)
                willDestroyIconList.Add(iconObj);
        }

        protected void MakeSlotThumbnailIcon(Blackboard gameInfo, bool isJackpot, bool destroyOnDisappear = true)
        {
            string gameTitle = gameInfo.GetValue<string>("gameTitle");
            Transform parent = controller.transform;
            string parentName = isJackpot ? "Anchor/Layout/Jackpot Info Area/Anchor/Image Area" :
                "Anchor/Layout/Kudo Center Group Area/Invite Thumbnail Area/Anchor";
            GameObject iconObj = MetaIconUtils.MakeSlotThumbnailIconObjectFromGameTitle(gameTitle, parent, parentName);

            if (destroyOnDisappear)
                willDestroyIconList.Add(iconObj);
        }

        protected void MakeWheelIcon(bool destroyOnDisappear = true)
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Icon Wheel";
            var imageAreaElement = ContextUtils.FindElement(root, "Jackpot Info Area/Image Area", FULL);
            Transform parent = imageAreaElement.transform;
            GameObject iconObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);

            if (destroyOnDisappear)
                willDestroyIconList.Add(iconObj);
        }

        protected void SendBiKudo(Blackboard info, string type, string kudoType)
        {
            string targetUserId = info.GetValue<string>("userId");
            int kudoId = info.GetValue<int>("kudoId");
            SendBiKudo(type, targetUserId, kudoId, kudoType);
        }

        protected void SendBiKudo(string type, string kudoType)
        {
            SendBiKudo(type, null, 0, kudoType);
        }

        protected void SendBiKudo(string type, string targetUserId, int kudoId, string kudoType)
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();

            // type: string ("trigger", "click"),
            // target_user_id: string,
            // kudo_id: number,
            // kudo_type: string ("kudo_jackpot", "kudo_tournament", "kudo_receive"),

            customData["type"] = type;
            customData["target_user_id"] = targetUserId;
            customData["kudo_id"] = kudoId;
            customData["kudo_type"] = kudoType;
            customData["slot_enter_context_id"] = BiEventUtils.GetSlotEnterContextID();

            Analytics.CustomEvent("client_kudo", customData);
        }

        protected IEnumerator OnAcceptCoroutine(Blackboard info, string kudoType, KudoLikeType kudoLikeType)
        {
            SendBiKudo(info, "click", kudoType);
            UpdateUserSyncInfo(info, kudoLikeType, kudoType);
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Like Text", "FEED_ACCEPTED", CHILDREN);
            anim.SetBool("IsAccepted", true);

            yield return new WaitForSeconds(KUDO_ACCEPTED_DISPLAY_TIME);
        }

        protected void UpdateUserSyncInfo(Blackboard info, KudoLikeType likeType, string kudoType)
        {
            string targetUserId = info.GetValue<string>("userId");
            int kudoId = info.GetValue<int>("kudoId");
            BagelCodeClientAPI.Kudo(targetUserId, kudoId, likeType, kudoType,
            (response) =>
            {
                if (response.error == Error.OK)
                    BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
            },
            (error) => { });
        }

        protected void SetCenterTextElement(string textKey, object args = null, object args2 = null)
        {
            MetaContextElementUtils.SimpleSetActive(root, "Center Kudo Text", true);
            if(args2 != null)
            {
                MetaContextElementUtils.SimpleSetTextGlobal(root, "Center Kudo Text", textKey, FULL, args, args2);
            }
            else if (args != null)
            {
                MetaContextElementUtils.SimpleSetTextGlobal(root, "Center Kudo Text", textKey, FULL, args);
            }
            else
            {
                MetaContextElementUtils.SimpleSetTextGlobal(root, "Center Kudo Text", textKey, FULL);
            }
        }

        protected void SetButtonTextActive(int idx, bool setTo)
        {
            string elementPath = string.Format("Button Accept/Text 0{0}", idx + 1);
            MetaContextElementUtils.SimpleSetActive(root, elementPath, setTo, FULL);
        }

        protected void SetButtonTextReceiveRp()
        {
            SetText(0, "FEED_BUTTON_KUDO_TEXT");
            long earnRp = BlackboardUtils.FindValue<long>("/values/reward/SEND_KUDO/rp");
            SetText(1, "FEED_BUTTON_KUDO_RP", earnRp);
        }

        protected void SetText(int idx, string textKey, object args = null)
        {
            string elementPath = string.Format("Button Accept/Text 0{0}", idx + 1);
            if(args != null)
            {
                MetaContextElementUtils.SimpleSetTextGlobal(root, elementPath, textKey, FULL, args);
            }
            else
            {
                MetaContextElementUtils.SimpleSetTextGlobal(root, elementPath, textKey, FULL);
            }
        }

        protected void SetJackpotText(string textKey)
        {
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Jackpot Info Area/Text", textKey, FULL);
        }

        protected void SetFlexibleText(string textKey, object args = null)
        {
            if(args != null)
            {
                MetaContextElementUtils.SimpleSetTextGlobal(root, "Flexible Kudo Text", textKey, CHILDREN, args);
            }
            else
            {
                MetaContextElementUtils.SimpleSetTextGlobal(root, "Flexible Kudo Text", textKey, CHILDREN);
            }
        }

        protected void SetProfileActive(int count, bool setTo)
        {
            for(int i = 0; i < count; ++i)
            {
                string elementPath = string.Format("Profile Area {0}", i);
                MetaContextElementUtils.SimpleSetActive(root, elementPath, setTo, CHILDREN);
            }
        }

        protected IEnumerator SetProfileCoroutine(int idx, Blackboard info, bool useProfileBB = false)
        {
            var profileElement = ContextUtils.FindElement(root, string.Format("Profile Area {0}", idx), CHILDREN);
            var pictureElement = ContextUtils.FindElement(profileElement, "Profile Picture Normal", CHILDREN);
            var photoImageElement = ContextUtils.FindElement(pictureElement, "Image", CHILDREN);

            // Set Element Active
            MetaContextElementUtils.SetActive(profileElement, info != null);
            if (info == null) yield break;

            if (useProfileBB)
            {
                info = info.GetValue<Blackboard>("profile");
            }

            // Set Photo Image
            string profileUrl = info.GetVariable<string>("profileUrl")?.value;
            MetaContextElementUtils.SetWebImage(photoImageElement, profileUrl, CacheType.MemCache, false, null);

            // Set Tier Group
            int tier = info.GetValue<int>("tier");
            int tierGroup = MetaSystem.GetTierGroup(tier);
            MetaContextElementUtils.SetPropertySafty(pictureElement, tierGroup);
        }

        protected IEnumerator SetFakeProfileCoroutine(int idx, string profileUrl, bool useProfileBB = false)
        {
            var profileElement = ContextUtils.FindElement(root, string.Format("Profile Area {0}", idx), CHILDREN);
            var pictureElement = ContextUtils.FindElement(profileElement, "Profile Picture Normal", CHILDREN);
            var photoImageElement = ContextUtils.FindElement(pictureElement, "Image", CHILDREN);

            // Set Element Active
            MetaContextElementUtils.SetActive(profileElement, true);
            
            // Set Photo Image
            MetaContextElementUtils.SetWebImage(photoImageElement, profileUrl, CacheType.MemCache, false, null);

            // Set Tier Group
            int tier = (int)Random.Range(3f, 12f);
            int tierGroup = MetaSystem.GetTierGroup(tier);
            MetaContextElementUtils.SetPropertySafty(pictureElement, tierGroup);

            yield break;
        }

        protected void SetUserNameText(string textKey, object args = null, object args2 = null)
        {
            if (args2 != null)
            {
                MetaContextElementUtils.SimpleSetTextGlobal(
                    root, "User Name Kudo Text", textKey, CHILDREN, args, args2);
            }
            else if (args != null)
            {
                MetaContextElementUtils.SimpleSetTextGlobal(
                    root, "User Name Kudo Text", textKey, CHILDREN, args);
            }
            else
            {
                MetaContextElementUtils.SimpleSetTextGlobal(
                    root, "User Name Kudo Text", textKey, CHILDREN);
            }
        }

        //

        private bool MakeKudoScene()
        {
            if(string.IsNullOrEmpty(sceneName))
            {
                if (ApplicationSettings.LogTest())
                    Debug.LogError("KudoData.MakeKudoPrefab failure. prefabName is null or empty.");
                return false;
            }

            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string kudoAreaPath = "Popup Manager/Interaction/In Game Interaction/Kudo Area";
            GameObject parentObj = GameObject.Find(kudoAreaPath);
            if(parentObj == null)
            {
                if (ApplicationSettings.LogTest())
                    Debug.LogError("Parent is null");
                return false;
            }

            Transform parent = parentObj.transform;

            var kudoObj = MetaObjectUtils.MakeScene(bundle, sceneName, parent, "", false);

            var kudoController = kudoObj.GetComponent<KudoController>();
            if(kudoController == null)
            {
                if (ApplicationSettings.LogTest())
                    Debug.LogError("KudoData.MakeKudoPrefab failure. kudoObj hasn't <KudoController>");
                return false;
            }

            controller = kudoController;
            return true;
        }

        protected void MakeKudoIconPrefab(ContextElement iconAreaElement, string iconName, bool destroyOnDisappear = true)
        {
            if (iconAreaElement == null || string.IsNullOrEmpty(iconName))
                return;

            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            GameObject iconObj = MetaObjectUtils.MakePrefab(bundle, iconName, iconAreaElement.transform);

            if (destroyOnDisappear)
                willDestroyIconList.Add(iconObj);
        }

        protected string GetBossRaidersIconName(int themeId = -1)
        {
            string iconName = "";
            if (themeId < 0)
            {
                EventInfo eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.BOSS_RAIDERS, true);
                if (eventInfo != null)
                    iconName = string.Format("Kudo Boss Raiders Kudo Icon_{0}", ((EventDataBossRaiders)eventInfo.constraints).themeId);
            }
            else
                iconName = string.Format("Kudo Boss Raiders Kudo Icon_{0}", themeId);

            return iconName;
        }
    }
}
