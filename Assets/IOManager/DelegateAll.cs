
namespace SboxSpace
{
    /*
    public enum CmdTypeClover
    {

        ResultEnum = 10000,     //开奖结果
        InitEnum,               //初始参数
        LoopEnum,               //每轮参数
        SysEnum,                //系统参数
        SetEnum,                //设置参数
        AccEnum,                //报账数据
        CodeEnum,               //打码数据
        AdvEnum,                //进阶数据
        GetPerEnum,             //获取百分比数据
        SetPerEnum,				//获取百分比数据
        ErrorEnum,              //错误标识，接收到的命令不是上述命令标记为此类型，并按原来数据返回
        Rev5250 = 11111,           //接收加密卡打包数据
    }
*/

    /// <summary>
    /// 检查硬件是否连接
    /// 1:ui 2:加密卡4:系统
    /// </summary>
    /// <param name="kind">硬件失败的ID</param> 
    public delegate void CheckeConnectEorroDelegate(int kind);
    /// <summary>
    /// 开分
    /// </summary>
    public delegate void KeyInDelegate();
    /// <summary>
    /// 洗分
    /// </summary>
    public delegate void KeyOutDelegate();
    /// <summary>
    /// 退币按键
    /// </summary>
    public delegate void CollectDelegate();
    /// <summary>
    /// 投币信号
    /// </summary>
    /// <param name="count">当前投币累加数 最大255</param>
    public delegate void CoinInDelegae(int count);
    /// <summary>
    /// 退币信号
    /// </summary>
    /// <param name="count">当前退币累加数 最大255</param>
    public delegate void CoinOutDelegae(int count);
    /// <summary>
    /// 彩票信号
    /// </summary>
    /// <param name="count">当前彩票累加数 最大255</param>
    public delegate void TickOutDelegae(int count);
    /// <summary>
    /// 进钱数目
    /// </summary>
    /// <param name="count"></param>
    public delegate void MoneyInDelage(int count);
    /// <summary>
    /// 出钱数目
    /// </summary>
    /// <param name="count"></param>
    public delegate void MoneyOutDelage(int count);
    /// <summary>
    /// Bet1
    /// </summary>
    /// <param name="no"></param>
    public delegate void Bet1Deleate();
    /// <summary>
    /// Bet2
    /// </summary>
    /// <param name="no"></param>
    public delegate void Bet2Deleate();
    /// <summary>
    /// Bet3
    /// </summary>
    /// <param name="no"></param>
    public delegate void Bet3Deleate();
    /// <summary>
    /// Bet4
    /// </summary>
    /// <param name="no"></param>
    public delegate void Bet4Deleate();
    /// <summary>
    /// Bet5
    /// </summary>
    /// <param name="no"></param>
    public delegate void Bet5Deleate();
    /// <summary>
    /// X1
    /// </summary>
    /// <param name="no"></param>
    public delegate void X1Deleate();
    /// <summary>
    /// X2
    /// </summary>
    /// <param name="no"></param>
    public delegate void X2Deleate();
    /// <summary>
    /// X3
    /// </summary>
    /// <param name="no"></param>
    public delegate void X3Deleate();
    /// <summary>
    /// X4
    /// </summary>
    /// <param name="no"></param>
    public delegate void X4Deleate();
    /// <summary>
    /// X5
    /// </summary>
    /// <param name="no"></param>
    public delegate void X5Deleate();
    /// <summary>
    /// 接收算法卡数据
    /// </summary>
    /// <param name="packet"></param>
    public delegate void RevDataIdeaDeleate( int cmd, int[] data);
    /// <summary>
    /// io 初始化 0为成功，其他为失败
    /// </summary>
    /// <param name="flag"></param>
    public delegate void IntIoParameterDeleate(int flag);
    /// <summary>
    /// 小键盘按键信息
    /// </summary>
    /// <param name="flag"></param>
    public delegate void KeypadDeleate(int no);
    /// <summary>
    /// 退币超时
    /// </summary>
    public delegate void CoinOutTimeDelegate();
    /// <summary>
    /// 退彩票超时
    /// </summary>
    public delegate void TicketOutTimeDelegate();
    /// <summary>
    /// 退币或者退彩票是否发送到达 0为到达成功
    /// </summary>
    /// <param name="flag"></param>
    public delegate void BackSendCoinTicketDelegate(int flag);
    /// <summary>
    /// 返回灯打开是否成功 0为打开成功
    /// </summary>
    /// <param name="flag"></param>
    public delegate void BackSendLightOpenDelegate(int flag);
    /// <summary>
    /// 返回灯关闭是否成功 0为打开成功
    /// </summary>
    /// <param name="flag"></param>
    public delegate void BackSendLightCloseDelegate(int flag);
    /// <summary>
    /// 其他干扰数据异常提示
    /// </summary>
    /// <param name="info"></param>
    public delegate void RevOtherDataDelegate(string info);
    /// <summary>
    /// 设置/测试键
    /// </summary>
    public delegate void SetOrTestKeyDelegate();
    /// <summary>
    /// 报账键
    /// </summary>
    public delegate void AccountKeyDelegate();
    /// <summary>
    /// 写存储数据返回
    /// </summary>
    /// <param name="success"></param>
    public delegate void WriteDataBackDelegate(int success);
    /// <summary>
    /// 读存储数据返回
    /// </summary>
    /// <param name="success"></param>
    /// <param name="data"></param>
    public delegate void ReadDataBackDelegate(int[] data);
    /// <summary>
    /// (比倍/说明键)
    /// </summary>
    public delegate void GambleOrHelpDelegate();
    /// <summary>
    /// (取分/自动键)
    /// </summary>
    public delegate void TakeWinOrAutoDelegate();
    /// <summary>
    /// (服务键)
    /// </summary>
    public delegate void ServiceDelegate();
    /// <summary>
    /// Play (玩/重复投注键)
    /// </summary>
    public delegate void StartPlayDelegate();
    /// <summary>
    /// 最大开始
    /// </summary>
    public delegate void MaxStartDelegate();
    /// <summary>
    /// 最大开始
    /// </summary>
    public delegate void NormalStartDelegate();
    /// <summary>
    /// 打印机初始化
    /// </summary>
    public delegate void PrintIntDelegate(int ret);
    /// <summary>
    /// 打印机浓度
    /// </summary>
    public delegate void PrintNDDelegate( int ret);
    /// <summary>
    /// 打印机字体
    /// </summary>
    public delegate void PrintFontDelegate(int ret);
    /// <summary>
    /// 打印机行距
    /// </summary>
    public delegate void PrintLineDelegate( int ret);
    /// <summary>
    /// 打印机切纸
    /// </summary>
    public delegate void PrintCutDelegate(int ret);
    /// <summary>
    /// 打印机内容
    /// </summary>
    public delegate void PrintContentDelegate(int ret);
    /// <summary>
    /// 打印机电磁铁
    /// </summary>
    public delegate void PrintElecDelegate(int a,int b);
    /// <summary>
    /// 打印机马达
    /// </summary>
    public delegate void PrintMotorDelegate(int a,int b);
    /// <summary>
    /// 打印机状态
    /// </summary>
    public delegate void PrintStateDelegate(int[] ret);

    public delegate void CasherStatusProcess(int cmd, int[] ret);
    public delegate void PrinterStatusProcess(int cmd, int[] ret);

}




