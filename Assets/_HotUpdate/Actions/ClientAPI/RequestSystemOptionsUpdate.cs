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
    public class RequestSystemOptionsUpdate : ActionTask
    {
        public BBParameter<bool> pushNotification;
        public BBParameter<bool> kudoJackpot;
        public BBParameter<bool> kudoTournament;
        public BBParameter<bool> globalChatNotification;
        public BBParameter<bool> kudoNewUserNotification;
        public BBParameter<bool> enablePipMode;

        protected override string info { get { return "Request System Options Update"; } }

        protected override void OnExecute()
        {
            BagelCodeClientAPI.SystemOptionsUpdateRequest( pushNotification.value,
                                                           kudoJackpot.value,
                                                           kudoTournament.value,
                                                           globalChatNotification.value,
                                                           kudoNewUserNotification.value,
                                                           enablePipMode.value,
            (response) =>
            {

            },
            (error) =>
            {

            });

            EndAction(true);
        }
    }

}

