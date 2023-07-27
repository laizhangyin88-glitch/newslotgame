using System.Collections;
using System.Collections.Generic;
using ParadoxNotion;
using SlotMaker;
namespace GameStudio.Slot.EDM.Popup
{
    public class EDMPopupAdmin : FeatureModule
    {
        public List<EDMPopup> popupList = new List<EDMPopup>();

        private void Awake()
        {
            RegisterEvent("EDM_POPUP_OPEN", OnReceivedPopupEvent);
        }

        private void OnReceivedPopupEvent(EventData eventData)
        {
            int value = (int)eventData.value;
            StartCoroutine(PopupCoroutine(value));
        }

        public IEnumerator PopupCoroutine(int popupIndex)
        {
            EDMPopup popup = popupList[popupIndex];
            popup.gameObject.SetActive(true);
            yield return popup.StartCoroutine(popup.ActiveCoroutine());
            popup.gameObject.SetActive(false);
            ContentEvent.SendEvent<int>("EDM_POPUP_END", popupIndex);
        }
    }
}