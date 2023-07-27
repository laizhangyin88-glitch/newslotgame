using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeCanvas.Framework;
using Sirenix.OdinInspector;

namespace SlotMaker
{
	[CreateAssetMenu(fileName="New ChangeNestedGraph", menuName="SlotMaker/Refactor/ChangeNestedGraph")]
	public class GraphRefactor_ChangeNestedGraph : GraphRefactor
	{
		public GraphType graphType = GraphType.FSM | GraphType.BehaviourTree;
		public List<string> includePaths;
		public List<string> excludePaths;

		public Graph source;
		public Graph target;

		[Button]
		public void Run()
		{
			Run(false);
		}

		public override List<RefactoringResult> Run(bool readOnly)
		{
			return Run(graphType, includePaths, excludePaths, readOnly);
		}

		public override int Run(Graph graph, bool readOnly)
		{
			string json;
			List<UnityEngine.Object> references;
			graph.GetSerializationData(out json, out references);

			int found = 0;

			int count = references.Count;
			for (int i = 0; i < count; ++i)
			{
				if (references[i] == source)
				{
					if (!readOnly)
						references[i] = target;

					++found;
				}
			}

			if (found > 0)
			{
				if (!readOnly)
				{
					graph.Deserialize(json, false, references);
					EditorUtility.SetDirty(graph);
				}
			}

			return found;
		}
	}
}