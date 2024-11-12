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
    public class GetSurveyIAM : ActionTask<Blackboard>
    {
        public BBParameter<Blackboard> saveIamInfo;

        protected override string info
        {
            get { return string.Format("Get Survey IAM Info(Legacy. using to GetIAMFromType !!)"); }
        }

        protected override void OnExecute()
        {
            List<Blackboard> tmpList = BlackboardQueryUtils.GetIAMListFromType(InAppMessageType.SURVEY_POPUP);

            if (tmpList.Count > 0)
            {
                saveIamInfo.value = tmpList[0];
                EndAction(true);
            }
            else
            {
                if (ApplicationSettings.LogSystem())
                    Debug.LogWarning("[IAM]" + InAppMessageType.SURVEY_POPUP.ToString() + " is not exist");
                EndAction(false);
            }
        }
    }

}
