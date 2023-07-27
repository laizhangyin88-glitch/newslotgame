using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Symbol")]
public class SymbolAnimatorPlay : ActionTask
{
    public BBParameter<Animator> animator;
    public int stateNameHash;

    protected override string info { get { return string.Format("{0}.Play({1})", animator, GlobalSymbolAssets._AnimatorHashToString(stateNameHash)); } }

    protected override void OnExecute()
    {
        animator.value.Play(stateNameHash, -1, 0f);

        EndAction();
    }
}

}
