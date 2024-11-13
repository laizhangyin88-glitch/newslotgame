using UnityEngine;
using System.Collections;
using ParadoxNotion;

namespace SlotMaker
{

public enum BoolSetModes
{
	False  = 0,
	True   = 1,
	Toggle = 2
};

public enum ClampOperationMethod
{
    Clamp = 0,
    Repeat = 1
};

public enum BitwiseOperationMethod
{
	Set,
	Add,
	Subtract,
	And,
	Or,
	ExclusiveOr
};

public static class OperationUtils
{
	public static string GetOperationString(OperationMethod om)
	{
		if (om == OperationMethod.Set)
			return " = ";

		if (om == OperationMethod.Add)
			return " += ";

		if (om == OperationMethod.Subtract)
			return " -= ";

		if (om == OperationMethod.Multiply)
			return " *= ";

		if (om == OperationMethod.Divide)
			return " /= ";

		return string.Empty;
	}

	public static string GetOperationString(BitwiseOperationMethod om)
	{
		if (om == BitwiseOperationMethod.Set)
			return " = ";

		if (om == BitwiseOperationMethod.Add)
			return " += ";

		if (om == BitwiseOperationMethod.Subtract)
			return " -= ";

		if (om == BitwiseOperationMethod.And)
			return " &= ";

		if (om == BitwiseOperationMethod.Or)
			return " |= ";

		if (om == BitwiseOperationMethod.ExclusiveOr)
			return " ^= ";

		return string.Empty;
	}

	public static bool Operate(bool a, BoolSetModes setTo)
	{
		return (setTo == BoolSetModes.Toggle) ? !a : ((int)setTo == 1);
	}

    public static int Operate(int a, int b, OperationMethod om)
	{
		if (om == OperationMethod.Set)
			return b;
		if (om == OperationMethod.Add)
			return a + b;
		if (om == OperationMethod.Subtract)
			return a - b;
		if (om == OperationMethod.Multiply)
			return a * b;
		if (om == OperationMethod.Divide)
			return a / b;
		return a;
	}

	public static int Operate(int a, int b, BitwiseOperationMethod om)
	{
		if (om == BitwiseOperationMethod.Set)
			return b;
		if (om == BitwiseOperationMethod.Add)
			return a + b;
		if (om == BitwiseOperationMethod.Subtract)
			return a - b;
		if (om == BitwiseOperationMethod.And)
			return a & b;
		if (om == BitwiseOperationMethod.Or)
			return a | b;
		if (om == BitwiseOperationMethod.ExclusiveOr)
			return a ^ b;
		return a;
	}

	public static uint Operate(uint a, uint b, OperationMethod om)
	{
		if (om == OperationMethod.Set)
			return b;
		if (om == OperationMethod.Add)
			return a + b;
		if (om == OperationMethod.Subtract)
			return a - b;
		if (om == OperationMethod.Multiply)
			return a * b;
		if (om == OperationMethod.Divide)
			return a / b;
		return a;
	}

	public static long Operate(long a, long b, OperationMethod om)
	{
		if (om == OperationMethod.Set)
			return b;
		if (om == OperationMethod.Add)
			return a + b;
		if (om == OperationMethod.Subtract)
			return a - b;
		if (om == OperationMethod.Multiply)
			return a * b;
		if (om == OperationMethod.Divide)
			return a / b;
		return a;
	}

	public static ulong Operate(ulong a, ulong b, OperationMethod om)
	{
		if (om == OperationMethod.Set)
			return b;
		if (om == OperationMethod.Add)
			return a + b;
		if (om == OperationMethod.Subtract)
			return a - b;
		if (om == OperationMethod.Multiply)
			return a * b;
		if (om == OperationMethod.Divide)
			return a / b;
		return a;
	}

	public static float Operate(float a, float b, OperationMethod om)
	{
		if (om == OperationMethod.Set)
			return b;
		if (om == OperationMethod.Add)
			return a + b;
		if (om == OperationMethod.Subtract)
			return a - b;
		if (om == OperationMethod.Multiply)
			return a * b;
		if (om == OperationMethod.Divide)
			return a / b;
		return a;
	}

	public static double Operate(double a, double b, OperationMethod om)
	{
		if (om == OperationMethod.Set)
			return b;
		if (om == OperationMethod.Add)
			return a + b;
		if (om == OperationMethod.Subtract)
			return a - b;
		if (om == OperationMethod.Multiply)
			return a * b;
		if (om == OperationMethod.Divide)
			return a / b;
		return a;
	}

    public static Vector3 Operate(Vector3 a, Vector3 b, OperationMethod om)
    {
        if (om == OperationMethod.Set)
            return b;
        if (om == OperationMethod.Add)
            return a + b;
        if (om == OperationMethod.Subtract)
            return a - b;
        if (om == OperationMethod.Multiply)
            return Vector3.Scale(a, b);
        if (om == OperationMethod.Divide)
            return new Vector3( (a).x/(b).x, (a).y/(b).y, (a).z/(b).z );
        return a;
    }

	public static string GetCompareString(CompareMethod cm){

		if (cm == CompareMethod.EqualTo)
			return " == ";

		if (cm == CompareMethod.GreaterThan)
			return " > ";

		if (cm == CompareMethod.LessThan)
			return " < ";

		if (cm == CompareMethod.GreaterOrEqualTo)
			return " >= ";

		if (cm == CompareMethod.LessOrEqualTo)
			return " <= ";

		return string.Empty;
	}

	public static bool Compare(int a, int b, CompareMethod cm)
	{
		if (cm == CompareMethod.EqualTo)
			return a == b;
		if (cm == CompareMethod.GreaterThan)
			return a > b;
		if (cm == CompareMethod.LessThan)
			return a < b;
		if (cm == CompareMethod.GreaterOrEqualTo)
			return a >= b;
		if (cm == CompareMethod.LessOrEqualTo)
			return a <= b;
		return true;
	}

	public static bool Compare(uint a, uint b, CompareMethod cm)
	{
		if (cm == CompareMethod.EqualTo)
			return a == b;
		if (cm == CompareMethod.GreaterThan)
			return a > b;
		if (cm == CompareMethod.LessThan)
			return a < b;
		if (cm == CompareMethod.GreaterOrEqualTo)
			return a >= b;
		if (cm == CompareMethod.LessOrEqualTo)
			return a <= b;
		return true;
	}

	public static bool Compare(long a, long b, CompareMethod cm)
	{
		if (cm == CompareMethod.EqualTo)
			return a == b;
		if (cm == CompareMethod.GreaterThan)
			return a > b;
		if (cm == CompareMethod.LessThan)
			return a < b;
		if (cm == CompareMethod.GreaterOrEqualTo)
			return a >= b;
		if (cm == CompareMethod.LessOrEqualTo)
			return a <= b;
		return true;
	}

	public static bool Compare(ulong a, ulong b, CompareMethod cm)
	{
		if (cm == CompareMethod.EqualTo)
			return a == b;
		if (cm == CompareMethod.GreaterThan)
			return a > b;
		if (cm == CompareMethod.LessThan)
			return a < b;
		if (cm == CompareMethod.GreaterOrEqualTo)
			return a >= b;
		if (cm == CompareMethod.LessOrEqualTo)
			return a <= b;
		return true;
	}

	public static bool Compare(float a, float b, CompareMethod cm, float floatingPoint)
	{
		if (cm == CompareMethod.EqualTo)
			return Mathf.Abs(a - b) <= floatingPoint;
		if (cm == CompareMethod.GreaterThan)
			return a > b;
		if (cm == CompareMethod.LessThan)
			return a < b;
		if (cm == CompareMethod.GreaterOrEqualTo)
			return a >= b;
		if (cm == CompareMethod.LessOrEqualTo)
			return a <= b;
		return true;
	}

	public static bool Compare(double a, double b, CompareMethod cm, float floatingPoint)
	{
		if (cm == CompareMethod.EqualTo)
			return Mathf.Abs((float)(a - b)) <= floatingPoint;
		if (cm == CompareMethod.GreaterThan)
			return a > b;
		if (cm == CompareMethod.LessThan)
			return a < b;
		if (cm == CompareMethod.GreaterOrEqualTo)
			return a >= b;
		if (cm == CompareMethod.LessOrEqualTo)
			return a <= b;
		return true;
	}

    public static string GetOperationString(ClampOperationMethod om)
    {
        return (om == ClampOperationMethod.Clamp) ? "Clamp" : "Repeat";
    }

    public static int Operate(int value, int minValue, int maxValue, ClampOperationMethod om)
    {
        if (om == ClampOperationMethod.Clamp)
            return Mathf.Clamp(value, minValue, maxValue);
        else if (om == ClampOperationMethod.Repeat)
        {
            int range = maxValue - minValue;

            while (value < minValue)
                value += range;

            while (value >= maxValue)
                value -= range;

            return value;
        }

        return value;
    }
}

}
