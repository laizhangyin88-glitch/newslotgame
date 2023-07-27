using UnityEngine;
using System.Collections;

namespace SlotMaker
{

public interface IGrid
{
	int BeginColumn    { get; set; }
	int BeginRow       { get; set; }
	int EndColumn      { get; set; }
	int EndRow         { get; set; }
	int ColumnCount    { get; }
	int RowCount       { get; }
	int ExpandTopCount { get; set; }
}

public class Grid : IGrid
{
	public int BeginColumn    { get; set; }
	public int BeginRow       { get; set; }
	public int EndColumn      { get; set; }
	public int EndRow         { get; set; }
	public int ColumnCount    { get { return EndColumn - BeginColumn; } }
	public int RowCount       { get { return EndRow - BeginRow; } }
	public int ExpandTopCount { get; set; }
}

/*
public interface ICell
{
	int column { get; set; }
	int row    { get; set; }
}
*/

}
