using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using ParadoxNotion;
using ParadoxNotion.Services;

namespace SlotMaker
{

[RequireComponent(typeof(MessageRouter))]
public class DragMessageDispatcher : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
	private MessageRouter _router = null;
	protected MessageRouter router { get { return _router ?? (_router = GetComponent<MessageRouter>()); } }

	private const string ON_BEGIN_DRAG = "OnBeginDrag";
	private const string ON_END_DRAG = "OnEndDrag";

	public void OnBeginDrag(PointerEventData eventData)
	{
		router.Dispatch(ON_BEGIN_DRAG, eventData);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		router.Dispatch(ON_END_DRAG, eventData);
	}
}

}
