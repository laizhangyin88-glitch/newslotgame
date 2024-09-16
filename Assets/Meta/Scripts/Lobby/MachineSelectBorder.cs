using Sirenix.OdinInspector;
using SlotMaker;
using System;
using System.Collections.Generic;
using UnityEngine;



public enum SortType
{
    /// <summary>从左到右</summary>
    L2R = 0,
    R2L = 1,
    T2B = 2,
    B2T = 3,
    /// <summary>先从左到右，再从上到下</summary>
    L2R_T2B = 4,
    /*L2R_B2T = 5,
    R2L_T2B = 6,
    R2L_B2T = 7,
    T2B_L2R = 8,
    T2B_R2L = 9,
    B2T_L2R = 10,
    B2T_R2L = 11,*/
    ByIndex = 12,
}

public abstract class MachineSelectBorderState : MonoBehaviour
{
    public virtual void SetBtnSelected() { }
    public virtual void SetBtnHighlight() { }
    public virtual void SetBtnNormal() { }
    public virtual void SetBtnDisable() { }
    public virtual void SetBtnEnable() { }
    public virtual void SetBtnPressed() { }
}

public class MachineSelectBorder : MonoBehaviour
{
    // Start is called before the first frame update
    //public string name = "";

    public GameObject selectBorder;

    private MachineSelectBorderState state => selectBorder?.GetComponent<MachineSelectBorderState>();
  

    public int index = -1;

    public string mark = "";

    public bool isAutoIndex = false;

    public SortType autosSort = SortType.L2R;

    [HideInInspector]
    public bool isInit = false;


    static bool isReflashRegion = false;
    static bool isVisibleReflashRegion = false;



    public bool isIgnore = false;

    bool _isSelected = false;
    public bool isSelected
    {
        get
        {
            //return selectBorder.active;
            return _isSelected;
        }
        set
        {

            _isSelected = value;
#if !UNITY_EDITOR
            // 非机台不显示边框
            if (!ApplicationSettings.Instance.isMachine)
            {
                //selectBorder?.SetActive(false);
                SetSelectBorderEnable(false);
                return;
            }    
#endif
            // 使用机台灯选择时，不显示边框
            KeyValuePair<bool, string> kv = MachineSelectManager.Instance.GetIgonreBorderWhenUseLightBtnSelect();

            if (kv.Key && kv.Value == mark)
            {
                //selectBorder?.SetActive(false);
                SetSelectBorderEnable(false);
            }
            else
            {
                //selectBorder?.SetActive(value);
                SetSelectBorderSelected(value);
            }
        }
    }

    public void SetSelectBorderSelected(bool value)
    {
        if (state != null)
        {
            if (value)
                state.SetBtnSelected();
            else
                state.SetBtnNormal();
        }
        else
        {
            selectBorder?.SetActive(value);
        }
    }
    public void SetSelectBorderEnable(bool value)
    {
        if (state != null)
        {
            if (value)
                state.SetBtnEnable();
            else
                state.SetBtnDisable();
        }
        else
        {
            selectBorder?.SetActive(value);
        }
    }

    private Transform[] _FindParent(List<MachineSelectBorder> _comps, int index)
    {
        /*int index = -1;
        List<MachineSelectBorder> _comps = new List<MachineSelectBorder>(comps);
        if (comps == null)
        {
            comps = GameObject.FindObjectsOfType<MachineSelectBorder>();
            _comps = new List<MachineSelectBorder>();
            foreach (var item in comps)
            {
                if (item.mark == mark && item.gameObject.active)
                {
                    _comps.Add(item);
                }
            }
        }*/

        List<Transform> tfmLst = new List<Transform>();
        for (int i = 0; i < _comps.Count; i++)
        {
            tfmLst.Add(_comps[i].transform);
        }

        bool isFind = false;

        for (int i = 0; i < 8; i++)
        {
            for (int j = tfmLst.Count - 1; j > 0; j--)
            {
                isFind = true;
                if (tfmLst[j].parent != tfmLst[j - 1].parent)
                {
                    isFind = false;
                    for (int k = 0; k < tfmLst.Count; k++)
                    {
                        tfmLst[k] = tfmLst[k].parent;
                    }
                    break;
                }
            }
            if (isFind)
            {
                break;
            }
        }

        if (!isFind)
        {
            Debug.LogError("【MachineSelectBorder】ERR : No common parent node is found.");
            return null;
        }
        else
        {
            Debug.LogWarning($"【MachineSelectBorder】common parent node {tfmLst[0].parent.name}  child node {tfmLst[index].name}  child index = {tfmLst[index].GetSiblingIndex()}");
        }

        return new Transform[] { tfmLst[0].parent, tfmLst[index] };
    }



    [Button]
    void test_GetPosition()
    {
        //本地坐标
        Vector3 locPos = new Vector3(0, 0, 0);
        // 转换为世界坐标
        Vector3 worPosA = transform.TransformPoint(locPos);

        Debug.Log($"【MachineSelectBorder】 world pos x ={worPosA.x} y = {worPosA.y} z = {worPosA.z}");
    }

    [Button]
    void test_GetRect()
    {
        Vector3[] corners = new Vector3[4];//"左下", "左上", "右上", "右下"
        gameObject.GetComponent<RectTransform>().GetWorldCorners(corners);
        Vector3 worPosA;
        worPosA = corners[0];
        Debug.Log($"【MachineSelectBorder】左下 world pos x ={worPosA.x} y = {worPosA.y} z = {worPosA.z}");
        worPosA = corners[1];
        Debug.Log($"【MachineSelectBorder】左上 world pos x ={worPosA.x} y = {worPosA.y} z = {worPosA.z}");
        worPosA = corners[2];
        Debug.Log($"【MachineSelectBorder】右上 world pos x ={worPosA.x} y = {worPosA.y} z = {worPosA.z}");
        worPosA = corners[3];
        Debug.Log($"【MachineSelectBorder】右下 world pos x ={worPosA.x} y = {worPosA.y} z = {worPosA.z}");
    }

    [Button]
    void test_GetIndex()
    {
        try
        {
            int index = -1;
            MachineSelectBorder[] comps = GameObject.FindObjectsOfType<MachineSelectBorder>();

            List<MachineSelectBorder> _comps = new List<MachineSelectBorder>();
            foreach (var item in comps)
            {
                if (item.mark == mark && item.gameObject.active)
                {
                    _comps.Add(item);
                }
            }
            List<Transform> tfmLst = new List<Transform>();
            for (int i = 0; i < _comps.Count; i++)
            {
                tfmLst.Add(_comps[i].transform);
                if (_comps[i].transform == transform)
                {
                    index = i;
                }
            }
            bool isFind = false;

            for (int i = 0; i < 5; i++)
            {
                for (int j = tfmLst.Count - 1; j > 0; j--)
                {
                    isFind = true;
                    if (tfmLst[j].parent != tfmLst[j - 1].parent)
                    {
                        isFind = false;
                        for (int k = 0; k < tfmLst.Count; k++)
                        {
                            tfmLst[k] = tfmLst[k].parent;
                        }
                        break;
                    }
                }
                if (isFind)
                {
                    break;
                }
            }

            if (!isFind)
            {
                Debug.LogError("【MachineSelectBorder】ERR : No common parent node is found.");
            }
            else
            {
                Debug.LogWarning($"【MachineSelectBorder】common parent node {tfmLst[0].parent.name}  my index = {tfmLst[index].GetSiblingIndex()}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"【MachineSelectBorder】ERR : {e}");
        }

    }


    public delegate bool IsExchange<T>(T x, T y);
    public static List<T1> DoBubbling<T1>(List<T1> lst, IsExchange<T1> isExchange)
    {
        int n = lst.Count;
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                if (isExchange(lst[j], lst[j + 1])) //true 时交换
                {
                    // 交换 array[j] 和 array[j + 1]  
                    T1 temp = lst[j];
                    lst[j] = lst[j + 1];
                    lst[j + 1] = temp;
                }
            }
        }
        return lst;
    }


    void setAutoIndex()
    {
        MachineSelectBorder[] comps = GameObject.FindObjectsOfType<MachineSelectBorder>();

        List<MachineSelectBorder> _comps = new List<MachineSelectBorder>();
        foreach (var item in comps)
        {
            if (item.mark == mark) // && item.gameObject.active)
            {
                _comps.Add(item);
            }
        }

        switch (autosSort)
        {
            case SortType.L2R:
                /*_comps.Sort((a, b) =>
                {
                    //本地坐标
                    Vector3 locPos = new Vector3(0, 0, 0);

                    // 转换为世界坐标
                    Vector3 worPosA = a.transform.TransformPoint(locPos);
                    Vector3 worPosB = b.transform.TransformPoint(locPos);

                    return (int)(worPosA.x - worPosB.x); //返回值小于0，则b排在a后面 
                });*/

                _comps = DoBubbling<MachineSelectBorder>(_comps, (a, b) =>
                {

                    //本地坐标
                    Vector3 locPos = new Vector3(0, 0, 0);
                    // 转换为世界坐标
                    Vector3 worPosA = a.transform.TransformPoint(locPos);
                    Vector3 worPosB = b.transform.TransformPoint(locPos);

                    /*
                    Vector3[] corners = new Vector3[4];//"左下", "左上", "右上", "右下"
                    a.gameObject.GetComponent<RectTransform>().GetWorldCorners(corners);
                    Vector3 worPosA = corners[1];
                    Vector3[] corners2 = new Vector3[4];
                    b.gameObject.GetComponent<RectTransform>().GetWorldCorners(corners2);
                    Vector3 worPosB = corners2[1];
                    
                     */

                    //Debug.Log($"worPosA.x  = {worPosA.x} worPosB.x{worPosB.x} ");
                    return worPosA.x > worPosB.x;

                });


                break;
            case SortType.R2L:

                /* _comps.Sort((a, b) =>
                 {
                     //本地坐标
                     Vector3 locPos = new Vector3(0, 0, 0);
                     // 转换为世界坐标
                     Vector3 worPosA = a.transform.TransformPoint(locPos);
                     Vector3 worPosB = b.transform.TransformPoint(locPos);

                     return (int)(worPosB.x - worPosA.x); //返回值小于0，则b排在a后面 
                 });*/
                _comps = DoBubbling<MachineSelectBorder>(_comps, (a, b) =>
                {
                    //本地坐标
                    Vector3 locPos = new Vector3(0, 0, 0);
                    // 转换为世界坐标
                    Vector3 worPosA = a.transform.TransformPoint(locPos);
                    Vector3 worPosB = b.transform.TransformPoint(locPos);
                    return worPosA.x < worPosB.x;
                });
                break;
            case SortType.T2B:
                {
                    int n02 = _comps.Count;
                    for (int i = 0; i < n02 - 1; i++)
                    {
                        for (int j = 0; j < n02 - i - 1; j++)
                        {

                            //Vtor3 locPos = new Vector3(0, 0, 0);
                            //Vector3 worPosA = _comps[j].transform.TransformPoint(locPos);
                            //Vector3 worPosB = _comps[j + 1].transform.TransformPoint(locPos);

                            //Vector3 worPosA = _comps[j].transform.parent.TransformPoint(_comps[j].transform.localPosition);
                            //Vector3 worPosB = _comps[j + 1].transform.parent.TransformPoint(_comps[j + 1].transform.localPosition);

                            Vector3[] corners = new Vector3[4];//"左下", "左上", "右上", "右下"
                            _comps[j].gameObject.GetComponent<RectTransform>().GetWorldCorners(corners);
                            Vector3 worPosA = corners[1];
                            //Debug.Log($"T2B worPosA 001 = {_comps[j].transform.TransformPoint(worPosA)} ");
                            //Debug.Log($"T2B worPosA 002 = {_comps[j].transform.parent.TransformPoint(worPosA)} ");
                            Vector3[] corners2 = new Vector3[4];
                            _comps[j + 1].gameObject.GetComponent<RectTransform>().GetWorldCorners(corners2);
                            Vector3 worPosB = corners2[1];
                            //Debug.Log($"T2B worPosA = {worPosA} worPosB = {worPosB}");
                            //Debug.Log($"T2B worPosA.y = {worPosA.y} worPosB.y = {worPosB.y}  is = {_comps[j].gameObject == _comps[j + 1].gameObject} Count = {_comps.Count}");
                            if (worPosA.y < worPosB.y) // 如果当前元素小于下一个元素，则交换它们  
                            {
                                // 交换 array[j] 和 array[j + 1]  
                                MachineSelectBorder temp = _comps[j];
                                _comps[j] = _comps[j + 1];
                                _comps[j + 1] = temp;
                            }
                        }
                    }

                    break;
                }
            case SortType.B2T:
                {
                    int n02 = _comps.Count;
                    for (int i = 0; i < n02 - 1; i++)
                    {
                        for (int j = 0; j < n02 - i - 1; j++)
                        {

                            Vector3 locPos = new Vector3(0, 0, 0);
                            Vector3 worPosA = _comps[j].transform.TransformPoint(locPos);
                            Vector3 worPosB = _comps[j + 1].transform.TransformPoint(locPos);
                            Debug.Log($"B2T worPosA.y = {worPosA.y} worPosB.y = {worPosB.y}");
                            Debug.Log($"B2T worPosA.x = {worPosA.x} worPosB.x = {worPosB.x}");
                            if (worPosA.y > worPosB.y) // 如果当前元素大于下一个元素，则交换它们  
                            {
                                // 交换 array[j] 和 array[j + 1]  
                                MachineSelectBorder temp = _comps[j];
                                _comps[j] = _comps[j + 1];
                                _comps[j + 1] = temp;
                            }
                        }
                    }

                    break;
                }
            case SortType.L2R_T2B:
                {
                    /*_comps.Sort((a, b) =>
                    {
                        Vector3 locPos = new Vector3(0, 0, 0);
                        Vector3 worPosA = a.transform.TransformPoint(locPos);
                        Vector3 worPosB = b.transform.TransformPoint(locPos);
                        return   (int)(worPosB.y - worPosA.y); //返回值小于0，则b排在a后面 (从上到下排列)
                    });*/

                    int n = _comps.Count;
                    for (int i = 0; i < n - 1; i++)
                    {
                        for (int j = 0; j < n - i - 1; j++)
                        {

                            Vector3 locPos = new Vector3(0, 0, 0);
                            Vector3 worPosA = _comps[j].transform.TransformPoint(locPos);
                            Vector3 worPosB = _comps[j + 1].transform.TransformPoint(locPos);

                            if (worPosA.y < worPosB.y) // 如果当前元素小于下一个元素，则交换它们  
                            {
                                // 交换 array[j] 和 array[j + 1]  
                                MachineSelectBorder temp = _comps[j];
                                _comps[j] = _comps[j + 1];
                                _comps[j + 1] = temp;
                            }
                        }
                    }

                    /*string name = "从上到下 = ";
                    _comps.ForEach(item =>
                    {
                        name += $" {item.transform.parent.name}";
                    });
                    Debug.Log(name);*/
                    List<List<MachineSelectBorder>> lst = new List<List<MachineSelectBorder>>();
                    int k = 0;
                    lst.Add(new List<MachineSelectBorder>());
                    for (int i = 0; i < _comps.Count; i++)
                    {
                        if (lst[k].Count == 0)
                        {
                            lst[k].Add(_comps[i]);
                        }
                        else
                        {
                            //本地坐标
                            Vector3 locPos = new Vector3(0, 0, 0);
                            // 转换为世界坐标
                            Vector3 worPosA = lst[k][lst[k].Count - 1].transform.TransformPoint(locPos);
                            Vector3 worPosB = _comps[i].transform.TransformPoint(locPos);


                            float def = Math.Abs(worPosA.y - worPosB.y);
                            //Debug.Log($"def = {def}");
                            if (def < 0.2)//同一行
                            {
                                lst[k].Add(_comps[i]);
                            }
                            else
                            {
                                k++;
                                lst.Add(new List<MachineSelectBorder>());
                                lst[k].Add(_comps[i]);
                            }
                        }
                    }
                    /*
                    name = $"分组 = {lst.Count} =";
                    lst.ForEach(item =>
                    {
                        string n1 = "";
                        item.ForEach((it) =>
                        {
                            n1 += $" {it.transform.parent.name}";
                        });
                        name += $" {n1} || ";
                    });
                    Debug.Log(name);*/

                    _comps = new List<MachineSelectBorder>();
                    for (int i = 0; i < lst.Count; i++)
                    {
                        /*lst[i].Sort((a, b) =>
                        {
                            Vector3 locPos = new Vector3(0, 0, 0);
                            Vector3 worPosA = a.transform.TransformPoint(locPos);
                            Vector3 worPosB = b.transform.TransformPoint(locPos);
                            return (int)(worPosA.x - worPosB.x); //返回值小于0，则b排在a后面 (从左到右)
                        });*/


                        int n1 = lst[i].Count;
                        for (int i1 = 0; i1 < n1 - 1; i1++)
                        {
                            for (int j1 = 0; j1 < n1 - i1 - 1; j1++)
                            {
                                Vector3 locPos = new Vector3(0, 0, 0);
                                Vector3 worPosA = lst[i][j1].transform.TransformPoint(locPos);
                                Vector3 worPosB = lst[i][j1 + 1].transform.TransformPoint(locPos);
                                if (worPosA.x > worPosB.x) // (为true时交换)如果当前元素大于下一个元素，则交换它们  
                                {
                                    // 交换 array[j] 和 array[j + 1]  
                                    MachineSelectBorder temp = lst[i][j1];
                                    lst[i][j1] = lst[i][j1 + 1];
                                    lst[i][j1 + 1] = temp;
                                }
                            }
                        }
                        _comps.AddRange(lst[i]);
                    }
                    break;
                }
            case SortType.ByIndex:
                {
                    List<Transform> tfmLst = new List<Transform>();
                    foreach (var item in _comps)
                    {
                        tfmLst.Add(item.transform);
                    }
                    bool isFind = false;

                    for (int i = 0; i < 10; i++)
                    {
                        for (int j = tfmLst.Count - 1; j > 0; j--)
                        {
                            isFind = true;
                            if (tfmLst[j].parent != tfmLst[j - 1].parent)
                            {
                                isFind = false;
                                for (int k3 = 0; k3 < tfmLst.Count; k3++)
                                {
                                    tfmLst[k3] = tfmLst[k3].parent;
                                }
                                break;
                            }
                        }
                        if (isFind)
                        {
                            break;
                        }
                    }

                    if (!isFind)
                    {
                        Debug.LogError("【ERR】:No common parent node is found.");
                    }
                    else
                    {
                        Debug.LogWarning($"common parent node {tfmLst[0].parent.name}");

                        int n = tfmLst.Count;
                        for (int i = 0; i < n - 1; i++)
                        {
                            for (int j = 0; j < n - i - 1; j++)
                            {
                                if (tfmLst[j].GetSiblingIndex() > tfmLst[j + 1].GetSiblingIndex())
                                {
                                    Transform tfm = tfmLst[j];
                                    tfmLst[j] = tfmLst[j + 1];
                                    tfmLst[j + 1] = tfm;

                                    // 交换 array[j] 和 array[j + 1]  
                                    MachineSelectBorder temp = _comps[j];
                                    _comps[j] = _comps[j + 1];
                                    _comps[j + 1] = temp;
                                }
                            }
                        }
                    }
                    break;
                }
        }

        comps = _comps.ToArray();

        string timestampStr = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        string msb_mark = timestampStr.Substring(timestampStr.Length - 5);
        for (int i = 0; i < comps.Length; i++)
        {
            comps[i].index = i;
            comps[i].isInit = true;
            if (!comps[i].transform.name.Contains("__"))
            {
                comps[i].transform.name = $"Machine Select Border__{msb_mark}_{i}";
            }
        }
    }




    private void Awake()
    {
        if (selectBorder == null)
            selectBorder = transform.Find("Selected")?.gameObject;
    }

    void Start()
    {
        MachineSelectBorder.isReflashRegion = true;

        if (isAutoIndex && !isInit)
        {
            setAutoIndex();
        }
        isInit = true;

        if (mark == "SPIN")
        {
            isSelected = index == 0 && MachineSelectManager.Instance.isChangeButtonRegion();
            //selectBorder.SetActive(index == 0 && MachineSelectManager.Instance.isChangeButtonRegion());
        }
        else
        {
            isSelected = index == 0;
            //selectBorder.SetActive(index == 0); // 默认 index = 0 时显示
        }

        if (mark == "SPIN"
            && MachineSelectManager.Instance.isCloseSpinBorder())
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
    }

    public static void ShowBorder(string mark)
    {

#if !UNITY_EDITOR
        // 非机台不显示边框
        if (!ApplicationSettings.Instance.isMachine)
            return;
#endif

        MachineSelectBorder[] comps = GameObject.FindObjectsOfType<MachineSelectBorder>();

        for (int i = 0; i < comps.Length; i++)
        {
            if (comps[i].mark == mark && comps[i].gameObject.active && comps[i]._isSelected)
            {
                //comps[i].selectBorder?.SetActive(true);
                comps[i].SetSelectBorderEnable(true);
            }
        }
    }
    public static void DisShowBorder(string mark)
    {
#if !UNITY_EDITOR
        // 非机台不显示边框
        if (!ApplicationSettings.Instance.isMachine)
            return;
#endif

        MachineSelectBorder[] comps = GameObject.FindObjectsOfType<MachineSelectBorder>();

        for (int i = 0; i < comps.Length; i++)
        {
            if (comps[i].mark == mark && comps[i].gameObject.active)
            {
                //comps[i].selectBorder?.SetActive(false);
                comps[i].SetSelectBorderEnable(false);
            }
        }
    }

    public static void IgnoreBorder(string mark, int index)
    {
        MachineSelectBorder[] comps = GameObject.FindObjectsOfType<MachineSelectBorder>();

        for (int i = 0; i < comps.Length; i++)
        {
            if (comps[i].mark == mark && comps[i].index == index)
            {
                comps[i].isIgnore = true;
            }
        }
    }
    public static void ClearAllIgnoreBorder(string mark)
    {
        MachineSelectBorder[] comps = GameObject.FindObjectsOfType<MachineSelectBorder>();

        for (int i = 0; i < comps.Length; i++)
        {
            if (comps[i].mark == mark)
            {
                comps[i].isIgnore = false;
            }
        }
    }




    public static void ResetAutoIndex(string mark)
    {
        MachineSelectBorder[] comps = GameObject.FindObjectsOfType<MachineSelectBorder>();

        List<MachineSelectBorder> _comps = new List<MachineSelectBorder>();

        MachineSelectBorder comp = null;

        for (int i = 0; i < comps.Length; i++)
        {
            if (comps[i].mark == mark && comps[i].gameObject.active)
            {
                comp = comps[i];
                break;
            }
        }

        if (comp != null && comp.isAutoIndex)
        {
            comp.setAutoIndex();
        }
    }

    private void OnEnable()
    {


        // Debug.Log("【show】: on enable");

        //已经时初始化过了，隐藏按钮重新可见时，
        if ((MachineSelectManager.Instance.isInitGameBtnRegion && globalStore.nowGameID != -1)
        || (MachineSelectManager.Instance.isInitHallBtnRegion && globalStore.nowGameID == -1))
        {
            if (MachineSelectManager.Instance.isChangeButtonRegion())
            {
                List<string> marks = MachineSelectManager.Instance.GetButtonRegionLst();

                if (marks.Contains(mark))
                {
                    MachineSelectBorder.isVisibleReflashRegion = true;
                }

            }
        }
    }


    private void OnDisable()
    {

        Debug.Log("【show】: on disable");

        //已经时初始化过了，隐藏自己前，发现自己选择款可见，

        if ((MachineSelectManager.Instance.isInitGameBtnRegion && globalStore.nowGameID != -1)
        || (MachineSelectManager.Instance.isInitHallBtnRegion && globalStore.nowGameID == -1))
        {
            if (MachineSelectManager.Instance.isChangeButtonRegion())
            {
                List<string> marks = MachineSelectManager.Instance.GetButtonRegionLst();
                // if (marks.Contains(mark) && selectBorder.active)
                if (marks.Contains(mark) && isSelected)
                {
                    isSelected = false;
                    MachineSelectBorder.isVisibleReflashRegion = true;
                }
            }
        }
    }


    void Update()
    {


        if (MachineSelectBorder.isVisibleReflashRegion)
        {
            MachineSelectBorder.isVisibleReflashRegion = false;
            MachineSelectManager.Instance.ReflashGameBtnRegion();
            MachineSelectManager.Instance.ReflashHallBtnRegion();
        }

        if (MachineSelectBorder.isReflashRegion)
        {
            MachineSelectBorder.isReflashRegion = false;
            //在首次进入大厅或进入游戏时，刷新下显示区域
            if (!MachineSelectManager.Instance.isInitGameBtnRegion)
            {
                MachineSelectManager.Instance.ReflashGameBtnRegion();
            }

            if (!MachineSelectManager.Instance.isInitHallBtnRegion)
            {
                MachineSelectManager.Instance.ReflashHallBtnRegion();
            }
        }



    }
}
