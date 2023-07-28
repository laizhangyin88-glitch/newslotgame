using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameStudio.Slot;
using GameStudio.Slot.MRS.Utility;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;

public class MRSWildRespinController : FeatureModule
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float symbolSpace;
    [SerializeField] private int bounceCountBaseSpin;
    [SerializeField] private int bounceCountReSpin;

    private SlotMachine slotMachine;
    private Reel reel;

    private void Awake()
    {
        RegisterEvent("InitalizeWildRespin", OnInitalize);
        RegisterEvent("WildRespin", OnWildRespin);
    }

    private void OnInitalize(EventData eventData)
    {
        slotMachine = ContentCustomData.GetSlotData(0).slotMachine.GetComponent<SlotMachine>();
        reel = slotMachine.GetReel(1) as Reel;
        reel.GetComponent<DefaultReelMovement>().onPrepareStopped.AddListener(OnPrepareStoppedReel);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        reel?.GetComponent<DefaultReelMovement>().onPrepareStopped.RemoveListener(OnPrepareStoppedReel);
    }

    private void OnWildRespin(EventData eventData)
    {
        var spin = BlackboardUtils.FindVariable<Blackboard>(null, "./spin");
        List<int> startReelOutputList = MRSUtility.TryGetGlobalBlackBoardVariable<List<int>>("./customData/lastReelOutputList");
        List<int> targetReelOutputList = MRSUtility.TryGetGlobalBlackBoardVariable<List<int>>("./customData/currentReelOutputList");
        List<int> downReelOutputList = MRSUtility.TryGetGlobalBlackBoardVariable<List<int>>("./customData/downReelOutputList");
        StartCoroutine(WildRespinReSpinCoroutine(bounceCountReSpin, startReelOutputList, targetReelOutputList, downReelOutputList));
    }

    private void OnPrepareStoppedReel(BaseReel reel)
    {
        if (MRSUtility.TryGetGlobalBlackBoardVariable<bool>("./customData/wildRespin") == true)
        {
            MRSUtility.SendEvent("OnContentUIDetailEvent", "SLOTFRAME_FIRE_ENABLE");
            var spin = BlackboardUtils.FindVariable<Blackboard>(null, "./spin");
            List<int> startReelOutputList = MRSUtility.TryGetLocalBlackBoardVariable<List<int>>(ContentBlackboardUtils.GetBonusResponse(spin.value, 21001), "reelOutputListForClient");
            List<int> targetReelOutputList = MRSUtility.TryGetGlobalBlackBoardVariable<List<int>>("./spin/response/result/reelOutputList");
            StartCoroutine(WildRespinBaseSpinCoroutine(bounceCountBaseSpin, startReelOutputList, targetReelOutputList, startReelOutputList));
        }
    }

    private IEnumerator WildRespinReSpinCoroutine(int bounceCount, List<int> startReelOutputList, List<int> targetReelOutputList, List<int> downReelOutputList)
    {
        MRSUtility.TrySetGlobalBlackBoardVariable<bool>("./customData/wildRespin", true);
        MRSUtility.PlaySound("Re-Spin Trigger");

        reel.GetComponent<DefaultReelMovement>().velocity = Vector3.zero;
        reel.GetComponent<DefaultReelMovement>().acceleration = Vector3.zero;
        reel.GetComponent<DefaultReelMovement>().displacement = Vector3.zero;

        List<int> topReelOutputList = downReelOutputList.ToList();
        topReelOutputList[1] -= 4;

        MRSUtility.PlaySound("Reel Up and Down");
        MRSUtility.ChangeSnapShot("Content_Expectation");

        //방향 랜덤
        float moveDirection = Random.Range(0, 100) > 50 ? 1f : -1f;

        int moveIndex = 0;
        if (moveDirection == 1f) { moveIndex = downReelOutputList[1] - startReelOutputList[1]; }
        else if (moveDirection == -1f) { moveIndex = startReelOutputList[1] - topReelOutputList[1]; }

        float moveDistance = symbolSpace * moveIndex;
        float moveDuraction = moveDistance / moveSpeed;
        yield return StartCoroutine(MoveTowardCoroutine(moveDuraction, moveDistance, moveDirection));

        //방향 전환
        for (int i = 0; i < bounceCount; i++)
        {
            moveDirection = -moveDirection;
            moveDistance = symbolSpace * 4f;
            moveDuraction = moveDistance / moveSpeed;
            yield return StartCoroutine(MoveTowardCoroutine(moveDuraction, moveDistance, moveDirection));
        }

        moveDirection = -moveDirection;
        moveIndex = 0;
        if (moveDirection == 1f) // 아래로
            moveIndex = targetReelOutputList[1] - (downReelOutputList[1] - 4);
        else if (moveDirection == -1f) // 위로
            moveIndex = downReelOutputList[1] - targetReelOutputList[1];

        if (targetReelOutputList[1] - startReelOutputList[1] > 0)
            RollBottomUpReelSymbols(Mathf.Abs(targetReelOutputList[1] - startReelOutputList[1]));
        else
            RollTopDownReelSymbols(Mathf.Abs(targetReelOutputList[1] - startReelOutputList[1]));

        if (moveIndex == 0)
        {
            yield return StartCoroutine(MoveTowardCoroutine(moveDuraction, moveDistance, moveDirection));
            MRSUtility.TrySetGlobalBlackBoardVariable<bool>("./customData/wildRespin", false);
            (reel.movement as DefaultReelMovement).OnPrepareStopped();
            yield return null;
            MRSUtility.StopSound("Reel Up and Down");
            yield break;
        }

        moveDistance = symbolSpace * moveIndex;
        moveDuraction = moveDistance / moveSpeed;
        yield return StartCoroutine(MoveTowardCoroutine(moveDuraction, moveDistance, moveDirection));
        MRSUtility.TrySetGlobalBlackBoardVariable<bool>("./customData/wildRespin", false);
        (reel.movement as DefaultReelMovement).OnPrepareStopped();
        yield return null;
        MRSUtility.ChangeSnapShot("Content_Main");
        MRSUtility.StopSound("Reel Up and Down");
    }

    private IEnumerator WildRespinBaseSpinCoroutine(int bounceCount, List<int> startReelOutputList, List<int> targetReelOutputList, List<int> downReelOutputList)
    {
        MRSUtility.TrySetGlobalBlackBoardVariable<bool>("./customData/wildRespin", true);
        MRSUtility.PlaySound("Re-Spin Trigger");
        yield return null;
        MRSUtility.SendEvent("OnContentUIDetailEvent", "SLOTFRAME_WILD_RESPIN");
        reel.GetComponent<DefaultReelMovement>().velocity = Vector3.zero;
        reel.GetComponent<DefaultReelMovement>().acceleration = Vector3.zero;
        reel.GetComponent<DefaultReelMovement>().displacement = Vector3.zero;

        float offset = 0;
        MRSUtility.PlaySound("Reel Up and Down");
        MRSUtility.ChangeSnapShot("Content_Expectation");

        BaseSymbol blankSymbol = null;
        for (int i = 0; i < 3; i++)
        {
            blankSymbol = slotMachine.GetSymbol(1, i).symbolIndex == 9 ? slotMachine.GetSymbol(1, i) : null;
            if (blankSymbol != null)
            {
                offset = (blankSymbol.transform as RectTransform).anchoredPosition3D.y;
                break;
            }
        }
        float moveDirection = -1f;
        float moveDistance = symbolSpace * 4f + offset;
        float moveDuraction = moveDistance / moveSpeed;
        yield return StartCoroutine(MoveTowardCoroutine(moveDuraction, moveDistance, moveDirection));

        //방향 전환
        for (int i = 0; i < bounceCount; i++)
        {
            moveDirection = -moveDirection;
            moveDistance = symbolSpace * 4f;
            moveDuraction = moveDistance / moveSpeed;
            yield return StartCoroutine(MoveTowardCoroutine(moveDuraction, moveDistance, moveDirection));
        }

        moveDirection = -moveDirection;
        int moveIndex = 0;
        if (moveDirection == 1f)
        { // 아래로
            moveIndex = targetReelOutputList[1] - (downReelOutputList[1] - 4);
        }
        else if (moveDirection == -1f) // 위로
        {
            moveIndex = downReelOutputList[1] - targetReelOutputList[1];
        }
        RollTopDownReelSymbols(downReelOutputList[1] - targetReelOutputList[1]);
        if (moveIndex == 0)
        {
            yield return StartCoroutine(MoveTowardCoroutine(moveDuraction, moveDistance, moveDirection));
            MRSUtility.TrySetGlobalBlackBoardVariable<bool>("./customData/wildRespin", false);
            (reel.movement as DefaultReelMovement).OnPrepareStopped();
            yield return null;
            MRSUtility.StopSound("Reel Up and Down");
            yield break;
        }
        moveDistance = symbolSpace * moveIndex;
        moveDuraction = moveDistance / moveSpeed;
        yield return StartCoroutine(MoveTowardCoroutine(moveDuraction, moveDistance, moveDirection));
        MRSUtility.TrySetGlobalBlackBoardVariable<bool>("./customData/wildRespin", false);
        (reel.movement as DefaultReelMovement).OnPrepareStopped();
        yield return null;
        MRSUtility.ChangeSnapShot("Content_Main");
        MRSUtility.StopSound("Reel Up and Down");
    }

    private void RollTopDownReelSymbols(int offset)
    {
        reel.PopBackSymbols(offset);
        reel.PushFrontSymbols(offset);
    }

    private void RollBottomUpReelSymbols(int offset)
    {
        reel.PopFrontSymbols(offset);
        reel.PushBackSymbols(offset);
    }

    private IEnumerator MoveTowardCoroutine(float duraction, float distance, float direction)
    {
        List<Vector3> startLocalPositionList = reel.symbols.Select(symbol => (symbol.transform as RectTransform).anchoredPosition3D).ToList();
        List<Vector3> targetLocalPositionList = startLocalPositionList.Select(target => target + new Vector3(0, distance * direction, 0)).ToList();

        float currentTime = Time.time;
        float startTime = currentTime;
        float targetTime = currentTime + duraction;

        while (currentTime < targetTime)
        {
            for (int i = 0; i < reel.symbols.Count; i++)
                (reel.symbols[i].transform as RectTransform).anchoredPosition3D = Vector3.Lerp(startLocalPositionList[i], targetLocalPositionList[i], (currentTime - startTime) / duraction);
            currentTime = Time.time;
            yield return null;
        }

        for (int i = 0; i < reel.symbols.Count; i++)
            (reel.symbols[i].transform as RectTransform).anchoredPosition3D = targetLocalPositionList[i];
    }
}
