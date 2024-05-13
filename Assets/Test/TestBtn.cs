using SlotMaker.Tasks.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestBtn : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject inputObject;

    public GameObject inputList;
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void InputClearClick()
    {
        inputObject.GetComponent<InputField>().text = "";
        inputList.GetComponent<InputField>().text ="";
        globalStore.test_is_free_spin = 0;
        globalStore.test_spin_tab =  new int[] { };
    }
    public void InputOkClick()
    {
        try
        {

            string inputTxt = inputObject.GetComponent<InputField>().text == "" || inputObject.GetComponent<InputField>().text == null ?
                "0" : inputObject.GetComponent<InputField>().text;
            globalStore.test_is_free_spin = int.Parse(inputTxt);
                //int.Parse(inputObject.GetComponent<InputField>().text);
            string lstStr = inputList.GetComponent<InputField>().text ?? "";
            string[] lstStrs = lstStr.Replace(" ", "").Split(',') ?? new string[] { };

            List<int> temp = new List<int>();
            for (int i = 0; i < lstStrs.Length; i++)
            {
                if (lstStrs[i] != "" && lstStrs[i]!= null)
                {
                    temp.Add(int.Parse(lstStrs[i]));
                } 
            }
            globalStore.test_spin_tab = temp.ToArray();

        }
        catch (Exception e)
        {
            Debug.LogWarning(e, this);
        }
        
    }

    public void SpeedX10()
    {
        Time.timeScale = 10;
    }
    public void SpeedX2()
    {
        Time.timeScale = 2;
    }
    public void SpeedX1()
    {
        Time.timeScale = 1;
    }
}
