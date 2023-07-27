using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Symbol")]
public class SymbolSetMaterialByIndex : ActionTask<BaseSymbol>
{
    public BBParameter<MeshRenderer> meshRenderer;
    public BBParameter<int> materialId;
    public BBParameter<int> index;

    protected override string info { get { return string.Format("{0}.meterials[{1}] = {2}", meshRenderer, index, materialId); } }

    protected override void OnExecute()
    {
        var material = agent.symbolAssets.GetMaterial(agent.symbolInfo.symbol, materialId.value);
        var materials = meshRenderer.value.materials;
        materials[index.value] = material;
        meshRenderer.value.materials = materials;

        EndAction();
    }
}

}
