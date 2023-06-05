//
//  FacebookController.h
//  Unity-iPhone
//
//  Created by Ruman on 1/6/17.
//
//

#ifndef FacebookController_h
#define FacebookController_h
#import "UnityAppController.h"
#import <Foundation/Foundation.h>
#import <FBSDKCoreKit/FBSDKCoreKit.h>
#import <FBSDKLoginKit/FBSDKLoginKit.h>
#import <FBSDKShareKit/FBSDKShareKit.h>
#import <FBAudienceNetwork/FBAdSettings.h>


@interface FacebookController : NSObject<FBSDKSharingDelegate, FBSDKGameRequestDialogDelegate>
@property (retain, nonatomic) NSMutableString* unityObject;
@property (retain, nonatomic) NSMutableString* unityMethodName;

- (void)FBLogin:(UIViewController *)view;
- (void)FBLogout;
- (void)FBShare:(UIViewController *)view
       imageUrl:(NSString *)url
     contentUrl:(NSString *)contentUrl
  encodedAction:(NSString *)encodedAction
   contentTitle:(NSString *)title
    contentDesc:(NSString *)desc;
- (void)FBGameRequest:(UIViewController *)view
              message:(NSString *)message
                title:(NSString *)title;
- (NSString*)FBGetAccessToken;

@end

static FacebookController* g_facebookController = nil;
static FacebookController* getFacebookController() {
    if (g_facebookController == nil) g_facebookController = [[FacebookController alloc] init];
    return g_facebookController;
}

#endif /* FacebookController_h */
