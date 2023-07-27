using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using SlotMaker.IoC;
using SlotMaker.Rendering.Clipping2D;
using UnityEngine;
using BagelCode.Slots.GDD;
using UnityEngine.Assertions;
using UnityEngine.Events;
using SlotMaker.Tasks.Actions;
using UnityEngine.Audio;
using System.IO;
using System.Security.Permissions;
using System.Linq;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace BagelCode.Slots.GDD.Utillity
{
    public class GDDLogger
    {
        static private Queue<string> logStack = new Queue<string>();

        static public string GetLogStackString()
        {
            string outputString = string.Empty;
            StackTrace st = new StackTrace(new StackFrame(true));
            for (int i = 0; i < st.FrameCount; i++)
            {
                outputString += $"Stack Trace Index {i}\n";
                StackFrame sf = st.GetFrame(i);
                outputString += $"[{sf.GetFileName()}.{sf.GetFileLineNumber()/sf.GetFileColumnNumber()}] - {sf.GetMethod()}\n";
            }
            return outputString;
        }
    }
}