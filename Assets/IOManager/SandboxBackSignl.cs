using System;
using System.Runtime.InteropServices;

[Serializable, StructLayout(LayoutKind.Sequential, Pack=1)]

public struct CmdBackSignl   //命令反馈的信号
{

    public int   cmd_err ;  //指令错误标志

    public int   light_open_success;  //开灯操作成功
    public int   light_close_success; //关灯操作成功
    public int   coin_payout_success;  //退币成功 
    public int   coin_stopout_success;  //停止退币成功 
    
    public int   codetab_open_success ;  //码表走起
    public int   codetab_close_success ;  //码表停下
    
    public int   data_set_success ; //设定日期

    public CmdBackSignl(int n)
    {

       this.cmd_err = 0;  //指令错误标志

        this.light_open_success = 0x10;  //开灯操作成功
        this.light_close_success=0x10; //关灯操作成功？
        this.coin_payout_success=0x10;  //退币成功 
        this.coin_stopout_success = 0x10;  //停止退币成功 
    
        this.codetab_open_success = 0x10;  //码表走起
        this.codetab_close_success = 0x10;  //码表停下
        this.data_set_success = 0x10; //设定日期
    }
}
/*****
  4种状态：0:成功 0x01 失败  0x11 进行中  0x10 不操作  
  *****/