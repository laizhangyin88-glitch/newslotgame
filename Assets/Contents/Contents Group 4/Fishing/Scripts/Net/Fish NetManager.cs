using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishNetManager : MonoSingleton<FishNetManager>
    {
        List<FishNet> AllNetInsList;
        List<FishNet> CurrentNetInsList = new List<FishNet>();
        string NetPrefabName;
        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            InitData();
        }

        private void InitData()
        {
            AllNetInsList = new List<FishNet>();
            NetPrefabName = "Net";
        }

        public FishNet GetNet(Vector3 transPos)
        {
            FishNet tempNet;
            if (AllNetInsList != null && AllNetInsList.Count > 0)
            {
                tempNet = AllNetInsList[0];
                AllNetInsList.RemoveAt(0);
                if (tempNet != null)
                {
                    tempNet.ResetNetState();
                    tempNet.SetNetPosition(transPos);
                    CurrentNetInsList.Add(tempNet);
                    return tempNet;
                }
                else
                {
                    Debug.LogError("Failed to get net");
                }
            }
            else
            {
                GameObject netItem = FishGameObjectPoolManager.Instance.GetGameObject(NetPrefabName, PoolType.NetPool);
                if (netItem != null)
                {
                    tempNet = new FishNet(netItem);
                    tempNet.SetNetPosition(transPos);
                    CurrentNetInsList.Add(tempNet);
                    return tempNet;
                }
                else
                {
                    Debug.LogError("Failed to get net item");
                }
            }
            return null;
        }

        public void Update()
        {
            for (int i = 0; i < CurrentNetInsList.Count; i++)
            {
                CurrentNetInsList[i].Update();
            }
        }

        public void AddNet(FishNet netIns)
        {
            CurrentNetInsList.Remove(netIns);
            netIns.Destroy();
            AllNetInsList.Add(netIns);
        }
    }
}

