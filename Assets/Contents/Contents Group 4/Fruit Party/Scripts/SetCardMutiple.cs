using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SetCardMutiple : MonoBehaviour
{
    public int index;

    private TextMeshProUGUI textMeshProUGUI;

    private void Start()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        SetInfo();
    }

    private void SetInfo()
    {
        Dictionary<int, Dictionary<int, int>> paytables = BlackboardUtils.FindVariable<Dictionary<int, Dictionary<int, int>>>("./gameNew/paytables").value;
        if(paytables != null)
        {
            var temp = paytables[index ];
            int[] array = new int[temp.Count];
            int i = 0;
            string str = "";
            foreach (var item in temp)
            {
                array[i] = item.Value;
                i++;
            }
            for (global::System.Int32 j = array.Length - 1; j >= 0; j--)
            {
                str += (array[j] + "\n"); 
            }
            textMeshProUGUI.text = str; 
        }
    }
}
