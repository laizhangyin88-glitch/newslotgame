using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using SlotMaker.Json;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class RequestStatusMatch : ActionTask
{
    public BBParameter<string> email;
    public BBParameter<int> programIndex;
    public BBParameter<int> appIndex;
    public BBParameter<int> statusIndex;
    public BBParameter<string> userId;
    public BBParameter<string> otherVipProgramApp;
    public BBParameter<string> otherVipProgramStatus;
    public BBParameter<byte[]> proofImage;


    protected override string info { get { return "RequestStatusMatch"; } }

    protected override void OnExecute()
    {
        StatusMatchVipProgram program = StatusMatchVipProgram.OTHER;
        StatusMatchVipProgramApp app = StatusMatchVipProgramApp.OTHER;
        StatusMatchVipProgramStatus status = StatusMatchVipProgramStatus.OTHER;
        string otherProgramAppName = "";
        string otherVipProgramStatusName = "";


        switch (programIndex.value)
        {
            case 1: // playtika
                program = (StatusMatchVipProgram)programIndex.value;
                app = (StatusMatchVipProgramApp)(appIndex.value + 1);
                status = (StatusMatchVipProgramStatus)(statusIndex.value + 1);
            break;
            case 2: // loyalty
                program = (StatusMatchVipProgram)programIndex.value;
                app = (StatusMatchVipProgramApp)(appIndex.value + 4);
                status = (StatusMatchVipProgramStatus)(statusIndex.value + 5);
            break;
            case 3: // huuge
                program = (StatusMatchVipProgram)programIndex.value;
                app = (StatusMatchVipProgramApp)(appIndex.value + 8);
                status = (StatusMatchVipProgramStatus)(statusIndex.value + 11);
            break;
            case 4: // ss
                program = (StatusMatchVipProgram)programIndex.value;
                app = (StatusMatchVipProgramApp)(appIndex.value + 9);
                status = (StatusMatchVipProgramStatus)(statusIndex.value + 21);
            break;
            case 5: // jj
                program = (StatusMatchVipProgram)programIndex.value;
                app = (StatusMatchVipProgramApp)(appIndex.value + 10);
                status = (StatusMatchVipProgramStatus)(statusIndex.value + 28);
            break;
            case 6: // other
                program = StatusMatchVipProgram.OTHER;
                app = StatusMatchVipProgramApp.OTHER;
                status = StatusMatchVipProgramStatus.OTHER;
                otherProgramAppName = otherVipProgramApp.value;
                otherVipProgramStatusName = otherVipProgramStatus.value;
            break;
        }

        // byte[] test = System.Text.Encoding.UTF8.GetBytes("GIF87a");

        BagelCodeClientAPI.StatusMatchRequest(email.value, program, app, status,
        otherProgramAppName, otherVipProgramStatusName, userId.value, proofImage.value,
        (response) =>
        {
            Blackboard bb = agent.GetComponent<Blackboard>();
            ClientAPI2Blackboard.Serialize(bb, response);
            EndAction(true);
        },
        (error) =>
        {
            GlobalErrorHandler.GlobalError(error);
        });
    }
}

}

