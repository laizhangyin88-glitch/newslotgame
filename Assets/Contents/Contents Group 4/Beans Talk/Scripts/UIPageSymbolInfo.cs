using Dreamteck.Splines.Primitives;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class UIPageSymbolInfo : MonoBehaviour
{

    public int symbol;
    public int numb;
    Text compText => GetComponent<Text>();
    void Start()
    {
        Dictionary<int, Dictionary<int, int>> paytables = BlackboardUtils.FindVariable<Dictionary<int, Dictionary<int, int>>>("./gameNew/paytables").value;
        Dictionary<int, int> changeCode = BlackboardUtils.FindVariable<Dictionary<int, int>>("./gameNew/changeCode").value;

        int _symbol = symbol;
        for (int i=0; i<changeCode.Count; i++)
        {
           //(int key1,int value1) = changeCode.ElementAt(i);
            int value = changeCode.ElementAt(i).Value;
            if (value == _symbol)
            {
                _symbol = changeCode.ElementAt(i).Key;
                break;
            }
        }
        compText.text = $"{paytables[_symbol][numb]}";
    }
}
