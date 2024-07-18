using GameUtil;
using ParadoxNotion;
using SlotMaker;
using TMPro;
using UnityEngine;


public enum BoyChoseDoorState
{
    None = -1,
    Climb,
    Idle,
    Chose0,
    Chose1,
    Chose2,
}
public class DoorBonus : MonoBehaviour
{
    BoyChoseDoorState boyState = BoyChoseDoorState.None;

    public static readonly string ON_CONTENT_UI_EVENT = "OnContentUIEvent";


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


        cxeRoot = transform.GetComponent<ContextElement>();
        cxeRoot.UpdateContext(true);

        cxeDoor0 = ContextUtils.FindElement(cxeRoot, "Door Chose 01/Animator/door0", ContextSearchingType.FullNameSearch) as IContextClickable;
        cxeDoor1 = ContextUtils.FindElement(cxeRoot, "Door Chose 01/Animator/door1", ContextSearchingType.FullNameSearch) as IContextClickable;
        cxeDoor2 = ContextUtils.FindElement(cxeRoot, "Door Chose 01/Animator/door2", ContextSearchingType.FullNameSearch) as IContextClickable;



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


        cxeDoor0.RemoveAllListener();
        cxeDoor1.RemoveAllListener();
        cxeDoor2.RemoveAllListener();
        cxeDoor0.AddListenerOnClick((ContextElement sender) => {
            if (boyState != BoyChoseDoorState.Idle)
                return;
            ClearTimerCountDown();
            Debug.Log($" == {sender.gameObject.name}");
            cxeAnimatorDoorChose.SetIntProperty(0);//animatorDoorChose.SetInteger("ChoseDoor", 0);
        });
        cxeDoor1.AddListenerOnClick((ContextElement sender) => {
            if (boyState != BoyChoseDoorState.Idle)
                return;
            ClearTimerCountDown();
            Debug.Log($" == {sender.gameObject.name}");
            cxeAnimatorDoorChose.SetIntProperty(1);//animatorDoorChose.SetInteger("ChoseDoor", 1);
        });
        cxeDoor2.AddListenerOnClick((ContextElement sender) => {
            if (boyState != BoyChoseDoorState.Idle)
                return;
            ClearTimerCountDown();
            Debug.Log($" == {sender.gameObject.name}");
            cxeAnimatorDoorChose.SetIntProperty(2);  //animatorDoorChose.SetInteger("ChoseDoor", 2);
        });

    }

    private void OnEnable()
    {
        GSManager.Instance.GetHandler("SB Bubble Appear BGM").Play();
    }

    private void OnDisable()
    {
        GSManager.Instance.GetHandler("SB Bubble Appear").Stop();
    }

    //private void OnDisable()
    private void OnDestroy()
    {
        MessageDispatcher.UnRegister(ON_CONTENT_UI_EVENT, OnContentUIEvent);

        cxeDoor0.RemoveAllListener();
        cxeDoor1.RemoveAllListener();
        cxeDoor2.RemoveAllListener();

        ClearTimerCountDown();     
    }

    public void OnContentUIEvent(EventData eventData){

        if (eventData.name == "Start Door")
        {
            boyState = BoyChoseDoorState.Climb; //爬树
            cxeDoorChose.gameObject.SetActive(true);           
        }
        else if (eventData.name == "BoyClimbFinish")
        {
            boyState = BoyChoseDoorState.Idle;
            StartTimerCountDown();
        }
        else if (eventData.name == "ChoseDoor0")
        {
            boyState = BoyChoseDoorState.Chose0;
            cxeDoorChose.gameObject.SetActive(false);
            cxeMiniGame0.gameObject.SetActive(true);
        }
        else if (eventData.name == "ChoseDoor1")
        {
            boyState = BoyChoseDoorState.Chose1;
            cxeDoorChose.gameObject.SetActive(false);
            cxeMiniGame1.gameObject.SetActive(true);
        }
        else if (eventData.name == "ChoseDoor2")
        {
            boyState = BoyChoseDoorState.Chose2;
            cxeDoorChose.gameObject.SetActive(false);
            cxeMiniGame2.gameObject.SetActive(true);
        }
        else if (eventData.name == "BSTMiniGameFinish")
        {
            boyState = BoyChoseDoorState.None;
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

}
