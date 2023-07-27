using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using TMPro;

public class CTCWildSymbolBehaviour : SymbolBehaviour
{
    public override void OnEntry() {
        symbol.GetComponentInChildren<TextMeshProUGUI>(true).gameObject.SetActive(false);
        symbol.GetComponentsInChildren<SpriteRenderer>(true)[1].gameObject.SetActive(false);
    }
    public override void OnWin() { GetCachedObject(1).SetActive(true); PlayAnimation("Invisible"); }
    public override void OnSkip()
    {
        GetCachedObject(1).SetActive(false); GetCachedObject(0).SetActive(false); PlayAnimation("Idle"); }
    public override void OnStopEffect() { GetCachedObject(0).SetActive(true); PlayAnimation("Idle"); }
    public override void OnPrepareStop() { }
}
