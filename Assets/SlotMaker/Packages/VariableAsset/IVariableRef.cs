using System;

namespace NodeCanvas.Framework
{
    public interface IVariableRef
    {
        Type varType { get; }
        object value { get; set; }
    }
}