using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Sounds")]
public class UnPauseGameSound : ActionTask
{
    public BBParameter<GameSound> gameSound;

    protected override string info { get { return "UnPause " + gameSound; } }

    protected override void OnExecute()
    {
        gameSound.value.UnPause();

        EndAction();
    }
}

}
