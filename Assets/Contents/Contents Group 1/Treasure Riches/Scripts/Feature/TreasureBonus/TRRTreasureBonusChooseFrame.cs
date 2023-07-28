using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace BagelCode.Slots.TRR
{
    public class TRRTreasureBonusChooseFrame : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private List<TRRCommunityBoardPlayer> targetPlayers;
        [SerializeField] private TextMeshProUGUI numberText;

        public List<TRRCommunityBoardPlayer> TargetPlayerList { get { return targetPlayers; } }
        public void TriggerWin()
        {
            foreach (var each in targetPlayers)
            {
                each.TriggerWin();
            }
            animator.SetTrigger("Win");
        }
        public void InActiveElements()
        {
            foreach (var each in targetPlayers)
            {
                each.InActive();
            }
        }
        public void SetMultiplierText(string Text) { numberText.text = Text; }
    }
}