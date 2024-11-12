using UnityEngine;
using UnityEngine.UI;
using InnerKeyboard;

public class TestKeyboard : MonoBehaviour
{

    private Text EditText;
    private Button TextBtn;

    private KeyboardParam KeyboardPara = new KeyboardParam("");  //键盘参数

    private Text NowEditText;
    private void Awake()
    {
        EditText = transform.Find("Image/Text").GetComponent<Text>();
        TextBtn = EditText.GetComponent<Button>();
    }

    // Start is called before the first frame update
    void Start()
    {
        TextBtn.onClick.AddListener(() => ClickText(EditText));
    }
    void ClickText(Text sender)
    {
        NowEditText = sender;
        KeyboardPara.InputStr = sender.text;
        if (Keyboard.Instance)
            Keyboard.Instance.ShowKeyboard(KeyboardPara, EditCallBack);
    }

    void EditCallBack(KeyboardParam kbpara)
    {
        NowEditText.text = kbpara.OutputStr;
    }
}