using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Sounds")]
public class PlayGameSound : ActionTask
{
    public BBParameter<GameSound> gameSound;
    public BBParameter<bool> excludePlaying;

    protected override string info { get { return "Play " + gameSound; } }

    protected override void OnExecute()
    {
        if (gameSound.value == null)
        {
            EndAction();
            return;
        }

        if (excludePlaying.value && (gameSound.value.IsPlaying && !gameSound.value.IsFading))
        {
            EndAction();
            return;
        }

        gameSound.value.Play();
        EndAction();
    }
}

}
