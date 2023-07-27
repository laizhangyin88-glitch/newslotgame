using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class HiddenObjectsUV : MonoBehaviour
{

    public RawImage FlowRawImage;
    public float FlowSpeed = 0.2f;


    private void Update()
    {

        Rect uvRect = FlowRawImage.uvRect;
        uvRect.x -= FlowSpeed * Time.deltaTime;
        uvRect.y += FlowSpeed * Time.deltaTime;
        FlowRawImage.uvRect = uvRect;

    }
}
