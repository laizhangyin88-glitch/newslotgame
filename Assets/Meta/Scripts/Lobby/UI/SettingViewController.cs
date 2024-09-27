using ParadoxNotion;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingViewController : MonoBehaviour
{
    public enum SettingType
     {
        Music,
        Sound,
    }

    public SettingType settingType;

    [SerializeField]
    private Slider music_slider;
    [SerializeField]
    private Slider sound_slider;
    [SerializeField]
    private PIDButton onClose;

    private Transform selected;

    private Vector3[] posVectors;

    private bool isChange = false;

    private int currentValue = 0;

    private float interval = 0;
    // Start is called before the first frame update
    void Start()
    {
        selected = transform.Find("Anchor/Content/selected");
        isChange = false;
        currentValue = 0;
        if (ApplicationSettings.Instance.isMachine)
        {
            selected.gameObject.SetActive(true);
            posVectors = new Vector3[2] { new Vector3(0, 10, 0), new Vector3(0, -85, 0)};
            settingType = SettingType.Music;
            MachineSelectManager.Instance.SetCustomsButton(
                new GameCustomsButton()
                {
                    mark = "Setting",
                    btnType = GameCustomsButtonType.BtnAll,
                    dicBtnAndSound = new Dictionary<string, string>()
                    {
                        ["BtnPre"] = GameCustomsButton.SOUND_DEFAULT,
                        ["BtnNext"] = GameCustomsButton.SOUND_DEFAULT,
                        ["BtnExit"] = GameCustomsButton.SOUND_DEFAULT,
                        ["BtnSwitch"] = GameCustomsButton.SOUND_DEFAULT,
                    }
                }
            );
            MessageDispatcher.Register(EVTType.ON_MACHINE_BUTTON_EVENT, OnMachineButtonEvent);
        }
        else
        {
            selected?.gameObject.SetActive(false);
        }

        if (music_slider != null)
        {
            music_slider.onValueChanged.AddListener(OnMusicSliderChange);
            GSManager.Instance.MusicVolume = PlayerPrefs.GetFloat("MUTE_MUSIC", 1);
            music_slider.value = GSManager.Instance.MusicVolume;
        }
        if(sound_slider != null)
        {
            sound_slider.onValueChanged.AddListener(OnSoundSliderValueChange);
            GSManager.Instance.SfxVolume = PlayerPrefs.GetFloat("MUTE_SFX", 1);
            sound_slider.value = GSManager.Instance.SfxVolume;
        }
        if(onClose != null)
        {
            onClose.onClick.AddListener(OnCloseBtn);
        }
    }

    public void OnMachineButtonEvent(EventData eventData)
    {
        if (eventData.name.StartsWith("GameCustomsButton/"))
        {
            Debug.LogError(eventData.name);
            string btnName = eventData.name.Replace("GameCustomsButton/", "");
            switch (btnName)
            {
                case "BtnPre_DOWN":
                    isChange = true;
                    currentValue = -1;
                    SetSlideValue(currentValue);
                    break;
                case "BtnNext_DOWN":
                    isChange = true;
                    currentValue = 1;
                    SetSlideValue(currentValue);
                    break;
                case "BtnPre_UP":
                case "BtnNext_UP":
                    isChange = false;
                    interval = 0;
                    break;
                case "BtnSwitch_DOWN":
                    OnSwitchBtn();
                    break;
                case "BtnExit_DOWN":
                    OnCloseBtn();
                    break;
            }
        }
    } 

    private void OnSwitchBtn()
    {
        settingType = settingType == SettingType.Music ? SettingType.Sound : SettingType.Music;
        switch (settingType)
        {
            case SettingType.Music:
                selected.transform.localPosition = posVectors[0];
                break;
            case SettingType.Sound:
                selected.transform.localPosition = posVectors[1];
                break;
            default:
                break;
        }
    }

    private void SetSlideValue(int value)
    {
        float tempValue = 0;
        switch (settingType)
        {
            case SettingType.Music:
                music_slider.value += ((tempValue += Time.deltaTime) * value);
                break;
            case SettingType.Sound:
                sound_slider.value += ((tempValue += Time.deltaTime) * value);
                break;
            default:
                break;
        }
    }


    private void Update()
    {
        if (isChange)
        {
            if((interval -= Time.deltaTime) <= 0)
            {
                interval = Time.deltaTime * 5f;
                SetSlideValue(currentValue);
            }
        }
    }

    private void OnDestroy() 
    {
        if(music_slider != null)
        {
            music_slider.onValueChanged.RemoveAllListeners();
        }
        if(sound_slider != null)
        {
            sound_slider.onValueChanged.RemoveAllListeners();
        }
        if(onClose != null)
        {
            onClose.onClick.RemoveAllListeners();
        }
        MachineSelectManager.Instance.ClearCustomsButton("Setting");
        MessageDispatcher.UnRegister(EVTType.ON_MACHINE_BUTTON_EVENT, OnMachineButtonEvent);
    }
     
    private void OnCloseBtn()
    {
        PopupManager.Instance.Close(gameObject); 
        Destroy(gameObject);
    }

    private void OnMusicSliderChange(float value)
    {
        GSManager.Instance.MusicVolume = value;
        PlayerPrefs.SetFloat("MUTE_MUSIC", value);
    }

    private void OnSoundSliderValueChange(float value)
    {
        GSManager.Instance.SfxVolume = value;
        PlayerPrefs.SetFloat("MUTE_SFX", value);
    }
}
