using BagelCode;
using BagelCode.Tasks.Actions.Contents;
using Dreamteck.Splines.Primitives;
using NodeCanvas.Framework;
using SimpleJSON;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EventData = ParadoxNotion.EventData;

public enum BoyStealEggState
{
    None = -1,
    WaitWomanWork,
    Move,
    Steal,
    StealFinish,
    MoveMiddle,
    Hide,
    File,
}




public class MiniGame0 : MonoBehaviour
{

    Transform root,bg,doors;
    Animator ani_Woman, ani_Boy, ani_Fire, ani_Egg, ani_Goose, ani_Door1, ani_Door2, ani_Door3, ani_Door4, ani_Door5;
    Button btn_Door1, btn_Door2, btn_Door3, btn_Door4, btn_Door5;
    TextMeshProUGUI betNum, winNum, scoreNum, countDownText;
    //List<bool> openList = new List<bool>() { true,false,true,true,false};
    int fireIndex = 2;//着火的门index
    int clickIndex = 0;
    Dictionary<int, Vector2> dicPos = new Dictionary<int, Vector2>();

    /*
    Dictionary<int, int> dic_Score = new Dictionary<int, int>
    {
        [1] = 100,
        [2] = 200,
        [3] = 300,
        [4] = 400,
        [5] = 500,
    };*/

    const float BOY_STEAL_Y = -234;
    const float BOY_HIDE_Y = -193;

    Vector2 startPos01 = new Vector2(387, BOY_STEAL_Y); 
    Vector2 endPos01 = new Vector2(-197, BOY_STEAL_Y);
    Vector2 middlePos01 = new Vector2(60, BOY_STEAL_Y);


    BoyStealEggState boyState = BoyStealEggState.None;


    Coroutine _cor;
    List<int> lst_Score = new List<int>() { 100, 200, 300, 400, 500 };


    void Awake()
    {
        root = transform.Find("Animator/Anchor");
        bg = root.Find("fg");
        doors = root.Find("doors");
        ani_Woman = root.Find("woman").GetComponent<Animator>();
        ani_Boy = root.Find("boy").GetComponent<Animator>();
        ani_Fire = root.Find("fire").GetComponent<Animator>();
        ani_Egg = root.Find("egg").GetComponent<Animator>();
        ani_Goose = root.Find("goose").GetComponent<Animator>();
        ani_Door1 = root.Find("doors/door1").GetComponent<Animator>();
        ani_Door2 = root.Find("doors/door2").GetComponent<Animator>();
        ani_Door3 = root.Find("doors/door3").GetComponent<Animator>();
        ani_Door4 = root.Find("doors/door4").GetComponent<Animator>();
        ani_Door5 = root.Find("doors/door5").GetComponent<Animator>();
        btn_Door1 = ani_Door1.GetComponent<Button>();
        btn_Door2 = ani_Door2.GetComponent<Button>();
        btn_Door3 = ani_Door3.GetComponent<Button>();
        btn_Door4 = ani_Door4.GetComponent<Button>();
        btn_Door5 = ani_Door5.GetComponent<Button>();
        scoreNum = root.Find("score").GetComponent<TextMeshProUGUI>();
        betNum = root.Find("betWin/bet/betNum").GetComponent<TextMeshProUGUI>();
        winNum = root.Find("betWin/win/winNum").GetComponent<TextMeshProUGUI>();
        countDownText = root.Find("countDown").GetComponent<TextMeshProUGUI>();

        dicPos.Add(1, ani_Door1.transform.localPosition);
        dicPos.Add(2, ani_Door2.transform.localPosition);
        dicPos.Add(3, ani_Door3.transform.localPosition);
        dicPos.Add(4, ani_Door4.transform.localPosition);
        dicPos.Add(5, ani_Door5.transform.localPosition);

        AddBtnEvent();


        //Debug.Log($"【ani_Boy】 x={ani_Boy.transform.localPosition.x}   y={ani_Boy.transform.localPosition.y}");

    }

    public static readonly string ON_CONTENT_UI_EVENT = "OnContentUIEvent";

    private void Start()
    {
        MessageDispatcher.Register(ON_CONTENT_UI_EVENT, OnContentUIEvent);
        
        //BoyHide();
        //BoyFire(3);
    }

    void OnEnable()
    {
        //Debug.Log("BST  i am enable");

        Dictionary<string, object> req = new Dictionary<string, object>
        {
            {"jackpot_game_index",0}
        };

        NetManager.Instance.Post(RPCName.newClaimBonus, req,
        (res) =>
        {
            string resStr = res.ToString();
            Debug.Log(resStr);

            lst_Score = new List<int>();
            foreach (JSONNode item in res["game_result"]["jackpot_game_result_list"])
            {
                lst_Score.Add((int)item);
            }

            Blackboard bonusBB = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(ContentBlackboard.Get(), "bonus");
            BlackboardUtils.SetOrCreateValue(bonusBB, "responseNew", resStr);
            BlackboardUtils.SetOrCreateValue(bonusBB, "bonusName", "egg");
            //BeginBonusNew.CreatBonus(resStr,"egg");

            BoyStealEgg(startPos01);
        },
        (error) =>
        {
            GlobalErrorHandler.GlobalError(error);
        });


    }
    void OnDisable()
    {
       // Debug.Log("BST  i am disenable");
        ClearCor();
    }

    private void OnDestroy()
    {
        dicPos?.Clear();
        RemoveBtnEvent();
        ClearCor();
        MessageDispatcher.UnRegister(ON_CONTENT_UI_EVENT, OnContentUIEvent);
    }
    public void OnContentUIEvent(EventData eventData)
    {

        if (eventData.name == "BSTRawEgg")//下蛋
        {
            ani_Egg.Play("Show Egg");
        }
        else if (eventData.name == "BSTWomanWorkFinish")//偷蛋
        {
            boyState = BoyStealEggState.WaitWomanWork;
        }
        else if (eventData.name == "BSTBoySteal")//偷蛋
        {
            ani_Goose.SetTrigger("MissBoy");//ani_Goose.Play("Raw Egg");
        }
        else if (eventData.name == "BSTBoyStealFinish")//偷蛋结束
        {
            boyState = BoyStealEggState.StealFinish;
        }
        else if (eventData.name == "BSTBoyMoveMiddle")//偷蛋结束
        {
            boyState = BoyStealEggState.MoveMiddle;
        }
    }


    void AddBtnEvent()
    {
        btn_Door1.onClick.AddListener(ClickDoor1);
        btn_Door2.onClick.AddListener(ClickDoor2);
        btn_Door3.onClick.AddListener(ClickDoor3);
        btn_Door4.onClick.AddListener(ClickDoor4);
        btn_Door5.onClick.AddListener(ClickDoor5);
    }
    void RemoveBtnEvent()
    {
        btn_Door1.onClick.RemoveListener(ClickDoor1);
        btn_Door2.onClick.RemoveListener(ClickDoor2);
        btn_Door3.onClick.RemoveListener(ClickDoor3);
        btn_Door4.onClick.RemoveListener(ClickDoor4);
        btn_Door5.onClick.RemoveListener(ClickDoor5);
    }

    void ClickDoor1() { OnClickDoor(1); }
    void ClickDoor2() { OnClickDoor(2); }
    void ClickDoor3() { OnClickDoor(3); }
    void ClickDoor4() { OnClickDoor(4); }
    void ClickDoor5() { OnClickDoor(5); }


    void OnClickDoor(int index)
    {
        Debug.Log($"@ click = {index}");

        if (boyState != BoyStealEggState.MoveMiddle)
            return;
        boyState = BoyStealEggState.None;

        if (lst_Score.Count == 0)
        {
            BoyFire(index);
        }
        else
        {
            int j;
            do
            {
                j = Random.Range(1, 5);
            } while (j == index);
            BoyHide(index, j);
        }
    }





    void ClearCor()
    {
        if (_cor != null)
            StopCoroutine(_cor);
        _cor = null;
    }

    void BoyStealEgg(Vector2 startPos)
    {
        ClearCor();
        _cor = StartCoroutine(_BoyStealEgg(startPos));
    }
    private IEnumerator _BoyStealEgg(Vector2 startPos)
    {

        ani_Woman.Play("To Work");
        ani_Goose.Play("Raw Egg");

        countDownText.text = "";

        boyState = BoyStealEggState.None;

        yield return new WaitUntil(() => boyState == BoyStealEggState.WaitWomanWork);

        yield return new WaitForSeconds(0.5f);

        SetDoorCanOpenState(true);

        ani_Boy.transform.localPosition = new Vector2(startPos.x, startPos.y);

        int idx = doors.GetSiblingIndex();
        ani_Boy.transform.SetSiblingIndex(idx+1);

        ani_Boy.Play("Move");
        boyState = BoyStealEggState.Move;

        float time = (startPos.x - endPos01.x) * 0.01f;
        if (time > 2.5f)
            time = 2.5f;
        if (time < 1f)
            time = 1f;

        //Debug.Log($" @ time = {time}");

        AsyncActionUtils.ApplyLocalMovement(this, ani_Boy.transform, startPos, endPos01, time, TweenUtils.VectorTweenLinear, 0, () =>
        {
            boyState = BoyStealEggState.Steal;
            ani_Boy.Play("Steal");
            ani_Goose.Play("Miss Boy"); //ani_Goose.SetTrigger("TriggerMissBoy");//
        });

        yield return new WaitUntil(() => boyState == BoyStealEggState.StealFinish);


        if(lst_Score.Count > 0)
        {
            scoreNum.text = lst_Score[0].ToString();
            lst_Score.RemoveAt(0);
        }



        AsyncActionUtils.ApplyLocalMovement(this, ani_Boy.transform, endPos01, middlePos01, 0.1f, TweenUtils.VectorTweenLinear, 0, () =>{});

        yield return new WaitUntil(() => boyState == BoyStealEggState.MoveMiddle);

        // 倒计时
        // 门闪缩

        bool isDoorLight = false;
        int i = 4;
        while(true)
        {
            countDownText.text = i.ToString();
            SetDoorCanOpenState(isDoorLight);
            yield return new WaitForSeconds(1f);
            isDoorLight =  !isDoorLight;
            if (--i < 0)
            {
                break;
            }
        }
        SetDoorCanOpenState(false);

        scoreNum.text = "";
        OnClickDoor(Random.Range(1, 5));
    }


    void BoyHide(int clickIndex, int fireIndex)
    {
        ClearCor();
        _cor = StartCoroutine(_BoyHide(clickIndex,fireIndex));
    }

    private IEnumerator _BoyHide(int clickIndex, int fireIndex)
    {
        scoreNum.text = "";

        ani_Door1.Play(1 == clickIndex || 1 == fireIndex ? "Open" : "Default");
        ani_Door2.Play(2 == clickIndex || 2 == fireIndex ? "Open" : "Default");
        ani_Door3.Play(3 == clickIndex || 3 == fireIndex ? "Open" : "Default");
        ani_Door4.Play(4 == clickIndex || 4 == fireIndex ? "Open" : "Default");
        ani_Door5.Play(5 == clickIndex || 5 == fireIndex ? "Open" : "Default");

        if (dicPos.TryGetValue(clickIndex, out var pos) && dicPos.TryGetValue(fireIndex, out var firePos))
        {
            ani_Boy.transform.localPosition = new Vector2(pos.x, BOY_HIDE_Y);

            int idx = bg.GetSiblingIndex();
            ani_Boy.transform.SetSiblingIndex(idx);
            ani_Boy.Play("Hide");

            ani_Fire.transform.localPosition = new Vector2(firePos.x, firePos.y);

        }
        ani_Goose.Play("Raw Egg");

        ani_Woman.Play("From Work");

        yield return new WaitForSeconds(2f);

        // 等待
        ani_Woman.Play("Idle");
        yield return new WaitForSeconds(1f);

        yield return _BoyStealEgg(new Vector2(pos.x, BOY_STEAL_Y));

    }


    void BoyFire(int doorNumb)
    {
        ClearCor();
        _cor = StartCoroutine(_BoyFire(doorNumb, doorNumb));
    }


    private IEnumerator _BoyFire(int clickIndex, int fireIndex)
    {
        scoreNum.text = "";

        ani_Door1.Play(1 == clickIndex || 1 == fireIndex ? "Open" : "Default");
        ani_Door2.Play(2 == clickIndex || 2 == fireIndex ? "Open" : "Default");
        ani_Door3.Play(3 == clickIndex || 3 == fireIndex ? "Open" : "Default");
        ani_Door4.Play(4 == clickIndex || 4 == fireIndex ? "Open" : "Default");
        ani_Door5.Play(5 == clickIndex || 5 == fireIndex ? "Open" : "Default");


        if (dicPos.TryGetValue(clickIndex, out var pos) && dicPos.TryGetValue(fireIndex, out var firePos))
        {
            ani_Boy.transform.localPosition = new Vector2(pos.x, BOY_STEAL_Y);

            int idx = doors.GetSiblingIndex();
            ani_Boy.transform.SetSiblingIndex(idx+1);

            ani_Boy.Play("Fire");

            ani_Fire.transform.localPosition = new Vector2(firePos.x, firePos.y);
        }
        ani_Goose.Play("Raw Egg");

        ani_Woman.Play("From Work");

        yield return new WaitForSeconds(1.5f);

        ani_Woman.Play("Amazed");

        yield return new WaitForSeconds(2f);

        MessageDispatcher.Dispatch("OnContentUIEvent", new EventData("BSTMiniGameFinish")); //发给脚本
        EventSender.SendGlobalEvent(new EventData("BSTMiniGameFinish")); //发给NodeCanvas （类型：OnCustomEvent）
    }



    void SetDoorCanOpenState(bool isDefault)
    {
        ani_Door1.Play(isDefault ? "Default" : "CanOpen");
        ani_Door2.Play(isDefault ? "Default" : "CanOpen");
        ani_Door3.Play(isDefault ? "Default" : "CanOpen");
        ani_Door4.Play(isDefault ? "Default" : "CanOpen");
        ani_Door5.Play(isDefault ? "Default" : "CanOpen");
    }







}
