using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class InformationDataBase : MonoBehaviour
    {
        protected object[] dataArgs;

        public virtual void SetInformationData(object[] datas = null)
        {
            dataArgs = datas;
        }

        public virtual T GetInformationData<T>(int index)
        {
            if (dataArgs != null && dataArgs.Length > index && index > -1)
                return (T)Convert.ChangeType(dataArgs[index], typeof(T));
            return default(T);
        }
    }
}