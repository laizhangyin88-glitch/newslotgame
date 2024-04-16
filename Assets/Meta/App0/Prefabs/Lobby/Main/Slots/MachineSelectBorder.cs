using ParadoxNotion;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineSelectBorder : MonoBehaviour
{
    // Start is called before the first frame update
    //public string name = "";

    public int index = -1;

    public GameObject selectBorder;
    void Start()
    {
        if (selectBorder == null)
            selectBorder = transform.FindChild("Selected")?.gameObject;

        if (index == 0)
        {
            selectBorder.SetActive(true);
        }
        else
        {
            selectBorder.SetActive(false);
        }
    }

    private void OnDestroy()
    {
    }


    void Update()
    {

    }
}
