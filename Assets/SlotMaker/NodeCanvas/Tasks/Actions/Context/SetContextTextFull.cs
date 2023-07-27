using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

using NodeCanvas.Framework.Internal;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SetContextTextFull : ActionTask<ContextElement>
{
    public BBParameter<string> elementName;
    public ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch;
    
    public BBParameter<string> key;
    public StringTable.StringTableType tableType;

    public List<BBObjectParameter> args = new List<BBObjectParameter>();
    
    protected override string info
    {
        get 
        {
            if (key != null && !key.isNone)
            {
                return string.Format("{0}.Find({1}).text = ({2})({3}{4})", agentInfo, elementName, tableType, key, (args == null) ? "<b>NULL</b>" : ArgsToString());
            }
            else
            {
                return string.Format("{0}.Find({1}).text = {2}", agentInfo, elementName, (args != null && args.Count > 0) ? args[0].ToString() : "<b>NULL</b>");
            }
        }
    }
    
    protected override void OnExecute()
    {
        bool error = false;
        
        var textElement = ContextUtils.FindElement(agent, elementName.value, searchingType) as IContextText;
        if (textElement != null)
        {
            if (key != null && !key.isNone)
            {
                textElement.SetText(StringTableUtils.GetString(tableType, key.value, out error, ArgsToObjectArray()));
            }
            else
            {
                textElement.SetText(args[0].value.ToString());
            }
        }
        else 
        {
            error = true;
            
            if (ApplicationSettings.LogSystem())
                Debug.LogWarning("[Context] " + elementName.value + " is not exist.");
        }
        
        EndAction(!error);
    }
    
    private object[] ArgsToObjectArray()
    {
        int count = args.Count;
        if (count == 0) return null;
        
        object[] array = new object[count];
        for (int i = 0; i < count; ++i)
        {
            array[i] = args[i].value;
        }
        return array;
    }
    
    private string ArgsToString()
    {
        var builder = new StringBuilder("");
        for (int i = 0; i < args.Count; ++i)
        {
            builder.Append(", ");
            builder.Append(args[i].ToString());
        }
        return builder.ToString();
    }
}

}
