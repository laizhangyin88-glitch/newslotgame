using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Services;
using NodeCanvas.Framework;

namespace SlotMaker
{
	public class MetaUIEventGraphStarter : MonoBehaviour
	{
		public string eventName;

		private const string ON_META_UI_EVENT = "OnMetaUIEvent";

		private GraphOwner _owner = null;
		protected GraphOwner owner { get { return _owner ?? (_owner = GetComponent<GraphOwner>()); } }

		private void OnEnable()
		{
			MessageDispatcher.Register(ON_META_UI_EVENT, Dispatch);
		}

		private void OnDisable()
		{
			MessageDispatcher.UnRegister(ON_META_UI_EVENT, Dispatch);
		}

		public void Dispatch(EventData eventData)
		{
			if (eventData.name.Equals(eventName))
				owner.StartBehaviour();
		}
	}
}
