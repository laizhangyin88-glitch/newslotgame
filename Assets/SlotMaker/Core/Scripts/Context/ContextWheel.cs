using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;

namespace SlotMaker
{

public class ContextWheel : ContextCompositor, IContextBooleanProperty, IContextFloatProperty
{
    public BigWheel wheel;
    
    public void SetBooleanProperty(bool value)
    {
        //animator.SetBool(propertyName, value);
    }

    public bool GetBooleanProperty()
    {
        return true;
        //return animator.GetBool(propertyName);
    }

    public void SetFloatProperty(float value)
    {
        var rotation = wheel.transform.rotation;
        var eulerAngles = rotation.eulerAngles;
        eulerAngles.z = value;
        rotation.eulerAngles = eulerAngles;
        wheel.transform.rotation = rotation;
    }

    public float GetFloatProperty()
    {
        return wheel.transform.rotation.eulerAngles.z;
    }
}

}
