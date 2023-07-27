using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using SlotMaker.IoC.Strategy;

namespace SlotMaker.Slots.Tasks.Actions.String
{
    [Category("✶ Slots/Utils/String")]
    public class SetFormatString : ActionTask
    {
        public StringSources source;
        public BBParameter<string> format;
        public List<BBObjectParameter> args = new List<BBObjectParameter>();
        public BBParameter<string> saveAs;

        protected override string info
        {
            get 
            { 
                return string.Format("{0} = Format({1}, args)", saveAs, GetFormat());
            }
        }

        protected override void OnExecute()
        {
            saveAs.value = GetString();
            EndAction();
        }

        public string GetString()
        {
            var objArgs = GetArgs();
            if (objArgs == null) return format.value;
#if UNITY_EDITOR
            try
            {
                return string.Format(StringTableUtils.customProvider, GetFormat(), objArgs);
            }
            catch (System.FormatException e)
            {
                return format.value;
            }
#else
            return string.Format(StringTableUtils.customProvider, GetFormat(), objArgs);
#endif
        }

        protected string GetFormat()
        {
            return (source != null) ? source.GetString(format.value) : format.value;
        }

        protected object[] GetArgs()
        {
            int count = args.Count;
            if (count == 0)
                return null;
            
            List<object> list = new List<object>();
            foreach (var arg in args)
            {
                if (arg == null)
                    list.Add(arg);
                else if (arg is IEnumerable)
                {
                    foreach (var item in (IEnumerable)arg)
                    {
                        list.Add(item);
                    }
                }
                else 
                    list.Add(arg.value);
            }
            return list.ToArray();
        }

#if UNITY_EDITOR
        protected override void OnTaskInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUI.changed)
            {
                foreach (var arg in args)
                {
                    if (arg != null)
                    {
                        if (arg.bb == null)
                            arg.bb = blackboard;
                        
                        if (arg.varRef != null)
                            arg.SetType(arg.refType);
                    }
                }
            }
        }
#endif
    }
}