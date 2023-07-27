using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

using SlotMaker;

namespace BagelCode
{
// Scratched from https://github.com/PimDeWitte/UnityMainThreadDispatcher
public class MainThreadDispatcher : MonoWeakSingleton<MainThreadDispatcher> {
	private readonly Queue<Action> _executionQueue = new Queue<Action>();

    public void Awake() {
        // To initialise Instance in Main Thread
        MainThreadDispatcher instance = MainThreadDispatcher.Instance;
    }

	public void Update() {
        // Check Count first to avoid locking in every Update
        if (_executionQueue.Count > 0) {
            lock(_executionQueue) {
                while (_executionQueue.Count > 0) {
                    _executionQueue.Dequeue().Invoke();
                }
            }
        }
	}

	public void Enqueue(IEnumerator action) {
		lock (_executionQueue) {
			_executionQueue.Enqueue (() => {
				StartCoroutine(action);
			});
		}
	}

	public void Enqueue(Action action)
	{
		Enqueue(ActionWrapper(action));
	}
	
	IEnumerator ActionWrapper(Action a)
	{
		a();
		yield return null;
	}
}

}