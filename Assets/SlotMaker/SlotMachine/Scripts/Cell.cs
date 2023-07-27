using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    [Serializable]
    public class Cell : ICloneable
    {
        public int column;
        public int row;

        public Cell()
        {
            column = 0;
            row = 0;
        }

        public Cell(int column, int row)
        {
            this.column = column;
            this.row = row;
        }

        public object Clone()
        {
            return new Cell(this.column, this.row);
        }

        public override int GetHashCode() { return GetHashCode(column, row); }

        public static int GetHashCode(int column, int row)
    	{
    		return column * 10000 + row;
    	}

        public static Cell UnPack(int totalColumn, int totalRow, int value, MajorOrder majorOrder)
        {
            if (majorOrder == MajorOrder.ColumnMajor)
                return new Cell(value / totalRow, value % totalRow);
            return new Cell(value / totalColumn, value % totalColumn);
        }

        public static Cell UnPack(int firstValue, int secondValue, MajorOrder majorOrder)
        {
            if (majorOrder == MajorOrder.ColumnMajor)
                return new Cell(firstValue, secondValue);
            return new Cell(secondValue, firstValue);
        }

        public static int Pack(int totalColumn, int totalRow, Cell cell, MajorOrder majorOrder)
        {
            if (majorOrder == MajorOrder.ColumnMajor)
                return cell.column * totalRow + cell.row;
            return cell.column + cell.row * totalColumn;
        }
    }

    public enum MajorOrder
    {
        ColumnMajor,
        RowMajor
    }

    public enum MajorSortOrder
    {
        DoNothing,
        ColumnMajorAscendingOrder,
        ColumnMajorDescendingOrder,
        RowMajorAscendingOrder,
        RowMajorDescendingOrder
    }

    public static class MajorSortOrderCellComparer
    {
        private static IComparer<Cell> columnMajorAscendingCellComparer = new ColumnMajorAscendingCellComparer();
        private static IComparer<Cell> columnMajorDescendingCellComparer = new ColumnMajorDescendingCellComparer();
        private static IComparer<Cell> rowMajorAscendingCellComparer = new RowMajorAscendingCellComparer();
        private static IComparer<Cell> rowMajorDescendingCellComparer = new RowMajorDescendingCellComparer();
        public static IComparer<Cell> Get(MajorSortOrder sortOrder)
        {
            switch (sortOrder)
            {
            case MajorSortOrder.ColumnMajorAscendingOrder:
                return columnMajorAscendingCellComparer;
            case MajorSortOrder.ColumnMajorDescendingOrder:
                return columnMajorDescendingCellComparer;
            case MajorSortOrder.RowMajorAscendingOrder:
                return rowMajorAscendingCellComparer;
            case MajorSortOrder.RowMajorDescendingOrder:
                return rowMajorDescendingCellComparer;
            }
            return null;
        }
    }

    public class ColumnMajorAscendingCellComparer : IComparer<Cell>
    {
        public int Compare(Cell a, Cell b)
        {
            if (a.column != b.column)
                return a.column.CompareTo(b.column);
            return a.row.CompareTo(b.row);
        }
    }

    public class ColumnMajorDescendingCellComparer : IComparer<Cell>
    {
        public int Compare(Cell a, Cell b)
        {
            if (a.column != b.column)
                return b.column.CompareTo(a.column);
            return b.row.CompareTo(a.row);
        }
    }

    public class RowMajorAscendingCellComparer : IComparer<Cell>
    {
        public int Compare(Cell a, Cell b)
        {
            if (a.row != b.row)
                return a.row.CompareTo(b.row);
            return a.column.CompareTo(b.column);
        }
    }

    public class RowMajorDescendingCellComparer : IComparer<Cell>
    {
        public int Compare(Cell a, Cell b)
        {
            if (a.row != b.row)
                return b.row.CompareTo(a.row);
            return b.column.CompareTo(a.column);
        }
    }

    [Serializable]
    public class Cell3 : IEquatable<Cell3>
    {
        public int x;
        public int y;
        public int z;

        public Cell3() {}

        public Cell3(int x_, int y_, int z_)
        {
            x = x_;
            y = y_;
            z = z_;
        }

        public bool Equals(Cell3 other)
        {
            return x == other.x && y == other.y && z == other.z;
        }

        public override bool Equals(System.Object obj)
        {
            return this.Equals(obj as Cell3);
        }

        public override int GetHashCode()
        {
            return x * 1000000 + y * 1000 + z;
        }
    }
}
