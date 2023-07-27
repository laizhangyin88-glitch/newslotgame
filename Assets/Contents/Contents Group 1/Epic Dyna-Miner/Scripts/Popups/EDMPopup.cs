using System.Collections;
using UnityEngine;
namespace GameStudio.Slot.EDM.Popup
{
    public abstract class EDMPopup : MonoBehaviour
    {
        public abstract IEnumerator ActiveCoroutine();
    }
}