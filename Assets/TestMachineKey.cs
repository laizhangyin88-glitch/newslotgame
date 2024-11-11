using SlotMaker;
using UnityEngine;
using static SBoxApi.SBoxSandbox;

public class TestMachineKey : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (ApplicationSettings.Instance.isMachine)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                MachineSelectManager.Instance.BtnSpinDOWN();
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                MessageDispatcher.Dispatch("MachineBtnEvent", new ParadoxNotion.EventData<int>("LightBtnSelect", 0));
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                MessageDispatcher.Dispatch("MachineBtnEvent", new ParadoxNotion.EventData<int>("LightBtnSelect", 1));
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                MessageDispatcher.Dispatch("MachineBtnEvent", new ParadoxNotion.EventData<int>("LightBtnSelect", 2));
            }


            //{ "BtnSpin", SBOX_SWITCH.SWITCH_ENTER },
            //{ "BtnPre", SBOX_SWITCH.SWITCH_YELLOW },
            //{ "BtnNext", SBOX_SWITCH.SWITCH_BET4 },
            //{ "BtnExit", SBOX_SWITCH.SWITCH_SWITCH },
            //{ "BtnSwitch", SBOX_SWITCH.SWITCH_BET5 },
            //{ "BtnBetUp", SBOX_SWITCH.SWITCH_RED },
            //{ "BtnBetDown", SBOX_SWITCH.SWITCH_GREEN },
            //{ "BtnBetMax", SBOX_SWITCH.SWITCH_AUTO},
            //{ "BtnHelp", SBOX_SWITCH.SWITCH_ESC},

            ////spin按钮
            if (Input.GetKeyDown(KeyCode.Q))///
            {
                MachineSelectManager.Instance.BtnPreDOWN();
            }
            if (Input.GetKeyUp(KeyCode.Q))
            {
                MachineSelectManager.Instance.BtnPreUP();
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                MachineSelectManager.Instance.BtnNextDOWN();
            }
            if (Input.GetKeyUp(KeyCode.W))
            {
                MachineSelectManager.Instance.BtnNextUP();
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                MachineSelectManager.Instance.BtnSwitchDOWN();
            }

            if (Input.GetKeyUp(KeyCode.S))
            {
                MachineSelectManager.Instance.BtnReturnDOWN();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                MachineSelectManager.Instance.BtnBetDownDOWN();
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                MachineSelectManager.Instance.BtnBetUpDOWN();
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                MachineSelectManager.Instance.BtnBetMaxDOWN();
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                MachineSelectManager.Instance.BtnHelpDOWN();
            }
        }
    }
}
