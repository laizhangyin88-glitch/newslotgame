using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace SlotMaker
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(DestroyMask))]
    public class Popup : MonoBehaviour, IEventRouterDataObject
    {
        public bool ignoreCounting = false;
        private string guid;

        public string GetGuid()
        {
            if (string.IsNullOrEmpty(guid))
                guid = System.Guid.NewGuid().ToString();
            return guid;
        }

        public string GetObjectName()
        {
            return gameObject.name;
        }

        public void Open()
        {
            int layer = transform.root.gameObject.layer;
            Transform[] children = gameObject.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < children.Length; ++i)
            {
                children[i].gameObject.layer = layer;
            }
        }

    	public void Close()
    	{
    		PopupManager.Instance.Close();

            if (ApplicationSettings.LogTest())
    		    Debug.Log("[Popup] Close: " + gameObject);
    	}

    	public void InActive()
    	{
            if (ApplicationSettings.LogTest())
    		    Debug.Log("[Popup] InActive: " + gameObject);

            GetComponent<CanvasGroup>().interactable = false;
    	}

    	public void Active()
    	{
            if (ApplicationSettings.LogTest())
    		    Debug.Log("[Popup] Active: " + gameObject);

            GetComponent<CanvasGroup>().interactable = true;
    	}

        public void UpdateSortingLayer(int sortingOrder)
        {
            var canvas = GetComponent<Canvas>();
            var rootCanvas = transform.parent.GetComponentInParent<Canvas>();
            canvas.sortingLayerID = rootCanvas.sortingLayerID;
            canvas.sortingOrder = sortingOrder;

            OverrideSortingLayer[] overriders = GetComponentsInChildren<OverrideSortingLayer>(true);
            for (int i = 0; i < overriders.Length; ++i)
            {
                overriders[i].UpdateSortingLayer();
            }
        }

        private void Awake()
        {
            var canvas = GetComponent<Canvas>();
            canvas.overrideSorting = true;
        }
    }
}
