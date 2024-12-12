using System.Collections;
using System.Collections.Generic;
using BagelCode;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;

public class AvatarSynchronization : MonoBehaviour
{

    public WebImageController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<WebImageController>();
        controller.SetWebImage(NetData_Login.Instance.UserProfileUrl, true);
    }
}
