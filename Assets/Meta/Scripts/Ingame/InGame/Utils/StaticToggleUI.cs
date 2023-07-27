using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    public class StaticToggleUI : MonoBehaviour
    {
        public bool startToUpdate;

        public string key;
        public bool reverse;

        public void Start()
        {
            if(startToUpdate)
                Toggle();
        }

        public void Toggle()
        {
            bool toggle = BlackboardUtils.FindValue<bool>(key);
            gameObject.SetActive(reverse ? !toggle : toggle);
        }
    }
}
