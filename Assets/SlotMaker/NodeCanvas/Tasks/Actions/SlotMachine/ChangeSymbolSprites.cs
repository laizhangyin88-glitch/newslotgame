using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/SlotMachine")]
public class ChangeSymbolSprites : ActionTask
{
    public BBParameter<List<Sprite>> sprites;

    protected override string info { get { return string.Format("ChangeSymbolSprites({0})", sprites); } }

    protected override void OnExecute()
    {
        ContentCustomData.Instance.symbolSprite.sprites = Clone(sprites.value);
        EndAction();
    }

    private List<Sprite> Clone(List<Sprite> sprites)
    {
        List<Sprite> spritesNew = new List<Sprite>();
        for (int i = 0; i < sprites.Count; ++i)
        {
            spritesNew.Add(sprites[i]);
        }
        return spritesNew;
    }
}

}
