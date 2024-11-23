
/*=========================================
* Author: springDong
* Description: SpringGUI.Calendar
* This component you only need to listen onDayClick/onMonthClick/onYearClick three interfaces
* Interface return CurrentDateTime class data.
==========================================*/

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.Events;
using Object = UnityEngine.Object;
using TMPro;

namespace SpringGUI
{

    public class Calendar : UIBehaviour
    {
        private int hourValue;
        public int HourValue { get => hourValue; set => hourValue = value; }


        private int minuteValue;

        public int MinuteValue { get => minuteValue; set => minuteValue = value; }

        #region click events
        public class DayClickEvent : UnityEvent<DateTime> { }
        public class MonthClickEvent : UnityEvent<DateTime> { }
        public class YearClickEvent : UnityEvent<DateTime> { }

        private DayClickEvent m_onDayClickEvent = new DayClickEvent();
        public DayClickEvent onDayClick
        {
            get { return m_onDayClickEvent; }
            set { m_onDayClickEvent = value; }
        }
        private MonthClickEvent m_onMonthClickEvent = new MonthClickEvent();
        public MonthClickEvent onMonthClick
        {
            get { return m_onMonthClickEvent; }
            set { m_onMonthClickEvent = value; }
        }
        private YearClickEvent m_onYearClickEvent = new YearClickEvent();
        public YearClickEvent onYearClick
        {
            get { return m_onYearClickEvent; }
            set { m_onYearClickEvent = value; }
        }

        public Action OnClickConfirmEvent;

        #endregion

        #region private && public members
        private DateTime m_selectDT = DateTime.Today;
        private readonly CalendarData m_calendarData = new CalendarData();
        private Transform m_Transform = null;
        [HideInInspector]
        public E_CalendarType CalendarType = E_CalendarType.Day;
        public E_DisplayType DisplayType = E_DisplayType.Chinese;
        private TextMeshProUGUI _timeButtonText = null;
        private GameObject _weeksGameObject = null;
        private GameObject _daysGameObejct = null;
        private GameObject _monthGameObject = null;
        private readonly List<DMY> _daysPool = new List<DMY>();
        private readonly List<DMY> _monthYearPool = new List<DMY>();
        private Button hourBtnAdd;
        private Button hourBtnReduce;
        private TextMeshProUGUI hourTxt;
        private Button minuteBtnAdd;
        private Button minuteBtnReduce;
        private TextMeshProUGUI minuteTxt;
        private Button bgBtn;
        private Button BtnConfirm;
        #endregion

        protected override void Awake()
        {
            m_Transform = transform;
            _timeButtonText = m_Transform.Find("Title/TimeButton/Text").GetComponent<TextMeshProUGUI>();
            _weeksGameObject = m_Transform.Find("Container/Weeks").gameObject;
            _daysGameObejct = m_Transform.Find("Container/Days").gameObject;
            _monthGameObject = m_Transform.Find("Container/Months").gameObject;
            var weekPrefab = _weeksGameObject.transform.Find("WeekTemplate").gameObject;
            var dayPrefab = _daysGameObejct.transform.Find("DayTemplate").gameObject;
            var monthPrefab = _monthGameObject.transform.Find("MonthTemplate").gameObject;
            WeekGenerator(weekPrefab, _weeksGameObject.transform);
            DayGenerator(dayPrefab, _daysGameObejct.transform);
            MonthGenerator(monthPrefab, _monthGameObject.transform);
            m_Transform.Find("Title/NextButton").GetComponent<Button>().onClick.AddListener(OnNextButtonClick);
            m_Transform.Find("Title/LastButton").GetComponent<Button>().onClick.AddListener(OnLastButtonClick);
            m_Transform.Find("Title/TimeButton").GetComponent<Button>().onClick.AddListener(OnTimeButtonClick);
            hourBtnAdd = transform.Find("Time/Image/ButtonAdd").GetComponent<Button>();
            hourBtnReduce = transform.Find("Time/Image/ButtonReduce").GetComponent<Button>();
            minuteBtnAdd = transform.Find("Time/Image1/ButtonAdd").GetComponent<Button>();
            minuteBtnReduce = transform.Find("Time/Image1/ButtonReduce").GetComponent<Button>();
            hourTxt = transform.Find("Time/Image/HourTxt").GetComponent<TextMeshProUGUI>();
            minuteTxt = transform.Find("Time/Image1/MinutesTxt").GetComponent<TextMeshProUGUI>();
            bgBtn = transform.Find("bgBtn")?.GetComponent<Button>();
            BtnConfirm = transform.Find("Confirm").GetComponent<Button>();
            if (bgBtn != null)
            {
                bgBtn.onClick.AddListener(OnClickBgBtn); 
            }
            BtnConfirm.onClick.AddListener(OnClickConfirm);
            hourBtnAdd.onClick.AddListener(OnClickHourAddBtn);
            hourBtnReduce.onClick.AddListener(OnClickHourReduceBtn);
            minuteBtnAdd.onClick.AddListener(OnClickMinuteAddBtn);
            minuteBtnReduce.onClick.AddListener(OnClickMinuteReduceBtn);
            ResetShow();
        }

        private void ResetShow()
        {
            var now = DateTime.Now;
            HourValue = now.Hour;
            MinuteValue = now.Minute;
            hourTxt.text = now.Hour < 10 ? "0" + now.Hour : now.Hour.ToString();
            minuteTxt.text = now.Minute < 10 ? "0" + now.Minute : now.Minute.ToString();
            Refresh();
        }

        private void OnClickHourAddBtn()
        {
            SetHourTxt(1);
        }

        private void OnClickMinuteAddBtn()
        {
            SetMinuteTxt(1);
        }
        private void OnClickHourReduceBtn()
        {
            SetHourTxt(-1);
        }

        private void OnClickMinuteReduceBtn()
        {
            SetMinuteTxt(-1);
        }

        private void SetHourTxt(int value)
        {
            HourValue += value;
            if (HourValue < 0)
            {
                HourValue = 23;
            }
            if (HourValue > 23)
            {
                HourValue = 0;
            }
            hourTxt.text = HourValue < 10 ? "0" + HourValue : HourValue.ToString();
        }

        private void SetMinuteTxt(int value)
        {
            MinuteValue += value;
            if (MinuteValue < 0)
            {
                MinuteValue = 59;
            }
            if (MinuteValue > 59)
            {
                MinuteValue = 0;
            }
            minuteTxt.text = MinuteValue < 10 ? "0" + MinuteValue : MinuteValue.ToString();
        }

        private void OnClickConfirm()
        {
            if (OnClickConfirmEvent != null)
            {
                OnClickConfirmEvent();
            }
            gameObject.SetActive(false);
        }

        private void OnClickBgBtn()
        {
            this.gameObject.SetActive(false);
        }

        #region operation functions
        private void OnTimeButtonClick()
        {
            if (CalendarType == E_CalendarType.Month)
                CalendarType = E_CalendarType.Year;
            if (CalendarType == E_CalendarType.Day)
            {
                CalendarType = E_CalendarType.Month;
                calendarTypeChange(false);
            }
            Refresh();
        }
        private void OnNextButtonClick()
        {
            //SetDayColor();
            if (CalendarType == E_CalendarType.Day)
                m_selectDT = m_selectDT.AddMonths(1);
            else if (CalendarType == E_CalendarType.Month)
                m_selectDT = m_selectDT.AddYears(1);
            else
                m_selectDT = m_selectDT.AddYears(12);
            Refresh();
        }
        private void OnLastButtonClick()
        {
            //SetDayColor();
            if (CalendarType == E_CalendarType.Day)
                m_selectDT = m_selectDT.AddMonths(-1);
            else if (CalendarType == E_CalendarType.Month)
                m_selectDT = m_selectDT.AddYears(-1);
            else
                m_selectDT = m_selectDT.AddYears(-12);
            Refresh();
        }
        #endregion

        #region days && weeks && months generator
        private void WeekGenerator(GameObject weekPrefab, Transform parent)
        {
            for (int i = 0; i < 7; i++)
            {
                GameObject week = prefabGenerator(weekPrefab, parent);
                week.GetComponent<TextMeshProUGUI>().text = getWeekName(i.ToString());
            }
            Destroy(weekPrefab);
        }
        private void DayGenerator(GameObject dayPrefab, Transform parent)
        {
            for (int i = 0; i < 42; i++)
            {
                GameObject day = prefabGenerator(dayPrefab, parent);
                DMY dmy = day.AddComponent<DMY>();
                day.GetComponent<Button>().onClick.AddListener(() =>
                {
                    if (dmy.GetTxtColor() == Color.gray) return;
                    m_selectDT = dmy.CurrentDateTime;
                    onDayClick.Invoke(dmy.CurrentDateTime);
                    Refresh();
                    SetDayColor(dmy);
                });
                _daysPool.Add(dmy);
            }
            Destroy(dayPrefab);
        }

        private void SetDayColor(DMY dMY = null)
        {
            for (int i = 0; i < _daysPool.Count; i++)
            {

                if (dMY != null && _daysPool[i] == dMY)
                {
                    _daysPool[i].SetColor(new Color(1, 0, 1, 1));
                }
                else
                {
                    _daysPool[i].SetColor(new Color(0.3f, 0.3f, 0.3f, 1));
                }
            }
        }

        private void MonthGenerator(GameObject monthPrefab, Transform parent)
        {
            for (int i = 0; i < 12; i++)
            {
                GameObject month = prefabGenerator(monthPrefab, parent);
                DMY dmy = month.AddComponent<DMY>();
                month.GetComponent<Button>().onClick.AddListener(() =>
                {
                    m_selectDT = dmy.CurrentDateTime;
                    if (CalendarType == E_CalendarType.Month)
                    {
                        CalendarType = E_CalendarType.Day;
                        calendarTypeChange(true);
                        onMonthClick.Invoke(dmy.CurrentDateTime);
                    }
                    if (CalendarType == E_CalendarType.Year)
                    {
                        CalendarType = E_CalendarType.Month;
                        onYearClick.Invoke(dmy.CurrentDateTime);
                    }
                    Refresh();
                });
                _monthYearPool.Add(dmy);
            }
            Destroy(monthPrefab);
        }
        private GameObject prefabGenerator(GameObject prefab, Transform parent)
        {
            GameObject go = Object.Instantiate(prefab);
            go.transform.SetParent(parent);
            go.transform.localScale = Vector3.one;
            return go;
        }
        private string getWeekName(string weekName)
        {
            switch (DisplayType)
            {
                case E_DisplayType.Standard:
                    switch (weekName)
                    {
                        case "0":
                            return "Sun";
                        case "1":
                            return "Mon";
                        case "2":
                            return "Tue";
                        case "3":
                            return "Wed";
                        case "4":
                            return "Thu";
                        case "5":
                            return "Fri";
                        case "6":
                            return "Sat";
                        default:
                            return "";
                    }
                case E_DisplayType.Chinese:
                    switch (weekName)
                    {
                        case "0":
                            return "日";
                        case "1":
                            return "一";
                        case "2":
                            return "二";
                        case "3":
                            return "三";
                        case "4":
                            return "四";
                        case "5":
                            return "五";
                        case "6":
                            return "六";
                        default:
                            return "";
                    }
                default:
                    return "";
            }

        }
        private void calendarTypeChange(bool isDays)
        {
            _weeksGameObject.SetActive(isDays);
            _daysGameObejct.SetActive(isDays);
            _monthGameObject.SetActive(!isDays);
        }
        #endregion

        #region refresh calendar all component
        private void Refresh()
        {
            RefreshCalendar();
            RefreshTimeButtonContent();
        }
        private void RefreshTimeButtonContent()
        {
            switch (CalendarType)
            {
                case E_CalendarType.Day:
                    if (DisplayType == E_DisplayType.Standard) _timeButtonText.text = m_selectDT.ToString("yyyy-MM-dd");
                    else _timeButtonText.text = m_selectDT.Year + "年" + m_selectDT.Month + "月" + m_selectDT.Day + "日";
                    break;
                case E_CalendarType.Month:
                    if (DisplayType == E_DisplayType.Standard) _timeButtonText.text = m_selectDT.Year + "-" + m_selectDT.Month;
                    else _timeButtonText.text = m_selectDT.Year + "年" + m_selectDT.Month + "月";
                    break;
                case E_CalendarType.Year:
                    if (DisplayType == E_DisplayType.Standard) _timeButtonText.text = m_selectDT.Year.ToString();
                    else _timeButtonText.text = m_selectDT.Year + "年";
                    break;
            }
        }

        public string GetCalendarValue()
        {
            return string.Format("{0} {1}:{2}", _timeButtonText.text, hourTxt.text, minuteTxt.text);
        }

        public string GetDateTxt()
        {
            return _timeButtonText.text;
        }
        public string GetHourTxt()
        {
            return hourTxt.text;
        }

        public string GetMinuteTxt()
        {
            return minuteTxt.text;
        }

        private void RefreshCalendar()
        {
            if (CalendarType == E_CalendarType.Day) RefreshDays(m_calendarData.Days(m_selectDT));
            else if (CalendarType == E_CalendarType.Month) RefreshMonths(m_calendarData.Months(m_selectDT));
            else RefreshYears(m_calendarData.Years(m_selectDT));
        }
        private void RefreshDays(List<DateTime> dateTimes)
        {
            for (int i = 0; i < _daysPool.Count; i++)
            {
                var fontColor = Color.white;
                if (dateTimes[i].Month != m_selectDT.Month)
                    fontColor = Color.gray;
                _daysPool[i].SetDay(dateTimes[i], DisplayType, fontColor);
            }
        }
        private void RefreshMonths(List<DateTime> dateTimes)
        {
            for (int i = 0; i < _monthYearPool.Count; i++)
                _monthYearPool[i].SetMonth(dateTimes[i], DisplayType);
        }
        private void RefreshYears(List<DateTime> dateTimes)
        {
            for (int i = 0; i < _monthYearPool.Count; i++)
                _monthYearPool[i].SetYear(dateTimes[i], DisplayType);
        }
        #endregion
    }

    public enum E_CalendarType
    {
        Day,
        Month,
        Year
    }
    public enum E_DisplayType
    {
        Standard,
        Chinese
    }
    public class CalendarData
    {
        public List<DateTime> Days(DateTime month)
        {
            List<DateTime> days = new List<DateTime>();
            DateTime firstDay = new DateTime(month.Year, month.Month, 1);
            DayOfWeek week = firstDay.DayOfWeek;
            int lastMonthDays = (int)week;
            if (lastMonthDays.Equals(0))
                lastMonthDays = 7;
            for (int i = lastMonthDays; i > 0; i--)
                days.Add(firstDay.AddDays(-i));
            for (int i = 0; i < 42 - lastMonthDays; i++)
                days.Add(firstDay.AddDays(i));
            return days;
        }
        public List<DateTime> Months(DateTime year)
        {
            List<DateTime> months = new List<DateTime>();
            DateTime firstMonth = new DateTime(year.Year, 1, 1);
            months.Add(firstMonth);
            for (int i = 1; i < 12; i++)
                months.Add(firstMonth.AddMonths(i));
            return months;
        }
        public List<DateTime> Years(DateTime year)
        {
            List<DateTime> years = new List<DateTime>();
            for (int i = 5; i > 0; i--)
                years.Add(year.AddYears(-i));
            for (int i = 0; i < 7; i++)
                years.Add(year.AddYears(i));
            return years;
        }
    }
    public class DMY : UIBehaviour
    {
        public DateTime CurrentDateTime { get; set; }
        private TextMeshProUGUI _text = null;
        private Image _image;
        private bool isInit = false;
        protected override void Awake()
        {
            _text = transform.Find("Text").GetComponent<TextMeshProUGUI>();
            _image = transform.GetComponent<Image>();
            isInit = false;
        }
        public void SetDay(DateTime dateTime, E_DisplayType displayType, Color fontColor)
        {
            CurrentDateTime = dateTime;
            if (!isInit)
            {
                if (dateTime.Day == DateTime.Now.Day)
                {
                    SetColor(new Color(1, 0, 1, 1));
                }
                else
                {
                    SetColor(new Color(0.3f, 0.3f, 0.3f, 1));
                }
                isInit = true;
            }
            _text.text = dateTime.Day.ToString();
            _text.color = fontColor;
        }
        public void SetMonth(DateTime dateTime, E_DisplayType displayType)
        {
            CurrentDateTime = dateTime;
            _text.text = getMonthString(dateTime.Month.ToString(), displayType);
        }
        public void SetYear(DateTime dateTime, E_DisplayType displayType)
        {
            CurrentDateTime = dateTime;
            _text.text = dateTime.Year.ToString();
        }
        private string getMonthString(string month, E_DisplayType displayType)
        {
            switch (displayType)
            {
                case E_DisplayType.Standard:
                    switch (month)
                    {
                        case "1":
                            return "Jan.";
                        case "2":
                            return "Feb.";
                        case "3":
                            return "Mar.";
                        case "4":
                            return "Apr.";
                        case "5":
                            return "May.";
                        case "6":
                            return "Jun.";
                        case "7":
                            return "Jul.";
                        case "8":
                            return "Aug.";
                        case "9":
                            return "Sept.";
                        case "10":
                            return "Oct.";
                        case "11":
                            return "Nov.";
                        case "12":
                            return "Dec.";
                        default:
                            return "";
                    }
                case E_DisplayType.Chinese:
                    switch (month)
                    {
                        case "1":
                            return "一月";
                        case "2":
                            return "二月";
                        case "3":
                            return "三月";
                        case "4":
                            return "四月";
                        case "5":
                            return "五月";
                        case "6":
                            return "六月";
                        case "7":
                            return "七月";
                        case "8":
                            return "八月";
                        case "9":
                            return "九月";
                        case "10":
                            return "十月";
                        case "11":
                            return "十一月";
                        case "12":
                            return "十二月";
                        default:
                            return "";
                    }
                default:
                    return "";
            }
        }

        public void SetColor(Color color)
        {
            _image.color = color;
        }

        public Color GetTxtColor()
        {
            return _text.color;
        }
    }
}
