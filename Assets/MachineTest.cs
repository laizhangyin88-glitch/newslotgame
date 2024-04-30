using ParadoxNotion;
using SlotMaker;
using SlotMaker.Keno.Events;
using SlotMaker.Tasks.Actions;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
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
                //MachineSelectManager.Instance.BtnAddCoin();
                MachineSelectManager.Instance.PurchaseCreditRequest(1, 10000);//加分
            }

            if (Input.GetKeyUp(KeyCode.KeypadMinus))
            {
                // MachineSelectManager.Instance.BtnMinusCoin();
                Debug.Log($"【printer】: All dollar = {BlackboardUtils.FindVariable<long>(null, "/me/credit").value / 1000}");
                int credit = (int)(BlackboardUtils.FindVariable<long>(null, "/me/credit").value / 1000) * 1000;
                MachineSelectManager.Instance.PurchaseCreditRequest(2, credit, () =>
                {
                });//减分
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



        }
    }
}
