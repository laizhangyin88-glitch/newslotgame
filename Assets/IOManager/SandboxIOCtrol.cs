using System;
using System.Runtime.InteropServices;
public enum bdKeyCode
{
    Key_Up = 100,      
    Key_Down = 104,
    Key_Left = 102,
    Key_Right = 103,

    
    Key_KeyOut = 17,    //下分键
    Key_CoinOut = 101,  //退币键
    Key_Cancle = 9,     
    Key_Confirm = 8,

    Key_Bet1 = 0,       //停1键, 闪电键
    Key_Bet2 = 2,       //停2键, 返回游戏
    Key_Bet3 = 4,       //停3键, 面额选择
    Key_Bet4 = 6,       //停4键, 行线选择
    Key_Bet5 = 12,       //停5键, 押分切换
    Key_Undefine = 30,
    Key_Rules = 18,     //说明键
    Key_Auto = 14,      //自动键
    Key_Tab = 16,       //最大押分
    Key_KeyUnkown1 = 23,//开分键
    Key_KeyUnkown2 = 29,//洗分键
    Key_KeyCoinIn  = 24, 

    Key_Set = 28,      //设置键
    Key_Start = 10,    //Start
    Key_Account = 27,  //查账键
};
public enum smKey
{
    smKey_OK = 1,
    smKey_DOWN = 0,
    smKey_Game = 2,
};
public enum IOKey3588
{
    //[0]
    Key_Set = 3,      //设置按键
    Key_Start = 6,      //开始按键
    Status_IOBoard = 20,//IO板状态
    Status_Door9 = 21,  //门状态
    Status_Door6 = 22,  //门状态
    Status_Door3 = 23,  //门状态
    Status_Casher1 = 24,//纸钞机开关1
    Status_Casher2 = 25,//纸钞机开关2
    Status_Door8 = 26,  //门状态
    Status_MDoor = 27,  //门状态
    Status_Door5 = 28,  //门状态
    Status_Door4 = 29,  //门状态
    Status_Door2 = 30,  //门状态
    Status_Door1 = 31,  //门状态
    //[1]
    Status_Battery = 37,//电池状态
    Status_ExtraDoor = 38,//机壳门状态
    Status_ExtraKeySet = 39,//机壳设置
    //[2]
    Status_CFCard = 0,//CF卡状态
}
public enum Light3588
{
    Light_Start = 10
}
public enum Light3288
{
    Light_Bet1 = 13,
    Light_Bet2 = 11,
    Light_Bet3 = 9,
    Light_Bet4 = 23,
    Light_Bet5 = 10,

    Light_DownSco = 21,
    Light_Start = 12,
    Light_Auto = 8,
    Light_Select = 22,
    Light_Help = 20,
};

[Serializable, StructLayout(LayoutKind.Sequential, Pack = 1)]


public struct SandboxIOCtrol
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    public byte[] skey;  //8个小键盘 
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    public byte[] swtab;  //拨表 
    [MarshalAs(UnmanagedType.ByValArray, SizeConst=32)]  //19后是保留的不管
    public byte[] key;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]  //
    public byte[] iokey3588;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]  //
    public byte[] iokey35881;
    public int coinouttime; //退币超时 
    public int tickouttime;  //退彩票超时 

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
    public int[] counter;    //12个计数器,0退币,1退彩票,2投币,3纸币进,4纸币出

    public string ver;    // 版本
    public bool m_connt;  //主板连接 
    public bool i_connt;  //算法卡连接
    public int io_init;  //io控制板初始化

    public int cmd_err;  //指令错误标志

    public int m_cmd_success; //发送主板指令是否成功
    public int i_cmd_success; //发送算法卡指令是否成功
    public bool update;       //更新数据

    public SandboxIOCtrol(int n)
    {
        this.skey = new byte[8];
        this.swtab = new byte[8];
        this.key = new byte[32];
        this.iokey3588 = new byte[64];
        this.iokey35881 = new byte[64];
        this.iokey3588[(int)IOKey3588.Status_MDoor] = 1;
        this.coinouttime = 0;
        this.tickouttime = 0;

        this.counter = new int[12];

        this.ver = "";    // 版本
        this.m_connt = false;  //主板连接 
        this.i_connt = false;  //算法卡连接
        this.io_init = 1;  //io控制板初始化

        cmd_err = 0;
        m_cmd_success = 0x10; //4种状态：0:成功 0x01 失败  0x11 进行中  0x10 不操作  
        i_cmd_success = 0x10;

        update = false ;
    }
}

[Serializable, StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SandboxData
{
    public int year;
    public int month;
    public int day;
    public int hour;
    public int minute;
    public int saveok;
    public SandboxData(int n)
    {
        year = 2022;
        month = 1;
        day = 1;
        hour = 0;
        minute = 0;
        saveok = 0;
    }
}
