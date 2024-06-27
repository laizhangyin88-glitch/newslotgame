using BagelCode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommonMiniGameController : MonoBehaviour
{
    private List<CommonMiniGameItemController> commonMiniGameItemControllers = new List<CommonMiniGameItemController>();
    private Button redBtn;
    private Button blueBtn;
    private Image BlackImage;
    private Image red_Image;
    private Image blue_Image;
    public GameObject item;
    private Transform itemParent;

    private int CurrentIndex = 0;

    private void Start()
    {
        CurrentIndex = 0;

        redBtn = transform.Find("red_Button").GetComponent<Button>();
        blueBtn = transform.Find("blue_Button").GetComponent<Button>();
        red_Image = transform.Find("Item/red_Image").GetComponent<Image>();
        blue_Image = transform.Find("Item/blue_Image").GetComponent<Image>();
        BlackImage = transform.Find("Item/BlackImage").GetComponent<Image>();
        itemParent = transform.Find("ScrollView/Viewport/Content").transform;

        redBtn.onClick.RemoveAllListeners();
        blueBtn.onClick.RemoveAllListeners();
        red_Image.gameObject.SetActive(false);
        blueBtn.gameObject.SetActive(false);
        BlackImage.gameObject.SetActive(true);

        redBtn.onClick.AddListener(OnClickRedBtn);
        blueBtn.onClick.AddListener(OnClickBlueBtn);

        InitList();
    }

    private void OnClickRedBtn()
    {

    }

    private void OnClickBlueBtn()
    {

    }

    private void ShowResult(bool isShowRed)
    {
        AsyncActionUtils.ApplyRotation(this, BlackImage.transform, Vector3.zero, new Vector3(0, 90, 0), 0.5f, TweenUtils.VectorTweenLinear, 0, () =>
        {
            BlackImage.gameObject.SetActive(false);
            red_Image.gameObject.SetActive(isShowRed);
            blue_Image.gameObject.SetActive(!isShowRed);
        });
    }

    private void Reset()
    {
        BlackImage.transform.rotation = Quaternion.identity;
        BlackImage.gameObject.SetActive(true);
        
        red_Image.gameObject.SetActive(false);
        blue_Image.gameObject.SetActive(false);
    }

    private void InitList()
    {
        if(item != null)
        {
            for (int i = 0; i < 11; i++)
            {
                GameObject temp = Instantiate(item) as GameObject;
                temp.transform.SetParent(itemParent.transform, false);
                CommonMiniGameItemController controller = temp.AddComponent<CommonMiniGameItemController>();
                controller.OnInit();
                commonMiniGameItemControllers.Add(controller);
            }
        }
    }
}
