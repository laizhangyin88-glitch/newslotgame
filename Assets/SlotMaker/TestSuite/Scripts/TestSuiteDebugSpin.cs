using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NodeCanvas.Framework;
using TMPro;
using SlotMaker.Json;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;

namespace SlotMaker.TestSuite
{
    public class TestSuiteDebugSpin : MonoBehaviour 
    {
        public GameObject anchor;

        public RectTransform debugSelectRect;
        public RectTransform speedRect;
        // public RectTransform filterRect;
        // public RectTransform scrollRect;
        // public RectTransform spinTypeRect;
        // public RectTransform customDebugScrollRect;
        public RectTransform buttonRect;
        // public RectTransform customDebugButtonRect;
        public RectTransform adminDebugRect;
        public RectTransform customDebugRect;

        public Transform contentTransform;
        public Transform spinTypeTransform;
        public Transform customDebugTransform;

        public GameObject buttonPrefab;
        public GameObject spinTypebuttonPrefab;
        public GameObject customDebugInputPrefab;
        public GameObject adminDebugPrefab;
        public GameObject customDebugPrefab;
        public InputField filterInputField;
        public TextMeshProUGUI AdminAndCustomText;

        private int debugSpinType = 0;
        private string HOME_DEBUG_TYPE = "Home";

#if DEV && !NEW_NET
        private List<DebugSpin> debugSpinList;
        private List<DebugSpin> currentDebugSpinList = new List<DebugSpin>();
        private List<GameObject> customDebugList = new List<GameObject>();
        private List<string> spinTypeList = new List<string>();
        private Dictionary<string, GameObject> debugSpinObjects = new Dictionary<string, GameObject>();

        private const float DEFAULT_UI_WIDTH = 200f;
        private const float DEFAULT_UI_EXPANDING_SCALE = 1.4f;
 
        private int debugType = 0;
        private void Awake()
        {
            var game = BlackboardUtils.FindVariable<Blackboard>("./game");
            if (game != null)
            {
                StartCoroutine(DrawDebugSpin());
            }
            float preservedUIScale = PlayerPrefs.GetFloat("TestSuite.DebugSpinUIScale", 1f);
            ResizeUI(DEFAULT_UI_WIDTH, preservedUIScale);
        }

        private IEnumerator DrawDebugSpin()
        {
            string keys = PlayerPrefs.GetString("CustomDebugKeys", "");
            string values = PlayerPrefs.GetString("CustomDebugValues", "");
            if (keys.Equals(""))
            {
                AddCustomDebugInput();
                customDebugList[0].transform.GetChild(0).GetComponent<InputField>().text = "reel_output_list";
            }
            else
            {
                string[] keyList = keys.Split('/');
                string[] valueList = values.Split('/');
                for (int i = 0; i < keyList.Length - 1; i++)
                {
                    string key = keyList[i].Replace(" ","");
                    string value = valueList[i].Replace(" ","");
                    AddCustomDebugInput();
                    customDebugList[i].transform.GetChild(0).GetComponent<InputField>().text = key;
                    customDebugList[i].transform.GetChild(1).GetComponent<InputField>().text = value;
                }
            }
            ClearScrollView();
            var game = BlackboardUtils.FindVariable<Blackboard>("./game");
            int gameId = game.value.GetValue<int>("gameId");
            
            StartCoroutine(TestSuiteManager.Instance.GetDebugSpins(gameId, (ret) => debugSpinList = ret));
            yield return new WaitUntil(() => debugSpinList != null);

            int debugIndex = 1;
            spinTypeList.Add(HOME_DEBUG_TYPE);
            foreach (var debugSpin in debugSpinList)
            {
                currentDebugSpinList.Add(debugSpin);
                GameObject go = GameObject.Instantiate(buttonPrefab) as GameObject;
                go.transform.SetParent(contentTransform, false);

                if (!debugSpin.tag.Equals("") && !spinTypeList.Contains(debugSpin.tag))
                {
                    spinTypeList.Add(debugSpin.tag);
                }

                go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = debugIndex.ToString();
                go.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = debugSpin.title;

                go.GetComponent<Button>().onClick.AddListener(
                    () =>
                    {
                        OnClickDebugSpin(debugSpin.code, debugSpin.DebugSequenceList);
                    }
                );
                debugSpinObjects[debugSpin.title] = go;
                ++debugIndex;
            }

            foreach (var spinType in spinTypeList)
            {
                GameObject go = GameObject.Instantiate(spinTypebuttonPrefab) as GameObject;
                go.transform.SetParent(spinTypeTransform, false);

                go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = spinType.ToString();
                go.GetComponent<Button>().onClick.AddListener(
                    () =>
                    {
                        OnClickSpinType(spinType);
                    }
                );
            }

            filterInputField.text = PlayerPrefs.GetString("DebugSpinFilter", "");

            // scrollRect.gameObject.SetActive(true);
            string debugType = PlayerPrefs.GetString("DebugType", "");
            if (!debugType.Equals(""))
            {
                if (debugType.Equals("admin")) SelectAdminDebug();
                else if (debugType.Equals("custom")) SelectCustomDebug();
            }
            else
            {
                SelectAdminDebug();
            }

            yield return null;
        }

        private void OnClickSpinType(string spinType) {
            currentDebugSpinList.Clear();
            foreach (var debugSpin in debugSpinList)
            {
                var go = debugSpinObjects[debugSpin.title];

                if (spinType.Equals(HOME_DEBUG_TYPE))
                {
                    go.SetActive(true);
                    currentDebugSpinList.Add(debugSpin);
                }
                else
                {
                    if (spinType.Equals(debugSpin.tag))
                    {
                        go.SetActive(true);
                        currentDebugSpinList.Add(debugSpin);
                    }
                    else
                    {
                        go.SetActive(false);
                    }
                }
            }
            OnFilterChanged(filterInputField.text);
        }

        private void OnClickDebugSpin(int code, List<DebugSequence> DebugSequenceList)
        {
            if (DebugSequenceList != null) 
            {
                TestSuiteManager.Instance.Run(DebugSequenceList);
            }

            Close();
        }

        private void ClearScrollView() 
        {
            int childs = contentTransform.childCount;
            for (int i = childs - 1; i >= 0; i--)
            {
                GameObject.Destroy(contentTransform.GetChild(i).gameObject);
            }
        }

        public void OnFilterChanged(string filter)
        {
            PlayerPrefs.SetString("DebugSpinFilter", filter);
            string[] tokens = filter.Split(' ');
            List<Regex> regs = new List<Regex>();
            foreach (var token in tokens)
            {
                regs.Add(new Regex(token, RegexOptions.IgnoreCase | RegexOptions.Compiled));
            }

            foreach (var debugSpin in currentDebugSpinList) 
            {
                var go = debugSpinObjects[debugSpin.title];
                go.SetActive(false);

                if (regs.Count == 0)
                {
                    go.SetActive(true);
                    continue;
                }

                foreach (var reg in regs)
                {
                    if (reg.IsMatch(debugSpin.title))
                    {
                        go.SetActive(true);
                        break;
                    }
                }
            }
        }

        public void AddCustomDebugInput()
        {
            GameObject go = GameObject.Instantiate(customDebugInputPrefab) as GameObject;
            go.transform.SetParent(customDebugTransform, false);
            go.GetComponent<TestSuiteCustomDebugSpin>().index = customDebugList.Count;

            customDebugList.Add(go);
        }

        public void DeleteCustomDebugInput()
        {
            if (customDebugList.Count == 0) return;
            GameObject go = customDebugList[customDebugList.Count - 1];
            customDebugList.Remove(go);
            Destroy(go);
        }

        public void SendCustomDebug()
        {
            string debugParam = "{";
            for (var i = 0; i < customDebugList.Count; i++)
            {
                GameObject customDebug = customDebugList[i];
                string key = customDebug.transform.GetChild(0).GetComponent<InputField>().text;
                string value = customDebug.transform.GetChild(1).GetComponent<InputField>().text;

                if (key.Equals("") || value.Equals(""))
                {
                    continue;
                }
                if (i != 0)
                {
                    debugParam = string.Concat(debugParam, ",");
                }

                key = key.Replace(' ', '_');

                if (value.Contains(",") && !value.StartsWith("["))
                {
                    value = string.Concat("[", value, "]");
                }
                
                debugParam = string.Concat(debugParam, "\"", key, "\"");
                debugParam = string.Concat(debugParam, ":", value);
            }
            debugParam = string.Concat(debugParam, "}");
            Debug.Log(debugParam);
            DebugSequence DebugSequence = new DebugSequence();
            DebugSequence.debugParam = debugParam;

            var DebugSequenceList = new List<DebugSequence>();
            DebugSequenceList.Add(DebugSequence);

            TestSuiteManager.Instance.Run(DebugSequenceList);

            Close();
        }

        private void ResizeUI(float width, float scale)
        {
            float scaledWidth = width * scale;
            speedRect.sizeDelta = new Vector2(scaledWidth, 30f);
            debugSelectRect.sizeDelta = new Vector2(scaledWidth, 30f);
            // filterRect.sizeDelta = new Vector2(scaledWidth, 30f);
            // scrollRect.sizeDelta = new Vector2(scaledWidth, 200f);
            // spinTypeRect.sizeDelta = new Vector2(scaledWidth, 30f);
            // customDebugScrollRect.sizeDelta = new Vector2(scaledWidth, 170f);
            buttonRect.sizeDelta = new Vector2(scaledWidth, 0f);
            // customDebugButtonRect.sizeDelta = new Vector2(scaledWidth, 30f);
            customDebugRect.sizeDelta = new Vector2(scaledWidth, 240f);
            adminDebugRect.sizeDelta = new Vector2(scaledWidth, 240f);
            anchor.transform.localScale = new Vector3(scale, scale, 1f);

            anchor.SetActive(false);
            anchor.SetActive(true);
        }

        public void SelectAdminDebug()
        {
            customDebugPrefab.SetActive(false);
            adminDebugPrefab.SetActive(true);
            PlayerPrefs.SetString("DebugType", "admin");
            AdminAndCustomText.SetText("Custom");
            debugType = 0;
        }

        public void SelectCustomDebug()
        {
            customDebugPrefab.SetActive(true);
            adminDebugPrefab.SetActive(false);
            PlayerPrefs.SetString("DebugType", "custom");
            AdminAndCustomText.SetText("Default");
            debugType = 1;
        }

        public void ToogleDebugType()
        {
            if (debugType == 0)
            {
                SelectCustomDebug();
            }
            else
            {
                SelectAdminDebug();
            }
        }

        public void ToogleWindowSize()
        {
            float desiredScale = anchor.transform.localScale.x != 1 ? 1f : DEFAULT_UI_EXPANDING_SCALE;

            ResizeUI(DEFAULT_UI_WIDTH, desiredScale);
            PlayerPrefs.SetFloat("TestSuite.DebugSpinUIScale", desiredScale);
        }

        public void SetGameSpeed(float gameSpeed)
        {
            Time.timeScale = gameSpeed;
        }

        public void Close()
        {
            GameObject.Destroy(gameObject);
            string keys = "";
            string values = "";
            for (var i = 0; i < customDebugList.Count; i++)
            {
                GameObject customDebug = customDebugList[i];
                string key = customDebug.transform.GetChild(0).GetComponent<InputField>().text;
                string value = customDebug.transform.GetChild(1).GetComponent<InputField>().text;

                key = key.Replace(' ', '_');

                if (value.Contains(",") && !value.StartsWith("["))
                {
                    value = string.Concat("[", value, "]");
                }
                
                keys += key + " /";
                values += value + " /";
            }

            PlayerPrefs.SetString("CustomDebugKeys", keys);
            PlayerPrefs.SetString("CustomDebugValues", values);
        }

        public void ForceClose()
        {
            Close();
            DebugSequence DebugSequence = new DebugSequence();
            DebugSequence.debugParam = "{}";

            var DebugSequenceList = new List<DebugSequence>();
            DebugSequenceList.Add(DebugSequence);

            TestSuiteManager.Instance.Run(DebugSequenceList);
        }

        EventSystem system = EventSystem.current;
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
            {
                if (system.currentSelectedGameObject.name.Equals("Key"))
                {
                    int prevIndex = system.currentSelectedGameObject.GetComponentInParent<TestSuiteCustomDebugSpin>().index - 1;
                    if (prevIndex < 0)
                    {
                        return ;
                    }
                    customDebugList[prevIndex].GetComponent<TestSuiteCustomDebugSpin>().value.Select();
                }
                else
                {
                    Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnLeft();
                    if (next != null)
                    {
                        next.Select();
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (system.currentSelectedGameObject.name.Equals("Value"))
                {
                    int nextIndex = system.currentSelectedGameObject.GetComponentInParent<TestSuiteCustomDebugSpin>().index + 1;
                    if (customDebugList.Count == nextIndex)
                    {
                        AddCustomDebugInput();
                    }
                    customDebugList[nextIndex].GetComponent<TestSuiteCustomDebugSpin>().key.Select();
                }
                else
                {
                    Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnRight();
                    if (next != null)
                    {
                        next.Select();
                    }
                }
            }
        }
#endif
    }
}
