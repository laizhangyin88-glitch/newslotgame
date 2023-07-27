using NodeCanvas.Framework;

namespace BagelCode.InAppMessage
{
    public class InAppMessagePreviewController : InAppMessageBase
    {
        public override void LoadIAM(string bundleName, Blackboard loadIamInfo, long iamEndTimestamp)
        {
            isPreview = true;
            base.LoadIAM(bundleName, loadIamInfo, iamEndTimestamp);
        }

        public void ReloadIam(Blackboard newIamInfo)
        {
            iamInfo = newIamInfo;
            DestroyComponents();
            LoadIAM(bundleName, iamInfo, endTimestamp);
        }

        private void DestroyComponents()
        {
            componentObjList?.ForEach(c => Destroy(c));
            componentObjList?.Clear();
        }
    }
}
