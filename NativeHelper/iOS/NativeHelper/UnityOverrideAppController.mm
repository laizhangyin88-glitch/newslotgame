//
//  AppDelegateListener.mm
//  Unity-iPhone
//
//  Created by Ruman on 1/6/17.
//
//

#import "UnityOverrideAppController.h"

@implementation UnityOverrideAppController

- (void)applicationDidBecomeActive:(UIApplication *)application
{
    [super applicationDidBecomeActive:application];
}

- (BOOL)application:(UIApplication*)application didFinishLaunchingWithOptions:(NSDictionary*)launchOptions
{
    [super application:application didFinishLaunchingWithOptions:launchOptions];
    
    [[FBSDKApplicationDelegate sharedInstance] application:application
                             didFinishLaunchingWithOptions:launchOptions];

    [[SKPaymentQueue defaultQueue] addTransactionObserver:[BagelcodeIAP sharedManager]];
    
    NSString *yourAppToken = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"adjust_app_token"];
    NSString *environment = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"adjust_env"];
    NSString *adjEnv;
    if ([environment isEqualToString:@"production"])
    {
        adjEnv = ADJEnvironmentProduction;
    }
    else
    {
        adjEnv = ADJEnvironmentSandbox;
    }
    NSString *adjustAppSecret = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"adjust_app_secret"];
    NSString *adjustInfo1 = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"adjust_info1"];
    NSString *adjustInfo2 = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"adjust_info2"];
    NSString *adjustInfo3 = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"adjust_info3"];
    NSString *adjustInfo4 = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"adjust_info4"];
    
    ADJConfig *adjustConfig = [ADJConfig configWithAppToken:yourAppToken
                                                environment:adjEnv];
    
    self.adjustHelper = [[AdjustHelper alloc] init];
    [adjustConfig setDelegate:self.adjustHelper];
    //NSLog(@"Adjust app_secret:%@, info1:%@, info2:%@, info3:%@, info4:%@", adjustAppSecret, adjustInfo1, adjustInfo2, adjustInfo3, adjustInfo4);
    [adjustConfig setAppSecret:(NSUInteger)[adjustAppSecret integerValue] info1:(NSUInteger)[adjustInfo1 integerValue] info2:(NSUInteger)[adjustInfo2 integerValue] info3:(NSUInteger)[adjustInfo3 integerValue] info4:[adjustInfo4 integerValue]];

    // logs
    [adjustConfig setLogLevel:ADJLogLevelVerbose];
    
    [Adjust appDidLaunch:adjustConfig];
    
    UILocalNotification *localNotif = [launchOptions objectForKey:UIApplicationLaunchOptionsLocalNotificationKey];
    
    if (localNotif)
    {
        // NSLog(@"local push");
        NSNumber *getPushTimestamp = [localNotif.userInfo objectForKey:@"getPushTimestamp"];
        NSString *title = [localNotif.userInfo objectForKey:@"title"];
        NSString *type = [localNotif.userInfo objectForKey:@"type"];

        NSString *data = [NSString stringWithFormat:@"%@:%@:%@", type, title, [getPushTimestamp stringValue]];
        // NSLog(@"%@", data);
        
        UnitySendMessage("NativeHelper", "OnLocalPushMessage", [data UTF8String]);
        // NSLog(@"end localpush");
    }
    
    NSDictionary *remoteNotif = [launchOptions objectForKey:UIApplicationLaunchOptionsRemoteNotificationKey];
    
    if (remoteNotif && remoteNotif[@"data"])
    {
        UnitySendMessage("NativeHelper", "OnPendingMessage", [remoteNotif[@"data"] UTF8String]);
    }

    [FIRApp configure];

    // OneSignal Initialization 
    NSString *onesignalAppId = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"onesignal_app_id"];
    [OneSignal initWithLaunchOptions:launchOptions];
    [OneSignal setAppId:onesignalAppId];
    id notificationOpenedBlock = ^(OSNotificationOpenedResult *result) {
        // This block gets called when the user reacts to a notification received
        OSNotification* notification = result.notification;
        
        if (notification.additionalData) {
            NSDictionary* additionalData = notification.additionalData;
            NSString* notificationId = notification.notificationId;
            
            if (notificationId) {
                NSString *notiData = [NSString stringWithFormat:@"%@:%@", @"lastNotificationId", notificationId];
                UnitySendMessage("NativeHelper", "SetBlackboardValue", [notiData UTF8String]);
            }

            if (additionalData[@"data"]) {
                NSLog(@"Onesignal Data: %@", additionalData[@"data"]);
                
                if (additionalData[@"data"] != nil && additionalData[@"data"] != (NSString*)[NSNull null])
                    UnitySendMessage("NativeHelper", "OnPendingMessage", [additionalData[@"data"] UTF8String]);
            }
        }
    };
    [OneSignal setNotificationOpenedHandler:notificationOpenedBlock];

    id notifWillShowInForegroundHandler = ^(OSNotification *notification, OSNotificationDisplayResponse completion) {
        NSLog(@"Received Notification - %@", notification.notificationId);
        completion(nil);
        // if ([notification.notificationId isEqualToString:@"silent_notif"]) {
        //     completion(nil);
        // } else {
        //     completion(notification);
        // }
    };
    [OneSignal setNotificationWillShowInForegroundHandler:notifWillShowInForegroundHandler];
    
    // promptForPushNotifications will show the native iOS notification permission prompt.
    // We recommend removing the following code and instead using an In-App Message to prompt for notification permission (See step 8)
    [OneSignal promptForPushNotificationsWithUserResponse:^(BOOL accepted) {
        NSLog(@"User accepted notifications: %d", accepted);
        
        OSDeviceState *deviceState = [OneSignal getDeviceState];
        NSString *onesignalUserId = deviceState.userId;
        if (onesignalUserId && onesignalUserId.length) {
            NSLog(@"Onesignal Id: %@", onesignalUserId);
            UnitySendMessage("NativeHelper", "UpdateOneSignalToken", [onesignalUserId UTF8String]);
        }
    }];
    
    OSDeviceState *deviceState = [OneSignal getDeviceState];
    NSString *onesignalUserId = deviceState.userId;
    if (onesignalUserId && onesignalUserId.length) {
        NSLog(@"Onesignal Id: %@", onesignalUserId);
        UnitySendMessage("NativeHelper", "UpdateOneSignalToken", [onesignalUserId UTF8String]);
    }
    
    // Facebook deferred deeplink
    if (launchOptions[UIApplicationLaunchOptionsURLKey] == nil) {
        [FBSDKAppLinkUtility fetchDeferredAppLink:^(NSURL *url, NSError *error) {
            if (error) {
                NSLog(@"Received error while fetching deferred app link %@", error);
            }
            if (url) {
                UnitySendMessage("NativeHelper", "OnOpenUrl", [[url absoluteString] UTF8String]);
            }
        }];
    }
    
    return YES;
}

- (BOOL)application:(UIApplication *)app openURL:(NSURL *)url options:(NSDictionary<NSString *,id> *)options {
    BOOL handled = [[FBSDKApplicationDelegate sharedInstance] application:app
                                                                  openURL:url
                                                                 options:options];
    // return handled; adjust debug
    [Adjust appWillOpenUrl:url];
    NSLog(@"ios9 deeplink url %@", url);
    UnitySendMessage("NativeHelper", "OnOpenUrl", [[url absoluteString] UTF8String]);
    return true;
}

- (BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation{
    
    NSMutableArray* keys	= [NSMutableArray arrayWithCapacity:3];
    NSMutableArray* values	= [NSMutableArray arrayWithCapacity:3];
    
#define ADD_ITEM(item)    do{ if(item) {[keys addObject:@#item]; [values addObject:item];} }while(0)
    
    ADD_ITEM(url);
    ADD_ITEM(sourceApplication);
    ADD_ITEM(annotation);
    
#undef ADD_ITEM
    
    NSDictionary* notifData = [NSDictionary dictionaryWithObjects:values forKeys:keys];
    AppController_SendNotificationWithArg(kUnityOnOpenURL, notifData);
    
    BOOL handled = [[FBSDKApplicationDelegate sharedInstance] application:application
                                                                  openURL:url
                                                        sourceApplication:sourceApplication
                                                               annotation:annotation];
    // return handled; adjust debug
    [Adjust appWillOpenUrl:url];
    NSLog(@"old style deeplink url %@", url);
    UnitySendMessage("NativeHelper", "OnOpenUrl", [[url absoluteString] UTF8String]);
    return true;
}

- (BOOL)application:(UIApplication *)application continueUserActivity:(NSUserActivity *)userActivity
 restorationHandler:(void (^)(NSArray *restorableObjects))restorationHandler {
    // NSLog(@"continueUserActivity");
    
    if ([[userActivity activityType] isEqualToString:NSUserActivityTypeBrowsingWeb]) {
        NSURL *url = [userActivity webpageURL];
        
        [Adjust appWillOpenUrl:url];
        
        NSString *deepLinkScheme = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"DeepLinkScheme"];
        NSURL *oldStyleDeeplink = [Adjust convertUniversalLink:url scheme:deepLinkScheme];
        NSLog(@"deeplink url %@", url);
        NSLog(@"oldstykle %@", oldStyleDeeplink);
        UnitySendMessage("NativeHelper", "OnOpenUrl", [[oldStyleDeeplink absoluteString] UTF8String]);
    }
    
    // Apply your logic to determine the return value of this method
    return YES;
    // or
    // return NO;
}

// We must remove this code when Unity version is 2019.4.37f1 or later.
// https://forum.unity.com/threads/crashes-when-you-force-close-an-ios-app-with-another-view-controller-up.874180/#post-7951183
- (UIWindowScene*)pickStartupWindowScene:(NSSet<UIScene*>*)scenes API_AVAILABLE(ios(13.0), tvos(13.0))
{
    // if we have scene with UISceneActivationStateForegroundActive - pick it
    // otherwise UISceneActivationStateForegroundInactive will work
    //   it will be the scene going into active state
    // if there were no active/inactive scenes (only background) we should allow background scene
    //   this might happen in some cases with native plugins doing "things"
    UIWindowScene *foregroundScene = nil, *backgroundScene = nil;
    for (UIScene* scene in scenes)
    {
        if (![scene isKindOfClass: [UIWindowScene class]])
            continue;
        UIWindowScene* windowScene = (UIWindowScene*)scene;
 
        if (scene.activationState == UISceneActivationStateForegroundActive)
            return windowScene;
        if (scene.activationState == UISceneActivationStateForegroundInactive)
            foregroundScene = windowScene;
        else if (scene.activationState == UISceneActivationStateBackground)
            backgroundScene = windowScene;
    }
 
    return foregroundScene ? foregroundScene : backgroundScene;
}

@end

IMPL_APP_CONTROLLER_SUBCLASS(UnityOverrideAppController)
