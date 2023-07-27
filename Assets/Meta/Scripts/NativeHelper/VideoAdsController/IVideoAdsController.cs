using UnityEngine;
using System.Collections.Generic;

namespace BagelCode
{

public interface IVideoAdsController
{
    void Initialize(string gameObjectName);
    void LoadPlacement(string placement);
    bool IsVideoAdsAvailable(string placement);
    void ShowRewardedVideo(string placement, string methodName);
    long GetPlacementReward(string placement);
    void IntegrationTest();
}

}
