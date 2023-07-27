using System.Collections;
using UnityEngine;
using System;

namespace GameStudio.Slot.CTC
{
    public class CTCUtill
    {
        public static IEnumerator CallActionAfterDelay(Action action, float delay)
        {
            yield return new WaitForSeconds(delay);
            action();
        }
    }
}