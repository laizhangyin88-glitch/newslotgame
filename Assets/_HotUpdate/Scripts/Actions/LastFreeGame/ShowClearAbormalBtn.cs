using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{
    [Category("★ Extend/LastFreeGame")]
    public class ShowClearAbormalBtn : ActionTask
    {
        public BBParameter<float> delay;

        protected override string info => $"{delay.value}秒后显示断线重连界面的异常修复按钮";

        protected override void OnExecute()
        {
            LoginMaskController ui = PopupManager.Instance.GetComponentInChildren<LoginMaskController>();
            if(ui == null)
            {
                EndAction();
                return;
            }

            ui.DelayShowTips(delay.value, () =>
            {
                EndAction();
            });
        }
    }

}
