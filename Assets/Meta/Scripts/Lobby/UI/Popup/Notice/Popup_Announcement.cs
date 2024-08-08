using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;

public class Popup_Announcement : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textCom1;
    [SerializeField] private TextMeshProUGUI _textCom2;
    [SerializeField] private Button _closeBtnCom;


    public List<NoticeData> NoticeDatas => MainBlackboard.Get().GetValue<List<NoticeData>>("notices");
    public List<NoticeData> NoticeDatasIsSystem => NoticeDatas?.FindAll((item) => item.status != 0 && item.is_system != 0);
    public List<NoticeData> NoticeDatasNotSystem => NoticeDatas?.FindAll((item) => item.status != 0 && item.is_system == 0);

    // Start is called before the first frame update
    void Start()
    {
        _textCom1.text = GenerateNoticeTextByDataList(NoticeDatasNotSystem);
        _textCom2.text = GenerateNoticeTextByDataList(NoticeDatasIsSystem);
        _closeBtnCom.onClick.AddListener(() =>
        {
            PopupManager.Instance.Close();
            Destroy(this.gameObject);
        });
    }

    private string GenerateNoticeTextByDataList(List<NoticeData> noticeDatas)
    {
        if (noticeDatas == null || noticeDatas.Count <= 0)
            return null;

        StringBuilder ret = new StringBuilder();
        noticeDatas.ForEach((nd) =>
        {
            ret.AppendLine($"<b>{nd.title}</b>");
            ret.AppendLine(nd.content);
            ret.AppendLine();
        });

        return ret.ToString();
    }
}
