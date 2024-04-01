using SboxSpace;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//输入框

public class GameInput : MonoBehaviour
{
    public static GameInput Inst;

    public int Input_state = 0;

    public GameObject inputobj;

    private Text input_text;
    private Text ts_text;
   
    private GameObject[] numobj = new GameObject[13];
    
    private List<int> input_list = new List<int>(); //输入信息
    private int input_type = 1;
    private int maxlen = 8  ;
    private int waitnext = 0;
    private int keyindex = 9;
    private float dtime = 0 ;
    private int[] keynum = new int[12] {1,2,3,4,5,6,7,8,9,10,0,11 };
    private float[,] xy = new float[12, 2] { 
    {-136f,51f },{-5f,51f },{125f,51f },
    {-136f,-62f },{-5f,-62f },{125f,-62f },
    {-136f,-181f },{-5f,-181f },{125f,-181f },
    {-136f,-304f },{-5f,-304f },{125f,-304f }
    };

    private int sys_pwd = 0;
    private int sys_macid = 0;

    private void Awake()
    {
        if (Inst == null)
            Inst = this;
    }

    void Start()
    {
       for(int i = 0; i < 13; i++)
       {
            string str = "bj/B" + i.ToString();
            numobj[i]  = this.transform.Find(str).gameObject;
        }
        input_text = this.transform.Find("bj/text_bj/Input").GetComponent<Text>();
        ts_text = this.transform.Find("bj/ts").GetComponent<Text>();

        inputobj.SetActive(false);

    }

    private void OnApplicationQuit()
    {
        Close_input();
    }

    void Update()
    {
        if (inputobj.activeSelf && Input_state == 0 )
        {
            if (waitnext > 1) //等待一下
            {
                dtime += Time.deltaTime;
                if (dtime > 2.0f)
                {
                    dtime = 0;

                    if (waitnext == 2)
                    {
                        Input_state = 1;
                        Close_input(); 
                    }
                    else
                    {
                        input_list.Clear();
                        Display_pswd();
                        ts_text.text = "机器编号";
                        input_type = 1;
                    }
                    waitnext = 0;
                }
            }
            else if (waitnext == 1)
            {
                Set_macid();
            }

            if (IOManager.Inst != null)
            {
                if (waitnext == 0)
                {
                    if (IOManager.Inst.cckeyio.key[6] > 0) //左边
                        UpdateKeyMenu(3);
                    else if (IOManager.Inst.cckeyio.key[5] > 0) //右边
                        UpdateKeyMenu(4);
                    else if (IOManager.Inst.cckeyio.key[7] > 0) //确定
                        UpdateKeyMenu(1);   
                }
            }
        }
    }

    public void Init_input()
    {
        inputobj.SetActive(true);
        Add_imglisten();
        input_list.Clear();
        Display_pswd();
        ts_text.text = "机器编号";
        keyindex = 9;
        numobj[12].transform.localPosition = new Vector3(xy[keyindex, 0], xy[keyindex, 1], 0);
        Input_state = 0;
    }
    public void Close_input()
    {
        Free_imglisten();
        inputobj.SetActive(false);
    }

    private void Add_imglisten()
    {
        for(int i = 0; i < numobj.Length; i++)
        {
            int p = i;
            EventListener.AddEventListenr(numobj[i]).onclick = delegate { Input_pswd(p); };
        }
    }

    private void Free_imglisten()
    {
        for (int i = 0; i < numobj.Length; i++)
        {
            EventListener.RemoveEventListener(numobj[i]);
        }
    }

    //显示输入
    private void Display_pswd()
    {
        string str = "";
        if (input_type == 0)  //密码类型
        {
            for (int i = 0; i < input_list.Count; i++)
                str += " * ";
        }
        else
        {
            for (int i = 0; i < input_list.Count; i++)
                str += input_list[i].ToString();
        }
        input_text.text = str;
    }

    private void Get_curinput()
    {
        if(input_type == 0 )  //密码
        {
            sys_pwd = 0;
            for (int i = 0; i < input_list.Count; i++)
                sys_pwd = sys_pwd * 10 + input_list[i];
            waitnext = 1;
            IO5255.Inst.Send_sysmacid(sys_pwd, sys_macid);
        }
        else
        {
            sys_macid = 0;
            for (int i = 0; i < input_list.Count; i++)
                sys_macid = sys_macid * 10 + input_list[i];
            input_list.Clear();
            Display_pswd();
            if (sys_macid > 9999999)
            {
                ts_text.text = "系统密码";
                input_type = 0;
            }
            else
            {
                ts_text.text = "格式错误";
                waitnext = 3;
            }
        }
    }

    //输入
    private void Input_pswd(int index)
    {
        int index_x = 0;
        if (index == 12)
        {
            index_x = keynum[keyindex];
        }
        else
        {
            index_x = index;

            for (int i = 0; i < 12; i++)
            {
                if (index_x == keynum[i])
                {
                    keyindex = i;
                    numobj[12].transform.localPosition = new Vector3(xy[keyindex, 0], xy[keyindex, 1], 0);
                    break;
                }
            }
        }

        if (index_x == 10)  //删除
        {
            input_list.Clear();
            Display_pswd();
        }
        else
        {
            if (index_x == 11)  //确定
            {
                if (input_list.Count == maxlen)
                {
                    Get_curinput();
                }
            }
            else
            {
                if (input_list.Count < maxlen)
                {
                    input_list.Add(index_x);
                    Display_pswd();
                }
              
            }
        }
    }

        private void UpdateKeyMenu(int key)
        {
                if (key == 1)
                {
                    Input_pswd(keynum[keyindex]);
                }
                else if (key == 4)
                {
                    keyindex--;
                    if (keyindex < 0) keyindex = 11;
                    numobj[12].transform.localPosition = new Vector3(xy[keyindex, 0], xy[keyindex, 1], 0);
                }
                else if (key == 3)
                {
                    keyindex++;
                    if (keyindex > 11) keyindex = 0;
                    numobj[12].transform.localPosition = new Vector3(xy[keyindex, 0], xy[keyindex, 1], 0);
                }
        }

    //设置机台编号
    private void Set_macid()
    {
            if (IO5255.Inst.checkok == 2) //收到，设置成功
            {
                IO5255.Inst.checkok = 0;
                waitnext = 3;
                if (IO5255.Inst.dataSet.checkinfo_ok == 10) //OK 
                {
                    waitnext = 2;
                    ts_text.text = "设置成功";
                }
                else if (IO5255.Inst.dataSet.checkinfo_ok == 1)
                {
                    ts_text.text = "密码错误";
                   // IO5255.Inst.Clear_cmd();
                }
                else 
                {
                    ts_text.text = "重复设置";
                  //  IO5255.Inst.Clear_cmd();
                }
            }
            else if (IO5255.Inst.checkok == 4)
            {
                waitnext = 3;
                IO5255.Inst.checkok = 0;
                ts_text.text = "通讯故障";
            }

    }
}
