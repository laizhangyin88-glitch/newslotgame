//
//  VideoAdsController.h
//  Unity-iPhone
//
//  Created by Ruman on 13/06/2017.
//
//

#ifndef VideoAdsController_h
#define VideoAdsController_h
#import "IronSource/IronSource.h"
#import "IronSourceAdQuality.h"
#import "UnityAppController.h"

@interface VideoAdsController : NSObject<ISRewardedVideoDelegate, ISLogDelegate>
@property (retain, nonatomic) NSMutableString* unityObject;
@property (retain, nonatomic) NSMutableString* unityMethodName;

@end

static VideoAdsController* g_videoAdsController = nil;
static VideoAdsController* getVideoAdsController(){
    if (g_videoAdsController == nil) g_videoAdsController = [[VideoAdsController alloc] init];
    return g_videoAdsController;
}

#endif /* VideoAdsController_h */
