using System.Collections;
using System.Collections.Generic;
using GameStudio.Slot.IIP.Popup;
using GameStudio.Slot.IIP.Utility;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameStudio.Slot.IIP.Feature
{
    public class IIPCommunityGameUIAdmin : FeatureModule
    {
        [Header("Animator")]
        public Animator mainAnimator;
        public Animator treasureAnimator;
        public Animator spinLeftUIAnimator;
        public List<Animator> treasureAnimatorList = new List<Animator>();

        [Space(20)]
        [Header("Popup")]
        public GameObject introPopup;
        public IIPCommunityGameMoreSpinPopup moreSpinPopup;
        public IIPCommunityGameCongratulationsPopup congratulationPopup;
        public IIPCommunityGameSecondPhasePopup secondPhasePopup;

        [Space(20)]
        [Header("Text")]
        public TextMeshProUGUI spinLeftCountText;
        public TextMeshProUGUI spinLeftText;
        public TextMeshProUGUI resultPopupCreditText;
        public TextMeshProUGUI resultPopupMultiplierText;
        public TextMeshProUGUI totalCreditText;

        [Space(20)]
        [Header("Image")]
        public List<Image> playerProfileImageList = new List<Image>();
        public Sprite defaultUserImage;

        [Space(20)]
        [Header("Monobehaviour")]
        public IIPCommunityGameGauge penguinGaugeAdmin;

        [Space(20)]
        [Header("Value")]
        public float totalCreditNumberIncreaseDuration;

        private long targetTotalCreditNumber;
        private long startTotalCreditNumber;
        private long currentTotalCreditNumber;

        private float increaseTime;
        private bool isIncreasing;

        private int appearedTreasureIndex;
        private int openedTreasureIndex;

        public void UpdateSpinLeftCount(int count)
        {
            spinLeftCountText.text = $"{count}";
            if (count == 1) spinLeftText.text = $"SPIN LEFT";
            else spinLeftText.text = $"SPINS LEFT";
        }

        private void Awake()
        {
            RegisterEvent("IIPInitializeCommunityGame", OnInitializeCommunityGame);
            RegisterEvent("IIPEndIntroPopup", OnEndIntroPopup);
            RegisterEvent("IIPOpenIntroPopup", OnOpenIntroPopup);

            RegisterEvent("IIPCongratulationPopup", OnCongratulationPopupPopup);
            RegisterEvent("IIPEndCongratulationPopup", OnEndCongratulationPopupPopup);
            RegisterEvent("IIPAppearTreasureUI", AppearTreasureLeftCount);
            RegisterEvent("IIPOpenTreasure", OpenTreasureLeft);
            RegisterEvent("IIPCaculateFinalMultiplier", CalculateFinalMutliplier);
            RegisterEvent("IIPOnMoreSpinArrive", OnMoreSpinArrive);
        }

        private void Update()
        {
            if (isIncreasing)
            {
                if (Time.time - increaseTime < totalCreditNumberIncreaseDuration)
                {
                    currentTotalCreditNumber = (long)Mathf.Lerp(startTotalCreditNumber, targetTotalCreditNumber, (Time.time - increaseTime) / totalCreditNumberIncreaseDuration);
                    totalCreditText.text = FormatUtility.CommaNumberFormat(currentTotalCreditNumber);
                }
                else
                {
                    IIPUtility.StopSound("Credit Count");
                    IIPUtility.PlaySound("Credit Count End");
                    SetTotalCredit(targetTotalCreditNumber, true);
                }
            }
        }
        private void OnMoreSpinArrive(EventData eventData)
        {
            spinLeftUIAnimator.SetTrigger("Add");
        }
        private void OnCongratulationPopupPopup(EventData eventData)
        {
            congratulationPopup.gameObject.SetActive(true);
        }

        private void OnEndCongratulationPopupPopup(EventData eventData)
        {
            congratulationPopup.gameObject.SetActive(false);
        }
        private void OnEndIntroPopup(EventData eventData)
        {
            introPopup.SetActive(false);
        }
        private void OnOpenIntroPopup(EventData eventData)
        {
            introPopup.SetActive(true);
        }

        public void OnCollectTreasure(bool isMine)
        {
            OpenTreasureLeft(null);
            if (!isMine)
                treasureAnimatorList[openedTreasureIndex - 1].SetTrigger("Gray");
        }

        private void OnInitializeCommunityGame(EventData eventData)
        {
            penguinGaugeAdmin.Initialize();
            appearedTreasureIndex = 0;
            openedTreasureIndex = 0;

            List<Blackboard> userGameList = BlackboardUtils.FindVariable<List<Blackboard>>("./bonus/response/userGameResultList").value;
            for (int i = 0; i < 5; i++)
            {
                Blackboard userGameBB = userGameList[i];

                bool isBot = userGameBB.GetVariable<bool>("isBot").value;

                if (isBot == false)
                {
                    string profileUrl = userGameBB.GetVariable<string>("profileUrl").value;
                    LoadPlayerProfileImage(profileUrl, playerProfileImageList[i]);
                }
                else playerProfileImageList[i].sprite = defaultUserImage;
            }
        }

        private void LoadPlayerProfileImage(string profileUrl, Image targetImage)
        {
            WebImageDownloader.Instance.LoadWebImage(
         profileUrl,
         CacheType.MemCache,
         false,
         null,
         delegate (Sprite img)
         {
             targetImage.sprite = img;
         });
        }

        private void CalculateFinalMutliplier(EventData eventData)
        {
            StartCoroutine(CalculateFinalMutliplierCoroutine());
        }
        private IEnumerator CalculateFinalMutliplierCoroutine()
        {
            if (penguinGaugeAdmin.currentLevelIndex > 0)
                penguinGaugeAdmin.DisappearCurrentMultiplier();
            yield return new WaitForSeconds(0.5f);
            mainAnimator.SetBool("Result", true);
            resultPopupCreditText.text = currentTotalCreditNumber.ToString("#,##0");
            if (penguinGaugeAdmin.currentLevelIndex > 0)
            {
                mainAnimator.SetBool("Multiplier", true);
                if (penguinGaugeAdmin.GetCuurrentMultiplier() == 100)
                    mainAnimator.SetBool("FullGauge", true);
                else
                    resultPopupMultiplierText.text = $"X{penguinGaugeAdmin.GetCuurrentMultiplier()}";
                bool startCreditDirection = false;
                void OnStartCreditDirection(EventData eventData)
                {
                    if (eventData.name == "OnStartCreditDirection")
                        startCreditDirection = true;
                }
                ContentEvent.Register(OnStartCreditDirection);
                yield return new WaitUntil(() => startCreditDirection == true);
                yield return StartCoroutine(ApplyResultPopupDirection(currentTotalCreditNumber, currentTotalCreditNumber * penguinGaugeAdmin.GetCuurrentMultiplier()));
                ContentEvent.UnRegister(OnStartCreditDirection);
            }

            bool collectButtonDown = false;
            void OnCollectButton(EventData eventData)
            {
                if (eventData.name == "OnCollectButtonDown")
                    collectButtonDown = true;
            }
            ContentEvent.Register(OnCollectButton);
            yield return new WaitUntil(() => collectButtonDown == true);
            ContentEvent.UnRegister(OnCollectButton);
            mainAnimator.SetBool("Result", false);
            mainAnimator.SetBool("Multiplier", false);
            mainAnimator.SetBool("FullGauge", false);
            yield return new WaitForSeconds(2f);
            ContentEvent.SendEvent("IIPFinshCaculateFinalMultiplier");
        }

        private IEnumerator ApplyResultPopupDirection(long start, long end)
        {
            float duration = 1f;
            float startTime = Time.time;
            float endTime = Time.time + duration;
            float currentTime = startTime;
            long currentCredit = start;
            while (currentTime < endTime)
            {
                currentTime = Time.time;
                currentCredit = start + (long)((end - start) / (duration / (currentTime - startTime)));
                resultPopupCreditText.text = currentCredit.ToString("#,##0");
                yield return null;
            }
            currentCredit = end;
            resultPopupCreditText.text = currentCredit.ToString("#,##0");
        }

        private void AppearTreasureLeftCount(EventData eventData)
        {
            treasureAnimatorList[appearedTreasureIndex++].SetTrigger("Appear");
        }

        private void OpenTreasureLeft(EventData eventData)
        {
            treasureAnimatorList[openedTreasureIndex++].SetTrigger("Open");
        }

        public void SetTotalCredit(long totalCredit, bool immediately = false)
        {
            if (immediately == true)
            {
                targetTotalCreditNumber = totalCredit;
                currentTotalCreditNumber = totalCredit;
                totalCreditText.text = FormatUtility.CommaNumberFormat(totalCredit);
                isIncreasing = false;
            }
            else
            {
                IIPUtility.PlaySound("Credit Count");
                increaseTime = Time.time;
                startTotalCreditNumber = currentTotalCreditNumber;
                targetTotalCreditNumber = totalCredit;
                isIncreasing = true;
            }
        }
    }
}