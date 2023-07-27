using BagelCode.ClientModels;
using SlotMaker;
using UnityEngine;

namespace BagelCode.EpicAlbum
{
    public class EpicAlbumTabController : MonoBehaviour
    {
        private CategoryType categoryType;

        private ContextElement agent;
        private PIDButton button;
        private ContextElement baseOn;
        private ContextElement badgeArea;

        public void Init(CategoryType categoryType, EpicAlbumAssets epicAlbumAssets)
        {
            agent = GetComponent<ContextElement>();
            button = GetComponent<PIDButton>();
            agent.UpdateContext();
            baseOn = ContextUtils.FindElement(agent, "Base On", ContextSearchingType.ChildrenSearch);
            this.categoryType = categoryType;

            int index = (int) categoryType - 1;
            ContextElement categoryIcon = ContextUtils.FindElement(agent, "Icon Category", ContextSearchingType.ChildrenSearch);
            Sprite icon = epicAlbumAssets.categoryIconAssets[index];
            MetaContextElementUtils.SetSprite(categoryIcon, icon);

            ContextElement categoryName = ContextUtils.FindElement(agent, "Text Category Name", ContextSearchingType.ChildrenSearch);
            string name = StringTableUtils.GetString(StringTable.StringTableType.Global, "EPIC_ALBUM_CATEGORY_NAME_" + index);
            MetaContextElementUtils.SetText(categoryName, name);

            badgeArea = ContextUtils.FindElement(agent, "Badge Area", ContextSearchingType.ChildrenSearch);
        }

        public void Refresh()
        {
            Refresh(categoryType);
        }

        public void Refresh(CategoryType categoryType)
        {
            if (this.categoryType == categoryType)
            {
                MetaContextElementUtils.SetActive(baseOn , true);
                button.interactable = false;
            }
            else
            {
                MetaContextElementUtils.SetActive(baseOn , false);
                button.interactable = true;
            }

            bool newExist = BlackboardQueryUtils.CheckIfNewExistInCategory(this.categoryType);
            if (newExist)
            {
                MetaObjectUtils.UpdateBadge(badgeArea, true);
                MetaContextElementUtils.SetActive(badgeArea, true);
            }
            else
            {
                MetaContextElementUtils.SetActive(badgeArea, false);
            }

        }
    }
}
