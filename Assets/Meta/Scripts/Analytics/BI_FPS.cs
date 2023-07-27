using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.StateMachines;
using SlotMaker;

namespace BagelCode
{

public class BI_FPS : MonoBehaviour, IStateCallbackReceiver
{
	private Dictionary<string, Action> delegates = new Dictionary<string, Action>();

	private void Awake()
	{
		delegates["Lobby"]  = OnExitLobby;
		delegates["InGame"] = OnExitGame;
	}

	public void OnStateEnter(IState state)
	{
		if (delegates.ContainsKey(state.name))
			PerformanceAnalyzer.Instance.BeginSample();
	}

    public void OnStateUpdate(IState state)
    {
        
    }

	public void OnStateExit(IState state)
	{
		Action del = null;
		if (delegates.TryGetValue(state.name, out del))
			del();
	}

    private void OnExitLobby()
	{
		PerformanceAnalyzer.Instance.EndSample();
		var sample = PerformanceAnalyzer.Instance.sample;

		Analytics.CustomEvent("client_fps", new Dictionary<string, object>
		{
			{ "type", "slot_enter" },
			{ "average", sample.averageFPS },
			{ "percentile_10th", sample.lower10FPS },
			{ "percentile_1st", sample.lower1FPS },
			{ "total_sampling_time", sample.totalTimeSpan }
		});

        Analytics.CustomEvent("client_performance_metric", new Dictionary<string, object>
        {
            { "type", "slot_enter" },
            { "data", PerformanceAnalyzer.Instance.timeSamples }
        });
        PerformanceAnalyzer.Instance.timeSamples.Clear();
	}

	private void OnExitGame()
	{
		PerformanceAnalyzer.Instance.EndSample();
		var sample = PerformanceAnalyzer.Instance.sample;	
        int gameId = BlackboardUtils.FindVariable<int>(null, "./game/gameId").value;

		Analytics.CustomEvent("client_fps", new Dictionary<string, object>
		{
			{ "type", "slot_leave" },
			{ "average", sample.averageFPS },
			{ "percentile_10th", sample.lower10FPS },
			{ "percentile_1st", sample.lower1FPS },
			{ "total_sampling_time", sample.totalTimeSpan },
			{ "game_id", gameId }
		});
        
        Analytics.CustomEvent("client_performance_metric", new Dictionary<string, object>
        {
            { "type", "slot_leave" },
            { "data", PerformanceAnalyzer.Instance.timeSamples },
            { "game_id", gameId }
        });
        PerformanceAnalyzer.Instance.timeSamples.Clear();
	}
}

}
