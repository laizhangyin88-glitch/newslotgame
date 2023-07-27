using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace BagelCode
{
    public class PopupTermsOfUseController : MonoBehaviour
    {
        public Blackboard rootBlackboard;
        public Animator rootAnimator;

        private ContextElement rootElement;
        private List<ContextElement> titleList;
        private List<ContextElement> termsTextAreaList;
        private List<ContextElement> termsTextList;
        private ContextElement buttonElement;
        private ContextElement buttonTextElement;

        // A is new users, B is old users. 
        private const string TERMS_OF_USE_TEXT_A = "POPUP_TERMS_OF_USE_TEXT_A";
        private const string TERMS_OF_USE_TEXT_B = "POPUP_TERMS_OF_USE_TEXT_B";

        private const string TERMS_OF_USE_BUTTON_A = "POPUP_TERMS_OF_USE_OK_BUTTON_A";
        private const string TERMS_OF_USE_BUTTON_B = "POPUP_TERMS_OF_USE_OK_BUTTON_B";

        private string title;
        private string touMessage;
        private string contextID;
        private TermsOfUseType termsOfUseType;

        private bool isInit = false;

        private void Start()
        {
            InitProperty();
            UpdateVariables();

            BI_TOU(true);
        }

        private void OnDestroy()
        {
            if(MetaSystem.Instance != null)
            {
                MetaSystem.UnSubscribeBackButton(gameObject.GetHashCode());
            }
        }

        public void InitProperty()
        {
            if(isInit) return;

            rootBlackboard = gameObject.GetComponent<Blackboard>();
            rootAnimator   = gameObject.GetComponent<Animator>();

            // Init context. 
            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            titleList = new List<ContextElement>();
            termsTextAreaList = new List<ContextElement>();
            termsTextList = new List<ContextElement>();

            var contentsAreaElement = ContextUtils.FindElement(rootElement, "Contents Area", ContextSearchingType.ChildrenSearch);

            // Title List
            var titleArea = ContextUtils.FindElement(contentsAreaElement, "Title Text Area", ContextSearchingType.ChildrenSearch);
            titleList.Add(ContextUtils.FindElement(titleArea, "TOU Tilte Image A Newbie", ContextSearchingType.ChildrenSearch));
            titleList.Add(ContextUtils.FindElement(titleArea, "TOU Tilte Image A Oldbie", ContextSearchingType.ChildrenSearch));
            titleList.Add(ContextUtils.FindElement(titleArea, "TOU Tilte Image B Newbie", ContextSearchingType.ChildrenSearch));
            titleList.Add(ContextUtils.FindElement(titleArea, "TOU Tilte Image B Oldbie", ContextSearchingType.ChildrenSearch));

            // Text
            termsTextAreaList.Add(ContextUtils.FindElement(contentsAreaElement, "TOU Text A", ContextSearchingType.ChildrenSearch));
            termsTextAreaList.Add(ContextUtils.FindElement(contentsAreaElement, "TOU Text B", ContextSearchingType.ChildrenSearch));
            termsTextList.Add(ContextUtils.FindElement(termsTextAreaList[0], "Text", ContextSearchingType.ChildrenSearch));
            termsTextList.Add(ContextUtils.FindElement(termsTextAreaList[1], "Text", ContextSearchingType.ChildrenSearch));

            // Button
            buttonElement = ContextUtils.FindElement(rootElement, "Button Area/Button Agree", ContextSearchingType.FullNameSearch);
            buttonTextElement = ContextUtils.FindElement(buttonElement, "Text", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SetClickable(
                buttonElement,
                () =>{ OnClose(); }
            );

            // // Back Button Event. Lock.
            MetaSystem.SubscribeBackButton(gameObject.GetHashCode(), () => { });

            contextID = rootBlackboard.GetValue<string>("_biContextID");

            isInit = true;
        }

        private void UpdateVariables()
        {
            termsOfUseType = MainBlackboard.Get().GetValue<TermsOfUseType>("termsOfUseType");

            for(int i=0; i < titleList.Count; ++i)
                titleList[i].gameObject.SetActive(false);
            for(int i=0; i < termsTextAreaList.Count; ++i)
                termsTextAreaList[i].gameObject.SetActive(false);

            switch(termsOfUseType)
            {
                case TermsOfUseType.OLD_USER:
                    UpdateTermsB();
                    break;
                default:
                    UpdateTermsA();
                    break;
            }
        }

        private void UpdateTermsA()
        {
            // New user
            titleList[0].gameObject.SetActive(true);

            touMessage = StringTableUtils.GetString(StringTable.StringTableType.Global,
                    TERMS_OF_USE_TEXT_A,
                    MainBlackboard.Get().GetValue<string>("policyUrl"),
                    MainBlackboard.Get().GetValue<string>("termsOfUseUrl")
                );

            termsTextAreaList[0].gameObject.SetActive(true);
            MetaContextElementUtils.SetText(termsTextList[0], touMessage);

            MetaContextElementUtils.SetText(buttonTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, TERMS_OF_USE_BUTTON_A) );
        }

        private void UpdateTermsB()
        {
            // Old user.
            titleList[1].gameObject.SetActive(true);

            touMessage = StringTableUtils.GetString(StringTable.StringTableType.Global,
                    TERMS_OF_USE_TEXT_B,
                    MainBlackboard.Get().GetValue<string>("policyUrl"),
                    MainBlackboard.Get().GetValue<string>("termsOfUseUrl")
                );

            termsTextAreaList[1].gameObject.SetActive(true);
            MetaContextElementUtils.SetText(termsTextList[1], touMessage);

            MetaContextElementUtils.SetText(buttonTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, TERMS_OF_USE_BUTTON_B) );
        }

        private void OnClose()
        {
            BI_TOU(false);

            EventSender.SendCalleeCallback(gameObject);
            PopupManager.Instance.Close(gameObject);
            Destroy(gameObject);
        }

        private void BI_TOU(bool isTrigger)
        {
             var version = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "/values/misc/TERMS_OF_USE_VERSION");

            if (version == null)
                return;

            var eventData = new Dictionary<string, object>();
            eventData["context_id"] = contextID;
            eventData["terms_of_use_type"] = (int)termsOfUseType;
            eventData["type"] = isTrigger ? "trigger" : "agree";
            eventData["version"] = version.value;
            eventData["message"] = touMessage;

            Analytics.CustomEvent("client_terms_of_use_popup", eventData);
        }

#if UNITY_EDITOR
        [Button]
        public void TestTermsA()
        {
            for(int i=0; i < titleList.Count; ++i)
                titleList[i].gameObject.SetActive(false);
            for(int i=0; i < termsTextAreaList.Count; ++i)
                termsTextAreaList[i].gameObject.SetActive(false);

            UpdateTermsA();
        }

        [Button]
        public void TestTermsB()
        {
            for(int i=0; i < titleList.Count; ++i)
                titleList[i].gameObject.SetActive(false);
            for(int i=0; i < termsTextAreaList.Count; ++i)
                termsTextAreaList[i].gameObject.SetActive(false);

            UpdateTermsB();
        }
#endif
    }
}