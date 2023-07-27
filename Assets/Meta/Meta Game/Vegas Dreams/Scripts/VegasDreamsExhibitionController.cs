using UnityEngine;
using SlotMaker;
using System.Collections.Generic;
using System;
using BagelCode.ClientModels;
using System.Collections;
using NodeCanvas.Framework;
using BagelCode.OSA_Scroll;
using ParadoxNotion;

namespace BagelCode.VegasDreams
{
    public class VegasDreamsExhibitionController : EventMonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;
        private Animator anim;

        private bool isInit = false;
        private int lastLoadedThemeId = -1;
        private int lastSelectedIndex = 0;
        
        private ContextElement backgroundAreaElement;
        private GameObject backgroundObj;

        private OSA_ExhibitionItems osa;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        public void InitProperty()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            // Init Elements
            InitContents();
            StartCoroutine(LoadSeason(0));

            isInit = true;
        }

        private void InitContents()
        {
            osa = ContextUtils.FindElement(root, "Room Cell Area", CHILDREN).GetComponent<OSA_ExhibitionItems>();
            var contentElement = ContextUtils.FindElement(root, "Menu Dropdown/Content", FULL);

            var exhibitionList = VegasDreams.Utils.ExhibitionSeasonList;
            
            for (int i = 0; i < exhibitionList.Count; i++)
            {
                var exhibition = exhibitionList[i];
                var seasonName = exhibition.GetValue<string>("name");

                var obj = MetaObjectUtils.MakePrefab(
                    VegasDreams.Defines.CONTENTS_BUNDLE,
                    "Popup Vegas Dreams Exhibition Hall Season Button",
                    contentElement.transform
                );

                var objElement = obj.GetComponent<ContextElement>();
                objElement.UpdateContext(false);

                MetaContextElementUtils.SimpleSetText(objElement, "Text Season (TMP)", seasonName);
                
                var eventData = new EventData<int>(VegasDreams.Events.ON_CLICK_EXHIBITION_DROPDOWN_SEASON, i);
                MetaContextElementUtils.SetClickable(objElement, () =>
                    {
                        if (lastSelectedIndex == i) return;
                        EventSender.SendEvent(gameObject, eventData);
                    });
            }

            backgroundAreaElement = ContextUtils.FindElement(root, "Background Area", CHILDREN);

            var tabButtonElement = ContextUtils.FindElement(root, "Menu Tab", CHILDREN);
            MetaContextElementUtils.SetClickable(tabButtonElement,
                () =>
                {
                    UpdateDropDown();
                });

            var leftArrowButtonElement = ContextUtils.FindElement(root, "Left Arrow", CHILDREN);
            MetaContextElementUtils.SetClickable(leftArrowButtonElement,
                () =>
                {
                    osa.ScrollToPrev();
                });
            
            var rightArrowButtonElement = ContextUtils.FindElement(root, "Right Arrow", CHILDREN);
            MetaContextElementUtils.SetClickable(rightArrowButtonElement,
                () =>
                {
                    osa.ScrollToNext();
                });

            var closeButtonElement = ContextUtils.FindElement(root, "Button Close", CHILDREN);
            MetaContextElementUtils.SetClickable(closeButtonElement,
                () => EventSender.SendEvent(gameObject, VegasDreams.Events.ON_CLOSE));
        }

        public IEnumerator LoadSeason(int index)
        {
            var contextId = BlackboardUtils.GetOrCreateVariable<string>(bb, "contextId")?.value;

            var exhibition = VegasDreams.Utils.ExhibitionSeasonList[index];
            var seasonId = exhibition.GetValue<int>("id");
            var themeId = exhibition.GetValue<int>("themeId");
            var seasonName = exhibition.GetValue<string>("name");

            MetaContextElementUtils.SimpleSetActive(root, "Loading", true);
            MetaContextElementUtils.SimpleSetText(root, "Menu Tab/Text (TMP)", seasonName, FULL);
            
            yield return StartCoroutine(LoadTheme(themeId));

            bool isDone = false;
            
            BagelCodeClientAPI.RequestVegasDreamExhibition(seasonId,
                (response) =>
                {
                    ClientAPI2Blackboard.Serialize(bb, response);
                    isDone = true;
                },
                (error) =>
                {
                    GlobalErrorHandler.GlobalError(error);
                });

            yield return new WaitUntil(() => isDone);

            var buildingPresetList = BlackboardUtils.FindVariable<List<Blackboard>>(bb, "season/preset/buildingPresetList")?.value;
            VegasDreamsAnalytics.build_dream_exhibition_hall(contextId, buildingPresetList, seasonId);

            InitThemePrefabs(themeId);
            osa.CreateItemList(bb, themeId);

            yield return StartCoroutine(UnloadTheme());

            MetaContextElementUtils.SimpleSetActive(root, "Loading", false);
            anim.SetBool("Active", true);

            lastLoadedThemeId = themeId;
            lastSelectedIndex = index;
        }

        private void InitThemePrefabs(int themeId)
        {
            // Season Background Setting
            Destroy(backgroundObj);

            backgroundObj = MetaObjectUtils.MakePrefab(
                $"mgvegasdreamscontents{themeId}",
                $"Vegas Dreams Theme {themeId} Background",
                backgroundAreaElement.transform
            );
        }

        private IEnumerator LoadTheme(int themeId)
        {
            if (themeId == VegasDreams.Utils.SeasonThemeId) yield break; // if main theme is same current theme
            var bundles = new List<string>() { $"mgvegasdreamscontents{themeId}", $"mgvegasdreamstheme{themeId}obj" };
            
            foreach (var bundle in bundles)
            {
                AssetBundleManager.AddDLC(bundle);

                var operation = AssetBundleManager.LoadAssetBundle(bundle);
                yield return new WaitUntil(() => operation.IsDone());
            }
        }

        private IEnumerator UnloadTheme()
        {
            if (lastLoadedThemeId == -1 || lastLoadedThemeId == VegasDreams.Utils.SeasonThemeId) yield break; // If lastLoaded empty or current season bundle don't unload

            var bundles = new List<string>() { $"mgvegasdreamscontents{lastLoadedThemeId}", $"mgvegasdreamstheme{lastLoadedThemeId}obj" };
            foreach (var bundle in bundles)
            {
                AssetBundleManager.UnloadAssetBundle(bundle, false);
                var operation = Resources.UnloadUnusedAssets();
                yield return new WaitUntil(() => operation.isDone);
            }
        }

        private void UpdateDropDown()
        {
            var dropdownElement = ContextUtils.FindElement(root, "Menu Dropdown", CHILDREN);
            var tapElement = ContextUtils.FindElement(root, "Menu Tab", CHILDREN);

            var dropdownAnimator = dropdownElement.GetComponent<Animator>();
            var tapAnimator = tapElement.GetComponent<Animator>();

            var flag = tapAnimator.GetBool("Pressed");
            
            dropdownAnimator.SetBool("Active", !flag);
            tapAnimator.SetBool("Pressed", !flag);
        }
    }
}
