using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlotMaker.TestSuite
{
    public class TestSuiteCustomReport : MonoBehaviour 
    {
        public TMP_Dropdown dropdownProject;
        public TMP_Dropdown dropdownType;
        public InputField inputfieldTitle;
        public InputField inputfieldNotes;

        private string[] REPORT_TYPE = new string[2]
        {
            "bug",
            "en"
        };

        public static List<long> PROJECTS_CUSTOM_REPORT = new List<long>{ 860738391520830 };

        public void Submit()
        {
#if DEV
            var report = new Dictionary<string, object>();
            report["projects"] = PROJECTS_CUSTOM_REPORT;
            report["name"] = inputfieldTitle.text;
            report["notes"] = inputfieldNotes.text;
            report["screenshot"] = null;
            report["type"] = REPORT_TYPE[dropdownType.value];
            if (dropdownProject.value == 0)
            {
                var gameTitle = BlackboardUtils.FindVariable<string>("./game/gameTitle");
                report["content"] = (gameTitle != null) ? gameTitle.value : "unknown";

                var reportDetails = new Dictionary<string, object>();
                reportDetails["dump"] = BlackboardSerializer.Deserialize(ContentBlackboard.Get());
                report["details"] = reportDetails;
            }
            else if (dropdownProject.value == 1)
            {
                report["content"] = "meta";
            }
            
            TestSuiteManager.Instance.UpdateReportHeader(report);
            TestSuiteManager.Instance.Report(report);
#endif
            Close();
        }

        public void Close()
        {
            GameObject.Destroy(gameObject);
        }
    }
}
