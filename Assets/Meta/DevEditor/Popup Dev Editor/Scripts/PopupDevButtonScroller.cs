using UnityEngine;
using SlotMaker;
using System.Collections.Generic;
using NodeCanvas.Framework;

namespace BagelCode
{
    public abstract class PopupDevButtonScroller : EventMonoBehaviour
    {
        protected ContextElement root;

        protected abstract int CellCount();
        protected abstract string TitleText();
        protected abstract void OnClick(int i);
        protected abstract string CellText(int i);

        protected virtual bool IsValidIndex(int i) => true;

        protected List<GameObject> cellObjList = new List<GameObject>();
        protected List<string> cellTextList = new List<string>();

        protected ContextElement contentsArea;

        protected virtual void Start()
        {
            root = GetComponent<ContextElement>();

            root.UpdateContext(false);

            MetaContextElementUtils.SimpleSetClickable(root, "Button Close", Close);

            MetaContextElementUtils.SimpleSetText(root, "Title Area/Text", TitleText(), ContextSearchingType.FullNameSearch);

            contentsArea = ContextUtils.FindElement(root, "Dev Editor Contents Scroll Area/Contents", ContextSearchingType.FullNameSearch);

            UpdateCells();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            MetaSystem.SubscribeBackButton(gameObject.GetHashCode(), Close);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            MetaSystem.UnSubscribeBackButton(gameObject.GetHashCode());
        }

        protected virtual void UpdateCells()
        {
            cellObjList.Clear();
            cellTextList.Clear();

            string bundle = "testsuite";
            string asset = "Dev Button Cell";
            Transform parent = contentsArea.transform;

            int cellCount = CellCount();
            for (int i = 0; i < cellCount; ++i)
            {
                if (!IsValidIndex(i)) continue;

                // Init Cell
                var cellObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);
                cellObjList.Add(cellObj);

                var cellElement = cellObj.GetComponent<ContextElement>();
                cellElement.UpdateContext(true);

                var buttonElement = ContextUtils.FindElement(cellElement, "Button", ContextSearchingType.ChildrenSearch);

                int index = i;
                MetaContextElementUtils.SetClickable(buttonElement, () => OnClick(index));

                string cellText = CellText(i);
                cellTextList.Add(cellText);
                MetaContextElementUtils.SimpleSetText(buttonElement, "Text", cellText);
            }
        }

        private void Close()
        {
            MetaPopupUtils.ClosePopup(gameObject);
        }
    }
}
