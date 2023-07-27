using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Math")]
public class FloorDouble<T> : ActionTask 
{
    public BBParameter<object> targetValue;

    public BBParameter<T> saveAs;

    protected override string info
    {
        get 
        {
            return string.Format("{0} = {1}(Math.Floor({2}))", saveAs, typeof(T), targetValue);
        }
    }

    protected override void OnExecute()
    {
        double floorValue = System.Math.Floor(System.Convert.ToDouble(targetValue.value));
        saveAs.value = (T)System.Convert.ChangeType(floorValue, typeof(T));

        EndAction();
    }
}

}
