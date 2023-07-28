using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace GameStudio.Slot.TRL
{
    public class TRLTreasureBonusChooseFrame : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private List<TRLCommunityBoardPlayer> targetPlayers;
        [SerializeField] private TextMeshProUGUI numberText;

        public List<TRLCommunityBoardPlayer> TargetPlayerList { get { return targetPlayers; } }
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