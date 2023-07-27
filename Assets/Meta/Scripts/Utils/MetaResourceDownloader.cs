using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.IO;

namespace BagelCode
{
    [System.Serializable]
    public class ResourceInfo
    {
        public Object resource;
        public string path;
    }

    [CreateAssetMenu(fileName = "MetaResourceDownloader", menuName = "Meta/Resources/MetaResourceDownloader")]
    public class MetaResourceDownloader : ScriptableObject
    {
#if UNITY_EDITOR
        private const string BACKUP_PATH = "Assets/Meta/Resources/Backups";

        [PropertyOrder(0)]
        [Button(ButtonSizes.Medium, Name = "Backup Resources")]
        private void BackupResources()
        {
            Debug.Log("Start Backup");
            if (resourceList != null)
            {
                for (int i = 0; i < resourceList.Count; ++i)
                {
                    Object resource = resourceList[i].resource;
                    string resourcePath = AssetDatabase.GetAssetPath(resource);
                    string backupPath = GetBackupPath(resource);

                    FileUtil.CopyFileOrDirectory(resourcePath, backupPath);
                }
            }
            Debug.Log("Finish Backup");
        }

        [PropertyOrder(0)]
        [Button(ButtonSizes.Medium, Name = "Download Resources")]
        private void DownloadResources()
        {
            Debug.Log("Start Download");
            Object[] resources = Resources.LoadAll(BACKUP_PATH);

            if (resourceList != null)
            {
                for(int i = 0; i < resourceList.Count; ++i)
                {
                    string backupPath = GetBackupPath(resourceList[i].resource);
                    string destName = resourceList[i].path;

                    var backupResource = Resources.Load(backupPath);
                    if (backupResource == null) continue;

                    FileUtil.ReplaceFile(backupPath, destName);
                }
            }

            Debug.Log("Finish Download");
        }

        [PropertyOrder(0)]
        [Button(ButtonSizes.Medium, Name = "Clear Backups")]
        private void ClearBackups()
        {
            if (resourceList != null)
            {
                for (int i = 0; i < resourceList.Count; ++i)
                {
                    string backupPath = GetBackupPath(resourceList[i].resource);
                    FileUtil.DeleteFileOrDirectory(backupPath);
                }
            }

            resourceList.Clear();
        }
#endif

        [PropertyOrder(1)]
        public List<ResourceInfo> resourceList;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (resourceList != null)
            {
                for (int i = 0; i < resourceList.Count; ++i)
                {
                    // Set Path Automatically
                    Object resource = resourceList[i].resource;
                    if(resource != null && string.IsNullOrEmpty(resourceList[i].path))
                    {
                        resourceList[i].path = AssetDatabase.GetAssetPath(resource);
                        if(resourceList[i].path.Contains(BACKUP_PATH))
                        {
                            Debug.LogWarning("MetaResourceDownloader warning. The resource in the backup folder is bound.");
                        }
                    }
                }
            }
        }

        private string GetBackupPath(Object resource)
        {
            string resourceName = resource.name;
            string resourcePath = AssetDatabase.GetAssetPath(resource);
            string extension = Path.GetExtension(resourcePath);
            return BACKUP_PATH + "/" + resourceName + extension;
        }
#endif
    }
}
