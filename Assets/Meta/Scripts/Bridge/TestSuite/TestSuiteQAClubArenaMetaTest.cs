using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using TMPro;
using ParadoxNotion;

namespace BagelCode
{
    public class TestSuiteQAClubArenaMetaTest : MonoBehaviour
    {
        public Transform contentTransform;

        public TMP_InputField targetUserIdInputFilter;
        public TMP_InputField targetAliasInputFilter;

        public GameObject buttonPrefab;

        private Dictionary<string, string> dicData = new Dictionary<string, string>(); // userId, alias

        private readonly string CLUB_ARENA_META_DEBUG_TEST_KEY = "ClubArenaDebugData";

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            string debugDatas = PlayerPrefs.GetString(CLUB_ARENA_META_DEBUG_TEST_KEY, "");
            if (string.IsNullOrEmpty(debugDatas))
                return;

            string[] splitData = debugDatas.Split(';');
            if (splitData == null)
                return;

            for (int i = 0; i < splitData.Length; ++i)
            {
                string[] items = splitData[i].Split(':');
                if (items != null && items.Length == 2)
                    dicData.Add(items[0], items[1]);
            }

            InitItems();
        }

        private void InitItems()
        {
            CreateItems();
        }

        private void CreateItems()
        {
            if (contentTransform == null)
                return;

            ClearScrollView();

            int debugIndex = 1;
            foreach (var item in dicData)
            {
                GameObject go = GameObject.Instantiate(buttonPrefab) as GameObject;
                go.name = item.Key;
                go.transform.SetParent(contentTransform, false);

                go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = debugIndex.ToString();
                go.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.Value;
                go.GetComponent<Button>().onClick.AddListener(
                    () =>
                    {
                        OnClickItem(item.Key);
                    }
                );
                ++debugIndex;
            }
        }

        private void RefreshItems()
        {
            CreateItems();
            SaveData();
        }

        private void ClearScrollView()
        {
            int childs = contentTransform.childCount;
            for (int i = childs - 1; i >= 0; i--)
            {
                Destroy(contentTransform.GetChild(i).gameObject);
            }
        }

        private void SaveData()
        {
            if (dicData == null)
                return;
            string saveData = "";
            foreach (var item in dicData)
                saveData += item.Key + ":" + item.Value + ";";

            if (saveData.Length > 0)
                saveData.Substring(0, saveData.Length - 1);
            PlayerPrefs.SetString(CLUB_ARENA_META_DEBUG_TEST_KEY, saveData);
        }

        private void Close()
        {
            Destroy(gameObject);
        }

        private void OnClickItem(string userId)
        {
            targetUserIdInputFilter.text = userId;
            targetAliasInputFilter.text = dicData[userId];
        }

        public void OnClickApply()
        {
            if (targetUserIdInputFilter.text.Length == 36)
                ClubArenaUtils.TargetUserId = targetUserIdInputFilter.text;
            Close();
        }

        public void OnClickClose()
        {
            Close();
        }

        public void OnClickAdd()
        {
            if (dicData.ContainsKey(targetUserIdInputFilter.text))
                dicData[targetUserIdInputFilter.text] = GetTextTargetAlias();
            else
                dicData.Add(targetUserIdInputFilter.text, GetTextTargetAlias());

            RefreshItems();
        }

        public void OnClickRemove()
        {
            if (dicData.ContainsKey(targetUserIdInputFilter.text))
            {
                dicData.Remove(targetUserIdInputFilter.text);

                RefreshItems();
            }

            targetUserIdInputFilter.text = "";
            targetAliasInputFilter.text = "";
        }

        private string GetTextTargetAlias()
        {
            if (targetAliasInputFilter == null || string.IsNullOrEmpty(targetAliasInputFilter.text))
                return " ";
            return targetAliasInputFilter.text;
        }
    }
}