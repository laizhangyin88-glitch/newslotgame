using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class LoginProgressBarUV : MonoBehaviour
{

    public RawImage barRawImage;
    public float barSpeed = 1.2f;


    private void Update()
    {

        Rect uvRect = barRawImage.uvRect;
        uvRect.x -= barSpeed * Time.deltaTime;
        barRawImage.uvRect = uvRect;

    }
}
