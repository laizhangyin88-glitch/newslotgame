using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderControllerSound : MonoBehaviour
{
    private Slider slider;

    public bool isBGM = false;

    // Start is called before the first frame update
    void Start()
    {
        if (transform.parent.name.Contains("Settings Cell BGM"))
        {
            isBGM = true;
        }

        slider = GetComponent<Slider>();
        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener(OnValueChange);
         
        if (isBGM)
        {
            GSManager.Instance.MusicVolume = PlayerPrefs.GetFloat("MUTE_MUSIC", 1);
            slider.value = GSManager.Instance.MusicVolume;
        }
        else
        {
            GSManager.Instance.SfxVolume = PlayerPrefs.GetFloat("MUTE_SFX", 1);
            slider.value = GSManager.Instance.SfxVolume;
        }

    }

    private void OnValueChange(float value)
    {
        if (isBGM)
        {
            
            GSManager.Instance.MusicVolume = value;
            PlayerPrefs.SetFloat("MUTE_MUSIC", value);

        }
        else
        {
            GSManager.Instance.SfxVolume = value;
            PlayerPrefs.SetFloat("MUTE_SFX", value);
        }
    }

    private void OnDestroy()
    {
        if (slider != null)
        {
            slider.onValueChanged.RemoveAllListeners();
        }
    }
}
