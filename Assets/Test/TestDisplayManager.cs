using ParadoxNotion;
using SimpleJSON;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDisplayManager : MonoBehaviour
{
    public const string LOGINISGM = "is_gm";

    [SerializeField] private GameObject _testObj;

    void Start()
    {
        //如果非测试包，则关闭页面3(AutoUrl)
#if K3K_TEST || MARS_FORTUNE_TEST
        TestManager.Instance.SetAutoUrlEnable(true);
        SetTestManagerEnable(true);
#else
        TestManager.Instance.SetAutoUrlEnable(false);
        SetTestManagerEnable(false);
#endif
    }

    public void SetTestManagerEnable(bool enable)
    {
        _testObj.SetActive(enable);
    }

    private void OnEnable()
    {
        MessageDispatcher.Register(RPCName.login, OnReceiveLoginHandle);
    }

    private void OnDisable()
    {
        MessageDispatcher.UnRegister(RPCName.login, OnReceiveLoginHandle);
    }

    private void OnReceiveLoginHandle(EventData eventData)
    {
        EventData<JSONNode> jsonEventData = eventData as EventData<JSONNode>;
        bool isGM = false;
        if(jsonEventData.value.HasKey(LOGINISGM))
        {
            if(jsonEventData.value[LOGINISGM].AsInt == 1)
                isGM = true;
        }

        SetTestManagerEnable(isGM);
        if (isGM)
            TestManager.Instance.SetMaxPageIndex(2);
    }
}
