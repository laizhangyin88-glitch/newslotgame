using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlotMaker.Keno.Commands
{
    [CreateAssetMenu(fileName = "New ApplyNumberText", menuName = "SlotMaker/Keno/Commands/Ball/ApplyNumberText")]
    public class ApplyNumberTextCommand : Command<BallInstance, ContextTextMeshProUGUI>
    {
        protected override void OnExecute(BallInstance ball)
        {
            agent.SetText(ball.number.ToString());

            EndAction();
        }
    }
}
