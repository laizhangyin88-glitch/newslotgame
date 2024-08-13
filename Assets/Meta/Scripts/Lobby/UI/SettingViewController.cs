using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingViewController : MonoBehaviour
{
    [SerializeField]
    private Slider music_slider;
    [SerializeField]
    private Slider sound_slider;
    [SerializeField]
    private PIDButton onClose;
    // Start is called before the first frame update
    void Start()
    {
        if(music_slider != null)
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
