using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SetContextActionListPlayerPlay : ActionTask<ContextElement>
{
    protected override string info
    {
        get { return string.Format("Play ActionListPlayer", agent); }
    }

    protected override void OnExecute()
    {
        IContextPlayer player = agent as IContextPlayer;
        if (player == null)
        {
            Debug.LogError("[Context] " + agent.ContextName + " is not IContextPlayer");
            EndAction(false);
        }
        else
        {
            player.Play();
            EndAction();
        }
    }
}

}
