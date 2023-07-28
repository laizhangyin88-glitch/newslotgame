using System.Collections;
using System.Collections.Generic;
using GameStudio.Slot.TRL.Utillity;
using NodeCanvas.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameStudio.Slot.TRL
{
    public class TRLSpotLeftWindow : MonoBehaviour
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
            Blackboard bonusResult = TRLUtillity.TryGetGlobalBlackBoardVariable<Blackboard>("./bonus/response");

            //Get Bonus Variables From Bonus Result
            int targetColumn = TRLUtillity.TryGetLocalBlackBoardVariable<int>(bonusResult, "boardColumn");
            int targetRow = TRLUtillity.TryGetLocalBlackBoardVariable<int>(bonusResult, "boardRow");
            string userID = TRLUtillity.TryGetLocalBlackBoardVariable<string>(bonusResult, "userId");
            long collectAmount = TRLUtillity.TryGetLocalBlackBoardVariable<long>(bonusResult, "collectAmount");

            TRLCommunityBoardPlayer playerBoard = player[targetRow].transform.GetChild(targetColumn).GetComponent<TRLCommunityBoardPlayer>();
            playerBoard.InitializeForOwn();
            playerBoard.SetCollectAmountText(collectAmount);
            if (TRLUserProfileManager.Instance.IsLoading(userID))
            {
                playerBoard.WaitUntilLoad(userID);
            }
            else
            {
                Sprite userProfileImage = TRLUserProfileManager.Instance.GetUserProfileImage(userID);
                playerBoard.SetPlayerImage(userProfileImage);
                playerBoard.EnableUserProfile();
            }

            List<Blackboard> spotsData = spotLeftData;
            List<Blackboard> rowSpots = TRLUtillity.TryGetLocalBlackBoardVariable<List<Blackboard>>(spotsData[targetRow], "value");
            Blackboard colmnSpots = rowSpots[targetColumn];
            TRLUtillity.TrySetLocalBlackBoardVariable<bool>(colmnSpots, "collected", true);
            TRLUtillity.TrySetLocalBlackBoardVariable<int>(colmnSpots, "column", targetColumn);
            TRLUtillity.TrySetLocalBlackBoardVariable<int>(colmnSpots, "row", targetRow);
            TRLUtillity.TrySetLocalBlackBoardVariable<string>(colmnSpots, "userId", userID);
            TRLUtillity.TrySetLocalBlackBoardVariable<long>(colmnSpots, "credit", collectAmount);


            TRLUtillity.PlaySound("Spot Add");
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
                List<Blackboard> rowSpots = TRLUtillity.TryGetLocalBlackBoardVariable<List<Blackboard>>(spotsData[i], "value");
                for (int j = 0; j < 8; j++)
                {
                    Blackboard colmnSpots = rowSpots[j];
                    bool collected = TRLUtillity.TryGetLocalBlackBoardVariable<bool>(colmnSpots, "collected");
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
                List<Blackboard> rowSpots = TRLUtillity.TryGetLocalBlackBoardVariable<List<Blackboard>>(spotsData[i], "value");
                for (int j = 0; j < 8; j++)
                {
                    Blackboard colmnSpots = rowSpots[j];
                    bool collected = TRLUtillity.TryGetLocalBlackBoardVariable<bool>(colmnSpots, "collected");
                    player[i].transform.GetChild(j).GetComponent<TRLCommunityBoardPlayer>().ResetAnimation();
                    if (collected)
                    {
                        string userID = TRLUtillity.TryGetLocalBlackBoardVariable<string>(colmnSpots, "userId");
                        long collectAmount = TRLUtillity.TryGetLocalBlackBoardVariable<long>(colmnSpots, "credit");

                        string playerUserID = TRLUtillity.TryGetGlobalBlackBoardVariable<string>("/me/userId");

                        TRLCommunityBoardPlayer playerBoard = player[i].transform.GetChild(j).GetComponent<TRLCommunityBoardPlayer>();
                        if (playerUserID != userID && userID != "TEST") playerBoard.InitializeForOtherPlayer();
                        else playerBoard.InitializeForOwnForce();

                        if (TRLUserProfileManager.Instance.IsLoading(userID))
                        {
                            playerBoard.WaitUntilLoad(userID);
                        }
                        else
                        {
                            Sprite userProfileImage = TRLUserProfileManager.Instance.GetUserProfileImage(userID);
                            playerBoard.SetPlayerImage(userProfileImage);
                            playerBoard.EnableUserProfile();
                        }

                        playerBoard.SetCollectAmountText(collectAmount);
                        leftCount--;
                    }
                    player[i].transform.GetChild(j).GetComponent<TRLCommunityBoardPlayer>().SetIndexNumber((i * 8) + j + 1);
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
            if (TRLUtillity.TryGetGlobalBlackBoardVariable<bool>("./autoSpin") == false)
            {
                if (mainAnimator.GetBool("SpotLeft") == false)
                {
                    TRLContentsStorePollManager.Instance.UpdateContentStore(() =>
                     {
                         TRLContentsStorePollManager.Instance.enablePoll = false;
                         mainAnimator.SetBool("SpotLeft", !mainAnimator.GetBool("SpotLeft"));
                         mainAnimator.Update(Time.deltaTime);
                         ApplySpotsLeftData();
                     });
                }
                else
                {
                    mainAnimator.SetBool("SpotLeft", !mainAnimator.GetBool("SpotLeft"));
                    if (TRLContentsStorePollManager.Instance.communityGameTriggered == false)
                        TRLContentsStorePollManager.Instance.enablePoll = true;
                }
            }
        }
    }
}