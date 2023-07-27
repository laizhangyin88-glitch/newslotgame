using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;
using ParadoxNotion;
using ParadoxNotion.Services;
using NodeCanvas.Framework;

namespace BagelCode
{
    public class LobbySlotControllerUnlock : MonoBehaviour
    {
        private const string LOCK_ANI_PARAMETER_NAME = "Appear";

        public void InitUnlock(int id, int restriction, bool isWithoutButton)
        {
            if(isWithoutButton)
                SetUnlockWithoutButton(id, restriction);
            else
                SetUnlock(id, restriction);
        }

        private void SetUnlock(int id, int restriction)
        {
            ContextElement thisElement = GetComponent<ContextElement>();
            thisElement.UpdateContext(true);

            ContextElement unlockElement = ContextUtils.FindElement(thisElement, "Slot Unlock", ContextSearchingType.ChildrenSearch);
            unlockElement.gameObject.SetActive(false);

            MetaContextElementUtils.SimpleSetText(thisElement, "Text", restriction.ToString(), ContextSearchingType.ChildrenSearch);

            long unlockGemCost = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "values/misc/UNLOCK_GAME_GEM_COST").value;
            MetaContextElementUtils.SimpleSetText(thisElement, "Slot Unlock/Text Gem", StringTableUtils.GetString(StringTable.StringTableType.Global, "SLOT_UNLOCK_GEM_COST_TEXT", unlockGemCost), ContextSearchingType.FullNameSearch);

            IContextClickable clickableElement = thisElement as IContextClickable;
            if (clickableElement != null)
            {
                clickableElement.RemoveAllListener();
                clickableElement.AddListenerOnClick( OnClickArea );
            }

            Blackboard bb = GetComponent<Blackboard>();
            BlackboardUtils.SetOrCreateValue(bb, "gameID", id);
            BlackboardUtils.SetOrCreateValue(bb, "_gemPrice", unlockGemCost);
            ProductUtils.MakeSlotUnlockProduct(bb, unlockGemCost);
        }

        private void SetUnlockWithoutButton(int id, int restriction)
        {
            ContextElement thisElement = GetComponent<ContextElement>();
            thisElement.UpdateContext(true);

            MetaContextElementUtils.SimpleSetText(thisElement, "Text", restriction.ToString(), ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SimpleSetText(thisElement, "Speech Balloon Common/Text", StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_LOBBY_LEVEL_RESTRICTION_BALLOON_TEXT"), ContextSearchingType.FullNameSearch);

            IContextClickable clickableElement = thisElement as IContextClickable;
            if (clickableElement != null)
            {
                clickableElement.RemoveAllListener();
                clickableElement.AddListenerOnClick( OnClickWithout );
            }
        }

        private void OnClickWithout(ContextElement sender)
        {
            ContextElement targetElement = ContextUtils.FindElement(sender, "Speech Balloon Common", ContextSearchingType.ChildrenSearch);
            targetElement.gameObject.SetActive(true);
            targetElement.GetComponent<Animator>().SetTrigger(LOCK_ANI_PARAMETER_NAME);
        }

        private void OnClickArea(ContextElement sender)
        {
            ContextElement targetElement = ContextUtils.FindElement(sender, "Slot Unlock", ContextSearchingType.ChildrenSearch);
            targetElement.gameObject.SetActive(true);
            targetElement.GetComponent<Animator>().SetTrigger(LOCK_ANI_PARAMETER_NAME);

            long slotUnlockCost = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "values/misc/UNLOCK_GAME_GEM_COST").value;

            long meGem = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "me/gem").value;
            var unlockButtonArea = ContextUtils.FindElement(targetElement, "Button Area", ContextSearchingType.ChildrenSearch);

            ContextElement unlockButtonAreaElement = unlockButtonArea.GetComponent<ContextElement>();
            unlockButtonAreaElement.UpdateContext(true);

            var getMoreButton = ContextUtils.FindElement(unlockButtonAreaElement, "Button Get More", ContextSearchingType.ChildrenSearch);
            var unlockButton = ContextUtils.FindElement(unlockButtonAreaElement, "Button Unlock", ContextSearchingType.ChildrenSearch);
            if(meGem < slotUnlockCost)
            {
                if(unlockButton != null)
                    GameObject.Destroy(unlockButton.gameObject);
                if(getMoreButton == null)
                {
                    var unlockButtonObject = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Button Primary", unlockButtonArea.transform, "", "Button Get More");
                    ContextElement unlockButtonElement = unlockButtonObject.GetComponent<ContextElement>();
                    unlockButtonElement.UpdateContext(false);
                    MetaContextElementUtils.SimpleSetText(unlockButtonElement, "Text", StringTableUtils.GetString(StringTable.StringTableType.Global, "SLOT_UNLOCK_GEM_GET_MORE_TEXT"), ContextSearchingType.ChildrenSearch);

                    IContextClickable clickableElement = unlockButtonElement as IContextClickable;
        
                    if (clickableElement != null)
                    {
                        clickableElement.RemoveAllListener();
                        clickableElement.AddListenerOnClick( (ContextElement send) =>
                            {
                                PlayerPrefs.SetString("SHOP_OPENED_FROM_TYPE", "slot_unlock");
                                GraphOwner.SendGlobalEvent("OpenGemShop", send);
                            }
                        );
                    }
                }
            }
            else
            {
                if(getMoreButton != null)
                    GameObject.Destroy(getMoreButton.gameObject);
                if(unlockButton == null)
                {
                    var unlockButtonObject = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Button Purchase", unlockButtonArea.transform, "", "Button Unlock");
                    ContextElement unlockButtonElement = unlockButtonObject.GetComponent<ContextElement>();
                    unlockButtonElement.UpdateContext(false);
                    MetaContextElementUtils.SimpleSetText(unlockButtonElement, "Text", StringTableUtils.GetString(StringTable.StringTableType.Global, "SLOT_UNLOCK_NOW_TEXT"), ContextSearchingType.ChildrenSearch);

                    IContextClickable clickableElement = unlockButtonElement as IContextClickable;
                    if (clickableElement != null)
                    {
                        clickableElement.RemoveAllListener();
                        clickableElement.AddListenerOnClick( (ContextElement s) =>
                            {
                                MetaContextElementUtils.SendEvent( unlockButtonElement, "OnRequestUnlock", GetComponent<ContextElement>(), null);
                            } );
                    }
                }
            }
        }
    }
}
