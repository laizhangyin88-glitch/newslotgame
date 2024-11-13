using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    [CreateAssetMenu(fileName="New SceneInfoObject", menuName="SlotMaker/ScriptableObject/SceneInfoObject")]
    public class SceneInfoObject : ScriptableObject, ISerializationCallbackReceiver
    {
        [NonSerialized]
        public SceneInfo sceneInfo;
        public List<SerializableSceneInfo> serializableSceneInfos;
        
        public SceneInfo GetSceneInfo()
        {
            Deserialize();
            return sceneInfo;
        }
        
        public void SetSceneInfo(SceneInfo info)
        {
            sceneInfo = info;

            Serialize();
        }

        private void Serialize()
        {
            if (sceneInfo == null)
                sceneInfo = new SceneInfo();
            
            if (serializableSceneInfos == null)
                serializableSceneInfos = new List<SerializableSceneInfo>();
            else 
                serializableSceneInfos.Clear();

            AddSceneInfoToSerializableSceneInfo(sceneInfo);
        }
        
        private void AddSceneInfoToSerializableSceneInfo(SceneInfo info)
        {
            int childrenCount = info.children.Count;
            var serializableSceneInfo = new SerializableSceneInfo
            {
                bundleName = info.bundleName,
                assetName  = info.assetName,
                uniqueName = info.uniqueName,
                parentName = info.parentName,
                activate   = info.activate,
                childCount = childrenCount,
                indexOfFirstChild = serializableSceneInfos.Count + 1
            };
            serializableSceneInfos.Add(serializableSceneInfo);
            
            for (int i = 0; i < childrenCount; ++i)
                AddSceneInfoToSerializableSceneInfo(info.children[i]);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() {}
        
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Deserialize();
        }

        private void Deserialize()
        {
            if (serializableSceneInfos.Count > 0)
                ReadSceneInfoFromSerializableSceneInfo(0, out sceneInfo);
            else 
                sceneInfo = new SceneInfo();
        }
        
        private int ReadSceneInfoFromSerializableSceneInfo(int index, out SceneInfo info) 
        {
            var serializableSceneInfo = serializableSceneInfos[index];
            SceneInfo newSceneInfo = new SceneInfo() 
            {
                bundleName = serializableSceneInfo.bundleName,
                assetName  = serializableSceneInfo.assetName,
                uniqueName = serializableSceneInfo.uniqueName,
                parentName = serializableSceneInfo.parentName,
                activate   = serializableSceneInfo.activate,
                children   = new List<SceneInfo>()
            };
            
            for (int i = 0; i != serializableSceneInfo.childCount; ++i) 
            {
                SceneInfo childSceneInfo;
                index = ReadSceneInfoFromSerializableSceneInfo(++index, out childSceneInfo);
                newSceneInfo.children.Add(childSceneInfo);
            }
            info = newSceneInfo;
            return index;
        }
    }
}
