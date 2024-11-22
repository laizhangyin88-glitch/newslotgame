using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace BagelCode.Tasks.Actions.ClientAPI
{

    [Category("★ BagelCode/ClientAPI")]
    public class RequestWallOfEpicUploadImage : ActionTask<Blackboard>
    {
        public BBParameter<int> id;
        public BBParameter<byte[]> bytes;

        public BBParameter<bool> isAsync;
        
        protected override string info { get { return "Request Wall Of Epic Upload Image"; } }

        protected override void OnExecute()
        {
            if (isAsync.value)
            {
                BagelCodeClientAPI.WallOfEpicUploadImage(id.value, bytes.value,
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
                BagelCodeClientAPI.WallOfEpicUploadImage(id.value, bytes.value,
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
