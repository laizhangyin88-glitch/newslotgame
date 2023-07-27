using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Sounds")]
public class PauseGameSound : ActionTask
{
    public BBParameter<GameSound> gameSound;

    protected override string info { get { return "Pause " + gameSound; } }

    protected override void OnExecute()
    {
        gameSound.value.Pause();

        EndAction();
    }
}

}
