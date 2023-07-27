using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Condition
{

[Category("★ SlotMaker/PlayerPrefs")]
public class CheckStringPlayerPrefs : ConditionTask
{
    public BBParameter<string> valueA;
    public BBParameter<string> defaultValue;
    public BBParameter<string> valueB;

    protected override string info
    {
        get { return valueA + " == " + valueB; }
    }

    protected override bool OnCheck() 
    {
        string value = PlayerPrefs.GetString(valueA.value, defaultValue.value);
        return string.Equals(value, valueB.value);
    }
}

}