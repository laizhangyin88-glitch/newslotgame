using UnityEngine;
using System.Collections;

namespace SlotMaker
{

public static class ContextUtils
{
	public static ContextElement FindElement(ContextElement parentElement, string elementName, ContextSearchingType searchingType)
	{
		switch (searchingType)
		{
		case ContextSearchingType.ChildrenSearch:
            return parentElement.Find(elementName);
        case ContextSearchingType.ChildrenDeepSearch:
            return parentElement.Find(elementName, true);
        case ContextSearchingType.FullNameSearch:
            return parentElement.FindWithFullName(elementName);
        case ContextSearchingType.SelfContext:
        	return parentElement;
		}

		return null;
	}

    public static void SetText(ContextElement element, string text)
    {
        IContextText textElement = element as IContextText;
        if (textElement != null)
            textElement.SetText(text);
    }

    public static void SetGlobalText(ContextElement element, string key)
    {
        bool error;
        IContextText textElement = element as IContextText;
        if (textElement != null)
            textElement.SetText(StringTableUtils.GetString(StringTable.StringTableType.Global, key, out error));
    }

    public static void SetGlobalText(ContextElement element, string key, params object[] args)
    {
        IContextText textElement = element as IContextText;
        if (textElement != null)
            textElement.SetText(StringTableUtils.GetString(StringTable.StringTableType.Global, key, args));
    }

    public static void SetContentText(ContextElement element, string key, params object[] args)
    {
        IContextText textElement = element as IContextText;
        if (textElement != null)
            textElement.SetText(StringTableUtils.GetString(StringTable.StringTableType.Content, key, args));
    }
}

}
