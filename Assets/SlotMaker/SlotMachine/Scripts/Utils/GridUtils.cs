using UnityEngine;
using System.Collections;

namespace SlotMaker
{
	public static class GridUtils
	{
		public static void Copy(IGrid dst, IGrid src)
		{
			dst.BeginColumn    = src.BeginColumn;
			dst.BeginRow       = src.BeginRow;
			dst.EndColumn      = src.EndColumn;
			dst.EndRow         = src.EndRow;
			dst.ExpandTopCount = src.ExpandTopCount;
		}

		public static void Merge(IGrid grid1, IGrid grid2)
		{
			grid1.BeginColumn    = Mathf.Min(grid1.BeginColumn,    grid2.BeginColumn);
			grid1.BeginRow       = Mathf.Min(grid1.BeginRow,       grid2.BeginRow);
			grid1.EndColumn      = Mathf.Max(grid1.EndColumn,      grid2.EndColumn);
			grid1.EndRow         = Mathf.Max(grid1.EndRow,         grid2.EndRow);
			grid1.ExpandTopCount = Mathf.Max(grid1.ExpandTopCount, grid2.ExpandTopCount);
		}
	}
}
