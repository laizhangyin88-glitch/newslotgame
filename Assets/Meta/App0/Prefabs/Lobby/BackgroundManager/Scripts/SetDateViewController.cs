using SpringGUI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetDateViewController : MonoBehaviour
{
    private Calendar _Calendar;
    private Button ButtonClose;

    private TextMeshProUGUI _CurrentSelectTime;

    private void Start()
    {
        _Calendar = transform.Find("Image/calendar").GetComponent<Calendar>();
        ButtonClose = transform.Find("Image/ButtonClose").GetComponent<Button>();
        ButtonClose.onClick.AddListener(OnClickButtonClose);
        _CurrentSelectTime = transform.Find("Image/content/CurrentSelectTime").GetComponent<TextMeshProUGUI>();
        SetCurrentSelectTime(_Calendar.GetCalendarValue());
        _Calendar.OnClickConfirmEvent += OnClickButtonClose;
    }
    private void OnClickButtonClose()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        SetCurrentSelectTime(_Calendar.GetCalendarValue());
    }

    private void SetCurrentSelectTime(string value)
    {
        _CurrentSelectTime.text = string.Format("Select Date And Time: {0}", value);
    }
}
