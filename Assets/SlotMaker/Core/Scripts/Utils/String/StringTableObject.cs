using Sirenix.OdinInspector;
using SlotMaker.IoC.Strategy;
using SlotMaker.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SlotMaker
{
    [CreateAssetMenu(fileName = "New StringTable", menuName = "SlotMaker/ScriptableObject/StringTableObject")]
    public class StringTableObject : StringSources
    {
        [BoxGroup("GoogleSheet")]
        [PropertyOrder(0)]
        public bool importTargetLanguage = false;

        [ShowIf("importTargetLanguage")]
        [BoxGroup("GoogleSheet")]
        [PropertyOrder(1)]
        public string language = "";

        [BoxGroup("GoogleSheet")]
        [PropertyOrder(2)]
        public bool importAll = true;

        [BoxGroup("GoogleSheet")]
        [PropertyOrder(3)]
        public string googleSpreadSheetId;

        [BoxGroup("GoogleSheet")]
        [DisableIf("importAll")]
        [PropertyOrder(4)]
        public string tableName;

        [BoxGroup("GoogleSheet")]
        [PropertyOrder(5)]
        public StringTableObject patch;

        [BoxGroup("LocalSheet")]
        [PropertyOrder(10)]
        [TableList(IsReadOnly = false, ShowPaging = true)]
        public List<StringStringPairVariable> stringTable;

        public override List<StringStringPairVariable> Get()
        { return stringTable; }

        public override string[] GetNames()
        {
            return stringTable.Select(pair => pair.key).ToArray();
        }

        public override bool ExistsSource(string name)
        {
            return stringTable.Exists(pair => string.Equals(pair.key, name));
        }

        public override void AddSource(string name)
        {
            if (string.IsNullOrEmpty(GetString(name)))
            {
                stringTable.Add(new StringStringPairVariable { key = name });
            }
        }

        public override void RemoveSource(string name)
        {
            stringTable.RemoveAll(pair => string.Equals(pair.key, name));
        }

        public override string GetString(string name)
        {
            var found = stringTable.Find(pair => string.Equals(pair.key, name));
            return found != null ? found.value : null;
        }

        public override void SetString(string name, string value)
        {
            var found = stringTable.Find(pair => string.Equals(pair.key, name));
            if (found != null && !string.Equals(found.value, value))
            {
                found.value = value;
            }
        }

#if UNITY_EDITOR
        private GoogleSheet googleSheet;

        public void OnImport()
        {
            if (importTargetLanguage)
                ImportTargetLanguageOnly();
            else
                ImportGlobalLanguage();
        }

        public virtual void CompleteImport()
        {
            // TODO. Post process.
        }

        [HideIf("importTargetLanguage")]
        [Button(ButtonSizes.Medium, Name = "Import(Global)")]
        [ButtonGroup("GoogleSheet/Controller")]
        [PropertyOrder(5)]
        private void Import()
        {
            ImportGlobalLanguage();
        }

        [ShowIf("importTargetLanguage")]
        [Button(ButtonSizes.Medium, Name = "Import(Target)")]
        [ButtonGroup("GoogleSheet/Controller")]
        [PropertyOrder(5)]
        private void ImportTargetLanguage()
        {
            ImportTargetLanguageOnly();
        }

        public void ImportGlobalLanguage()
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

            if (importAll)
                googleSheet.GetAllTables();
            else
                googleSheet.GetTable(tableName);
        }

        private void Imported(GoogleSheet.QueryType query, List<string> objTypeNames, List<string> jsonData)
        {
            Debug.Log("Import All");
            stringTable.Clear();
            foreach (var json in jsonData)
            {
                stringTable.AddRange(
                    SlotSimpleJson.DeserializeObject<List<StringStringPairVariable>>(json));
            }

            Patch();
            CompleteImport();
        }

        [Button]
        [ButtonGroup("GoogleSheet/Controller")]
        [PropertyOrder(6)]
        private void Open()
        {
            Application.OpenURL("https://docs.google.com/spreadsheets/d/" + googleSpreadSheetId);
        }

        [Button]
        [ButtonGroup("GoogleSheet/Controller")]
        [PropertyOrder(7)]
        private void Patch()
        {
            if (patch == null) return;

            var map = new Dictionary<string, StringStringPairVariable>();
            foreach (var item in stringTable)
            {
                map[item.key] = item;
            }

            foreach (var item in patch.stringTable)
            {
                StringStringPairVariable ret = null;
                if (map.TryGetValue(item.key, out ret))
                {
                    ret.value = item.value;
                }
                else
                {
                    stringTable.Add(new StringStringPairVariable
                    {
                        key = item.key,
                        value = item.value
                    });
                }
            }
        }

        private void ImportTargetLanguageOnly()
        {
            if (string.IsNullOrEmpty(googleSpreadSheetId))
            {
                Debug.Log("Insert Google Spread Sheet ID!!!");
                return;
            }

            if (string.IsNullOrEmpty(language.Trim()))
            {
                Debug.Log(string.Format("{0} file. Empty language name!!!", name));
                return;
            }

            Debug.Log(string.Format("Import Target Language Only({0})", language));

            googleSheet = new GoogleSheet();
            googleSheet.webServiceUrl = EditorSettings.Instance.googleWebServiceUrl;
            googleSheet.servicePassword = EditorSettings.Instance.googleWebServicePassword;
            googleSheet.spreadsheetId = googleSpreadSheetId;
            googleSheet.processedResponseCallback.AddListener(LanguageOnlyImported);

            googleSheet.GetAllTables();
        }

        private void LanguageOnlyImported(GoogleSheet.QueryType query, List<string> objTypeNames, List<string> jsonData)
        {
            stringTable.Clear();
            foreach (var json in jsonData)
            {
                var dataList = SlotSimpleJson.DeserializeObject<List<Dictionary<string, string>>>(json);
                foreach (var data in dataList)
                {
                    if (data.ContainsKey(language) && !string.IsNullOrEmpty(data[language].Trim()))
                    {
                        var stringData = new StringStringPairVariable();
                        stringData.key = data["key"];
                        stringData.value = data[language];

                        stringTable.Add(stringData);
                    }
                }
            }

            Patch();
            CompleteImport();
        }

#endif
    }
}
