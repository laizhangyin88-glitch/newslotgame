using ParadoxNotion;
using UnityEngine;

namespace GameStudio.Slot.IIP.Popup
{
    public class IIPCommunityGameTriggerPopup : FeatureModule
    {
        [SerializeField] private Animator animator;

        private void Awake()
        {
            RegisterEvent("IIPCloseCommunityGameTriggerPopup", ClosePopup);
        }

        public void ClosePopup(EventData eventData)
        {
            animator.SetTrigger("Disappear");
        }
    }
}