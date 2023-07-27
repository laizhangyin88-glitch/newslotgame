using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace SlotMaker.Tasks.Actions.Contents
{
[Category("★ BagelCode/SlotMachine")]
public class PlaySymbolAnimation : ActionTask
{
    public BBParameter<GameObject> symbol;
    public BBParameter<string>  animationName;

    protected override string info
    {
        get { return string.Format("Play symbol {0} animation", animationName); }
    }

    protected override void OnExecute()
    {
        symbol.value.GetComponent<BaseSymbol>().Play(animationName.value);
        EndAction();
    }
}

}
