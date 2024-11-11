using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BagelCode
{

    public enum ErrorPopupType
    {
        /// <summary>
        /// 无显示
        /// </summary>
        None = 0,
        /// <summary>
        /// 只显示text文本
        /// </summary>
        TextOnly,
        /// <summary>
        /// 显示text文本和btn1按钮
        /// </summary>
        OK,
        /// <summary>
        /// 显示title标题，text文本和btn1按钮
        /// </summary>
        OkWithTitle,

        YesNo,
        /// <summary>
        /// 显示text文本和btn1按钮
        /// </summary>
        /// <remarks>
        /// 点击按钮会回到登录界面
        /// </remarks>
        SystemReset,
        /// <summary>
        /// 三个按钮，取消，确认，关闭
        /// </summary>
        YesNoClose,
    }

    public class ErrorPopupInfo
    {
        public ErrorPopupType type;

        public string title;
        public string text;
        public string buttonText1;
        public string buttonText2;

        public bool useXButton = false;

        public bool buttonAutoClose1 = true;
        public bool buttonAutoClose2 = true;

        public UnityAction callback1;
        public UnityAction callback2;
        public UnityAction callbackX;
    }

}
