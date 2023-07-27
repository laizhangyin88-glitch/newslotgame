using System.Collections.Generic;
using BagelCode.ClientModels;
using Sirenix.OdinInspector;
using SlotMaker;
using SlotMaker.Json;
using UnityEngine;

namespace BagelCode.BossRaiders
{
    [System.Serializable]
    public class MonsterData
    {
        [TableColumnWidth(60, Resizable = false)]
        public int index;
        [VerticalGroup("Data Column")]
        public string prefabName;
        [VerticalGroup("Data Column")]
        public string skinName;
        [VerticalGroup("Data Column")]
        public BossRaidersBossType type;
        [VerticalGroup("Data Column")]
        public BossRaidersBossColorType colorType;
        [VerticalGroup("Data Column")]
        public float scale;
        [VerticalGroup("Data Column")]
        public bool isBigSize;
        [VerticalGroup("Data Column")]
        public int bossIndex;
    }

    [CreateAssetMenu(fileName = "New BossRaidersMonsterDataAsset", menuName = "Meta/ScriptableObject/BossRaidersMonsterDataAsset")]
    public class BossRaidersMonsterDataAssets : ScriptableObject
    {
        [BoxGroup("GoogleSheet")]
        public string googleSpreadSheetId;

        [BoxGroup("GoogleSheet")]
        public string tableName;

        [TableList(ShowPaging = true, NumberOfItemsPerPage = 30)]
        public List<MonsterData> monsterDataList;

        public MonsterData GetData(int index)
        {
            if (index < 0 || monsterDataList == null) return null;
            if (monsterDataList.Count <= index) return monsterDataList[0];
            return monsterDataList[index];
        }

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
            Debug.Log("Import Monster Variation");
            monsterDataList.Clear();
            foreach (var json in jsonData)
            {
                Debug.Log(json);
                monsterDataList.AddRange(SlotSimpleJson.DeserializeObject<List<MonsterData>>(json));
            }
        }

        [Button(ButtonSizes.Medium, Name = "Open")]
        [ButtonGroup("GoogleSheet/Controller")]
        private void Open()
        {
            if (!string.IsNullOrEmpty(googleSpreadSheetId))
                Application.OpenURL("https://docs.google.com/spreadsheets/d/" + googleSpreadSheetId);
            else
                Debug.Log("Empty googleSpreadSheetId!!");
        }
#endif
    }
}