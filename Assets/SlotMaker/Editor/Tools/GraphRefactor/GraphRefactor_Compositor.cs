using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

namespace SlotMaker
{
	[CreateAssetMenu(fileName="New Compositor", menuName="SlotMaker/Refactor/Compositor")]
	public class GraphRefactor_Compositor : GraphRefactor
	{
		public List<GraphRefactor> refactors;

		[Button]
		public void Run()
		{
			Run(false);
		}

		public override List<RefactoringResult> Run(bool readOnly)
		{
			var result = new List<RefactoringResult>();
			
			foreach (var refactor in refactors)
			{
				result.AddRange(refactor.Run(readOnly));
			}

			return result;
		}
	}
}