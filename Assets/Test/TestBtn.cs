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
    void Start()
    {
        
    }

    static int numb = 0;

    // Update is called once per frame
    void Update()
    {
        
    }

    /*public void onInputEndEdit(string str)
    {
        try
        {
            //string str1 =transform.gameObject.GetComponent<InputField>
            //TestBtn.numb = int.Parse(str1);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e, this);
        }
    }*/
    public void SetDebugParam1()
    {
        globalStore.test_is_free_spin = 1;
    }

    public void SetDebugParam2()
    {
        globalStore.test_is_free_spin = 2;
    }


    public void SetDebugParam0()
    {
        globalStore.test_is_free_spin = 0;
    }

    public void SetDebugParam3()
    {
        globalStore.test_is_free_spin = 3;
    }

    public void InputOkClick()
    {
        try
        {
            //string str1 =transform.gameObject.GetComponent<InputField>
            //TestBtn.numb = int.Parse(str1);

            globalStore.test_is_free_spin = int.Parse(inputObject.GetComponent<InputField>().text);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e, this);
        }
        
    }
}
