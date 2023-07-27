using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode.EpicAlbum
{
    public class EpicAlbumCellController : MonoBehaviour
    {
        public int gameId;
        public Blackboard gameInfo;
        public Blackboard woeInfo;
        public CategoryType categoryType;
        public List<GameObject> starList = new List<GameObject>();
        
        private ContextElement agent;
        private PIDButton button;
        private int meLevel;
        private GameObject thumbObject;
        
        private ContextElement slotEmpty;
        private ContextElement epicWinDefault;
        private ContextElement epicWinDefaultText;
        private ContextElement epicWin;
        private ContextElement epicWinImage;
        private ContextElement epicWinLoading;
        private ContextElement imageMagnifier;
        private ContextElement newEpic;
        private ContextElement slotLock;
        private ContextElement textSlotLock;
        private ContextElement slotName;
        private ContextElement textSlotName;
        private ContextElement badgeArea;

        private ContextElement parent;
        
        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;
        private const int STAR_COUNT = 3;
        
        public void Init()
        {
            agent = GetComponent<ContextElement>();
            button = GetComponent<PIDButton>();
            meLevel = BlackboardUtils.FindVariable<int>(null, "/me/level").value;

            slotEmpty = ContextUtils.FindElement(agent, "Slot Empty", ContextSearchingType.ChildrenSearch);
            epicWinDefault = ContextUtils.FindElement(agent, "Epic Win Default", ContextSearchingType.ChildrenSearch);
            epicWinDefaultText = ContextUtils.FindElement(epicWinDefault, "Text", ContextSearchingType.ChildrenSearch);
            epicWin = ContextUtils.FindElement(agent, "Epic Win", ContextSearchingType.ChildrenSearch);
            epicWinImage = ContextUtils.FindElement(epicWin, "Image", ContextSearchingType.ChildrenSearch);
            epicWinLoading = ContextUtils.FindElement(epicWin, "Loading", ContextSearchingType.ChildrenSearch);
            imageMagnifier = ContextUtils.FindElement(agent, "Image Magnifier", ContextSearchingType.ChildrenSearch);
            newEpic = ContextUtils.FindElement(agent, "New Epic", ContextSearchingType.ChildrenSearch);
            slotLock = ContextUtils.FindElement(agent, "Slot Lock", ContextSearchingType.ChildrenSearch);
            textSlotLock = ContextUtils.FindElement(slotLock, "Text Slot Lock", ContextSearchingType.ChildrenSearch);
            slotName = ContextUtils.FindElement(agent, "Slot Name", ContextSearchingType.ChildrenSearch);
            textSlotName = ContextUtils.FindElement(slotName, "Text Slot Name", ContextSearchingType.ChildrenSearch);
            badgeArea = ContextUtils.FindElement(agent, "Badge Area", ContextSearchingType.ChildrenSearch);

            for(int i = 0; i < STAR_COUNT; ++i)
            {
                var starElement = ContextUtils.FindElement(agent, string.Format("Star On {0}", i+1), ContextSearchingType.ChildrenSearch);
                starElement.gameObject.SetActive(false);
                starList.Add(starElement.gameObject);
            }
        }

        public void Refresh()
        {
            Refresh(categoryType, gameId);
        }
        
        public void Refresh(CategoryType categoryType, int gameId)
        {
            this.categoryType = categoryType;
            this.gameId = gameId;
            
            gameInfo = gameId != -1 ? BlackboardQueryUtils.GetGameInfo(gameId) : null;
            woeInfo = gameId != -1 ? BlackboardQueryUtils.GetWOEInfo(categoryType, gameId) : null;
            int starCount = 0;

            if (gameId == -1 || gameInfo == null)
            {
                MetaContextElementUtils.SetActive(agent, false);
            }
            else
            {
                MetaContextElementUtils.SetActive(agent, true);
                RefreshThumbnail();
                RefreshSlotName();
                RefreshBadge();
                
                if (woeInfo != null)
                {
                    BlackboardQueryUtils.SetNewWOEForCategory(woeInfo.GetValue<int>("id"), false);
                    starCount = woeInfo.GetValue<int>("starCount");
                }
            }

            for(int i = 0; i < STAR_COUNT; ++i)
                starList[i].SetActive(starCount >= i+1);
        }

        private void SetInteractable(bool active)
        {
            button.interactable = active;
        }
        
        private void RefreshThumbnail()
        {
            if (woeInfo == null)
            {
                MetaContextElementUtils.SetActive(slotEmpty, true);
                MetaContextElementUtils.SetActive(epicWinDefault, false);
                MetaContextElementUtils.SetActive(epicWin, false);
                MetaContextElementUtils.SetActive(newEpic, false);
                MetaContextElementUtils.SetActive(imageMagnifier, true);
                
                GameUnlockStatus unlockStatus = gameInfo.GetValue<GameUnlockStatus>("unlockStatus");
                int levelRestriction = gameInfo.GetValue<int>("minLevel");
                    
                if (unlockStatus == GameUnlockStatus.UNLOCKED || unlockStatus == GameUnlockStatus.TEMP_UNLOCKED || meLevel >= levelRestriction)
                {
                    MetaContextElementUtils.SetActive(slotLock, false);
                    SetInteractable(true);
                }
                else
                {
                    MetaContextElementUtils.SetActive(slotLock, true);
                    MetaContextElementUtils.SetText(textSlotLock, levelRestriction.ToString());
                    SetInteractable(false);
                }
            }
            else
            {
                bool isNewWOE = BlackboardQueryUtils.CheckIfNewWOE(woeInfo.GetValue<int>("id"));

                MetaContextElementUtils.SetActive(slotEmpty, false);
                MetaContextElementUtils.SetActive(slotLock, false);
                
                if (isNewWOE)
                {
                    MetaContextElementUtils.SetActive(epicWinDefault, false);
                    MetaContextElementUtils.SetActive(epicWin, false);
                    MetaContextElementUtils.SetActive(newEpic, true);
                    MetaContextElementUtils.SetActive(imageMagnifier, false);
                }
                else
                {
                    MetaContextElementUtils.SetActive(newEpic, false);
                    MetaContextElementUtils.SetActive(imageMagnifier, true);
                    
                    MetaContextElementUtils.SetActive(epicWinImage, false);
                    MetaContextElementUtils.SetActive(epicWinLoading, true);
                    
                    string screenShotUrl = woeInfo.GetValue<string>("screenshotUrl");

                    if (!string.IsNullOrEmpty(screenShotUrl))
                    {
                        MetaContextElementUtils.SetActive(epicWinDefault, false);
                        MetaContextElementUtils.SetActive(epicWin, true);

                        MetaContextElementUtils.SetWebImage(epicWinImage, screenShotUrl, CacheType.FileCache, true, () =>
                        {
                            if (epicWinImage == null)
                                return;

                            // 179 * 148
                            MetaContextElementUtils.UpdateContextImageOrientationScaler(epicWinImage, 179f, 148f);
                            
                            MetaContextElementUtils.SetActive(epicWinImage, true);
                            MetaContextElementUtils.SetActive(epicWinLoading, false);
                        });
                    }
                    else
                    {
                        MetaContextElementUtils.SetActive(epicWinDefault, true);
                        MetaContextElementUtils.SetActive(epicWin, false);
                        
                        long winCredit = woeInfo.GetValue<long>("winCredit");

                        string winCreditText = StringTableUtils.GetString(StringTable.StringTableType.Global, "TEXT_COMMA_NUMBER", winCredit);
                        MetaContextElementUtils.SetText(epicWinDefaultText, winCreditText);
                    }
                }
                
                SetInteractable(true);
            }
        }

        private void RefreshSlotName()
        {
            string title = StringTableUtils.GetString(tableType, string.Format("GAME_TITLE_{0}", gameId));
            MetaContextElementUtils.SetText(textSlotName, title);
        }

        private void RefreshBadge()
        {
            if (woeInfo == null)
            {
                MetaContextElementUtils.SetActive(badgeArea, false); 
            }
            else
            {
                bool isNewWOE = BlackboardQueryUtils.CheckIfNewWOE(woeInfo.GetValue<int>("id"));

                if (isNewWOE)
                {
                    MetaObjectUtils.UpdateBadge(badgeArea, true);
                    
                    ContextElement badgeElement = ContextUtils.FindElement(badgeArea, "Badge", ContextSearchingType.ChildrenSearch);
                    StartCoroutine(SetBadgeValue(badgeElement as ContextAnimator));
                    
                    MetaContextElementUtils.SetActive(badgeArea, true);
                }
                else
                {
                    MetaContextElementUtils.SetActive(badgeArea, false);
                }
            }
        }

        private IEnumerator SetBadgeValue(ContextAnimator contextAnimator)
        {
            yield return null;
            contextAnimator.SetIntProperty(1);
        }
    }
}