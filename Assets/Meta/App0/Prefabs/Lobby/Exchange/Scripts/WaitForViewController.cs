using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForViewController : MonoBehaviour
{
   public static WaitForViewController Instance;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        Instance = this;
    }

    public void Close()
    {
        PopupManager.Instance.Close(this.gameObject);
        Destroy(gameObject);
    }
}
