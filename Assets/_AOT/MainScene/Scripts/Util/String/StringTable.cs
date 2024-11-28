using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker
{
	public class StringTable : MonoWeakSingleton<StringTable> 
	{
		public Blackboard globalBlackboard;
		public Blackboard contentBlackboard;
		public Blackboard badwordBlackboard;

		public enum StringTableType
		{
			Global,
			Content,
	        Badword
		};

		public static Blackboard Global()
		{
			return Instance.globalBlackboard;
		}

		public static Blackboard Content()
		{
			return Instance.contentBlackboard;
		}

		public static Blackboard Badword()
		{
			return Instance.badwordBlackboard;
		}

		public static Blackboard Get(StringTableType type)
		{
			switch (type)
			{
			case StringTableType.Global:
				return Global();
			case StringTableType.Content:
				return Content();
			case StringTableType.Badword:
				return Badword();
			}

			return null;
		}

	    public static string BadWordFilter(string origText)
	    {
	        var badWordBB = Badword();
	        string filteredText = origText;

	        if (badWordBB != null)
	        {
	            try
	            {
	                string pattern = @"\b{0}\b";
	                var tempList = badWordBB.variables.Values.ToList();
	                for (int i = 0; i < tempList.Count; i++)
	                {
	                    string key = string.Format(pattern, tempList[i].ToString());
	                    var badword = BlackboardUtils.FindVariable<string>(badWordBB, tempList[i].ToString()); 
	                    filteredText = Regex.Replace(filteredText, key, badword.value, RegexOptions.IgnoreCase);
	                }
	            }
	            catch(Exception)
	            {
	                filteredText = origText;
	            }
	        }

	        return filteredText;
	    }
	}
}
