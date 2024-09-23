using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using BagelCode.Slots.TRR.Utillity;

public class ButtonCommandPage : MonoBehaviour
{
    public const string ConfigPrefixFg = "free";
    public const string ConfigPrefixBonus = "bonus";
    public const string ConfigPrefixJP = "jp";
    public const string CommandKeyCode = "code";
    public const string CommandKeyList = "list";

    [Title("配置")]
    [SerializeField] private Color fgColor;
    [SerializeField] private Color bonusColor;
    [SerializeField] private Color jpColor;
    [SerializeField] private string fgTitleText = "免费游戏";
    [SerializeField] private string bonusTitleText = "奖金小游戏";
    [SerializeField] private string jpTitleText = "彩金游戏";

    [Title("对象引用")]
    [SerializeField] private TextMeshProUGUI _titleRef;
    [SerializeField] private Button _btnRef;

    private void OnEnable()
    {
        //活动时刷新数据
        UpdateCommand(TestManager.Instance.CurGameGmConfig);
    }


    /// <summary>
    /// 根据gmConfig数据更新命令行按钮的显示
    /// </summary>
    /// <param name="gmConfig"></param>
    public void UpdateCommand(JSONNode gmConfig)
    {
        //test功能是关闭的，对应的是展示包，不会执行任何操作
        if (TestManager.Instance.transform.parent.gameObject.activeSelf == false)
            return;

        //没有切到当前页时，不刷新显示
        if (gameObject.activeSelf == false)
            return;

        //不论有没有数据都先清除显示
        transform.RemoveAllChildren();

        if(gmConfig == null || gmConfig.IsNull)
        {
            var titleCom = Instantiate(_titleRef, transform);
            titleCom.color = Color.white;
            titleCom.text = "无指令";
            titleCom.gameObject.SetActive(true);
            return;
        }

        List<string> keyListBg = new List<string>();
        List<string> keyListBonus = new List<string>();
        List<string> keyListJp = new List<string>();
        foreach (var item in gmConfig.Keys)
        {
            if (item.StartsWith(ConfigPrefixFg))
               keyListBg.Add(item);
            else if(item.StartsWith(ConfigPrefixBonus))
                keyListBonus.Add(item);
            else if(item.StartsWith(ConfigPrefixJP))
                keyListJp.Add(item);
        }

        UpdateCommandByType(fgColor, fgTitleText, keyListBg, gmConfig);
        UpdateCommandByType(bonusColor, bonusTitleText, keyListBonus, gmConfig);
        UpdateCommandByType(jpColor, jpTitleText, keyListJp, gmConfig);
    }

    private void UpdateCommandByType(Color color, string titleText, List<string> keys, JSONNode gmConfig)
    {
        if (keys == null || keys.Count <= 0)
            return;

        var titleCom = Instantiate(_titleRef, transform);
        titleCom.color = color;
        titleCom.text = titleText;
        titleCom.gameObject.SetActive(true);

        foreach (var key in keys)
        {
            JSONNode curNode = gmConfig[key];
            bool isCode = curNode.HasKey(CommandKeyCode);
            JSONNode commandValueNode = curNode[isCode ? CommandKeyCode : CommandKeyList];
            string commandValue = commandValueNode.ToString().Replace("[", "").Replace("]", "");

            var btnCom = Instantiate(_btnRef, transform);
            var textCom = btnCom.GetComponentInChildren<TextMeshProUGUI>();
            textCom.color = color;
            textCom.text = key;
            btnCom.onClick.AddListener(() =>
            {
                TestManager.Instance.SetCode(isCode ? commandValue : "");
                TestManager.Instance.SetList(isCode ? "" : commandValue);
            });

            btnCom.gameObject.SetActive(true);
        }
    }
}
