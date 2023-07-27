using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode.MetaGame.Tasks.Actions
{
    [Category("★ BagelCode/Popup")]
    public class UpdateInformationPopup : ActionTask<ContextElement>
    {
        public BBParameter<string> bundle;
        public BBParameter<string> infoPrefabFormat;
        public BBParameter<List<string>> infoTextList;

        public BBParameter<int> pageCount;
        public BBParameter<int> index;

        public BBParameter<GameObject> prevPageObj;

        private ContextElement leftButtonElement;
        private ContextElement rightButtonElement;
        private ContextElement dotsAreaElement;
        private ContextElement imageAreaElement;
        private ContextElement infoTextElement;
        
        protected override string info
        {
            get { return "Update Information Popup"; }
        }

        protected override void OnExecute()
        {
            if(prevPageObj.value != null)
                GameObject.Destroy(prevPageObj.value);

            InitProperty();
            UpdateValues();

            EndAction();
        }

        private void InitProperty()
        {
            leftButtonElement = ContextUtils.FindElement(agent, "Arrow Left", ContextSearchingType.ChildrenSearch);
            rightButtonElement = ContextUtils.FindElement(agent, "Arrow Right", ContextSearchingType.ChildrenSearch);
            dotsAreaElement = ContextUtils.FindElement(agent, "Dots Area", ContextSearchingType.ChildrenSearch);
            imageAreaElement = ContextUtils.FindElement(agent, "Image Area", ContextSearchingType.ChildrenSearch);
            infoTextElement = ContextUtils.FindElement(agent, "Text", ContextSearchingType.ChildrenSearch);
        }

        private void UpdateValues()
        {
            leftButtonElement.GetComponent<PIDButton>().interactable = index.value > 0;
            rightButtonElement.GetComponent<PIDButton>().interactable = index.value < pageCount.value - 1;

            var go = MetaObjectUtils.MakePrefab(
                bundle.value,
                string.Format(infoPrefabFormat.value, index.value),
                imageAreaElement.transform,
                "",
                string.Format("Page {0:00}", index.value)
            );

            prevPageObj.value = go;
            
            MetaContextElementUtils.SetText(infoTextElement, infoTextList.value[index.value]);
            if (pageCount.value > 1)
                MetaContextElementUtils.SetIntProperty(dotsAreaElement, index.value);
        }
    }
}
