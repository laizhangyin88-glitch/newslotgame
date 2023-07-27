using System;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using ParadoxNotion;
using System.Collections;

namespace BagelCode
{
    public abstract class DevEditorSettings<T> : MonoBehaviour where T : Enum
    {
        protected abstract int Height();

        protected const int WIDTH = 5;

        public delegate void EventDelegate(EventData eventData);

        protected Blackboard currentBB;
        protected ContextElement root;
        protected GraphOwner owner;

        protected ContextElement contentsElement;

        protected Dictionary<T, string> elementNameDict = new Dictionary<T, string>();
        protected Dictionary<T, string> clickEventDict = new Dictionary<T, string>();
        protected Dictionary<T, string[]> buttonTextDict = new Dictionary<T, string[]>();
        protected Dictionary<T, string> playerPrefsKeyDict = new Dictionary<T, string>();
        protected Dictionary<T, System.Action> onClickMethodDict = new Dictionary<T, System.Action>();

        protected T[,] displayButtons;

        protected abstract void SetButtons();

        protected abstract void SetButtonFunctions();

        public void Init()
        {
            SetButtons();

            contentsElement = ContextUtils.FindElement(root, "Contents", ContextSearchingType.ChildrenSearch);

            // Make Buttons
            string bundle = "testsuite";
            string buttonGroupAssetName = "Dev Editor Button Group Scene";
            Transform parent = contentsElement.transform;
            int height = Height();
            for (int i = 0; i < height; ++i)
            {
                GameObject buttonGroupObj = MetaObjectUtils.MakeScene(bundle, buttonGroupAssetName, parent, "");

                buttonGroupObj.SetActive(true);

                for (int j = 0; j < WIDTH; ++j)
                {
                    T type = displayButtons[i, j];
                    if ((int)(object)type != 0)
                    {
                        var buttonObj = buttonGroupObj.transform.Find(string.Format("Button Layout/Button {0}", j)).gameObject;
                        buttonObj.name = elementNameDict[type];
                    }
                }
            }

            root.UpdateContext(true);

            // Button
            // Close
            MetaContextElementUtils.SimpleSetClickable(root, "Button Close", OnClickClose);

            // Setup Buttons
            T[] types = (T[])System.Enum.GetValues(typeof(T));
            for (int i = 0; i < types.Length; ++i)
            {
                T type = types[i];

                if (!elementNameDict.ContainsKey(type))
                    continue;

                ContextElement buttonElement;
                buttonElement = ContextUtils.FindElement(contentsElement, elementNameDict[type], ContextSearchingType.ChildrenSearch);

                if (buttonElement is IContextClickable clickableElement)
                {
                    clickableElement.RemoveAllListener();
                    clickableElement.AddListenerOnClick((ContextElement sender) =>
                    {
                        if (clickEventDict.ContainsKey(type))
                            owner.SendEvent(clickEventDict[type], sender);

                        if (onClickMethodDict.ContainsKey(type))
                            onClickMethodDict[type]?.Invoke();
                    });
                }
            }

            UpdateText();
        }

        private void Awake()
        {
            root = GetComponent<ContextElement>();
            owner = GetComponent<GraphOwner>();
            currentBB = GetComponent<Blackboard>();

            SetButtonFunctions();
        }

        //

        protected void OnClickClose()
        {
            UnSubscribeBackButton();

            PopupManager.Instance.Close(gameObject);

            var animator = GetComponent<Animator>();
            if (animator != null) animator.SetTrigger("Close");
        }

        protected void OnToggle(T type) => Toggle(type);

        //

        protected void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }

        protected void UnSubscribeBackButton()
        {
            MetaSystem.UnSubscribeBackButton(owner.GetHashCode());
        }

        protected void SendResetEvent()
        {
            MessageDispatcher.Dispatch("OnSystemEvent", new EventData("SystemReset"));
        }

        protected void Toggle(T type)
        {
            string key = playerPrefsKeyDict[type];
            int state = PlayerPrefs.GetInt(key, 0) + 1;
            if (state >= buttonTextDict[type].Length) state = 0;
            PlayerPrefs.SetInt(key, state);

            UpdateText();
        }

        protected virtual void UpdateText()
        {
            T[] types = (T[])System.Enum.GetValues(typeof(T));
            for (int i = 0; i < types.Length; ++i)
            {
                T type = types[i];

                if (!elementNameDict.ContainsKey(type))
                    continue;

                ContextElement element = ContextUtils.FindElement(contentsElement, elementNameDict[type] + "/Text", ContextSearchingType.FullNameSearch);
                if (element is IContextText textElement && buttonTextDict.ContainsKey(type))
                {
                    int state = buttonTextDict[type].Length == 1 ?
                        0 : PlayerPrefs.GetInt(playerPrefsKeyDict[type], 0);
                    textElement.SetText(buttonTextDict[type][state]);
                }
            }
        }
    }
}
