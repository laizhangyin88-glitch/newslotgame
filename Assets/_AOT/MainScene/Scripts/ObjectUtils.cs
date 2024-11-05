using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;

namespace SlotMaker
{
	public static class ObjectUtils
	{
		public static object DeepClone(object src)
		{
			object dst = null;
			using (MemoryStream ms = new MemoryStream())
			{
				BinaryFormatter bf = new BinaryFormatter();
				bf.Serialize(ms, src);
				ms.Position = 0;
				dst = bf.Deserialize(ms);
			}
			return dst;
		}

		public static List<object> DeepClone(List<object> src)
		{
			List<object> dst = new List<object>();
			for (int i = 0; i < src.Count; ++i)
			{
				dst.Add(DeepClone(src[i]));
			}
			return dst;
		}

		public static List<int> DeepClone(List<int> src)
		{
			List<int> dst = new List<int>();
			for (int i = 0; i < src.Count; ++i)
			{
				dst.Add(src[i]);
			}
			return dst;	
		}

		public static List<List<object>> DeepClone(List<List<object>> src)
		{
			List<List<object>> dst = new List<List<object>>();
			for (int i = 0; i < src.Count; ++i)
			{
				dst.Add(DeepClone(src[i]));
			}
			return dst;
		}
	}
}