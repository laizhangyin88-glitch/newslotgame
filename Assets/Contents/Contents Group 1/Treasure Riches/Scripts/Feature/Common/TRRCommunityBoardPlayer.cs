using System.Collections;
using System.Collections.Generic;
using BagelCode.Slots.TRR.Utillity;
using SlotMaker;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode.Slots.TRR
{
    public class TRRCommunityBoardPlayer : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        [SerializeField] private List<Animator> loadingAnimator = new List<Animator>();
        [SerializeField] private List<Image> playerDefaultImage = new List<Image>();
        [SerializeField] private List<Image> playerImage = new List<Image>();
        [SerializeField] private TextMeshProUGUI indexNumber;
        [SerializeField] private TextMeshProUGUI goldNumber;
        [SerializeField] private TextMeshProUGUI silverNumber;

        public void SetPlayerImage(Sprite setTo)
        {
            foreach (var each in playerImage) each.sprite = setTo;

            if (setTo == null)
            {
                foreach (var each in playerDefaultImage) each.gameObject.SetActive(true);
                foreach (var each in playerImage) each.gameObject.SetActive(false);
            }
        }
        public void ResetAnimation() => animator.Rebind();

        public void SetIndexNumber(int index)
        {
            indexNumber.text = $"{index}";
        }

        public void SetCollectAmountText(long amount)
        {
            silverNumber.text = string.Format(StringTableUtils.customProvider, "{0:SimpleNumber}", amount);
            goldNumber.text = string.Format(StringTableUtils.customProvider, "{0:SimpleNumber}", amount);
        }

        public void InitializeForOwn()
        {
            animator.SetBool("Gold", true);
            animator.SetBool("Silver", false);
            animator.Update(Time.deltaTime);
        }

        public void InitializeForOwnForce()
        {
            animator.SetBool("Gold", true);
            animator.SetBool("Silver", false);
            animator.SetBool("Force", true);
            animator.Update(Time.deltaTime);
        }

        public void InitializeForOtherPlayer()
        {
            animator.SetBool("Silver", true);
            animator.SetBool("Gold", false);
            animator.Update(Time.deltaTime);
        }
        public Coroutine WaitUntilLoad(string userId) => StartCoroutine(_WaitUntilLoad(userId));

        public void EnableUserProfile()
        {
            foreach (var each in playerDefaultImage) each.gameObject.SetActive(false);
            foreach (var each in playerImage) each.gameObject.SetActive(true);
            foreach (var each in loadingAnimator) if (each.gameObject.activeSelf == true && each.isActiveAndEnabled) each.SetBool("Active", false);
        }

        public IEnumerator _WaitUntilLoad(string userId)
        {
            yield return null;
            foreach (var each in loadingAnimator) each.SetBool("Active", true);

            foreach (var each in playerDefaultImage) each.gameObject.SetActive(true);
            foreach (var each in playerImage) each.gameObject.SetActive(false);

            yield return new WaitUntil(() => TRRProfileManager.Instance.IsLoading(userId) == false);


            foreach (var each in playerDefaultImage) each.gameObject.SetActive(false);
            foreach (var each in playerImage) each.gameObject.SetActive(true);
            SetPlayerImage(TRRProfileManager.Instance.GetUserProfileImage(userId));
            foreach (var each in loadingAnimator) each.SetBool("Active", false);
        }

        public void TriggerWin()
        {
            animator.SetBool("Win", true);
        }
        public void InActive()
        {
            animator.SetTrigger("InActive");
        }
    }
}