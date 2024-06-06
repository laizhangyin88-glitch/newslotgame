using SlotMaker;
using UnityEngine;

namespace BagelCode
{
    public class MachineTest : MonoBehaviour
    {

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                MachineSelectManager.Instance.BtnNext();
            }

            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                MachineSelectManager.Instance.BtnPre();
            }



            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                /* if (!MachineSelectManager.Instance.isMenuOpen()
                     && !MachineSelectManager.Instance.isPopCommon()
                        //&& !MachineSelectManager.Instance.isPopSysSettingSelect()
                        && MachineSelectManager.Instance.isChangeButtonRegion()) { 

                         MachineSelectManager.Instance.ChangeButtonRegionUp();
                 }*/
                MachineSelectManager.Instance.BtnBetUp();
            }

            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                /* if (!MachineSelectManager.Instance.isMenuOpen()
                     && !MachineSelectManager.Instance.isPopCommon()
                        //&& !MachineSelectManager.Instance.isPopSysSettingSelect()
                        && MachineSelectManager.Instance.isChangeButtonRegion())
                 {
                     MachineSelectManager.Instance.ChangeButtonRegionDown();
                 }
                */
                MachineSelectManager.Instance.BtnBetDown();
            }
            if (Input.GetKeyUp(KeyCode.X))
            {
                MachineSelectManager.Instance.BtnBetMax();

            }



            if (Input.GetKeyUp(KeyCode.KeypadPlus))
            {
                //MachineSelectManager.Instance.PurchaseCreditRequest(1, 10000);

                SBoxSanboxController.Instance.AddCredit(1);//加分
            }

            if (Input.GetKeyUp(KeyCode.KeypadMinus))
            {
                /*
                Debug.Log($"【printer】: All dollar = {BlackboardUtils.FindVariable<long>(null, "/me/credit").value / 1000}");
                int credit = (int)(BlackboardUtils.FindVariable<long>(null, "/me/credit").value / 1000) * 1000;

                SBoxSanboxController.Instance.DecreaseCredit(credit, () =>
                {
                });*/
                SBoxSanboxController.Instance.DecreaseCredit(1);//减分*/
            }




            if (Input.GetKeyUp(KeyCode.H)) //帮助
            {
                MachineSelectManager.Instance.BtnHelp();
            }



            if (Input.GetKeyUp(KeyCode.M)) //菜单
            {
                MachineSelectManager.Instance.BtnMenu();
            }

            if ((Input.GetKeyDown(KeyCode.R)))
            {
                MachineSelectManager.Instance.BtnReturn();

            }
            // Spin
            if (Input.GetKeyDown(KeyCode.F1))
            {
                MachineSelectManager.Instance.BtnSpinDown();
            }
            // Spin
            if (Input.GetKeyUp(KeyCode.F1))
            {
                MachineSelectManager.Instance.BtnSpinUp();
            }


            // K2
            if ((Input.GetKeyDown(KeyCode.F3)))
            {
                MachineSelectManager.Instance.BtnSwitch();

            }

            /*退币
            if ((Input.GetKeyDown(KeyCode.O)))
            {

                SBoxSanboxController[] comps = GameObject.FindObjectsOfType<SBoxSanboxController>();

                if (comps.Length > 0)
                {
                    comps[0].StartCoinOut();
                }
                else
                {
                    Debug.LogError(" 没找到组件 SBoxSanboxController");
                }

            }*/


            /* 测试退币
            if ((Input.GetKeyDown(KeyCode.O)))
            {
                Dictionary<string, object> req = new Dictionary<string, object>{};
                Debug.Log("请求退币");
                NetManager.Instance.Post(RPCName.checkReturnCoin, req,
                (res) =>
                {
                    Debug.Log($" res = {res.ToString()}");
                    Debug.Log($" 退币个数 = {res["money"]}");
                },
                (error) =>
                {
                    Debug.LogError(" 查询退币个数失败");
                });
            }
            if ((Input.GetKeyDown(KeyCode.P)))
            {
                Dictionary<string, object> req = new Dictionary<string, object>
                {
                   {"money",1}, //退币个数
                };
                Debug.Log("开始退币");
                NetManager.Instance.Post(RPCName.returnCoin, req,
                (res) =>
                {
                    Debug.Log($" 退币成功");
                },
                (error) =>
                {
                    Debug.LogError(" 退币失败");
                });
            }*/

            /* if ((Input.GetKeyDown(KeyCode.O)))
             {
                 Dictionary<string, object> req = new Dictionary<string, object>
                 {
                    {"money",5}, //退币个数
                 };
                 Debug.Log("请求充值");
                 NetManager.Instance.Post(RPCName.checkAddDollor, req,
                 (res) =>
                 {
                     if (res["is_success"] == 1)
                     {

                     }
                     Debug.Log($" 请求充值 res = {res.ToString()}");
                 },
                 (error) =>
                 {
                     Debug.LogError(" 请求充值失败");
                 });
             }
             if ((Input.GetKeyDown(KeyCode.P)))
             {
                 Dictionary<string, object> req = new Dictionary<string, object>
                 {
                    {"money",5}, //退币个数
                 };
                 Debug.Log("开始充值");
                 NetManager.Instance.Post(RPCName.agentRechargeToDeviceUser, req,
                 (res) =>
                 {
                     Debug.Log($" 充值成功");
                 },
                 (error) =>
                 {
                     Debug.LogError(" 充值失败");
                 });;
             }*/

            /*if ((Input.GetKeyDown(KeyCode.O)))
            {
                MachineSelectManager.Instance.BtnMinusCoin();
            }*/

            /*

            if ((Input.GetKeyDown(KeyCode.O)))
            {
                SBoxSanboxController.Instance.StartCoinOut();
            }*/



            if ((Input.GetKeyDown(KeyCode.O)))
            {
                SBoxSanboxController.Instance.AddCredit(1);
            }

            if ((Input.GetKeyDown(KeyCode.P)))
            {
                SBoxSanboxController.Instance.DecreaseCredit(1);
            }



        }

    }
}
