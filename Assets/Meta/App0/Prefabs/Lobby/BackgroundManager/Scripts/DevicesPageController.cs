using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SBoxApi;
using SlotMaker;

public class DevicesPageController : MonoBehaviour
{
    private bool isInit = false;
    public TMP_InputField InputFieldAngency;
    public TMP_InputField InputFieldEquipNumber;

    public TMP_Dropdown DropdownStirModel;
    public TMP_Dropdown DropdownOpenLogo;
    public TMP_Dropdown DropdownLanguageModel;
    public TMP_Dropdown DropdownCtrl;
    public TMP_Dropdown DropdownEquipBinding;
    public TMP_Dropdown DropdownEquipModel;

    private void OnEnable()
    {
        if (!isInit)
        {
            isInit = true;
            InitAngency();
            InitEquipNumber();
        }
    }

    private void InitEquipModel()
    {
        if(DropdownEquipModel != null)
        {
            DropdownEquipModel.options.Clear();
            for (int i = 0; i < 3; i++)
            {
                TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData();
                data.text = "equip model " + i.ToString();
                DropdownEquipModel.options.Add(data);
            }
            DropdownEquipModel.captionText.text = "equip model 0";
            DropdownEquipModel.onValueChanged.AddListener(OnDropdownEquipModelChange);
        }
    }

    private void OnDropdownEquipModelChange(int index)
    {

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
