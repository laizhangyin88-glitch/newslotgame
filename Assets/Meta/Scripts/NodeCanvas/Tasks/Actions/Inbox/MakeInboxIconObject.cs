using System;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using BagelCode.ClientModels;
using System.Collections.Generic;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Inbox")]
    public class MakeInboxIconObject : ActionTask<ContextElement>
    {
        public BBParameter<Blackboard>  inboxInfoBB;
        public BBParameter<GameObject> saveAs;
        public BBParameter<InboxCellData.IconType> iconType;
        public BBParameter<string> imageUrl;
        public BBParameter<bool> isLoaded;
        
        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;
        
        private const string ICON_AREA = "Icon Area";
        private const string COUPON_AREA = "Coupon Area";
        private const string IMAGE_TIER_AREA = "Image Tier Area";

        protected override void OnExecute()
        {
            if (saveAs != null)
                GameObject.Destroy(saveAs.value);

            if (iconType.value == InboxCellData.IconType.EMPTY || iconType.value == InboxCellData.IconType.GAME_THUMBNAIL)
            {
                EndAction(true);
                return;
            }
            
            string area = ICON_AREA;
            string iconName = StringTableUtils.GetString(tableType, String.Format("INBOX_ICON_{0}", iconType.value.ToString()));
            
            switch (iconType.value)
            {
                case InboxCellData.IconType.PURCHASE_COUPON:
                    area = COUPON_AREA;
                    break;
                case InboxCellData.IconType.TIER_UPGRADE:
                    area = IMAGE_TIER_AREA;
                    break;
            }
            
            ContextElement element = ContextUtils.FindElement(agent, area, ContextSearchingType.FullNameSearch);
            Transform parent = element.GetComponent<Transform>();
            
            saveAs.value = MetaIconUtils.MakeInboxIconObjectFromName(iconName, parent, "");
            element.UpdateContext(true);

            if (iconType.value == InboxCellData.IconType.WEB_IMAGE)
            {
                isLoaded.value = false;
                ContextElement webImageElement = ContextUtils.FindElement(agent, "Icon Area/Inbox Icon Web Image", ContextSearchingType.FullNameSearch);
                MetaContextElementUtils.SetWebImage(
                    webImageElement,
                    imageUrl.value,
                    CacheType.FileCache,
                    false,
                    () => { isLoaded.value = true; }
                );
            }
            
            EndAction(true);
        }
    }
}