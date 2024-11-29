using UnityEngine;
using System;
using System.IO;
using System.Collections;

namespace SlotMaker
{
    public class FileUtils 
    {
    	public static string GetCachedPath(string category)
    	{
    		return Application.temporaryCachePath+"/"+category;
    	}

        public static string GetDirectoryName(string path)
        {
            return System.IO.Path.GetDirectoryName(path);
        }

        public static string GetAssetDirectoryName(string path)
        {
            path = GetDirectoryName(path);
            return path.Remove(0, path.IndexOf("Assets"));
        }

    	public static void CreateFolder(string folderPath)
        {
            System.IO.Directory.CreateDirectory(folderPath);
        }

        public static bool ExistDirectory(string folderPath)
        {
            return System.IO.Directory.Exists(folderPath);
        }
        
        public static void DeleteFolder(string folderPath)
        {
            if (System.IO.Directory.Exists(folderPath))
            {
                System.IO.Directory.Delete(folderPath, true);
            }
        }

        public static void Write(string path, byte[] rawData, bool force = false)
        {
            if (!Exist(path) || force)
            {
                File.WriteAllBytes(path, rawData);
            }
        }

        public static byte[] Read(string path)
        {
        	byte []rawData = null;

            try 
            {
                rawData = System.IO.File.ReadAllBytes(path);
            } catch(Exception e)
            {
                Debug.Log(e.ToString());
            }
        	return rawData;
        }

        public static bool Exist(string path)
        {
        	return System.IO.File.Exists(path);
        }

        // OR
        public static bool Exist(string[] paths)
        {
        	for (int i = 0; i < paths.Length; ++i)
        		if (Exist(paths[i])) return true;
        	return false;
        }
    }
}
