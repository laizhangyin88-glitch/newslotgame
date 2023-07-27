using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace BagelCode
{

public class DynamicScrollToggleSimulator : MonoBehaviour
{
    private List<bool>               toggleValueList;
    private List<Toggle>             toggleList;
    private List<UnityAction<bool>>  callBackList;

    public void RegistCallBack(int index)
    {
        callBackList.Add( (bool isOn) => { toggleValueList[index] = isOn; } );
    }

    public void OnInitialize(int length, bool value = false)
    {
        toggleList      = new List<Toggle>();
        toggleValueList = new List<bool>();
        callBackList    = new List<UnityAction<bool>>();

        for (int i = 0; i < length; ++i)
        {
            toggleList.Add(null);
            toggleValueList.Add(value);
            RegistCallBack(i);
        }
    }

    public void SelectAll(bool value)
    {
        for (int i = 0; i < toggleValueList.Count; ++i)
        {
            if (toggleList[i] != null)
            {
                toggleList[i].isOn = value;
            }
            toggleValueList[i] = value;
        }
    }

    public void Push(int index, Toggle toggle)
    {
        toggle.isOn = toggleValueList[index];
        toggle.onValueChanged.RemoveListener( callBackList[index] );
        toggle.onValueChanged.AddListener( callBackList[index] );

        toggleList[index] = toggle;
    }

    public void Pop(int index)
    {
        toggleList[index].onValueChanged.RemoveListener( callBackList[index] );
        toggleList[index] = null;
    }

    public List<bool> GetToggleList()
    {
        return toggleValueList;
    }
}

}
