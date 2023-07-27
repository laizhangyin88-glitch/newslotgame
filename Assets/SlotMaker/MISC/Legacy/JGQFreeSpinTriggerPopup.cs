using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{

public class JGQFreeSpinTriggerPopup : MonoBehaviour 
{
    public void Initialize(List<Animator> buttonAnimators)
    {
        for (int i = 1; i < buttonAnimators.Count; ++i)
        {
            buttonAnimators[i].transform.Find("Free Spin Count Move").gameObject.SetActive(false);
            buttonAnimators[i].transform.Find("Free Spin Count").gameObject.SetActive(true);
            buttonAnimators[i].transform.Find("Multiplier Move").gameObject.SetActive(false);
            buttonAnimators[i].transform.Find("Multiplier").gameObject.SetActive(true);
        }
    }

    public void PlayFreeSpinCountAnimation(int index, List<Animator> buttonAnimators)
    {
        buttonAnimators[index+1].transform.Find("Free Spin Count Move").gameObject.SetActive(true);
        buttonAnimators[index+1].transform.Find("Free Spin Count").gameObject.SetActive(false);
    }

    public void PlayFreeSpinMultiplierAnimation(int index, List<Animator> buttonAnimators) 
    {
        buttonAnimators[index+1].transform.Find("Multiplier Move").gameObject.SetActive(true);
        buttonAnimators[index+1].transform.Find("Multiplier").gameObject.SetActive(false);
    }
}

}
