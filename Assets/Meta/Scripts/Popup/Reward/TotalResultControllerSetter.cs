using UnityEngine;
using NodeCanvas.Framework;

namespace BagelCode
{
    public class TotalResultControllerSetter : MonoBehaviour
    {
        private Blackboard bb;

        private void Start()
        {
            bb = GetComponent<Blackboard>();

            // Set Variables
            bool isFromDailySpin = bb.GetVariable<bool>("_isFromDailySpin")?.value ?? false;
            bool isFromHogDeal = bb.GetVariable<bool>("_isFromHogDeal")?.value ?? false;

            if (isFromDailySpin) gameObject.AddComponent<TotalResultPopupControllerDailySpin>();
            else if (isFromHogDeal) gameObject.AddComponent<TotalResultPopupControllerHogDeal>();
            else gameObject.AddComponent<TotalResultPopupControllerScratcher>();
        }
    }
}
