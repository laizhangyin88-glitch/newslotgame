using System.Collections;
using System.Collections.Generic;
using ParadoxNotion;
using UnityEngine;
namespace GameStudio.Slot.IIP.Feature
{
    public class IIPTopBoardAdmin : FeatureModule
    {
        [SerializeField] Animator animator;
        int ticketCount = 0;

        private void Awake()
        {
            RegisterEvent("AddCommunityTicket", OnAddCommunityTicket);
        }

        public void OnAddCommunityTicket(EventData eventData)
        {
            ticketCount++;
            animator.SetTrigger("Collect");
            animator.SetInteger("Ticket", ticketCount);
        }
    }
}