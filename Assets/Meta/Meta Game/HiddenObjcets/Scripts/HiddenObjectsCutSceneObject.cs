using System.Collections.Generic;
using Sirenix.OdinInspector;
using SlotMaker;
using SlotMaker.Json;
using UnityEngine;

namespace BagelCode.HiddenObjects
{
    [System.Serializable]
    public class CutSceneData
    {
        public string symbol;
        public int stage;
        public string name;
        public string character;
        public string position;
        public string message;
        public string triggerAnim;
    }

    [CreateAssetMenu(fileName = "HiddenObjectsCutSceneData", menuName = "Meta/ScriptableObject/HiddenObjectsCutSceneData")]
    public class HiddenObjectsCutSceneObject : ScriptableObject
    {
        [BoxGroup("GoogleSheet")]
        public string googleSpreadSheetId;

        [BoxGroup("GoogleSheet")]
        public string tableName;

        [BoxGroup("LocalSheet")]
        [TableList(IsReadOnly = true, ShowPaging = true,NumberOfItemsPerPage = 30)]
        public List<CutSceneData> cutSceneDataList;

#if UNITY_EDITOR
        private GoogleSheet googleSheet;

        [Button(ButtonSizes.Medium, Name = "Import")]
        [ButtonGroup("GoogleSheet/Controller")]
        private void Import()
        {
            if (string.IsNullOrEmpty(googleSpreadSheetId))
            {
                Debug.Log(string.Format("{0} file. Insert Google Spread Sheet ID!!!", name));
                return;
            }

            googleSheet = new GoogleSheet();
            googleSheet.webServiceUrl = EditorSettings.Instance.googleWebServiceUrl;
            googleSheet.servicePassword = EditorSettings.Instance.googleWebServicePassword;
            googleSheet.spreadsheetId = googleSpreadSheetId;
            googleSheet.processedResponseCallback.AddListener(Imported);

            googleSheet.GetTable(tableName);
        }

        private void Imported(GoogleSheet.QueryType query, List<string> objTypeNames, List<string> jsonData)
        {
            Debug.Log("Import All CutSceneData");
            cutSceneDataList.Clear();
            foreach (var json in jsonData)
            {
                cutSceneDataList.AddRange(SlotSimpleJson.DeserializeObject<List<CutSceneData>>(json));
            }
        }
#endif
    }
}
