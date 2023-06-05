//
//  VideoAdsController.mm
//  Unity-iPhone
//
//  Created by Ruman on 13/06/2017.
//
//

#import "VideoAdsController.h"

@implementation VideoAdsController

-(id)init
{
    self = [super init];
    if (self)
    {
        self.unityObject = [[NSMutableString alloc] initWithString:@""];
        self.unityMethodName = [[NSMutableString alloc] initWithString:@""];
    }
    return self;
}

- (void)Initialize:(BOOL)devBuild
{
    
    [IronSource setLogDelegate:self];
    [IronSource setRewardedVideoDelegate:self];
//    [IronSource setAdaptersDebug:YES];
    
    [IronSource setConsent:YES];

    NSString* appKey = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"IronsourceAppKey"];
    // NSLog(@"Ironsource App Key : %@", appKey);

    [IronSource initWithAppKey:appKey adUnits:@[IS_REWARDED_VIDEO]];
//    [ISIntegrationHelper validateIntegration];

    // Enable test mode. 
    ISAdQualityConfig *adQualityConfig = [ISAdQualityConfig config];

    // There are 5 different log levels:
    // IS_AD_QUALITY_LOG_LEVEL_ERROR,
    // IS_AD_QUALITY_LOG_LEVEL_WARNING,
    // IS_AD_QUALITY_LOG_LEVEL_INFO,
    // IS_AD_QUALITY_LOG_LEVEL_DEBUG,
    // IS_AD_QUALITY_LOG_LEVEL_VERBOSE
    if(devBuild)
    {
        NSLog(@"VideoAdsController: TEST_MODE");
        adQualityConfig.testMode = YES;
        adQualityConfig.logLevel = IS_AD_QUALITY_LOG_LEVEL_VERBOSE;
    }
    else
    {
        // The default is NO - set to true only to test your Ad Quality integration
        adQualityConfig.testMode = NO;
        // The default is IS_AD_QUALITY_LOG_LEVEL_INFO
        adQualityConfig.logLevel = IS_AD_QUALITY_LOG_LEVEL_INFO;
    }

    // Initialize
    [[IronSourceAdQuality getInstance] initializeWithAppKey:appKey andConfig:adQualityConfig];
}

- (BOOL)IsAdsAvailable:(NSString *)placement
{
    ISPlacementInfo * pInfo = [IronSource rewardedVideoPlacementInfo:placement];
    if (pInfo != NULL)
    {
        if ([placement isEqualToString:[pInfo placementName]])
        {
            return [self hasRewardedVideo] && ![self IsCappedPlacement:placement];
        }
    }

    return false;
}

- (BOOL)hasRewardedVideo
{
   return [IronSource hasRewardedVideo];
}

- (BOOL)IsCappedPlacement:(NSString *)placement
{
    return [IronSource isRewardedVideoCappedForPlacement:placement];
}

- (void)showRewardedVideo:(NSString *)placement
{
    [IronSource showRewardedVideoWithViewController:GetAppController().rootViewController placement:placement];
}

- (NSNumber *)getPlacementReward:(NSString *)placement
{
    ISPlacementInfo * pInfo = [IronSource rewardedVideoPlacementInfo:placement];
    if(pInfo != NULL)
    {
//        NSString * rewardName = [pInfo rewardName];
        NSNumber * rewardAmount = [pInfo rewardAmount];
        return rewardAmount;
    }
    return 0;
}

- (void)validateIntegration
{
    [ISIntegrationHelper validateIntegration];
}

//#pragma mark - ISLogDelegate
//- (void)sendLog:(NSString *)log level:(LogLevel)level tag:(LogTag)tag
//{
//    NSLog(@"%@", log);
//    NSLog(@"logLevel %d", level);
//    NSLog(@"logTag %u", tag);
//}

#pragma mark - ISRewardedVideoDelegate
//Called after a rewarded video has changed its availability.
//@param available The new rewarded video availability. YES if available //and ready to be shown, NO otherwise.
- (void)rewardedVideoHasChangedAvailability:(BOOL)available {
//    NSLog(@"rewardedVideoHasChangedAvailability %d", available);
    if (available){
        UnitySendMessage([self.unityObject UTF8String], [@"OnAvailabilityChanged" UTF8String], [@"true" UTF8String]);
    }
    else{
        UnitySendMessage([self.unityObject UTF8String], [@"OnAvailabilityChanged" UTF8String], [@"false" UTF8String]);
    }
    
}
//Called after a rewarded video has been viewed completely and the user is //eligible for reward.@param placementInfo An object that contains the //placement's reward name and amount.
- (void)didReceiveRewardForPlacement:(ISPlacementInfo *)placementInfo {
//    NSLog(@"rewardedVideodidReceiveRewardForPlacement");
    // UnitySendMessage([self.unityObject UTF8String], [self.unityMethodName UTF8String], [@"" UTF8String]);
//    placementInfo.rewardAmount
}
//Called after a rewarded video has attempted to show but failed.
//@param error The reason for the error
- (void)rewardedVideoDidFailToShowWithError:(NSError *)error {
//    NSLog(@"rewardedVideoDidFailToShowWithError : %@", error);
    UnitySendMessage([self.unityObject UTF8String], [self.unityMethodName UTF8String], [@"" UTF8String]);
}
//Called after a rewarded video has been opened.
- (void)rewardedVideoDidOpen {
//    NSLog(@"rewardedVideoDidOpen");
}
//Called after a rewarded video has been dismissed.
- (void)rewardedVideoDidClose {
//    NSLog(@"rewardedVideoDidClose");
    UnitySendMessage([self.unityObject UTF8String], [self.unityMethodName UTF8String], [@"" UTF8String]);
}
//Note: the events below are not available for all supported rewarded video ad networks. Check which events are available per ad network you choose //to include in your build.
//We recommend only using events which register to ALL ad networks you //include in your build.
//Called after a rewarded video has started playing.
- (void)rewardedVideoDidStart {
//    NSLog(@"rewardedVideoDidStart");
}
//Called after a rewarded video has finished playing.
- (void)rewardedVideoDidEnd {
//    NSLog(@"rewardedVideodidEnd");
}

@end

//
// Extern Part, C Level
//
//------------------------------------------------------------------------------------

extern "C"
{
    void _initializeVideoAdsController(const char* unityObjectName, BOOL devBuild)
    {
        VideoAdsController* videoAdsController = getVideoAdsController();
        [videoAdsController Initialize: devBuild];
        [videoAdsController.unityObject setString:[NSString stringWithUTF8String:unityObjectName]];
    }
    
    bool _isAdPlayable(const char* placement)
    {
        VideoAdsController* videoAdsController = getVideoAdsController();
        return [videoAdsController IsAdsAvailable:[NSString stringWithUTF8String:placement]];
    }
    
    void _showRewardedVideo(const char* placement, const char* methodName)
    {
        VideoAdsController* videoAdsController = getVideoAdsController();
        [videoAdsController showRewardedVideo:[NSString stringWithUTF8String:placement]];
        [videoAdsController.unityMethodName setString:[NSString stringWithUTF8String:methodName]];
    }
    
    long _getPlcaementReward(const char* placement)
    {
        VideoAdsController* videoAdsController = getVideoAdsController();
        NSNumber* reward = [videoAdsController getPlacementReward:[NSString stringWithUTF8String:placement]];
        
        return [reward longValue];
    }

    void _integrationADS()
    {
        VideoAdsController* videoAdsController = getVideoAdsController();
        [videoAdsController validateIntegration];
    }
}
