using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;

namespace BagelCode.Task.Actions
{

    [Category("★ BagelCode/IAM")]
    public class GetIAMFromType : ActionTask<Blackboard>
    {
        public BBParameter<InAppMessageType> iamType;

        public BBParameter<Blackboard> saveIamInfo;
        public BBParameter<int>        saveIamID;
        public BBParameter<bool>       saveIsExist;

        protected override string info
        {
            get { return string.Format("Get {0} IAM Info", iamType); }
        }

        protected override void OnExecute()
        {
            List<Blackboard> tmpList = BlackboardQueryUtils.GetIAMListFromType(iamType.value);

            if (tmpList.Count > 0)
            {
                saveIamInfo.value   = tmpList[0];
                saveIamID.value     = saveIamInfo.value.GetValue<int>("id");
                saveIsExist.value   = true;
            }
            else
            {
                saveIamInfo.value   = null;
                saveIamID.value     = 0;
                saveIsExist.value   = false;

                if (ApplicationSettings.LogSystem())
                    Debug.LogWarning("[IAM]" + iamType.value.ToString() + " is not exist");
            }

            EndAction();
        }
    }

}
