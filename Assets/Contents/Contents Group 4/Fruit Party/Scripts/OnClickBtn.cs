using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnClickBtn : MonoBehaviour
{
    public Button btn;
    // Start is called before the first frame update
    void Start()
    {
        if(btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnClickBtnDown);
        }
        Destroy(gameObject, 5);
    }

    private void OnClickBtnDown()
    {
        Destroy(gameObject);
    }
}
