using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/PlayerPrefs")]
public class SetBooleanPlayerPrefs1 : ActionTask
{
    public BBParameter<string> valueA;
    public BBParameter<bool> defaultValue;
    public BBParameter<bool> arg;

    protected override string info
    {
        get 
        { 
            return string.Format("Set {0} to {1}", valueA, arg);     
        }
    }

    protected override void OnExecute()
    {
        PlayerPrefs.GetInt(valueA.value, (defaultValue.value ? 1 : 0));
        PlayerPrefs.SetInt(valueA.value, (arg.value == true) ? 1 : 0);

        EndAction();
    }
}

}
