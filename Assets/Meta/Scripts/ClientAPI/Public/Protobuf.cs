// Refer to https://searchcode.com/codesearch/view/51494947/

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace BagelCode.Protobuf
{
public interface IProtoSerializable
{
    byte[] Serialize();
}

public enum Wire
{
    Varint = 0,              //int32, int64, UInt32, UInt64, SInt32, SInt64, bool, enum
    Fixed64 = 1,             //fixed64, sfixed64, double
    LengthDelimited = 2,     //string, bytes, embedded messages, packed repeated fields
    //Start = 3,         //  groups (deprecated)
    //End = 4,           //  groups (deprecated)
    Fixed32 = 5,             //32-bit    fixed32, SFixed32, float
}

public class Key
{
    public uint Field { get; set; }

    public Wire WireType { get; set; }

    public Key(uint field, Wire wireType)
    {
        this.Field = field;
        this.WireType = wireType;
    }

    public override string ToString()
    {
        return string.Format("[Key: {0}, {1}]", Field, WireType);
    }
}

public class ByteWriter
{
    static ByteWriter()
    {
        if (BitConverter.IsLittleEndian)
        {
            FromDouble = BitConverter.GetBytes;
            FromFloat = BitConverter.GetBytes;
            FromUInt64 = BitConverter.GetBytes;
            FromUInt32 = BitConverter.GetBytes;
            FromInt32 = BitConverter.GetBytes;
            FromInt64 = BitConverter.GetBytes;
        }
        else
        {
            FromDouble = FromDoubleLE;
            FromFloat = FromFloatLE;
            FromUInt64 = FromUInt64LE;
            FromUInt32 = FromUInt32LE;
            FromInt64 = FromInt64LE;
            FromInt32 = FromInt32LE;
        }
    }

    private static byte[] FromDoubleLE(double value)
    {
        var ret = BitConverter.GetBytes(value);

        Array.Reverse(ret);
        return ret;
    }

    private static byte[] FromFloatLE(float value)
    {
        var ret = BitConverter.GetBytes(value);

        Array.Reverse(ret);
        return ret;
    }

    private static byte[] FromUInt64LE(ulong value)
    {
        var ret = BitConverter.GetBytes(value);

        Array.Reverse(ret);
        return ret;
    }

    private static byte[] FromUInt32LE(uint value)
    {
        var ret = BitConverter.GetBytes(value);

        Array.Reverse(ret);
        return ret;
    }

    private static byte[] FromInt64LE(long value)
    {
        var ret = BitConverter.GetBytes(value);

        Array.Reverse(ret);
        return ret;
    }

    private static byte[] FromInt32LE(int value)
    {
        var ret = BitConverter.GetBytes(value);

        Array.Reverse(ret);
        return ret;
    }

    private delegate byte[] TBitConverter<T>(T value);

    private static readonly TBitConverter<double> FromDouble;

    private static readonly TBitConverter<float> FromFloat;

    private static readonly TBitConverter<uint> FromUInt32;

    private static readonly TBitConverter<ulong> FromUInt64;

    private static readonly TBitConverter<int> FromInt32;

    private static readonly TBitConverter<long> FromInt64;

    public static int Write(double value, byte[] target, int offset)
    {
        var bytes = FromDouble(value);

        for (int i = 0; i < bytes.Length; ++i)
        {
            target[offset + i] = bytes[i];
        }
        return 8;
    }

    public static int Write(float value, byte[] target, int offset)
    {
        var bytes = FromFloat(value);

        for (int i = 0; i < bytes.Length; ++i)
        {
            target[offset + i] = bytes[i];
        }
        return 4;
    }

    public static int WriteInt32(int value, byte[] target, int offset)
    {
        return WriteUInt32((uint)value, target, offset);
    }

    public static int WriteInt64(long value, byte[] target, int offset)
    {
        return WriteUInt64((ulong)value, target, offset);
    }

    public static int WriteUInt32(uint value, byte[] target, int offset)
    {
        int origOffset = offset;
        byte b;

        while (true)
        {
            b = (byte)(value & 0x7F);
            value = value >> 7;
            if (value == 0)
            {
                target[offset++] = b;
                break;
            }
            else
            {
                target[offset++] = (byte)(b | 0x80);
            }
        }
        return offset - origOffset;
    }

    public static int WriteUInt64(ulong value, byte[] target, int offset)
    {
        int origOffset = offset;
        byte b;

        while (true)
        {
            b = (byte)(value & 0x7F);
            value = value >> 7;
            if (value == 0)
            {
                target[offset++] = b;
                break;
            }
            else
            {
                target[offset++] = (byte)(b | 0x80);
            }
        }
        return offset - origOffset;
    }

    public static int WriteSInt32(int value, byte[] target, int offset)
    {
        return WriteUInt32((uint)((value << 1) ^ (value >> 31)), target, offset);
    }

    public static int WriteSInt64(long value, byte[] target, int offset)
    {
        return WriteUInt64((ulong)((value << 1) ^ (value >> 63)), target, offset);
    }

    public static int WriteFixed32(uint value, byte[] target, int offset)
    {
        var bytes = FromUInt32(value);

        for (int i = 0; i < bytes.Length; ++i)
        {
            target[offset + i] = bytes[i];
        }
        return 4;
    }

    public static int WriteFixed64(ulong value, byte[] target, int offset)
    {
        var bytes = FromUInt64(value);

        for (int i = 0; i < bytes.Length; ++i)
        {
            target[offset + i] = bytes[i];
        }
        return 8;
    }

    public static int WriteSFixed32(int value, byte[] target, int offset)
    {
        var bytes = FromInt32(value);

        for (int i = 0; i < bytes.Length; ++i)
        {
            target[offset + i] = bytes[i];
        }
        return 4;
    }

    public static int WriteSFixed64(long value, byte[] target, int offset)
    {
        var bytes = FromInt64(value);

        for (int i = 0; i < bytes.Length; ++i)
        {
            target[offset + i] = bytes[i];
        }
        return 8;
    }

    public static int Write(bool value, byte[] target, int offset)
    {
        target[offset] = (byte)(value ? 1 : 0);
        return 1;
    }

    public static int Write(string value, byte[] target, int offset)
    {
        return Write(Encoding.UTF8.GetBytes(value), target, offset);
    }

    public static int Write(byte[] value, byte[] target, int offset)
    {
        var len = WriteInt32(value.Length, target, offset);

        Buffer.BlockCopy(value, 0, target, offset + len, value.Length);

        return len + value.Length;
    }
}

public class ByteReader
{
    public ByteReader(byte[] _buffer)
    {
        m_buffer = _buffer;
        Position = 0;
        SetMark(_buffer.Length);
    }

    static ByteReader()
    {
        if (BitConverter.IsLittleEndian)
        {
            ToDouble = BitConverter.ToDouble;
            ToFloat = BitConverter.ToSingle;
            ToUInt64 = BitConverter.ToUInt64;
            ToUInt32 = BitConverter.ToUInt32;
        }
        else
        {
            ToDouble = ToDoubleLE;
            ToFloat = ToFloatLE;
            ToUInt64 = ToUInt64LE;
            ToUInt32 = ToUInt32LE;
        }
    }

    private delegate T TBitConverter<T>(byte[] buf, int len);

    private static readonly TBitConverter<double> ToDouble;

    private static readonly TBitConverter<float> ToFloat;

    private static readonly TBitConverter<uint> ToUInt32;

    private static readonly TBitConverter<ulong> ToUInt64;

    private byte[] m_buffer;

    private Stack<int> mMarks = new Stack<int>();

    public void SetMark(int mark)
    {
        int limit = Position + mark;

        if (limit > m_buffer.Length)
        {
            throw new InvalidOperationException();
        }
        mMarks.Push(limit);
    }

    public void UnsetMark()
    {
        mMarks.Pop();
    }

    public int Length
    {
        get
        {
            return mMarks.Peek();
        }
    }

    public int Position { get; private set; }

    public bool CanRead
    {
        get
        {
            return Position < Length;
        }
    }

    public double ReadDouble()
    {
        if (Position + 8 > Length)
        {
            throw new EndOfStreamException();
        }

        double ret = ToDouble(m_buffer, Position);
        Position += 8;
        return ret;
    }

    public float ReadFloat()
    {
        if (Position + 4 > Length)
        {
            throw new EndOfStreamException();
        }

        float ret = ToFloat(m_buffer, Position);
        Position += 4;
        return ret;
    }

    private byte ReadByte()
    {
        if (Position + 1 > Length)
        {
            throw new EndOfStreamException();
        }
        return m_buffer[Position++];
    }

    // varint, same as Read7BitEncodedInt, but check int-range
    public int ReadInt32()
    {
        int ret = 0;
        int shift = 0;
        int len;
        byte b = 0;

        for (len = 0; len < 5; ++len)
        {
            b = ReadByte();

            ret = ret | ((b & 0x7f) << shift);
            shift += 7;
            if ((b & 0x80) == 0)
            {
                break;
            }
        }

        if (len < 4)
        {
            return ret;
        }
        else if (len == 4)
        {
            if ((b & 0xF0) == 0)
            {
                return ret;
            }
            else
            {
                throw new FormatException("Not in range of int32");
            }
        }
        else
        {
            throw new FormatException("Too many bytes in what should have been a 7 bit encoded Int32.");
        }
    }

    public long ReadInt64()
    {
        long ret = 0;
        int shift = 0;
        int len;
        byte b = 0;

        for (len = 0; len < 10; ++len)
        {
            b = ReadByte();

            ret = ret | ((long)(b & 0x7f) << shift);
            shift += 7;
            if ((b & 0x80) == 0)
            {
                break;
            }
        }

        if (len < 9)
        {
            return ret;
        }
        else if (len == 9)
        {
            if ((b & 0xFE) == 0)
            {
                return ret;
            }
            else
            {
                throw new FormatException("Not in range of int64");
            }
        }
        else
        {
            throw new FormatException("Too many bytes in what should have been a 7 bit encoded Int64.");
        }
    }

    public uint ReadUInt32()
    {
        return (uint)ReadInt32();
    }

    public ulong ReadUInt64()
    {
        return (ulong)ReadInt64();
    }

    public int ReadSInt32()
    {
        uint val = (uint)ReadInt32();

        return (int)(val >> 1) ^ ((int)(val << 31) >> 31);
    }

    public long ReadSInt64()
    {
        ulong val = (ulong)ReadInt64();

        return (long)(val >> 1) ^ ((long)(val << 63) >> 63);
    }

    public uint ReadFixed32()
    {
        if (Position + 4 > Length)
        {
            throw new EndOfStreamException();
        }

        uint ret = ToUInt32(m_buffer, Position);
        Position += 4;
        return ret;
    }

    public ulong ReadFixed64()
    {
        if (Position + 8 > Length)
        {
            throw new EndOfStreamException();
        }

        ulong ret = ToUInt64(m_buffer, Position);
        Position += 8;
        return ret;
    }

    public int ReadSFixed32()
    {
        return (int)ReadFixed32();
    }

    public long ReadSFixed64()
    {
        return (long)ReadFixed64();
    }

    public bool ReadBool()
    {
        if (Position + 1 > Length)
        {
            throw new EndOfStreamException();
        }

        bool ret = m_buffer[Position] != 0;
        Position += 1;
        return ret;
    }

    public string ReadString()
    {
        int length = ReadInt32();

        if (length < 0)
        {
            throw new IOException("Invalid binary file (length < 0)");
        }

        if (length == 0)
        {
            return String.Empty;
        }

        string ret = Encoding.UTF8.GetString(m_buffer, Position, length);
        Position += length;
        return ret;
    }

    public byte[] ReadBytes()
    {
        int length = ReadInt32();

        if (length < 0)
        {
            throw new IOException("Invalid binary file (length < 0)");
        }

        if (length == 0)
        {
            return new byte[0];
        }

        if (Position + length > Length)
        {
            throw new EndOfStreamException();
        }

        byte[] ret = new byte[length];
        Buffer.BlockCopy(m_buffer, Position, ret, 0, length);

        Position += length;
        return ret;
    }

    public Key ReadKey()
    {
        uint n = (uint)ReadInt32();

        return new Key(n >> 3, (Wire)(n & 0x07));
    }

    public bool SkipIfExpected(byte[] expected)
    {
        if (expected == null)
        {
            return false;
        }

        if (Position + expected.Length > Length)
        {
            return false;
        }

        for (int i = 0; i < expected.Length; ++i)
        {
            if (m_buffer[Position + i] != expected[i])
            {
                return false;
            }
        }
        Position += expected.Length;
        return true;
    }

    public void Skip(Wire wireType)
    {
        switch (wireType)
        {
        case Wire.Fixed32:
            Position += 4;
            return;

        case Wire.Fixed64:
            Position += 8;
            return;

        case Wire.LengthDelimited:
            int length = ReadInt32();
            Position += length;
            return;

        case Wire.Varint:
            while ((ReadByte() & 0x80) != 0)
            {
            }
            return;

        default:
            throw new NotImplementedException("Unknown wire type: " + wireType);
        }
    }

    private static byte[] GetReverse(byte[] src, int offset, int length)
    {
        byte[] tmp = new byte[length];
        for (int i = 0; i < length; ++i)
        {
            tmp[length - 1 - i] = src[offset + i];
        }
        return tmp;
    }

    private static double ToDoubleLE(byte[] src, int offset)
    {
        return BitConverter.ToDouble(GetReverse(src, offset, 8), 0);
    }

    private static float ToFloatLE(byte[] src, int offset)
    {
        return BitConverter.ToSingle(GetReverse(src, offset, 8), 0);
    }

    private static uint ToUInt32LE(byte[] src, int offset)
    {
        return BitConverter.ToUInt32(GetReverse(src, offset, 8), 0);
    }

    private static ulong ToUInt64LE(byte[] src, int offset)
    {
        return BitConverter.ToUInt64(GetReverse(src, offset, 8), 0);
    }
}

public delegate T TRead<T>(ByteReader reader);

public class ProtobufReader
{
    public static double ReadDouble(ByteReader reader)
    {
        return reader.ReadDouble();
    }

    public static float ReadFloat(ByteReader reader)
    {
        return reader.ReadFloat();
    }

    public static int ReadInt32(ByteReader reader)
    {
        return reader.ReadInt32();
    }

    public static long ReadInt64(ByteReader reader)
    {
        return reader.ReadInt64();
    }

    public static uint ReadUInt32(ByteReader reader)
    {
        return reader.ReadUInt32();
    }

    public static ulong ReadUInt64(ByteReader reader)
    {
        return reader.ReadUInt64();
    }

    public static int ReadSInt32(ByteReader reader)
    {
        return reader.ReadSInt32();
    }

    public static long ReadSInt64(ByteReader reader)
    {
        return reader.ReadSInt64();
    }

    public static uint ReadFixed32(ByteReader reader)
    {
        return reader.ReadFixed32();
    }

    public static ulong ReadFixed64(ByteReader reader)
    {
        return reader.ReadFixed64();
    }

    public static int ReadSFixed32(ByteReader reader)
    {
        return reader.ReadSFixed32();
    }

    public static long ReadSFixed64(ByteReader reader)
    {
        return reader.ReadSFixed64();
    }

    public static bool ReadBool(ByteReader reader)
    {
        return reader.ReadBool();
    }

    public static string ReadString(ByteReader reader)
    {
        return reader.ReadString();
    }

    public static byte[] ReadBytes(ByteReader reader)
    {
        return reader.ReadBytes();
    }

    public static T ReadEnum<T>(ByteReader reader, T defaultValue) where T: struct, IConvertible
    {
        Type enumType = typeof(T);
        T value = (T)Enum.ToObject(enumType, ProtobufReader.ReadInt32(reader));

        if (Enum.IsDefined(enumType, value))
        {
            return (T)value;
        }
        else
        {
            return defaultValue;
        }
    }

    public static List<T> ReadList<T>(ByteReader reader, TRead<T> decoder)
    {
        reader.SetMark(reader.ReadInt32());
        var result = new List<T>();
        while (reader.CanRead)
        {
            result.Add(decoder(reader));
        }
        reader.UnsetMark();
        return result;
    }

    public static TRead<List<T> > ToReadPackedList<T>(TRead<T> decoder)
    {
        return delegate(ByteReader reader) {
                   return ReadList(reader, decoder);
        };
    }

    public static TRead<List<T> > ToReadList<T>(byte[] keyBytes, TRead<T> decoder)
    {
        return delegate(ByteReader reader) {
                   var result = new List<T>();
                   while (reader.SkipIfExpected(keyBytes))
                   {
                       result.Add(decoder(reader));
                   }
                   return result;
        };
    }

    public static TRead<T> ToLengthDelimited<T>(TRead<T> decoder)
    {
        return delegate(ByteReader reader) {
                   reader.SetMark(reader.ReadInt32());
                   var result = decoder(reader);
                   reader.UnsetMark();
                   return result;
        };
    }
}

public class LengthOf
{
    public const uint I1 = (uint)1 << 7;
    public const uint I2 = (uint)1 << 14;
    public const uint I3 = (uint)1 << 21;
    public const uint I4 = (uint)1 << 28;

    public const ulong L1 = (ulong)1 << 7;
    public const ulong L2 = (ulong)1 << 14;
    public const ulong L3 = (ulong)1 << 21;
    public const ulong L4 = (ulong)1 << 28;
    public const ulong L5 = (ulong)1 << 35;
    public const ulong L6 = (ulong)1 << 42;
    public const ulong L7 = (ulong)1 << 49;
    public const ulong L8 = (ulong)1 << 56;
    public const ulong L9 = (ulong)1 << 63;

    public static int LengthOfDouble(double value)
    {
        return 8;
    }

    public static int LengthOfFloat(float value)
    {
        return 4;
    }

    public static int LengthOfInt32(int value)
    {
        return LengthOfUInt32((uint)value);
    }

    public static int LengthOfInt64(long value)
    {
        return LengthOfUInt64((ulong)value);
    }

    public static int LengthOfUInt32(uint value)
    {
        return value < I1 ? 1
               : value < I2 ? 2
               : value < I3 ? 3
               : value < I4 ? 4
               :              5;
    }

    public static int LengthOfUInt64(ulong value)
    {
        return value < L1 ? 1
               : value < L2 ? 2
               : value < L3 ? 3
               : value < L4 ? 4
               : value < L5 ? 5
               : value < L6 ? 6
               : value < L7 ? 7
               : value < L8 ? 8
               : value < L9 ? 9
               :              10;
    }

    public static int LengthOfSInt32(int value)
    {
        return LengthOfUInt32((uint)((value << 1) ^ (value >> 31)));
    }

    public static int LengthOfSInt64(long value)
    {
        return LengthOfUInt64((ulong)((value << 1) ^ (value >> 63)));
    }

    public static int LengthOfFixed32(uint value)
    {
        return 4;
    }

    public static int LengthOfFixed64(ulong value)
    {
        return 8;
    }

    public static int LengthOfSFixed32(int value)
    {
        return 4;
    }

    public static int LengthOfSFixed64(long value)
    {
        return 8;
    }

    public static int LengthOfBool(bool value)
    {
        return 1;
    }

    public static int LengthOfString(string value)
    {
        int length = Encoding.UTF8.GetByteCount(value);

        return LengthOfInt32(length) + length;
    }

    public static int LengthOfBytes(byte[] value)
    {
        return LengthOfInt32(value.Length) + value.Length;
    }
}
}
