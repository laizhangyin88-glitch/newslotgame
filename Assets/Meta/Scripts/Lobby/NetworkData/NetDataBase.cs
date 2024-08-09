using NodeCanvas.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    public abstract class NetDataBase<TT> : Singleton<TT> where TT : NetDataBase<TT>, new()
    {
        /// <summary>
        /// 添加网络数据改变事件
        /// </summary>
        /// <remarks>
        /// 在数据值有真实改变的时候才会触发事件，而不是set的时候
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="handle"></param>
        public void AddNetDataChangeEvent(string path, Action<string, object> handle)
        {
            Variable variable = BlackboardUtils.FindVariable(null, path);
            if (variable == null)
            {
#if UNITY_EDITOR
                Debug.LogError($"添加数据变更事件失败：未查找到变量,{path}");
#endif
                return;
            }

            variable.onValueChanged += handle;
        }

        /// <summary>
        /// 删除网络事件改变事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="handle"></param>
        public void RemoveNetDataChangeEvent(string path, Action<string, object> handle)
        {
            Variable variable = BlackboardUtils.FindVariable(null, path);
            if (variable == null)
            {
#if UNITY_EDITOR
                Debug.LogError($"删除数据变更事件失败：未查找到变量,{path}");
#endif
                return;
            }

            variable.onValueChanged -= handle;
        }

        /// <summary>
        /// 获取网络数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T GetNetDataValue<T>(string path)
        {
            Variable<T> variable = BlackboardUtils.FindVariable<T>(path);
            if (variable == null)
            {
#if UNITY_EDITOR
                Debug.LogError($"获取数据失败：未查找到变量,{path}");
#endif
                return default;
            }

            return variable.value;
        }

        /// <summary>
        /// 设置网络数据
        /// </summary>
        /// <remarks>
        /// 会触发网络数据改变事件
        /// 在数据值有真实改变的时候才会触发事件，而不是set的时候
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="value"></param>
        public void SetNetDataValue<T>(string path, T value)
        {
            Variable<T> variable = BlackboardUtils.GetOrCreateVariable<T>(path);
            if (variable != null)
                variable.value = value;
        }
    }
}
