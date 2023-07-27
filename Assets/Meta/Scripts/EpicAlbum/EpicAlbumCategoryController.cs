using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode.EpicAlbum
{
    public class EpicAlbumCategoryController : MonoBehaviour, IEpicAlbumPageElement
    {
        public ScrollRect scroll;
        public GameObject cellPrefabObj;
        public List<EpicAlbumCategoryCellController> categoryList;

        private ContextElement agent;
        private ContextElement anchor;
        private ContextElement categoryName;
        private Animator animator;

        private EpicAlbumController epicAlbum;

        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

        private const string BUNDLE_NAME = "epicalbum";
        private const string CELL_ASSET_NAME = "Epic Album Cover Page Slot Cell";
        private const string ON_CLICK_CATEGORY_EVENT = "OnClickCategory";

        public void Init(ContextElement root)
        {
            epicAlbum = root.GetComponent<EpicAlbumController>();
            agent = GetComponent<ContextElement>();
            agent.UpdateContext();
            anchor = ContextUtils.FindElement(agent, "Anchor", ContextSearchingType.ChildrenSearch);
            animator = GetComponent<Animator>();

            var epicAlbumBBList = BlackboardQueryUtils.GetEpicAlbumInfoList();
            categoryList = new List<EpicAlbumCategoryCellController>();
            int index = 0;
            for (int i = 0; i < epicAlbumBBList.Count; i++)
            {
                if (BlackboardQueryUtils.IsEmptyAlbum(epicAlbumBBList[i]))
                    continue;

                var tab = MetaObjectUtils.MakePrefab(BUNDLE_NAME, CELL_ASSET_NAME, transform, "Anchor/Contents Area/Contents Layout/Scroll Rect/Viewport/Contents");
                var cellController = tab.GetComponent<EpicAlbumCategoryCellController>();
                cellController.Init(epicAlbumBBList[i], index, root);
                categoryList.Add(cellController);
                index++;
            }

            Invoke("ScrollInitPosition", 0.5f);
        }

        // Interface
        public void PageChangeFinished()
        {
            if(epicAlbum != null)
                epicAlbum.PageChangeFinished();
        }

        public void Play(int pageParameter)
        {
            animator.SetInteger("Page", pageParameter);
        }

        public void Activate(bool active)
        {
            MetaContextElementUtils.SetActive(anchor, active);
        }

        public void ScrollInitPosition()
        {
            scroll.verticalNormalizedPosition = 1f;
        }

        // use only bi event.
        public string GetCategoryBadgeStatus(CategoryType categoryType)
        {
            foreach(var controller in categoryList)
            {
                if(controller.categoryType == categoryType)
                    return controller.GetCategoryBadgeStatus();
            }

            return "Default";
        }
    }
}
