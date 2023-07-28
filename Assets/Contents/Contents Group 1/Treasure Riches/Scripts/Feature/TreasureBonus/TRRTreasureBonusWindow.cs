using System.Collections;
using System.Collections.Generic;
using BagelCode.Slots.TRR.Utillity;
using NodeCanvas.Framework;
using TMPro;
using UnityEngine;
namespace BagelCode.Slots.TRR
{
    public class TRRTreasureBonusWindow : MonoBehaviour
    {
        [SerializeField] private List<Transform> player = new List<Transform>();
        [SerializeField] private List<Animator> roundMultiplierAnimatior = new List<Animator>();
        [SerializeField] private List<GameObject> chooseFrame = new List<GameObject>();
        [SerializeField] private TextMeshProUGUI totalWinText;

        [HideInInspector] public bool isFrameMoving;
        [HideInInspector] public List<GameObject> currentFrame = new List<GameObject>();
        [HideInInspector] public List<bool> visitedIndexList = new List<bool>();

        private void OnEnable()
        {
            InitalizeCollectSpots();
        }

        public void InitalizeCollectSpots()
        {
            totalWinText.text = "0";
            List<Blackboard> spotsData = TRRUtillity.TryGetGlobalBlackBoardVariable<List<Blackboard>>("./bonus/response/bonusGameData/collectData");

            for (int i = 0; i < 3; i++)
            {
                List<Blackboard> rowSpots = TRRUtillity.TryGetLocalBlackBoardVariable<List<Blackboard>>(spotsData[i], "value");
                for (int j = 0; j < 8; j++)
                {
                    Blackboard colmnSpots = rowSpots[j];
                    bool collected = TRRUtillity.TryGetLocalBlackBoardVariable<bool>(colmnSpots, "collected");
                    player[i].transform.GetChild(j).GetComponent<TRRCommunityBoardPlayer>().ResetAnimation();
                    if (collected)
                    {
                        string userID = TRRUtillity.TryGetLocalBlackBoardVariable<string>(colmnSpots, "userId");
                        long collectAmount = TRRUtillity.TryGetLocalBlackBoardVariable<long>(colmnSpots, "credit");

                        string playerUserID = TRRUtillity.TryGetGlobalBlackBoardVariable<string>("/me/userId");
                        TRRCommunityBoardPlayer playerBoard = player[i].transform.GetChild(j).GetComponent<TRRCommunityBoardPlayer>();
                        if (playerUserID != userID && userID != "TEST") playerBoard.InitializeForOtherPlayer();
                        else playerBoard.InitializeForOwnForce();


                        if (TRRProfileManager.Instance.IsLoading(userID))
                        {
                            playerBoard.WaitUntilLoad(userID);
                        }
                        else
                        {
                            Sprite userProfileImage = TRRProfileManager.Instance.GetUserProfileImage(userID);
                            playerBoard.SetPlayerImage(userProfileImage);
                            playerBoard.EnableUserProfile();
                        }

                        playerBoard.SetCollectAmountText(collectAmount);
                    }
                    player[i].transform.GetChild(j).GetComponent<TRRCommunityBoardPlayer>().SetIndexNumber((i * 8) + j + 1);
                }
            }
        }

        public void ActiveRoundMultiplier(int round) => roundMultiplierAnimatior[round - 1].SetBool("Active", true);

        public void InActiveRoundMultiplier(int round) => roundMultiplierAnimatior[round - 1].SetBool("Active", false);

        public void SetFrameMultiplier(GameObject frameObject, int round)
        {
            int multiplier = 0;
            if (round == 1) multiplier = 3;
            if (round == 2) multiplier = 4;
            if (round == 3) multiplier = 5;
            if (round == 4) multiplier = 10;
            if (round == 5) multiplier = 15;
            if (round == 6) multiplier = 20;
            if (round == 7) multiplier = 50;
            frameObject.GetComponent<TRRTreasureBonusChooseFrame>().SetMultiplierText($"X{multiplier}");
        }

        public void MovementFrame(int pointCount, int[] targetPoint, int round, int moveCount)
        {
            isFrameMoving = true;
            currentFrame.Clear();
            for (int i = 0; i < pointCount; i++)
            {
                int direction = UnityEngine.Random.Range(0, 100) >= 50 ? 1 : -1;
                StartCoroutine(FrameMove(targetPoint[i], direction, round, moveCount));
            }
        }
        public Coroutine TotalWinCreditDirectionCoroutine(float duraction, long startCredit, long endCredit) => StartCoroutine(_TotalWinCreditDirectionCoroutine(duraction, startCredit, endCredit));
        public IEnumerator _TotalWinCreditDirectionCoroutine(float duraction, long startCredit, long endCredit)
        {
            float startTime = Time.time;
            float endTime = Time.time + duraction;
            float currentTime = startTime;
            long currentCredit;
            while (currentTime < endTime)
            {
                currentTime = Time.time;
                currentCredit = startCredit + (long)((endCredit - startCredit) / (duraction / (currentTime - startTime)));
                totalWinText.text = currentCredit.ToString("#,##0");
                yield return null;
            }
            currentCredit = endCredit;
            totalWinText.text = currentCredit.ToString("#,##0");
        }

        IEnumerator FrameMove(int targetPoint, int direction, int round, int moveCount)
        {
            float moveDelay = 0.45f;
            List<int> movePoint = new List<int>();
            int index = targetPoint;
            for (int i = 0; i < moveCount; i++)
            {
                movePoint.Add(index);
                index += direction;
                if (index >= chooseFrame.Count) { index = 0; }
                else if (index <= -1) index = chooseFrame.Count - 1;

                while (visitedIndexList[index] == true)
                {
                    index += direction;
                    if (index >= chooseFrame.Count) { index = 0; }
                    else if (index <= -1) index = chooseFrame.Count - 1;
                }
            }
            movePoint.Reverse();

            index = movePoint[0];
            int lastIndex = index;

            for (int i = 0; i < movePoint.Count; i++)
            {
                lastIndex = index;
                index = movePoint[i];

                chooseFrame[lastIndex].gameObject.SetActive(false);
                yield return null;
                chooseFrame[index].gameObject.SetActive(true);
                SetFrameMultiplier(chooseFrame[index].gameObject, round);
                //     TRRUtillity.PlaySound("Multiplier Move");
                TRRUtillity.SendEvent("OnSoundEvent", "MultiplierMove");
                yield return new WaitForSeconds(moveDelay);
                moveDelay *= 1.06f;
            }

            currentFrame.Add(chooseFrame[index]);

            visitedIndexList[index] = true;
            if (index == 0) visitedIndexList[13] = true;
            else if (index == 1) visitedIndexList[12] = true;
            else if (index == 2) visitedIndexList[11] = true;

            else if (index == 11) visitedIndexList[2] = true;
            else if (index == 12) visitedIndexList[1] = true;
            else if (index == 13) visitedIndexList[0] = true;

            isFrameMoving = false;
        }
    }
}