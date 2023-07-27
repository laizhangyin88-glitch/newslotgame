using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SlotMaker.Json;
using Sirenix.OdinInspector;

namespace SlotMaker
{
	[CreateAssetMenu(fileName="New ChangeStringValue", menuName="SlotMaker/Refactor/ChangeStringValue")]
	public class GraphRefactor_ChangeStringValue : GraphRefactor
	{
		public GraphType graphType = GraphType.FSM | GraphType.BehaviourTree;
		public List<string> includePaths;
		public List<string> excludePaths;

		public string source;
		public string target;
		public bool replace;

		const string TYPE_VALUE = "_value";

		[Button]
		public void Run()
		{
			Run(false);
		}

		public override List<RefactoringResult> Run(bool readOnly)
		{
			return Run(graphType, includePaths, excludePaths, readOnly);
		}

		protected override int VisitProperty(JsonObject property, bool readOnly)
		{
			int found = 0;

			object value;
			if (property.TryGetValue(TYPE_VALUE, out value))
			{
				if (value is string)
				{
					if (Compare((string)value))
					{
						if (!readOnly)
							property[TYPE_VALUE] = Replace((string)value);
						
						++found;
					}
				}
			}

			return found;
		}

		bool Compare(string text)
		{
			if (replace)
				return text.Contains(source);
			else 
				return string.Equals(text, source);
		}

		string Replace(string text)
		{
			if (replace)
				return text.Replace(source, target);
			else
				return target;
		}
	}
}