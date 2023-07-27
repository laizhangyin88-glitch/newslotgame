using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{

public class RegisterGameObjectToBlackboard : MonoBehaviour 
{
    public string path;

    private void OnEnable()
    {
        var variable = BlackboardUtils.GetOrCreateVariable<GameObject>(null, path);
        variable.value = gameObject;
    }
}

}
