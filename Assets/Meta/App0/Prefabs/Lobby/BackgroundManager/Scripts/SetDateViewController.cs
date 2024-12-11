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
        SetCurrentSelectTime(_Calendar.GetLastSelect());
        _Calendar.OnClickConfirmEvent += OnClickSaveBtn;
    }
    private void OnClickButtonClose()
    {
        Destroy(gameObject);
    }

    private void OnClickSaveBtn()
    {
        string date = _Calendar.GetLastSelectDate();
        if (!string.IsNullOrEmpty(date))
        {
            string[] temps = date.Split('-');
            int year = int.Parse(temps[0]);
            int month = int.Parse(temps[1]);
            int day = int.Parse(temps[2]); 
            int hour = GetNumber(_Calendar.GetHourTxt());
            int min = GetNumber(_Calendar.GetMinuteTxt());
            AndroidSystemHelper.Instance.SetSystemTime(year, month, day, hour, min, 0);
        }
        Destroy(gameObject);
    }

    private int GetNumber(string value)
    {
        if(value.StartsWith("0"))
        {
            return int.Parse(value[1].ToString());
        }
        else
        {
            return int.Parse(value);
        }
    }

    private void Update() 
    {
        SetCurrentSelectTime(_Calendar.GetLastSelect());
    }

    private void SetCurrentSelectTime(string value)
    {
        _CurrentSelectTime.text = string.Format("Select Date And Time: {0}", value);
    }
}
