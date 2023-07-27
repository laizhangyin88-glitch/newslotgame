using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Simulator")]
    public class DevSetApplicationURL : ActionTask
    {
        protected override string info
        {
            get
            { 
                return "Set Dev Base URL"; 
            }
        }

        protected override void OnExecute()
        {
#if DEV
            PlayerPrefs.SetString("Custom_URL", "");
            string devURL = PlayerPrefs.GetString("Dev_URL", "");
            bool isEnablePopup = PlayerPrefs.GetInt("Is_Enable_Change_URL_Popup", 0) == 0;

            if(!string.IsNullOrEmpty(devURL))
            {
                if (isEnablePopup)
                {
                    ErrorPopupInfo popupInfo = new ErrorPopupInfo();

                    popupInfo.type = ErrorPopupType.YesNo;
                    popupInfo.text = string.Format("<style=body>{0}\n Continue this URL?", devURL);
                    popupInfo.buttonText1 = "Default Server";
                    popupInfo.buttonText2 = "Continue";

                    popupInfo.callback1 = delegate
                    {
                        PlayerPrefs.SetInt("Is_Enable_Change_URL_Popup", 1);
                        EndAction();
                    };

                    popupInfo.callback2 = delegate
                    {
                        PlayerPrefs.SetString("Custom_URL", devURL);
                        PlayerPrefs.SetInt("Is_Enable_Change_URL_Popup", 1);
                        EndAction();
                    };

                    ErrorPopupHandler.Instance.OpenError(popupInfo);
                }
                else
                {
                    PlayerPrefs.SetString("Custom_URL", devURL);
                    EndAction();
                }
            }
            else
            {
                EndAction();
            }
#else
            EndAction();
#endif
        }
    }

}
