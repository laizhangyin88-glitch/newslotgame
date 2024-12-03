using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SBoxApi;
using SlotMaker;
using UnityEngine.UI;
using BagelCode;

public class DropdownItem
{
    GameObject gameObject;
    private TextMeshProUGUI text;
    public Button btn;
    public int index = 0;
    public DropdownItem(GameObject go)
    {
        gameObject = go;
        text = gameObject.transform.Find("Item Label").GetComponent<TextMeshProUGUI>();
        btn = gameObject.GetComponent<Button>();
    }

    public void SetText(string value)
    {
        text.text = value;
        SetActive(true);
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}

public class DevicesPageController : MonoBehaviour
{
    public GameObject dropdownItem;

    private bool isInit = false;
    public TMP_InputField InputFieldAngency;
    public TMP_InputField InputFieldEquipNumber;

    public TMP_Dropdown DropdownStirModel;
    public TMP_Dropdown DropdownOpenLogo;
    public TMP_Dropdown DropdownLanguageModel;
    public TMP_Dropdown DropdownCtrl;
    public TMP_Dropdown DropdownEquipBinding;
    public TMP_Dropdown DropdownEquipModel;

    private Button EquipModelBtn;
    private Button StirButton;
    private Button CtrlButton;
    private Button OpenButton;
    private Button BindingButton;

    private Button closeDropdownBtn;
    private Transform _DropdownList;
    private Transform content;
    private Transform _itemParent;

    private string current = "";

    private List<string> equipModelData;
    private List<string> stirData;
    private List<string> ctrlData;
    private List<string> openData;
    private List<string> bindingData = new List<string>() { "ON", "OFF" };

    private List<DropdownItem> _dropdownItems = new List<DropdownItem>();

    private void OnEnable()
    {
        if (!isInit)
        {
            isInit = true;
            InitAngency();
            InitEquipNumber();

            content = transform.Find("content");

            _DropdownList = transform.Find("content/DropdownList");
            _itemParent = _DropdownList.transform.Find("Viewport/Content");
            closeDropdownBtn = _DropdownList.transform.Find("closeDropdownBtn").GetComponent<Button>();
            closeDropdownBtn.onClick.AddListener(() => { _DropdownList.gameObject.SetActive(false); });
            _DropdownList.gameObject.SetActive(false);

            EquipModelBtn = transform.Find("EquipModel/EquipModelButton").GetComponent<Button>();
            StirButton = transform.Find("StirModel/StirButton").GetComponent<Button>();
            CtrlButton = transform.Find("CtrlModel/CtrlButton").GetComponent<Button>();
            OpenButton = transform.Find("OpenLogo/OpenButton").GetComponent<Button>();
            BindingButton = transform.Find("EquipBinding/BindingButton").GetComponent<Button>();

            StirButton.onClick.AddListener(OnClickStirBtn);
            EquipModelBtn.onClick.AddListener(OnClickEquipModelBtn);
            CtrlButton.onClick.AddListener(OnClickCtrlBtn);
            OpenButton.onClick.AddListener(OnClickOpenBtn);
            BindingButton.onClick.AddListener(OnClickBindingBtn);

            InitBtnDesc();
        }
    }

    private void OnClickBindingBtn()
    {
        if (_DropdownList.gameObject.activeSelf)
        {
            _DropdownList.gameObject.SetActive(false);
        }
        else
        {
            current = "binding model";
            ShowDropdownList(BindingButton.transform, bindingData);
        }
    }

    private void OnClickOpenBtn()
    {
        if (_DropdownList.gameObject.activeSelf)
        {
            _DropdownList.gameObject.SetActive(false);
        }
        else
        {
            current = "open model";
            if (openData == null)
            {
                List<string> data = new List<string>();
                for (int i = 0; i < 4; i++)
                {
                    var temp = "open model " + i.ToString();
                    data.Add(temp);
                }
                openData = data;
            }
            ShowDropdownList(OpenButton.transform, openData);
        }
    }

    private void OnClickCtrlBtn()
    {
        if (_DropdownList.gameObject.activeSelf)
        {
            _DropdownList.gameObject.SetActive(false);
        }
        else
        {
            current = "ctrl model";
            if (ctrlData == null)
            {
                List<string> data = new List<string>();
                for (int i = 0; i < 4; i++)
                {
                    var temp = "ctrl model " + i.ToString();
                    data.Add(temp);
                }
                ctrlData = data;
            }
            ShowDropdownList(CtrlButton.transform, ctrlData);
        }
    }

    private void OnClickStirBtn()
    {
        if (_DropdownList.gameObject.activeSelf)
        {
            _DropdownList.gameObject.SetActive(false);
        }
        else
        {
            current = "stir model";
            if (stirData == null)
            {
                List<string> data = new List<string>();
                for (int i = 0; i < 5; i++)
                {
                    var temp = "stir model " + i.ToString();
                    data.Add(temp);
                }
                stirData = data;
            }
            ShowDropdownList(StirButton.transform, stirData);
        }
    }

    private void InitBtnDesc()
    {
        EquipModelBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "equip model 0";
        StirButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "stir model 0";
        CtrlButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "ctrl model 0";
        OpenButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "open model 0";
        BindingButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "ON";
    }

    private void ShowDropdownList(Transform parent, List<string> data)
    {
        HideAllItem();
        _DropdownList.transform.SetParent(parent, false);
        _DropdownList.transform.localPosition = new Vector3(0, -20, 0);
        for (int i = 0; i < data.Count; i++)
        {
            if(i > _dropdownItems.Count - 1)
            {
                var item = GetDropdownItem(i);
                _dropdownItems.Add(item);
            }
            _dropdownItems[i].SetText(data[i]);
        }
        _DropdownList.transform.SetParent(content);
        _DropdownList.gameObject.SetActive(true);
    }

    private DropdownItem GetDropdownItem(int index)
    {
        GameObject item = Instantiate(dropdownItem);
        item.transform.SetParent(_itemParent);
        item.transform.localScale = Vector3.one;
        DropdownItem dropdown = new DropdownItem(item);
        dropdown.index = index;
        dropdown.btn.onClick.AddListener(() =>
        {
            OnClickDropdownBtn(dropdown.index);
        });
        return dropdown;
    }

    private void HideAllItem()
    {
        for (int i = 0; i < _dropdownItems.Count; i++)
        {
            _dropdownItems[i].SetActive(false);
        }
    }
    private void OnClickDropdownBtn(int index)
    {
        switch (current)
        {
            case "equip model": 
                EquipModelBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = equipModelData[index];
                break;
            case "stir model":
                StirButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = stirData[index];
                break;
            case "ctrl model":
                CtrlButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ctrlData[index];
                break;
            case "open model":
                OpenButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = openData[index];
                break;
            case "binding model":
                BindingButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = bindingData[index];
                break;
        }
        _DropdownList.gameObject.SetActive(false);
    }

    private void OnClickEquipModelBtn()
    {
        if(_DropdownList.gameObject.activeSelf)
        {
            _DropdownList.gameObject.SetActive(false);
        }
        else
        {
            current = "equip model";
            if (equipModelData == null)
            {
                List<string> data = new List<string>();
                for (int i = 0; i < 3; i++)
                {
                    var temp = "equip model " + i.ToString();
                    data.Add(temp);
                }
                equipModelData = data;
            }
            ShowDropdownList(EquipModelBtn.transform, equipModelData);
        }
    }

    private void OnDropdownEquipModelChange(int index)
    {
        Debug.LogError(index.ToString());
    }

    private void InitAngency()
    {
        if(InputFieldAngency != null)
        {
            InputFieldAngency.text = NetData_Login.Instance.UserAgent;
        }
    }
    private void InitEquipNumber()
    {
        if (InputFieldEquipNumber != null)
        {
            if (Application.isEditor)
            {
                InputFieldEquipNumber.text = "test111";
            }
            else
            {
                InputFieldEquipNumber.text = SBoxSandbox.DeviceId().ToString();
            }
        }
    }
}
