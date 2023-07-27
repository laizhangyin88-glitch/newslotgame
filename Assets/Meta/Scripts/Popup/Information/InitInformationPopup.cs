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
    public class InitInformationPopup : ActionTask<ContextElement>
    {
        public BBParameter<string> bundle;
        public BBParameter<string> dotPrefabFormat;
        public BBParameter<int> pageCount;
        public BBParameter<int> index;

        public BBParameter<string> infoTextFormat;
        public BBParameter<List<string>> saveAsInfoTextList;

        private const string ON_SELECT_RIGHT_INFO_EVENT = "OnSelectRightInfo";
        private const string ON_SELECT_LEFT_INFO_EVENT = "OnSelectLeftInfo";
        private const string ON_CLOSE_EVENT = "OnClose";

        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;
        
        protected override string info
        {
            get { return "Init Information Popup"; }
        }

        protected override void OnExecute()
        {
            index.value = 0;
            
            MakeDots();
            MakeInfoText();

            ContextElement rightButtonElement = ContextUtils.FindElement(agent, "Arrow Right", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetClickable(
                rightButtonElement,
                ON_SELECT_RIGHT_INFO_EVENT,
                false,
                false,
                SendEvent,
                ownerSystem
            );
            
            ContextElement leftButtonElement = ContextUtils.FindElement(agent, "Arrow Left", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetClickable(
                leftButtonElement,
                ON_SELECT_LEFT_INFO_EVENT,
                false,
                false,
                SendEvent,
                ownerSystem
            );
            
            ContextElement closeButtonElement = ContextUtils.FindElement(agent, "Close", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetClickable(
                closeButtonElement,
                ON_CLOSE_EVENT,
                false,
                false,
                SendEvent,
                ownerSystem
            );

            leftButtonElement.gameObject.SetActive(pageCount.value > 1);
            rightButtonElement.gameObject.SetActive(pageCount.value > 1);

            agent.GetComponent<Animator>().SetBool("Active", true);

            EndAction();
        }

        private void MakeDots()
        {
            ContextElement dotsAreaElement = ContextUtils.FindElement(agent, "Dots Area", ContextSearchingType.ChildrenSearch);
            if(pageCount.value > 1)
            {
                for (int i = 0; i < pageCount.value; i++)
                {
                    var go = MetaObjectUtils.MakePrefab(
                        MetaStringDefine.LOBBY_BUNDLE_NAME,
                        dotPrefabFormat.value,
                        dotsAreaElement.transform,
                        "",
                        string.Format("Dot {0}",i)
                    );
                }
                dotsAreaElement.UpdateContext(true);
                MetaContextElementUtils.SetIntProperty(dotsAreaElement, index.value);
            }
            else
            {
                dotsAreaElement.gameObject.SetActive(false);
            }
        }

        private void MakeInfoText()
        {
            var argsList = BlackboardUtils.GetOrCreateVariable<List<object[]>>(agent.GetComponent<Blackboard>(), "argsList");
#if UNITY_EDITOR
            if(argsList == null)
            {
                Debug.LogError("Use action to SetInformationPopupData !!!!!!!!!!!!!!!!!!");
            }
#endif
            saveAsInfoTextList.value = new List<string>();

            for(int i=0; i < pageCount.value; ++i)
            {
                if(argsList != null && argsList.value != null && argsList.value.Count > i && argsList.value[i] != null)
                {
                    saveAsInfoTextList.value.Add( StringTableUtils.GetString(tableType, string.Format(infoTextFormat.value, i), argsList.value[i]) );
                }
                else
                {
                    saveAsInfoTextList.value.Add( StringTableUtils.GetString(tableType, string.Format(infoTextFormat.value, i)) );
                }
            }
        }
    }
}
