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
                this.DelayAction(1, () => { group.enabled = false; });
                
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
        //int index = 0;
        //int[] temp =FruitPartyMiniGameController1.Instance.Results;
        //for (int i = 0; i < col; i++)
        //{
        //    for (int j = 0; j < row; j++)
        //    {
        //        if(j == ColMiddle)
        //        {
        //            bigRingItemControllers[i, j].ResultIndex = temp[index];
        //            index++;
        //        }
        //    }
        //}
    }

    public void StopSlotAnimation()
    {
        foreach (var item in bigRingItemControllers)
        {
            item.isMove = false;
            item.ResetPosition();
            item.PlayResult();
        }
    }

    public void PlayResult(int[] result)
    {
        int[] temp = result;
        for (int i = 0; i < col; i++)
        {
            bigRingItemControllers[i, 1].ResultIndex = temp[i];
        }
        StopSlotAnimation();
    }

    public void Pingpong(int index)
    {
        bigRingItemControllers[index, 1].Pingpong();
    }
}
