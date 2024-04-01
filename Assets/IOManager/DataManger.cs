using System;
using System.Runtime.InteropServices;

//读写数据用
[Serializable, StructLayout(LayoutKind.Sequential)]
public struct DataManger
{
    public byte _isok;
    public int  _len ;   //2的倍数
    public int  _addr;  //2的倍数，最高64K地址
    [MarshalAs(UnmanagedType.ByValArray, SizeConst=256)]
    public int[] _data;
    public DataManger(int n)
    {
        this._isok = 2;
        this._len = 0;
        this._addr = 0;
        this._data = new int[256];
    }
}

