using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace BagelCode.Tasks.Actions.ClientAPI
{

    [Category("★ BagelCode/ClientAPI")]
    public class UploadImage : ActionTask<Blackboard>
    {
        public BBParameter<byte[]> bytes;

        public BBParameter<bool> isAsync;
        
        protected override string info
        {
            get
            {
                if(isAsync != null && isAsync.value)
                    return "Request Upload Image - Async";
                return "Request Upload Image";
            }
        }

        protected override void OnExecute()
        {
            if (isAsync.value)
            {
                BagelCodeClientAPI.UploadImage(bytes.value,
                    (response) =>
                    {
                    },
                    (error) =>
                    {
                        GlobalErrorHandler.GlobalError(error);
                    });
                EndAction();
            }
            else
            {
                BagelCodeClientAPI.UploadImage(bytes.value,
                    (response) =>
                    {
                        ClientAPI2Blackboard.Serialize(agent, response);
                        EndAction();
                    },
                    (error) =>
                    {
                        GlobalErrorHandler.GlobalError(error);
                    });
            }
        }
    }

}

