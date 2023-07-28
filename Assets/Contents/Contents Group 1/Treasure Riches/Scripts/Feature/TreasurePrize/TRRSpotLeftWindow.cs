using System.Collections;
using System.Collections.Generic;
using BagelCode.Slots.TRR.Utillity;
using NodeCanvas.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode.Slots.TRR
{
    public class TRRSpotLeftWindow : MonoBehaviour
    {
        [SerializeField] private Animator mainAnimator;
        [SerializeField] private Button spotLeftButton;
        [SerializeField] private TextMeshProUGUI spotLeftNumberText;
        [SerializeField] private TextMeshProUGUI spotLeftText;

        [SerializeField] private List<Transform> player = new List<Transform>();

        public List<Blackboard> spotLeftData;
        public int leftCount = 24;
        public int spotLeftCount = 0;
        public Coroutine TreasurePrizeCollect() => StartCoroutine(_TreasurePrizeCollectSequence());

        public void SetLeftCount(int count)
        {
            leftCount = count;
            spotLeftNumberText.text = $"{leftCount}";
            spotLeftText.text = leftCount <= 1 ? "SPOT LEFT" : "SPOTS LEFT";
        }

        private IEnumerator _TreasurePrizeCollectSequence()
        {
            Blackboard bonusResult = TRRUtillity.TryGetGlobalBlackBoardVariable<Blackboard>("./bonus/response");

            //Get Bonus Variables From Bonus Result
            int targetColumn = TRRUtillity.TryGetLocalBlackBoardVariable<int>(bonusResult, "boardColumn");
            int targetRow = TRRUtillity.TryGetLocalBlackBoardVariable<int>(bonusResult, "boardRow");
            string userID = TRRUtillity.TryGetLocalBlackBoardVariable<string>(bonusResult, "userId");
            long collectAmount = TRRUtillity.TryGetLocalBlackBoardVariable<long>(bonusResult, "collectAmount");

            TRRCommunityBoardPlayer playerBoard = player[targetRow].transform.GetChild(targetColumn).GetComponent<TRRCommunityBoardPlayer>();
            playerBoard.InitializeForOwn();
            playerBoard.SetCollectAmountText(collectAmount);
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

            List<Blackboard> spotsData = spotLeftData;
            List<Blackboard> rowSpots = TRRUtillity.TryGetLocalBlackBoardVariable<List<Blackboard>>(spotsData[targetRow], "value");
            Blackboard colmnSpots = rowSpots[targetColumn];
            TRRUtillity.TrySetLocalBlackBoardVariable<bool>(colmnSpots, "collected", true);
            TRRUtillity.TrySetLocalBlackBoardVariable<int>(colmnSpots, "column", targetColumn);
            TRRUtillity.TrySetLocalBlackBoardVariable<int>(colmnSpots, "row", targetRow);
            TRRUtillity.TrySetLocalBlackBoardVariable<string>(colmnSpots, "userId", userID);
            TRRUtillity.TrySetLocalBlackBoardVariable<long>(colmnSpots, "credit", collectAmount);


            TRRUtillity.PlaySound("Spot Add");
            yield return new WaitForSeconds(44f / 60f);
            spotLeftNumberText.text = $"{--leftCount}";
            spotLeftText.text = leftCount <= 1 ? "SPOT LEFT" : "SPOTS LEFT";
            yield return new WaitForSeconds(2.5f);
        }


        public void UpdateSpotLeft(List<Blackboard> spotsDatas)
        {
            spotLeftData = spotsDatas;

            spotLeftCount = CalcSpotsLeftCount();

            spotLeftText.text = leftCount <= 1 ? "SPOT LEFT" : "SPOTS LEFT";
            spotLeftNumberText.text = $"{leftCount}";
        }
        private int CalcSpotsLeftCount()
        {
            leftCount = 24;
            List<Blackboard> spotsData = spotLeftData;
            for (int i = 0; i < 3; i++)
            {
                List<Blackboard> rowSpots = TRRUtillity.TryGetLocalBlackBoardVariable<List<Blackboard>>(spotsData[i], "value");
                for (int j = 0; j < 8; j++)
                {
                    Blackboard colmnSpots = rowSpots[j];
                    bool collected = TRRUtillity.TryGetLocalBlackBoardVariable<bool>(colmnSpots, "collected");
                    if (collected)
                        leftCount--;
                }
            }
            return leftCount;
        }


        public void ApplySpotsLeftData()
        {
            leftCount = 24;
            List<Blackboard> spotsData = spotLeftData;
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
                        leftCount--;
                    }
                    player[i].transform.GetChild(j).GetComponent<TRRCommunityBoardPlayer>().SetIndexNumber((i * 8) + j + 1);
                }

            }
            spotLeftText.text = leftCount <= 1 ? "SPOT LEFT" : "SPOTS LEFT";
            spotLeftNumberText.text = $"{leftCount}";
        }

        public void SetSpotLeftButtonActive(bool setTo)
        {
            spotLeftButton.enabled = setTo;
        }

        public void OpenWindow()
        {
            mainAnimator.SetBool("SpotLeft", true);
            mainAnimator.Update(Time.deltaTime);
            ApplySpotsLeftData();
        }

        public void CloseWindow()
        {
            mainAnimator.SetBool("SpotLeft", false);
        }

        public void ToggleButtonWindow()
        {
            if (TRRUtillity.TryGetGlobalBlackBoardVariable<bool>("./autoSpin") == false)
            {
                if (mainAnimator.GetBool("SpotLeft") == false)
                {
                    TRRContentsStorePollManager.Instance.UpdateContentStore(() =>
                     {
                         TRRContentsStorePollManager.Instance.enablePoll = false;
                         mainAnimator.SetBool("SpotLeft", !mainAnimator.GetBool("SpotLeft"));
                         mainAnimator.Update(Time.deltaTime);
                         ApplySpotsLeftData();
                     });
                }
                else
                {
                    mainAnimator.SetBool("SpotLeft", !mainAnimator.GetBool("SpotLeft"));
                    if (TRRContentsStorePollManager.Instance.communityGameTriggered == false)
                        TRRContentsStorePollManager.Instance.enablePoll = true;
                }
            }
        }
    }
}