using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;
namespace GameStudio.Slot.IIP.Feature
{
    public class IIPCommunityGameAdmin : FeatureModule
    {
        [Header("Animator")]
        public Animator mainAnimator;

        [Space(20)]
        [Header("Monobehaviour")]
        public IIPCommunityGameUIAdmin uiAdmin;
        public IIPMovePenguinsAdmin movePenguinAdmin;
        public IIPCollectIceAdmin iceCollectAdmin;
        public IIPCommunityGameTileAdmin tileAdmin;

        [Space(20)]
        [Header("Value")]
        public List<int> levelPenguinCondition = new List<int>();

        public int totalSpinCount { get; private set; }
        public int spunCount { get; private set; }
        public int treasureLeftCount { get; private set; }

        public int currentLevel { get; private set; }
        public int currentPhase { get; private set; }

        public long bonusBet { get; private set; }
        public long earnCredit { get; private set; }

        private void Awake()
        {
            RegisterEvent("IIPInitializeCommunityGame", OnInitializeCommunityGame);
            RegisterEvent("IIPBeforeCommunityGameSpin", BeforeSpin);
            RegisterEvent("IIPAppearHiddenPenguins", OnAppearHiddenPenguins);
            RegisterEvent("IIPOnMoreSpinArrive", OnMoreSpinArrive);
        }

        public void OnAppearHiddenPenguins(EventData evnetData)
        {
            StartCoroutine(_OnAppearHiddenPenguinsCoroutine());
        }
        private IEnumerator _OnAppearHiddenPenguinsCoroutine()
        {
            yield return tileAdmin.AppearHiddenPenguinsCoroutine();
            yield return new WaitForSeconds(1f);
            ContentEvent.SendEvent("IIPEndAppearHiddenPenguins");
        }

        public void OnInitializeCommunityGame(EventData evnetData)
        {
            currentLevel = 0;
            totalSpinCount = 8;
            spunCount = 0;
            earnCredit = 0;

            List<Blackboard> userGameList = BlackboardUtils.FindVariable<List<Blackboard>>("./bonus/response/userGameResultList").value;
            bonusBet = userGameList[0].GetVariable<long>("betMultiplier").value;
            treasureLeftCount = 3;
            SetEarnCredit(bonusBet);
            uiAdmin.UpdateSpinLeftCount(totalSpinCount);
            UpdatePhase(1);
        }

        public void OnPlayerCollectTreasure()
        {
            uiAdmin.OnCollectTreasure(true);
        }
        public void OnOtherPlayerCollectTreasure()
        {
            uiAdmin.OnCollectTreasure(false);
        }

        public void UpdatePhase(int phase)
        {
            currentPhase = phase;
            mainAnimator.SetInteger("Phase", phase);
            if (phase == 2)
                tileAdmin.SetSecondePhase();
        }

        public void SetEarnCredit(long credit)
        {
            earnCredit = credit;
            uiAdmin.SetTotalCredit(earnCredit, true);
        }
        public void AddEarnCredit(long credit)
        {
            earnCredit += credit;
            uiAdmin.SetTotalCredit(earnCredit, false);
        }

        public void BeforeSpin(EventData eventData)
        {
            spunCount++;
            uiAdmin.UpdateSpinLeftCount(totalSpinCount - spunCount);
        }

        private void OnMoreSpinArrive(EventData eventData)
        {
            if (currentLevel == 1 || currentLevel == 2) totalSpinCount += 1;
            else totalSpinCount += 2;
            uiAdmin.UpdateSpinLeftCount(totalSpinCount - spunCount);
        }

        public Coroutine UpdateLevelCoroutine() => StartCoroutine(_UpdateLevelCoroutine());
        private IEnumerator _UpdateLevelCoroutine()
        {
            if (currentLevel < uiAdmin.penguinGaugeAdmin.goalPointCondition.Count - 1)
            {
                if (movePenguinAdmin.savedPenguinCount >= levelPenguinCondition[currentLevel])
                {
                    currentLevel++;
                    yield return new WaitForSeconds(1.75f);

                    uiAdmin.moreSpinPopup.gameObject.SetActive(true);
                    yield return uiAdmin.moreSpinPopup.OpenPopupCoroutine(currentLevel <= 2 ? 1 : 2);
                    uiAdmin.moreSpinPopup.gameObject.SetActive(false);
                    yield return new WaitForSeconds(1.5f);

                    // Check if Level was reached to 2. Then, Update phase to 2.
                    if (currentLevel == 2)
                    {
                        uiAdmin.secondPhasePopup.gameObject.SetActive(true);
                        yield return uiAdmin.secondPhasePopup.OpenPopupCoroutine();
                        uiAdmin.secondPhasePopup.gameObject.SetActive(false);
                        yield return new WaitForSeconds(1f);
                        UpdatePhase(2);
                        yield return new WaitForSeconds(1.5f);
                        yield return StartCoroutine(_OnAppearHiddenPenguinsCoroutine());
                        yield return new WaitForSeconds(1.5f);
                    }
                }
            }
        }

    }
}