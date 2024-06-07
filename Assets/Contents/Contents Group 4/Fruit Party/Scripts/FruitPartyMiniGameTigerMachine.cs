using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 中间的老虎机的管理类
/// </summary>
public class FruitPartyMiniGameTigerMachine : MonoBehaviour
{
    public GameObject slotPrefab;
    //行
    public int row;
    //列
    public int col;
    /// <summary>
    /// 中间的序号
    /// </summary>
    public int ColMiddle;

    public BigRingItemController[,] bigRingItemControllers;

    
    // Start is called before the first frame update
    void Start()
    {
        bigRingItemControllers = new BigRingItemController[row, col];
        if (slotPrefab != null)
        {
            for (int i = 0; i < row; i++)
            {
                Transform parent = transform.GetChild(i);
                VerticalLayoutGroup group = parent.GetComponent<VerticalLayoutGroup>();
                group.enabled = true;
                for (int j = 0; j < col; j++)
                {
                    GameObject temp = Instantiate(slotPrefab);
                    temp.transform.parent = parent;
                    temp.transform.localPosition = Vector3.zero;
                    temp.transform.localRotation = Quaternion.identity;
                    temp.transform.localScale = Vector3.one;
                    BigRingItemController bigRingItemController = temp.GetComponent<BigRingItemController>();
                    bigRingItemController.Index = i;
                    bigRingItemControllers[i, j] = bigRingItemController;
                }
                MyTimerManagers.Instance.AddTimer(1, 1, () => { group.enabled = false; });
                
            }
        }
    }

    public void PlaySlotAnimation()
    {
        foreach (var item in bigRingItemControllers)
        {
            item.ResetPosition();
            item.isMove = true;
        }
        int index = 0;
        int[] temp =FruitPartyMiniGameController1.Instance.Results;
        for (int i = 0; i < col; i++)
        {
            for (int j = 0; j < row; j++)
            {
                if(j == ColMiddle)
                {
                    bigRingItemControllers[i, j].ResultIndex = temp[index];
                    index++;
                }
            }
        }
    }

    public void StopSlotAnimation()
    {
        foreach (var item in bigRingItemControllers)
        {
            item.isMove = false;
            item.ResetPosition();
            item.PlayResult(Random.Range(0, FruitPartyMiniGameController1.Instance.sprites.Length));
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    PlaySlotAnimation();
        //}
        //if (Input.GetKeyUp(KeyCode.W))
        //{
        //    StopSlotAnimation();
        //}
    }

}
