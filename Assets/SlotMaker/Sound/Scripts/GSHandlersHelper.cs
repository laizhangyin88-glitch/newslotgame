using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using SlotMaker.Json;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SlotMaker
{
	[RequireComponent(typeof(Blackboard))]
	public class GSHandlersHelper : MonoBehaviour
	{
        [BoxGroup("GoogleSheet")]
	    public string googleSpreadSheetId;

        [BoxGroup("GoogleSheet")]
	    public string tableName;

#if UNITY_EDITOR
        private class GSHandlerData
        {
            public string key             = "";
            public string bundleName      = "";
            public string assetName       = "";
            public string output          = "Default";
            public bool   loop            = false;
            public float  volume          = 1f;
            public float  delay           = 0f;
            public string playingType     = "Independent";
            public int    countLimit      = 0;
            public string fadeInEaseType  = "None";
            public float  fadeInTime      = 0f;
            public string fadeOutEaseType = "None";
            public float  fadeOutTime     = 0f;
            public bool   autoRelease     = true;
        }

        private GoogleSheet googleSheet;

        [HorizontalGroup("GoogleSheet/Button")]
        [Button]
        private void Import()
        {
            googleSheet = new GoogleSheet();
            googleSheet.webServiceUrl = EditorSettings.Instance.googleWebServiceUrl;
            googleSheet.servicePassword = EditorSettings.Instance.googleWebServicePassword;
            googleSheet.spreadsheetId = googleSpreadSheetId;
            googleSheet.processedResponseCallback.AddListener(Imported);

            if (string.IsNullOrEmpty(tableName))
                googleSheet.GetAllTables();
            else
                googleSheet.GetTable(tableName);
        }

        private void Imported(GoogleSheet.QueryType query, List<string> objTypeNames, List<string> jsonData)
        {
            var bb = GetComponent<Blackboard>();

            bb.variables.Clear();
            foreach (var json in jsonData)
            {
                var list = SlotSimpleJson.DeserializeObject<List<GSHandlerData>>(json);
                for (int i = 0; i < list.Count; ++i)
                {
                    var data = list[i];
                    bb.AddVariable(data.key, new GSHandler
                    {
                        clip = new GSClip
                        {
                            bundleName = data.bundleName,
                            assetName = data.assetName
                        },
                        output = (GSMixerGroup)System.Enum.Parse(typeof(GSMixerGroup), data.output),
                        loop = data.loop,
                        volume = data.volume,
                        delay = data.delay,
                        playingType = (GSPlayingType)System.Enum.Parse(typeof(GSPlayingType), data.playingType),
                        countLimit = data.countLimit,
                        fadeIn = data.fadeInTime > 0f ? new GSFadeInOut{ time = data.fadeInTime, easeType = (GSEaseType)System.Enum.Parse(typeof(GSEaseType), data.fadeInEaseType) } : null,
                        fadeOut = data.fadeOutTime > 0f ? new GSFadeInOut{ time = data.fadeOutTime, easeType = (GSEaseType)System.Enum.Parse(typeof(GSEaseType), data.fadeOutEaseType) } : null,
                        autoRelease = data.autoRelease
                    });
                }
            }
        }

        [HorizontalGroup("GoogleSheet/Button")]
        [Button]
        private void Open()
        {
            Application.OpenURL("https://docs.google.com/spreadsheets/d/" + googleSpreadSheetId);
        }

        const string CSV_HEADER = "key,bundleName,assetName,output,loop,volume,delay,playingType,countLimit,fadeInEaseType,fadeInTime,fadeOutEaseType,fadeOutTime,autoRelease";
        const string CSV_FORMAT = "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13}";

        [BoxGroup("Excel")]
        [Button]
        private void ExportToCSV()
        {
            string path = EditorUtility.SaveFilePanel("Export to CSV", "", googleSpreadSheetId + ".csv", "csv");
            if (!string.IsNullOrEmpty(path))
            {
                var variables = GetComponent<Blackboard>().variables;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(CSV_HEADER);
            foreach (var pair in variables)
            {
                var handler = pair.Value.value as GSHandler;
                sb.AppendLine(string.Format(CSV_FORMAT, 
                    pair.Key,
                    handler.clip != null ? handler.clip.bundleName : "",
                    handler.clip != null ? handler.clip.assetName : "",
                    handler.output, 
                    handler.loop, 
                    handler.volume,
                    handler.delay, 
                    handler.playingType, 
                    handler.countLimit, 
                    handler.fadeIn != null ? handler.fadeIn.easeType : GSEaseType.None,
                    handler.fadeIn != null ? handler.fadeIn.time : 0f,
                    handler.fadeOut != null ? handler.fadeOut.easeType : GSEaseType.None,
                    handler.fadeOut != null ? handler.fadeOut.time : 0f,
                    handler.autoRelease));
            }

                FileSystem.SaveTextAsset(path, sb.ToString());
            }
        }
#endif
	}
}
