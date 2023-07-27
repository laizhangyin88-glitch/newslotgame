using UnityEngine;
using SlotMaker.Json;

namespace SlotMaker
{
	public class FileSystem
	{
		public static bool Exists(string path)
		{
			return System.IO.File.Exists(path);
		}

		public static string LoadTextAsset(string path)
		{
			return System.IO.File.ReadAllText(path);
		}

		public static T LoadJsonAsset<T>(string path)
		{
			string text = LoadTextAsset(path);
			if (!string.IsNullOrEmpty(text))
			{
				return SlotSimpleJson.DeserializeObject<T>(text);
			}

			return default(T);
		}

		public static void SaveTextAsset(string path, string text)
		{
			System.IO.File.WriteAllText(path, text);
		}

		public static void SaveJsonAsset<T>(string path, T obj)
		{
			SaveTextAsset(path, SlotSimpleJson.SerializeObject(obj));
		}
	}
}
