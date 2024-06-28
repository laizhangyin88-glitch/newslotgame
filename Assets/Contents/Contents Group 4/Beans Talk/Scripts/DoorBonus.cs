using GameUtil;
using ParadoxNotion;
using SlotMaker;
using SlotMaker.Keno.Events;
using System.Runtime.Remoting.Contexts;
using TMPro;
using UnityEngine;

public class DoorBonus : MonoBehaviour
{
    public static readonly string ON_CONTENT_UI_EVENT = "OnContentUIEvent";
    //public static readonly string ON_CUSTOM_EVENT = "OnCustomEvent";  //

    int clickIndex = -1;
    LoopTimer _countDownTimer;

    ContextElement cxeRoot;


    //public Animator animatorDoorChose;
    ContextAnimator cxeAnimatorDoorChose;

    //public GameObject doorChose;
    ContextCompositor cxeDoorChose;


    //GameObject miniGame0;
    ContextCompositor cxeMiniGame0, cxeMiniGame1,cxeMiniGame2;

    IContextClickable cxeDoor0;
    IContextClickable cxeDoor1;
    IContextClickable cxeDoor2;

    //public TextMeshProUGUI countDownText;
    ContextTextMeshProUGUI cxeCountDown;
    void Start()
    {
        MessageDispatcher.Register(ON_CONTENT_UI_EVENT, OnContentUIEvent);
        //MessageDispatcher.Register(ON_CUSTOM_EVENT, OnCustomEvent);

        cxeRoot = transform.GetComponent<ContextElement>();
        cxeRoot.UpdateContext(true);

        cxeDoor0 = ContextUtils.FindElement(cxeRoot, "Door Chose 01/Animator/door0", ContextSearchingType.FullNameSearch) as IContextClickable;
        cxeDoor1 = ContextUtils.FindElement(cxeRoot, "Door Chose 01/Animator/door1", ContextSearchingType.FullNameSearch) as IContextClickable;
        cxeDoor2 = ContextUtils.FindElement(cxeRoot, "Door Chose 01/Animator/door2", ContextSearchingType.FullNameSearch) as IContextClickable;

        cxeDoor0.RemoveAllListener();
        cxeDoor1.RemoveAllListener();
        cxeDoor2.RemoveAllListener();

        cxeCountDown = ContextUtils.FindElement(cxeRoot, "Door Chose 01/Animator/countDown", ContextSearchingType.FullNameSearch) as ContextTextMeshProUGUI;

        cxeMiniGame0 = ContextUtils.FindElement(cxeRoot, "miniGame00", ContextSearchingType.ChildrenSearch) as ContextCompositor;
        cxeMiniGame1 = ContextUtils.FindElement(cxeRoot, "miniGame01", ContextSearchingType.ChildrenSearch) as ContextCompositor;
        cxeMiniGame2 = ContextUtils.FindElement(cxeRoot, "miniGame02", ContextSearchingType.ChildrenSearch) as ContextCompositor;

        cxeAnimatorDoorChose = ContextUtils.FindElement(cxeRoot, "Door Chose 01/Animator", ContextSearchingType.FullNameSearch) as ContextAnimator;

        cxeDoorChose = ContextUtils.FindElement(cxeRoot, "Door Chose 01", ContextSearchingType.ChildrenSearch) as ContextCompositor;



        //cxeMiniGame5 = ContextUtils.FindElement(cxeRoot, "miniGame5", ContextSearchingType.ChildrenSearch) as ContextCompositor;

        cxeMiniGame0.gameObject.SetActive(false);
        cxeMiniGame1.gameObject.SetActive(false);
        cxeMiniGame2.gameObject.SetActive(false);
    }

    //private void OnDisable()
    private void OnDestroy()
    {
        MessageDispatcher.UnRegister(ON_CONTENT_UI_EVENT, OnContentUIEvent);
        //MessageDispatcher.UnRegister(ON_CUSTOM_EVENT, OnCustomEvent);

        ClearTimerCountDown();     
    }

    public void OnCustomEvent(EventData eventData)
    {
        if (eventData.name == "BSTMiniGameFinish")
        {
            cxeDoorChose.gameObject.SetActive(false);
            cxeMiniGame0.gameObject.SetActive(false);
            cxeMiniGame1.gameObject.SetActive(false);
            cxeMiniGame2.gameObject.SetActive(false);
            //通知主状态机切换状态
        }
    }
    public void OnContentUIEvent(EventData eventData){

        if (eventData.name == "Start Door")
        {
            cxeDoor0.RemoveAllListener();
            cxeDoor1.RemoveAllListener();
            cxeDoor2.RemoveAllListener();
            //doorChose.SetActive(true);
            cxeDoorChose.gameObject.SetActive(true);
            //爬树
            Debug.Log("i am here");
        }
        else if (eventData.name == "BoyClimbFinish")
        {

            cxeDoor0.AddListenerOnClick((ContextElement sender) => {
                ClearTimerCountDown();
                Debug.Log($" == {sender.gameObject.name}");
                //animatorDoorChose.SetInteger("ChoseDoor", 0);
                cxeAnimatorDoorChose.SetIntProperty(0);
            });

            cxeDoor1.AddListenerOnClick((ContextElement sender) => {
                ClearTimerCountDown();
                Debug.Log($" == {sender.gameObject.name}");
                //animatorDoorChose.SetInteger("ChoseDoor", 1);
                cxeAnimatorDoorChose.SetIntProperty(1);
            });

            cxeDoor2.AddListenerOnClick((ContextElement sender) => {
                ClearTimerCountDown();
                Debug.Log($" == {sender.gameObject.name}");
                //animatorDoorChose.SetInteger("ChoseDoor", 2);
                cxeAnimatorDoorChose.SetIntProperty(2);
            });

            StartTimerCountDown();
        }
        else if (eventData.name == "ChoseDoor0")
        {
            //cxeDoor0.RemoveAllListener();
            //cxeDoor1.RemoveAllListener();
            //cxeDoor2.RemoveAllListener();

            //doorChose.SetActive(false);
            cxeDoorChose.gameObject.SetActive(false);
            cxeMiniGame0.gameObject.SetActive(true);
            //miniGame0.gameObject.SetActive(false);


            //通知主状态机切换状态
        }
        else if (eventData.name == "ChoseDoor1")
        {
            //cxeDoor0.RemoveAllListener();
            //cxeDoor1.RemoveAllListener();
            //cxeDoor2.RemoveAllListener();

            //doorChose.SetActive(false);
            cxeDoorChose.gameObject.SetActive(false);
            cxeMiniGame1.gameObject.SetActive(true);


            //通知主状态机切换状态
        }
        else if (eventData.name == "ChoseDoor2")
        {
            //cxeDoor0.RemoveAllListener();
            //cxeDoor1.RemoveAllListener();
            //cxeDoor2.RemoveAllListener();

            //doorChose.SetActive(false);
            cxeDoorChose.gameObject.SetActive(false);
            cxeMiniGame2.gameObject.SetActive(true);
        }
        else if (eventData.name == "BSTMiniGameFinish")
        {
            cxeDoorChose.gameObject.SetActive(false);
            cxeMiniGame0.gameObject.SetActive(false);
            cxeMiniGame1.gameObject.SetActive(false);
            cxeMiniGame2.gameObject.SetActive(false);
        }
    }



    void ClearTimerCountDown()
    {
        if (_countDownTimer != null)
            _countDownTimer.Cancel();
        _countDownTimer = null;
    }

    void StartTimerCountDown()
    {
        var timeVal = 4;

        cxeCountDown.UpdateContext(true);
        cxeCountDown.SetText(timeVal.ToString());

        ClearTimerCountDown();
        _countDownTimer = TimerExtensions.LoopAction(this, 1, (a) =>
        {
            --timeVal;
            if (timeVal < 0)
            {
                ClearTimerCountDown();

                clickIndex = 0;
                //animatorDoorChose.SetInteger("ChoseDoor", clickIndex);
                cxeAnimatorDoorChose.SetIntProperty(clickIndex);
                return;
            }
            else
            {
                cxeCountDown.SetText(timeVal.ToString());
            }
        });
    }



    public void testclick01()
    {
        Debug.Log("i am click 01");
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
