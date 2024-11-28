using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

namespace SlotMaker
{
	public static class StringTableUtils
	{
		public static IFormatProvider customProvider;

		public static Variable<string> GetVariable(StringTable.StringTableType tableType, string key)
		{
			var bb = StringTable.Get(tableType);
			if (bb == null)
			{
				Debug.LogError("[StringTable] Could not be found table " + tableType);
				return null;
			}

			return bb.GetVariable<string>(key);
		}

	    public static string GetString(StringTable.StringTableType tableType, string key, params object[] args)
	    {
	        var variable = GetVariable(tableType, key);
	        if (variable == null)
	        {
	            Debug.LogError("[StringTable] Could not be found (" + tableType + ") $" + key);
	            return string.Format("<color=red>${0}</color>", key);
	        }
	        return string.Format(customProvider, variable.value, args);
	    }

		public static string GetString(StringTable.StringTableType tableType, string key, out bool error)
		{
			error = false;
			var variable = GetVariable(tableType, key);
			if (variable == null)
			{
				error = true;
				Debug.LogError("[StringTable] Could not be found (" + tableType + ") $" + key);
				return string.Format("<color=red>${0}</color>", key);
			}
			return variable.value;
		}

		public static string GetString(StringTable.StringTableType tableType, string key, object arg1, out bool error)
		{
			error = false;
			var variable = GetVariable(tableType, key);
			if (variable == null)
			{
				error = true;
				Debug.LogError("[StringTable] Could not be found (" + tableType + ") $" + key);
				return string.Format("<color=red>${0}</color>", key);
			}
			return string.Format(customProvider, variable.value, arg1);
		}

		public static string GetString(StringTable.StringTableType tableType, string key, object arg1, object arg2, out bool error)
		{
			error = false;
			var variable = GetVariable(tableType, key);
			if (variable == null)
			{
				error = true;
				Debug.LogError("[StringTable] Could not be found (" + tableType + ") $" + key);
				return string.Format("<color=red>${0}</color>", key);
			}
			return string.Format(customProvider, variable.value, arg1, arg2);
		}

		public static string GetString(StringTable.StringTableType tableType, string key, object arg1, object arg2, object arg3, out bool error)
		{
			error = false;
			var variable = GetVariable(tableType, key);
			if (variable == null)
			{
				error = true;
				Debug.LogError("[StringTable] Could not be found (" + tableType + ") $" + key);
				return string.Format("<color=red>${0}</color>", key);
			}
			return string.Format(customProvider, variable.value, arg1, arg2, arg3);
		}

		public static string GetString(StringTable.StringTableType tableType, string key, object arg1, object arg2, object arg3, object arg4, out bool error)
		{
			error = false;
			var variable = GetVariable(tableType, key);
			if (variable == null)
			{
				error = true;
				Debug.LogError("[StringTable] Could not be found (" + tableType + ") $" + key);
				return string.Format("<color=red>${0}</color>", key);
			}
			return string.Format(customProvider, variable.value, arg1, arg2, arg3, arg4);
		}

		public static string GetString(StringTable.StringTableType tableType, string key, object arg1, object arg2, object arg3, object arg4, object arg5, out bool error)
		{
			error = false;
			var variable = GetVariable(tableType, key);
			if (variable == null)
			{
				error = true;
				Debug.LogError("[StringTable] Could not be found (" + tableType + ") $" + key);
				return string.Format("<color=red>${0}</color>", key);
			}
			return string.Format(customProvider, variable.value, arg1, arg2, arg3, arg4, arg5);
		}

	    public static string GetString(StringTable.StringTableType tableType, string key, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, out bool error)
	    {
	        error = false;
	        var variable = GetVariable(tableType, key);
	        if (variable == null)
	        {
	            error = true;
	            Debug.LogError("[StringTable] Could not be found (" + tableType + ") $" + key);
	            return string.Format("<color=red>${0}</color>", key);
	        }
	        return string.Format(customProvider, variable.value, arg1, arg2, arg3, arg4, arg5, arg6);
	    }

	    public static string GetString(StringTable.StringTableType tableType, string key, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, out bool error)
	    {
	        error = false;
	        var variable = GetVariable(tableType, key);
	        if (variable == null)
	        {
	            error = true;
	            Debug.LogError("[StringTable] Could not be found (" + tableType + ") $" + key);
	            return string.Format("<color=red>${0}</color>", key);
	        }
	        return string.Format(customProvider, variable.value, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
	    }

	    public static string GetString(StringTable.StringTableType tableType, string key, out bool error, params object[] args)
	    {
	        error = false;
	        var variable = GetVariable(tableType, key);
	        if (variable == null)
	        {
	            error = true;
	            Debug.LogError("[StringTable] Could not be found (" + tableType + ") $" + key);
	            return string.Format("<color=red>${0}</color>", key);
	        }
	        
	        if (args != null)
	            return string.Format(customProvider, variable.value, args);
	        else 
	            return string.Format(customProvider, variable.value);
	    }

	    public static void UpdateString(StringTable.StringTableType tableType, Dictionary<string, string> stringDict)
	    {
	        var bb = StringTable.Get(tableType);
	        if (bb == null)
	        {
	            Debug.LogError("[StringTable] Could not be found table " + tableType);
	            return;
	        }

	        foreach(KeyValuePair<string, string> stringData in stringDict)
	        {
	        	BlackboardUtils.SetOrCreateValue<string>(bb, stringData.Key, stringData.Value);
	        }
	    }
	}
}
