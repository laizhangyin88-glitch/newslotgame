using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishSingleCoinAlgorithm
    {
        FishGameUIManager gameUIManager;
        FishGameData gameData;
        Transform TargetTf;
        float Radius;
        float Width;
        float Height;
        float Offset;
        public FishSingleCoinAlgorithm()
        {
            InitData();
        }

        public void InitData()
        {
            gameUIManager =  FishGameUIManager.Instance;
            gameData = gameUIManager.gameData;
        }

        public List<Vector3> GetMuiltpleGoldEffect(Transform targetTf, float width, float height, float radius, float offset)
        {
            ResetState(targetTf, radius, width, height, offset);
            return BuildAlgo();
        }

        public void ResetState(Transform targetTf, float radius, float width, float height, float offset)
        {
            TargetTf = targetTf;
            Radius = radius;
            Width = width;
            Height = height;
            Offset = offset;
        }

        public List<Vector3> BuildAlgo()
        {
            List<Vector3> bornPosList = FishPoissonDiskSample.GetCoinCoordinate(Width, Height, Radius, TargetTf, Offset);
            return bornPosList;
        }
    }
}
