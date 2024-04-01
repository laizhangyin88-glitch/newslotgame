
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SboxSpace
{
    public class DataSet
    {
        public int ver = 0;
        public int token = 0;
        public int lineid = 0 ;
        public int macid  = 0 ;
        
      
        public int b_PeriodLevel = 1;  //牌路
        public int OneCoinVal = 1;
        public int BetTime = 1;        //倒计时
        public int COINX   = 1;         //投币代分
        public int LOTX    = 1 ;         //彩票-1币
        public int LOTX1   = 1 ;        //彩票-1张       

        public int MinSBet = 10;         //最小投注
        public int MaxSBet = 1000000;    //最大投注

        public int MaxTBet  = 0;         //限注
        public int MaxTWin  = 3000;     //限红

        public int[] Cm  = new int[3] { 1,10,100};


        //筹码
         public int b_PeriodMaxLose = 30000; //爆机
        public int g_display_mode = 0 ;//0分机1主机
        public int MaxPlayer = 11 ;

        public int PASSWORD = 0;  

       //算吗
        public int needsm = 0 ;   //是否需要算码
        public int zs = 0 ;      //算码基本信息
        public int zt = 0 ;
        public int smcount = 0;
        public int checkcode = 0 ; //验证码
        public int realcode = 0;

        public int checkinfo_ok = 0 ;  //检测信息是否OK,密码-算码等
        public int get5x3buf_ok = 0;

        public int[] result = new int[100];    //牌路
        public int[] lucky = new int[100];  //奖励标志
        public int[] printresult = new int[100] ; //打印结果
        public int  ju = 1 ;          //局数
        public int  gamecount = 1;  //场次 
        public int  mp_type = 0 ; //明牌
        public int  first_pk = 0 ; //第一个结果
        public int  two_pk = 0 ; //第二个结果
        public int  lucky_type = 0; //奖励类型
        public int  randjack = 0 ; //随机彩金
        public int  needprint = 0;
        
        public int[,] playerbet = new int[11,5];
        public int[]  playerwon = new int[11]  ;
        public int[]  playerjactype = new int[11];
        public int[]  playeraddrate = new int[11];
        public int[]  playerbounsmode = new int[11];
        public int[]  playerresult1 = new int[11];
        public int[]  playerresult2 = new int[11];

        public bool UseBox = false;
        public bool UseHoldBall = false;

        //主机账目
        public int cur_playin = 0 ;
        public int cur_playout = 0;


        public int mingpaiid = 0;
        public int bonusid = 0;

        public int mega_base = 1000000;
        public int major_base= 50000;
        public int minor_base= 5000;
        public int mini_base=  1000;

        public int mega_curr = 1000000;
        public int major_curr = 50000;
        public int minor_curr = 5000;
        public int mini_curr = 1000;

        public int mega_add =  1;
        public int major_add = 1;
        public int minor_add = 1;
        public int mini_add =  1;

        public int jack_type = 0;
        public int[,] rate_list = new int[2,5] { { 38, 38, 40, 40, 200 },{ 38, 38, 40, 40, 200 }};
        public int rate_index = 0;

        public int[] ballSet = new int[10] {1,2,3,4,5,0,0,0,0,0};

        public int currTimeLong = 0;


    }
}