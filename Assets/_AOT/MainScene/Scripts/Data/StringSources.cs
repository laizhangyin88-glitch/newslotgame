using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.IoC.Strategy
{
    [Serializable]
    public abstract class StringSources : ScriptableObject
    {
        public string this[string name]
        {
            get { return GetString(name); }
            set { SetString(name, value); }
        }

        public abstract List<StringStringPairVariable> Get();
        public abstract string[] GetNames();

        public abstract bool ExistsSource(string name);
        public abstract void AddSource(string name);
        public abstract void RemoveSource(string name);
        public abstract string GetString(string name);
        public abstract void SetString(string name, string value);
    }
}