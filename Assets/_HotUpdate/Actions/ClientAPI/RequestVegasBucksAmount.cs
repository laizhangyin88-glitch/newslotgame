using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/ClientAPI")]
    public class RequestVegasBucksAmount : ActionTask
    {
        public BBParameter<bool> isAvailableBucks;
        public BBParameter<bool> isSuccess;

        protected override string info
        {
            get { return "Request Refresh Bucks Amount"; }
        }

        protected override void OnExecute()
        {
            if (isAvailableBucks.value)
            {
                BagelCodeClientAPI.RequestBucksAmount(
                    (response) =>
                    {
                        BlackboardQueryUtils.UpdateUserBucks(response.userBucks);
                        isSuccess.value = true;
                        EndAction(true);
                    },
                    (error) =>
                    {
                        Debug.Log("Error : " + error.errorCode.ToString());
                        isSuccess.value = false;
                        EndAction(true);
                    });
            }
            else
            {
                isSuccess.value = false;
                EndAction(true);
            }
        }
    }
}