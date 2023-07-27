using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SlotMaker.Json;
using Sirenix.OdinInspector;

namespace SlotMaker
{
	[CreateAssetMenu(fileName="New Rename", menuName="SlotMaker/Refactor/Rename")]
	public class GraphRefactor_Rename : GraphRefactor
	{
		public GraphType graphType = GraphType.FSM | GraphType.BehaviourTree;
		public List<string> includePaths;
		public List<string> excludePaths;

		public string source;
		public string target;
		public bool replace;

		const string TYPE_NAME = "_name";

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

			object name;
			if (property.TryGetValue(TYPE_NAME, out name))
			{
				if (Compare((string)name))
				{
					if (!readOnly)
						property[TYPE_NAME] = Replace((string)name);

					++found;
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