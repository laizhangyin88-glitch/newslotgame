using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Reflection;

namespace ParadoxNotion.Services{

	///Automaticaly added to a gameobject when needed.
	///Handles forwarding Unity event messages to listeners that need them as well as Custom event forwarding.
	///Notice: this is a partial class! Add your own methods to forward events as you please.
	public partial class OriginalMessageRouter : MessageRouter
			, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler,
			IDragHandler, IScrollHandler, IUpdateSelectedHandler, ISelectHandler, IDeselectHandler, IMoveHandler, ISubmitHandler
	{
		public void OnPointerEnter(PointerEventData eventData){
			Dispatch("OnPointerEnter", eventData);
		}

		public void OnPointerExit(PointerEventData eventData){
			Dispatch("OnPointerExit", eventData);
		}

		public void OnPointerDown(PointerEventData eventData){
			Dispatch("OnPointerDown", eventData);
		}

		public void OnPointerUp(PointerEventData eventData){
			Dispatch("OnPointerUp", eventData);
		}

		public void OnPointerClick(PointerEventData eventData){
			Dispatch("OnPointerClick", eventData);
		}

		public void OnDrag(PointerEventData eventData){
			Dispatch("OnDrag", eventData);
		}

		public void OnDrop(BaseEventData eventData){
			Dispatch("OnDrop", eventData);
		}

		public void OnScroll(PointerEventData eventData){
			Dispatch("OnScroll", eventData);
		}

		public void OnUpdateSelected(BaseEventData eventData){
			Dispatch("OnUpdateSelected", eventData);
		}

		public void OnSelect(BaseEventData eventData){
			Dispatch("OnSelect", eventData);
		}

		public void OnDeselect(BaseEventData eventData){
			Dispatch("OnDeselect", eventData);
		}

		public void OnMove(AxisEventData eventData){
			Dispatch("OnMove", eventData);
		}

		public void OnSubmit(BaseEventData eventData){
			Dispatch("OnSubmit", eventData);
		}
	}
}