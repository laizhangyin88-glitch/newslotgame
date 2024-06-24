using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private Transform listParent;

    public GameObject selectItem;

    public GameObject[] GameList;

    private List<DoorSelectItem> doorSelectItems = new List<DoorSelectItem>();

    private void Start()
    {
        listParent = transform.Find("list");
        InitSelectItem();
    }

    private void InitSelectItem()
    {
        for (int i = 0; i < 3; i++)
        {
            if(selectItem != null)
            {
                GameObject go = Instantiate(selectItem);
                go.transform.SetParent(listParent, false);
                var select = go.GetComponent<DoorSelectItem>();
                select.SetIndex(i);
                select.DoorController = this;
                doorSelectItems.Add(select);
            }
        }
    }

    public void PlayMiniGame(int index)
    {
        switch (index)
        {
            case 0:
                var controller0 = GameList[index].GetComponent<FruitPartyMiniGameController1>();
                controller0.gameObject.SetActive(true);
                controller0.OnStart(); 
                break;
            case 1:
                var controller1 = GameList[index].GetComponent<FruitPartyMiniGameController2>();
                controller1.gameObject.SetActive(true);
                controller1.OnStart();
                break;
            case 2:
                var controller2 = GameList[index].GetComponent<FruitPartyMiniGameController3>();
                break;
        }
        this.gameObject.SetActive(false);
    }
}
