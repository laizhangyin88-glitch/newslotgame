using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/SlotMachine")]
public class ChangeSymbolSprite : ActionTask
{
    public BBParameter<int> symbolIndex;
    public BBParameter<Sprite> sprite;

    protected override string info { get { return string.Format("ChangeSymbolSprite({0}, {1})", symbolIndex, sprite); } }

    protected override void OnExecute()
    {
        ContentCustomData.Instance.symbolSprite.sprites[symbolIndex.value] = sprite.value;

        EndAction();
    }
}

}
