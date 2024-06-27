using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommonMiniGameItemController : MonoBehaviour
{
    private Image BlackImage;
    private Image red_Image;
    private Image blue_Image;
    private Image defalut_Image;

    public void OnInit()
    {
        red_Image = transform.Find("red_Image").GetComponent<Image>();
        blue_Image = transform.Find("blue_Image").GetComponent<Image>();
        BlackImage = transform.Find("BlackImage").GetComponent<Image>();
        BlackImage.color = Color.white;
        defalut_Image = transform.Find("Image").GetComponent<Image>();
        defalut_Image.gameObject.SetActive(true);
    }

    public void ShowImage(bool isRed)
    {
        red_Image.gameObject.SetActive(isRed);
        blue_Image.gameObject.SetActive(!isRed);
        defalut_Image.gameObject.SetActive(false);
    }

}
