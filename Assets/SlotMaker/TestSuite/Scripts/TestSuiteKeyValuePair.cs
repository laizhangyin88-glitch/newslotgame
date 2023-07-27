using System;﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SlotMaker.TestSuite
{
    public class TestSuiteKeyValuePair : MonoBehaviour 
    {
        public TextMeshProUGUI keyText;
        public TMP_InputField valueField;
        
        private string key;
        private object value;
        
        public void OnValueChanged(string val)
        {
    #if DEV
            if (value is string)
            {
                testCase.customData[key] = val;
            }
            else if (value is bool)
            {
                testCase.customData[key] = Convert.ToBoolean(val);
            }
            else
            {
                testCase.customData[key] = Convert.ToInt64(val);
            }
    #endif
        }
        
    #if DEV
        private TestCase testCase;
        
        public void Bind(TestCase testCase, string key)
        {
            this.testCase = testCase;
            
            this.key = key;
            this.value = testCase.customData[key];
            
            keyText.text = key;
            valueField.text = this.value.ToString();
        }
    #endif
    }
}
