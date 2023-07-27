using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{
[Category("★ BagelCode/SlotMachine/Legacy")]
public class PlaySymbolAnimation : ActionTask
{
    public BBParameter<BaseSymbol> symbol;
    public BBParameter<string>     animationName;
    protected override string info
    {
        get { return string.Format("[Legacy]Play symbol {0} animation", animationName); }
    }

    protected override void OnExecute()
    {
        symbol.value.Play(animationName.value);
        EndAction();
    }
}

}
