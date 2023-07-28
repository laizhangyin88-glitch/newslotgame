using SlotMaker;

public class MRSSymbolEventHandler : DefaultSymbolEventHandler
{
    public override void Apply(BaseSymbol symbol)
    {
        if ((symbol.column == 0 || symbol.column == 2) && symbol.symbolIndex == 0)
        {
            symbolPresets[symbol.symbolIndex].value[0].spriteIndex = 2;
            symbolPresets[symbol.symbolIndex].value[1].spriteIndex = 3;
        }
        else
        {
            symbolPresets[symbol.symbolIndex].value[0].spriteIndex = 0;
            symbolPresets[symbol.symbolIndex].value[1].spriteIndex = 1;
        }
        base.Apply(symbol);
    }
}
