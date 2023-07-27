using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Services;

namespace SlotMaker
{

[RequireComponent(typeof(GraphOwner))]
public class ManualGraphOwner : MonoBehaviour
{
	public GraphOwner.EnableAction enableAction = GraphOwner.EnableAction.EnableBehaviour;
	public GraphOwner.DisableAction disableAction = GraphOwner.DisableAction.DisableBehaviour;

	private GraphOwner _owner = null;
	protected GraphOwner owner { get { return _owner ?? (_owner = GetComponent<GraphOwner>()); } }

	private MessageRouter _router = null;
	protected MessageRouter router
	{
		get
		{
			if (_router == null)
			{
				_router = GetComponent<MessageRouter>();
				if (_router == null)
					_router = owner.gameObject.AddComponent<MessageRouter>();
			}
			return _router;
		}
	}

	private static bool isQuiting;
	private bool startCalled = false;

	private bool dispatched = false;
	private int autoUpdate = 0;

	public void StartBehaviour()
	{
		if (owner.isRunning)
			return;

		router.onDispatched += OnDispatch;
		dispatched = true;
		autoUpdate = 0;

		owner.StartBehaviour(false, null); //execute BeginAutoUpdate() in FSM
	}

	public void StopBehaviour()
	{
		if (!owner.isRunning)
			return;

		owner.StopBehaviour();
		router.onDispatched -= OnDispatch;
	}

	private void Start()
	{
		startCalled = true;
		if (enableAction == GraphOwner.EnableAction.EnableBehaviour)
			StartBehaviour();
	}

	private void OnEnable()
	{
		if (startCalled && enableAction == GraphOwner.EnableAction.EnableBehaviour)
			StartBehaviour();
	}

	private void OnDisable()
	{
		if (isQuiting)
			return;

		if (disableAction == GraphOwner.DisableAction.DisableBehaviour)
			StopBehaviour();
	}

	protected void OnApplicationQuit()
	{
		isQuiting = true;
	}

	private void Update()
	{
		if (dispatched || autoUpdate > 0)
		{
            dispatched = false;

			owner.UpdateBehaviour();
		}
	}

	public void OnDispatch()
	{
		dispatched = true;
	}

	public void BeginAutoUpdate()
	{
		++autoUpdate;
	}

	public void EndAutoUpdate(bool forceWaitFrame = false)
	{
        if (forceWaitFrame)
            dispatched = true;

		if (autoUpdate > 0)
			--autoUpdate;
	}
}

}
