using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestManager : MonoSingleton<TestManager>
{


    public GameObject inputCode;

    public GameObject inputList;

    public GameObject inputSpin;


    public GameObject inputAutoUrl;
    public string getSpin()
    {
        string res = inputSpin.GetComponent<InputField>().text??"";
        inputSpin.GetComponent<InputField>().text = "";
        return res;
    }
    public int getCode()
    {
        string res = inputCode.GetComponent<InputField>().text ?? "";
        inputCode.GetComponent<InputField>().text = "";

        if (res == "")
            return 0;
        return int.Parse(res);
    }
    public int[] getList()
    {

        string lstStr = inputList.GetComponent<InputField>().text ?? "";
        string[] lstStrs = lstStr.Replace(" ", "").Split(',') ?? new string[] { };

        List<int> temp = new List<int>();
        for (int i = 0; i < lstStrs.Length; i++)
        {
            if (lstStrs[i] != "" && lstStrs[i] != null)
            {
                temp.Add(int.Parse(lstStrs[i]));
            }
        }

        return temp.ToArray();
    }

    public string getAutoUrl()
    {
        return inputAutoUrl.GetComponent<InputField>().text ?? "";
    }
}
