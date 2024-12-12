using System;
using UnityEngine;
using UnityEngine.UI;

namespace InnerKeyboard
{

    //键盘参数类
    public class KeyboardParam
    {
        public string InputStr;
        public string OutputStr;

        public KeyboardParam(string InStr, string OutStr = "")
        {
            InputStr = InStr;
            OutputStr = OutStr;
        }
    }

    //委托事件类
    public class EventCommon
    {

        public delegate void CallBack<T>(T para);

        public delegate void NorEvent();
    }

    public class Keyboard : MonoBehaviour
    {
        private RectTransform KeyboardWindow;
        public GameObject ComBtnPref;
        private Transform Line0, Line1, Line2, Line3;
        private Button BackSpaceBtn, ShiftBtn, SpaceBtn, CancelBtn, EnterBtn, LangugeBtn, ClearBtn;
        private Image ShiftBG;

        private string[][] Line0_KeyValue = {
            new string[]{"`","1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "-", "="},
            new string[]{"·", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "_", "+" }};

        private string[][] Line1_KeyValue = {
            new string[]{"q","w", "e", "r", "t", "y", "u", "i", "o", "p", "[", "]", "\\"},
            new string[]{"Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "{", "}", "|" }};

        private string[][] Line2_KeyValue = {
            new string[]{"a","s", "d", "f", "g", "h", "j", "k", "l", ";", "'"},
            new string[]{"A", "S", "D", "F", "G", "H", "J", "K", "L", ":", "\""}};

        private string[][] Line3_KeyValue = {
            new string[]{"z","x", "c", "v", "b", "n", "m", ",", ".", "/"},
            new string[]{"Z", "X", "C", "V", "B", "N", "M", "<", ">", "?"}};


        private string NewEditeString = "";

        [HideInInspector]
        public bool isShift = false;
        private bool isShiftLock = false;
        private float ShiftTime = 0f;
        private Color32 LockColor = new Color32(127, 171, 179, 255);


        private event EventCommon.NorEvent OnShiftOn = null;
        private event EventCommon.NorEvent OnShiftOff = null;

        private KeyboardParam KeyboardPara = null;//键盘参数

        
        private EventCommon.CallBack<KeyboardParam> call = null; //回调函数

        private static Keyboard instance = null;

        public Action CancelBtnEvent;

        public Action ConfirmBtnEvent;

        public static Keyboard Instance
        {
            get { return instance; }
        }

        //Awake
        private void Awake()
        {
            KeyboardWindow = this.transform.GetComponent<RectTransform>();
            //ComBtnPref = Resources.Load<GameObject>("KeyItem");
            Line0 = KeyboardWindow.Find("KB_BG/KeyBtns/ComBtnLine0");
            Line1 = KeyboardWindow.Find("KB_BG/KeyBtns/ComBtnLine1");
            Line2 = KeyboardWindow.Find("KB_BG/KeyBtns/ComBtnLine2");
            Line3 = KeyboardWindow.Find("KB_BG/KeyBtns/ComBtnLine3");

            BackSpaceBtn = KeyboardWindow.Find("KB_BG/KeyBtns/BtnLine3/BackSpaceBtn").GetComponent<Button>();
            ShiftBtn = KeyboardWindow.Find("KB_BG/KeyBtns/BtnLine3/ShiftBtn").GetComponent<Button>();
            SpaceBtn = KeyboardWindow.Find("KB_BG/KeyBtns/BtnLine4/SpaceBtn").GetComponent<Button>();
            CancelBtn = KeyboardWindow.Find("KB_BG/KeyBtns/BtnLine4/right/CancelBtn").GetComponent<Button>();
            EnterBtn = KeyboardWindow.Find("KB_BG/KeyBtns/BtnLine4/right/EnterBtn").GetComponent<Button>();
            LangugeBtn = KeyboardWindow.Find("KB_BG/KeyBtns/BtnLine4/left/LangugeBtn").GetComponent<Button>();
            ClearBtn = KeyboardWindow.Find("KB_BG/KeyBtns/BtnLine4/left/ClearBtn").GetComponent<Button>();

            ShiftBG = ShiftBtn.GetComponent<Image>();
        }

        void Start()
        {
            KeyboardWindow.localScale = Vector3.zero;
            InitComBtn();

            BackSpaceBtn.onClick.AddListener(ClickBackSpace);
            ShiftBtn.onClick.AddListener(ClickShift);
            SpaceBtn.onClick.AddListener(ClickSpace);
            CancelBtn.onClick.AddListener(ClickCancel);
            EnterBtn.onClick.AddListener(ClickEnter);
            LangugeBtn.onClick.AddListener(ClickLanguage);
            ClearBtn.onClick.AddListener(ClickClear);

            instance = this;
        }


        //初始化按钮
        private void InitComBtn()
        {
            InstantLineComBtns(Line0, Line0_KeyValue);
            InstantLineComBtns(Line1, Line1_KeyValue);
            InstantLineComBtns(Line2, Line2_KeyValue);
            InstantLineComBtns(Line3, Line3_KeyValue);
        }

        //按行实例化按钮
        private void InstantLineComBtns(Transform LineTran,string[][] KeyValues)
        {
            for (int i = 0; i < KeyValues[0].Length; i++)
            {
                GameObject TempObj = GameObject.Instantiate<GameObject>(ComBtnPref);
                TempObj.transform.SetParent(LineTran);
                TempObj.transform.localScale = Vector3.one;
                ComBtn comBtnCtrl = TempObj.GetComponent<ComBtn>();
                if (comBtnCtrl != null)
                {
                    comBtnCtrl.SetKeyValue(KeyValues[0][i], KeyValues[1][i]);
                    OnShiftOn += comBtnCtrl.OnShiftOn;
                    OnShiftOff += comBtnCtrl.OnShiftOff;
                } 
            }
        }

        //输入内容追加
        public void AddComBtnString(string str)
        {
            NewEditeString += str;
            if (KeyboardPara != null)
                KeyboardPara.OutputStr = NewEditeString;
            call?.Invoke(KeyboardPara);
            if (isShift && !isShiftLock)
            {
                isShift = false;
                ShiftBG.color = new Color(128, 128, 128, 255);
                OnShiftOff();
            }
        }

  
        // 唤起键盘
        public void ShowKeyboard(KeyboardParam para, EventCommon.CallBack<KeyboardParam> call)
        {
            KeyboardPara = para;
            NewEditeString = KeyboardPara.InputStr;

            KeyboardWindow.localScale = Vector3.one;
            if (call != null)
                this.call = call;
        }

        //语言点击事件
        private void ClickLanguage()
        {

        }

        //取消点击事件
        private void ClickCancel()
        {
            NewEditeString = ""; 
            if (KeyboardPara != null)
                KeyboardPara.OutputStr = KeyboardPara.InputStr;
            call?.Invoke(KeyboardPara);
            KeyboardPara = null;
            //KeyboardWindow.localScale = Vector3.zero;
            if(CancelBtnEvent != null)
            {
                CancelBtnEvent();
            }
        }

        //确认点击事件
        private void ClickEnter()
        {
            if (KeyboardPara != null)
                KeyboardPara.OutputStr = NewEditeString;
            //KeyboardWindow.localScale = Vector3.zero;
            call?.Invoke(KeyboardPara);
            KeyboardPara = null;
            if(ConfirmBtnEvent != null)
            {
                ConfirmBtnEvent();
            }
        }

        //shift点击事件
        private void ClickShift()
        {
            if (Time.time - ShiftTime <= 0.5f)
            {
                ShiftTime = Time.time;
                isShift = true;
                isShiftLock = true;
                ShiftBG.color = LockColor;
                OnShiftOn();
            }
            else
            {
                if (isShift)
                {
                    ShiftTime = Time.time;
                    isShift = false;
                    isShiftLock = false;
                    ShiftBG.color = new Color(128, 128, 128, 255);
                    OnShiftOff();
                }
                else
                {
                    ShiftTime = Time.time;
                    isShift = true;
                    isShiftLock = false;
                    OnShiftOn();
                }
            }
        }

        //空格点击事件
        private void ClickSpace()
        {
            NewEditeString += " ";
            if (KeyboardPara != null)
                KeyboardPara.OutputStr = NewEditeString;
            call?.Invoke(KeyboardPara);
        }

        //清除点击事件
        private void ClickClear()
        {
            NewEditeString = ""; 
            if (KeyboardPara != null)
                KeyboardPara.OutputStr = NewEditeString;
            call?.Invoke(KeyboardPara);
        }

        //回退点击事件
        private void ClickBackSpace()
        {
            if (!string.IsNullOrEmpty(NewEditeString))
            {
                NewEditeString = NewEditeString.Substring(0, NewEditeString.Length - 1);
                if (KeyboardPara != null)
                    KeyboardPara.OutputStr = NewEditeString;
                call?.Invoke(KeyboardPara);
            }
        }

        private void OnDestroy()
        {
            instance = null;
        }


        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}
