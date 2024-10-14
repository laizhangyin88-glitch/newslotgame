using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExchangeTestBtn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClickBtn);
    }

    private void OnClickBtn()
    {
        var prefab = AssetBundleManager.LoadAsset<GameObject>("lobby0", "ExchangeView");
        GameObject go = GameObject.Instantiate(prefab) as GameObject;
        go.transform.SetParent(PopupManager.Instance.contents, false);
        PopupManager.Instance.Open(go);
    }
}
