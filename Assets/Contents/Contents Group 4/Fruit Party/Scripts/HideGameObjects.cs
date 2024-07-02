using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideGameObjects : MonoBehaviour
{
    public GameObject[] gameObjects;

    private void Start()
    {
        for (int i = 0; i < gameObjects.Length; i++)
        {
            gameObjects[i].gameObject.SetActive(false);
        }
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            if (BlackboardUtils.GetOrCreateVariable<bool>("./game/isEarnCredit").value)
            {
                BlackboardUtils.GetOrCreateVariable<bool>("./game/isEarnCredit").value = false;
                GameObject go = AssetBundleManager.LoadAsset<GameObject>("commonminigame", "CommonMiniGame");
                if (go != null)
                {
                    GameObject temp = Instantiate(go) as GameObject;
                    temp.transform.SetParent(transform, false);
                    temp.gameObject.SetActive(true);
                }
            }
        }
    }
}
