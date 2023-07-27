using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class PopupLevelMultiplierItem
    {

        private int index = 0;
        private bool isActive = false;
        private string typeValue = "";

        public int Index { get { return index; } }
        public bool IsActive { get { return isActive; } }
        public string TypeValue { get { return typeValue; } }

        public PopupLevelMultiplierItem(int _index)
        {
            index = _index;
        }

        public void SetData(bool _isActive, string _typeValue)
        {
            isActive = _isActive;
            typeValue = _typeValue;
        }
    }
}