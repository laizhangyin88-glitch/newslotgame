using SBoxApi;
using SlotMaker;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SBoxInit : MonoSingleton<SBoxInit>
{
    private UnityAction onComplete;

    public void Init(string matchIp, UnityAction onComplete = null)
    {
        SBoxModel.Instance.matchIp = matchIp;
        this.onComplete = onComplete;
        if (!transform.TryGetComponent(out SBox Sbox))
            gameObject.AddComponent<SBox>();

#if !UNITY_EDITOR
        SBox.Init();
#endif
        AddEventListener();
        StartCoroutine(CheckSBoxReady());
    }


    private void AddEventListener()
    {
        BlizzEvent.EventCenter.Instance.AddEventListener<int>(EventHandle.SBOX_READY, OnSBoxReady);
        BlizzEvent.EventCenter.Instance.AddEventListener<int>(SBoxEventHandle.SBOX_SADNBOX_RESET, OnSBoxSandboxReset);
    }

    private void OnSBoxReady(int machineId)
    {
        SBoxModel.Instance.machineId = machineId;
        SBoxModel.Instance.isReady = true;
    }

    private void OnSBoxSandboxReset(int result)
    {
        if (result != 0)
        {
            Debug.LogError("SBoxError When Call SBoxReset!");
            return;
        }
        onComplete?.Invoke();
    }

    private IEnumerator CheckSBoxReady()
    { 
        while (!SBoxModel.Instance.isReady)
        {
#if UNITY_EDITOR
            MatchDebugManager.Instance.SendUdpMessage(EventHandle.CHECK_SBOX_READY, BlizzUtils.Utils.LocalIP());
#else
            SBoxModel.Instance.isReady = SBoxSandbox.Ready();
#endif
            yield return new WaitForSeconds(0.5f);
        }

#if UNITY_EDITOR
        MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_RESET);
#else
        SBoxSandbox.Reset();
        SBoxSandboxListener.Instance.Init();
#endif
    }
}
