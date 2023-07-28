using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace GameStudio.Slot.NDS
{
    public class NDSTurboSpinController : MonoBehaviour
    {
        public void OnClickButton()
        {
            var variableA = BlackboardUtils.GetOrCreateVariable<bool>(null, "./customData/isTurbo");
            variableA.value = !variableA.value;
            GetComponent<Animator>().SetBool("On", variableA.value);
        }
    }
}
