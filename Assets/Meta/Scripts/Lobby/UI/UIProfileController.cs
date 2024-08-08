using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIProfileController : MonoBehaviour
{
    [SerializeField] private Image _iconCom;
    [SerializeField] private TextMeshProUGUI _idCom;
    [SerializeField] private Text _levelCom;
    [SerializeField] private GameObject _vipCom;
    [SerializeField] private Button _btnCom;

    // Start is called before the first frame update
    void Start()
    {
        _btnCom.onClick.AddListener(() =>
        {
            SlotMaker.Tasks.Actions.OpenLobbyPopup.LoadAndOpenLobbyPopup("lobby", "Popup_Profile", true, this.gameObject);
        });
    }

    // Update is called once per frame
    void Update()
    {

    }



    public Sprite GetProfile(string name)
    {
        return null;
    }
}
