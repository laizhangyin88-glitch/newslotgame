using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion;

namespace SlotMaker
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public class PopupManager : MonoWeakSingleton<PopupManager>
    {
        public Transform contents;
        public Transform overlay;

        private int lastSortingOrder = 1;
        public int sortingOrderSpace = 10;

    	public List<Popup> stack = new List<Popup>();

        private CanvasGroup mainCanvasGroup;
        private CanvasGroup gameCanvasGroup;

        private int _popupCount;
        public int popupCount { get { return _popupCount; } }

        private const string ON_META_UI_EVENT = "OnMetaUIEvent";
        private const string ON_CHANGED_POPUP_COUNT_EVENT = "ChangedPopupCount";

        private void Awake()
        {
            var mainCanvas = GameObject.Find("Main Canvas");
            if (mainCanvas != null)
                mainCanvasGroup = mainCanvas.GetComponent<CanvasGroup>();

            var gameCanvas = GameObject.Find("Game Canvas");
            if (gameCanvas != null)
                gameCanvasGroup = gameCanvas.GetComponent<CanvasGroup>();
        }

    	private Popup Peek()
    	{
    		return stack[stack.Count-1];
    	}

    	private bool Exist()
    	{
    		return stack.Count > 0;
    	}

    	private void Push(Popup popup)
    	{
    		stack.Add(popup);
            ++lastSortingOrder;
            if (!popup.ignoreCounting)
            {
                ++_popupCount;
                MessageDispatcher.Dispatch(ON_META_UI_EVENT, new EventData<int>(ON_CHANGED_POPUP_COUNT_EVENT, _popupCount));
            }
    	}

    	private Popup Pop()
    	{
            Popup popup = null;
    		if (Exist())
    		{
                if (!stack[stack.Count - 1].ignoreCounting)
                {
                    --_popupCount;
                    MessageDispatcher.Dispatch(ON_META_UI_EVENT, new EventData<int>(ON_CHANGED_POPUP_COUNT_EVENT, _popupCount));
                }

                popup = stack[stack.Count - 1];
    			stack.RemoveAt(stack.Count - 1);

                if(stack.Count == 0)
                    lastSortingOrder = 1;
    		}
    		else
    		{
    			Debug.Log("[PopupManager] There is no popup in 'PopupManager'.");
    		}
            return popup;
    	}

        private void PopAt(Popup popup)
        {
            if (Exist())
            {
                if (stack.Contains(popup))
                {
                    if (!popup.ignoreCounting)
                    {
                        --_popupCount;
                        MessageDispatcher.Dispatch(ON_META_UI_EVENT, new EventData<int>(ON_CHANGED_POPUP_COUNT_EVENT, _popupCount));
                    }

                    stack.Remove(popup);
                }

                if (stack.Count == 0)
                    lastSortingOrder = 1;
            }
            else
            {
                Debug.Log("[PopupManager] There is no popup in 'PopupManager'.");
            }
        }

    	public void Open(GameObject obj)
    	{
    		if (Exist())
    			Peek().InActive();
    		else
                SetInteractable(false);

            Popup popup = obj.GetComponent<Popup>();
    		if (popup == null)
                popup = obj.AddComponent<Popup>();

            popup.Open();

            if(!InternalEventRouter.Instance.IsEmpty)
                InternalEventRouter.Instance.Invoke( new EventRouterData("PopupOpen", popup) );

            popup.UpdateSortingLayer(lastSortingOrder * sortingOrderSpace);
            Push(popup);

            CameraManager.Get().GetComponent<Animator>().SetInteger("Popup Count", popupCount);
    	}

        public void Clear()
        {
            _popupCount = 0;
            List<Popup> newStack = new List<Popup>();

            int stackCount = stack.Count;
            for (int i = 0; i < stack.Count; ++i)
            {
                Popup popup = stack[i];

                if(popup != null)
                {
                    DestroyMask mask = popup.GetComponent<DestroyMask>();
                    if (DestroyMask.HasAttribute(mask.policy, DestroyMask.DestroyPolicy.Auto))
                    {
                        GameObject.Destroy(popup.gameObject);
                    }
                    else
                    {
                        newStack.Add(popup);
                        if (!popup.ignoreCounting)
                            ++_popupCount;
                    }
                }
            }

            foreach (Transform child in transform)
            {
                if (child.CompareTag("System"))
                {
                    foreach (Transform nestedChild in child)
                    {
                        DestroyMask mask = nestedChild.GetComponent<DestroyMask>();
                        if (mask != null && DestroyMask.HasAttribute(mask.policy, DestroyMask.DestroyPolicy.Auto))
                        {
                            GameObject.Destroy(nestedChild.gameObject);
                        }
                    }
                }
            }

            stack = newStack;
            if (stack.Count == 0)
                lastSortingOrder = 1;

            MessageDispatcher.Dispatch(ON_META_UI_EVENT, new EventData<int>(ON_CHANGED_POPUP_COUNT_EVENT, _popupCount));

            SetInteractable(true);
            CameraManager.Get().GetComponent<Animator>().SetInteger("Popup Count", popupCount);
        }

    	public void Close()
    	{
    		var popup = Pop();

            if(!InternalEventRouter.Instance.IsEmpty)
                InternalEventRouter.Instance.Invoke( new EventRouterData("PopupClose", popup) );

    		if (Exist())
    			Peek().Active();
    		else
    			SetInteractable(true);

            CameraManager.Get().GetComponent<Animator>().SetInteger("Popup Count", popupCount);
    	}

        public void Close(GameObject obj)
        {
            var popup = obj.GetComponent<Popup>();

            if(!InternalEventRouter.Instance.IsEmpty)
                InternalEventRouter.Instance.Invoke( new EventRouterData("PopupClose", popup) );

            PopAt(popup);
            if (Exist())
                Peek().Active();
            else
                SetInteractable(true);

            CameraManager.Get().GetComponent<Animator>().SetInteger("Popup Count", popupCount);
        }

        private void SetInteractable(bool interactable)
        {
            if (mainCanvasGroup != null) mainCanvasGroup.interactable = interactable;
            if (gameCanvasGroup != null) gameCanvasGroup.interactable = interactable;
        }
    }
}
