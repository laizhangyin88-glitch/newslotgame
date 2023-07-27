using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using SlotMaker.Json;
using TMPro;

namespace SlotMaker
{

public class PayTableEditor : EditorWindowBase<PayTableEditor>
{
    private TextAsset json;
    private int totalColumn;
    private int totalRow;

    private GameObject lineSquareObj;
    private Color activeColor;
    private int beginLineNumber;
    private int endLineNumber;

    public override string GetEditorName()
    {
        return "PayTable Editor";
    }

    [MenuItem("SlotMaker/Contents/PayTable Editor", false, 201)]
    private static void Initialize()
    {
        CreateWindow();
    }

    private void OnGUI()
    {
        json = ObjectField("Json", json, typeof(TextAsset), false) as TextAsset;
        totalColumn = IntField("Column", totalColumn);
        totalRow    = IntField("Row", totalRow);
        Space();

        lineSquareObj = ObjectField("Line Square", lineSquareObj, typeof(GameObject), true) as GameObject;
        activeColor = ColorField("Color", activeColor);
        beginLineNumber = IntField("Begin", beginLineNumber);
        endLineNumber = IntField("End", endLineNumber);
        if (Button("Build"))
        {
            var payLines = SlotSimpleJson.DeserializeObject<List<List<int>>>(json.text);
            for (int lineNumber = beginLineNumber - 1; lineNumber < endLineNumber; ++lineNumber)
            {
                var go = GameObject.Instantiate(lineSquareObj) as GameObject;
                go.name = string.Format("Line {0}", lineNumber);
                go.transform.SetParent(Selection.activeTransform, false);

                var text = go.transform.Find("Number").GetComponent<TextMeshProUGUI>();
                text.text = (lineNumber + 1).ToString();

                var squares = go.transform.Find("Squares");

                var payLine = payLines[lineNumber];
                for (int col = 0; col < totalColumn; ++col)
                {
                    squares.GetChild(col).GetChild(payLine[col]).GetComponent<Image>().color = activeColor;
                }
            }
        }
    }
}

}
