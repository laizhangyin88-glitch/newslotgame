#import "UnityAppController.h"
#import "Reachability.h"
#import "Keychain.h"
#import "FacebookController.h"
#import "TOCropViewController.h"
#import <Adjust/Adjust.h>
#import "AdjustHelper.h"
#import "UnityOverrideAppController.h"
#import <UserNotifications/UserNotifications.h>
#import <SurveyMonkeyiOSSDK/SurveyMonkeyiOSSDK.h>
#import "Firebase.h"

extern UIViewController* UnityGetGLViewController();

/*
 *  System Versioning Preprocessor Macros
 */
#define SYSTEM_VERSION_EQUAL_TO(v)                  ([[[UIDevice currentDevice] systemVersion] compare:v options:NSNumericSearch] == NSOrderedSame)
#define SYSTEM_VERSION_GREATER_THAN(v)              ([[[UIDevice currentDevice] systemVersion] compare:v options:NSNumericSearch] == NSOrderedDescending)
#define SYSTEM_VERSION_GREATER_THAN_OR_EQUAL_TO(v)  ([[[UIDevice currentDevice] systemVersion] compare:v options:NSNumericSearch] != NSOrderedAscending)
#define SYSTEM_VERSION_LESS_THAN(v)                 ([[[UIDevice currentDevice] systemVersion] compare:v options:NSNumericSearch] == NSOrderedAscending)
#define SYSTEM_VERSION_LESS_THAN_OR_EQUAL_TO(v)     ([[[UIDevice currentDevice] systemVersion] compare:v options:NSNumericSearch] != NSOrderedDescending)

//
// Image Library
//
@implementation UIImage (Extras)

#pragma mark -
#pragma mark Scale and crop image
- (UIImage *)imageWithImage:(CGSize)newSize
{
    UIImage *sourceImage = self;
    //UIGraphicsBeginImageContext(newSize);
    // In next line, pass 0.0 to use the current device's pixel scaling factor (and thus account for Retina resolution).
    // Pass 1.0 to force exact pixel size.
    UIGraphicsBeginImageContextWithOptions(newSize, NO, 1.0);
    [sourceImage drawInRect:CGRectMake(0, 0, newSize.width, newSize.height)];
    UIImage *newImage = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    return newImage;
}

@end

//
// Native Part, Objective-C Level
//

@interface NativeHelper : UIViewController <UIImagePickerControllerDelegate, UINavigationControllerDelegate, TOCropViewControllerDelegate, SMFeedbackDelegate>

@property (assign) BOOL isPickingThumnail;
@property (assign) BOOL isCropable;
@property (assign) CGSize thumbnailSize;
@property (assign) UIImage* savedThumbnailImage;
@property (retain, nonatomic) NSMutableString* unityObject;
@property (retain, nonatomic) NSMutableString* unityCallbackByPickingThumbnail;
@property (retain, nonatomic) NSMutableString* unityCallbackByCancelThumbnail;
@property (strong, nonatomic) Reachability *internetReachability;
@property (nonatomic, strong) SMFeedbackViewController * feedbackController;
@end

static NativeHelper* g_nativeHelper = nil;
static NativeHelper* getNativeHelper() {
    if (g_nativeHelper == nil) g_nativeHelper = [[NativeHelper alloc] init];
    return g_nativeHelper;
}


@implementation NativeHelper

// init

-(id)init
{
    self = [super init];
    if (self)
    {
        self.isPickingThumnail                  = NO;
        self.isCropable                         = NO;
        self.thumbnailSize                      = CGSizeMake(0, 0);
        self.unityObject                        = [[NSMutableString alloc] initWithString:@""];
        self.unityCallbackByPickingThumbnail    = [[NSMutableString alloc] initWithString:@""];
        self.unityCallbackByCancelThumbnail     = [[NSMutableString alloc] initWithString:@""];
        self.savedThumbnailImage                = nil;
        self.internetReachability               = [Reachability reachabilityForInternetConnection];
        //NSLog(@"nativehelper init");
    }

    return self;
}

- (BOOL)isNetworkAvailable
{
    // Not supported on iOS7, return always YES to avoid side-effect
    if (SYSTEM_VERSION_LESS_THAN(@"8.0")) return YES;

    BOOL isNetworkAvailable = YES;
    NetworkStatus status = [self.internetReachability currentReachabilityStatus];
    if (status == NotReachable) {
        isNetworkAvailable = NO;
    }
    
    return isNetworkAvailable;
}

- (NSString *)getServerBaseUrl
{
    return [[NSBundle mainBundle] objectForInfoDictionaryKey:@"ServerBaseUrl"];
}

- (NSString *)getChattingUrl
{
    return [[NSBundle mainBundle] objectForInfoDictionaryKey:@"ChattingUrl"];
}

- (NSString *)getAppDownloadUrl
{
    return [[NSBundle mainBundle] objectForInfoDictionaryKey:@"AppDownloadUrl"];
}

//
// Image Picker
//
-(void)pickImage
{
    self.isPickingThumnail = YES;
    UIImagePickerController *imagePickerController = [[UIImagePickerController alloc] init];
    [imagePickerController setSourceType:UIImagePickerControllerSourceTypePhotoLibrary];
    [imagePickerController setDelegate:self];
    [UnityGetGLViewController() presentViewController:imagePickerController animated:NO completion:nil];
}

-(void)imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary *)info
{
    if (self.isPickingThumnail)
    {
        if (self.isCropable)
        {
            UIImage *image = [info objectForKey:UIImagePickerControllerOriginalImage];
            
            TOCropViewController *cropViewController = [[TOCropViewController alloc] initWithImage:image];
            cropViewController.delegate = self;
            cropViewController.aspectRatioPreset = TOCropViewControllerAspectRatioPresetSquare; //Set the initial aspect ratio as a square
            cropViewController.aspectRatioLockEnabled = YES;
            cropViewController.aspectRatioPickerButtonHidden = YES;
            
            [picker dismissViewControllerAnimated:YES completion:^{
                [UnityGetGLViewController() presentViewController:cropViewController animated:YES completion:nil];
                
            }];
        }
        else
        {
            UIImage* pickedImage = [info objectForKey:UIImagePickerControllerOriginalImage];
            [getNativeHelper() afterPhotoPick:pickedImage];
        }
    }
}

- (void)cropViewController:(TOCropViewController *)cropViewController didCropToImage:(UIImage *)image withRect:(CGRect)cropRect angle:(NSInteger)angle
{
    [UnityGetGLViewController() dismissViewControllerAnimated:NO completion:nil];
    if (self.isPickingThumnail)
    {
        UIImage* thumbnailImage = [image imageWithImage:self.thumbnailSize];
        
        self.savedThumbnailImage = thumbnailImage;

        NSData* thumbnailJPG    = UIImageJPEGRepresentation(thumbnailImage, 1.0);
        NSString* encodedJPG    = [thumbnailJPG base64EncodedStringWithOptions:0];

        UnitySendMessage([self.unityObject UTF8String], [self.unityCallbackByPickingThumbnail UTF8String], [encodedJPG UTF8String]);
        self.isPickingThumnail = NO;
    }
}

- (void)cropViewController:(nonnull TOCropViewController *)cropViewController didFinishCancelled:(BOOL)cancelled NS_SWIFT_NAME(cropViewController(_:didFinishCancelled:))
{
    [UnityGetGLViewController() dismissViewControllerAnimated:NO completion:nil];
    if (self.isPickingThumnail)
    {
        UnitySendMessage([self.unityObject UTF8String], [self.unityCallbackByCancelThumbnail UTF8String], "");
        self.isPickingThumnail = NO;
    }
}

- (void)afterPhotoPick:(UIImage *)image
{
    [UnityGetGLViewController() dismissViewControllerAnimated:NO completion:nil];
    UIImage* thumbnailImage = [image imageWithImage:self.thumbnailSize];
    
    self.savedThumbnailImage = thumbnailImage;
    
    NSData* thumbnailJPG    = UIImageJPEGRepresentation(thumbnailImage, 1.0);
    NSString* encodedJPG    = [thumbnailJPG base64EncodedStringWithOptions:0];
    
    UnitySendMessage([self.unityObject UTF8String], [self.unityCallbackByPickingThumbnail UTF8String], [encodedJPG UTF8String]);
    self.isPickingThumnail = NO;
}

- (void)imagePickerControllerDidCancel:(UIImagePickerController *)picker
{
    [UnityGetGLViewController() dismissViewControllerAnimated:NO completion:nil];
    if (self.isPickingThumnail)
    {
        UnitySendMessage([self.unityObject UTF8String], [self.unityCallbackByCancelThumbnail UTF8String], "");
        self.isPickingThumnail = NO;
    }
}

-(uint64_t)getFreeDiskspace {
    uint64_t totalSpace = 0;
    uint64_t totalFreeSpace = 0;
    NSError *error = nil;
    NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
    NSDictionary *dictionary = [[NSFileManager defaultManager] attributesOfFileSystemForPath:[paths lastObject] error: &error];
    
    if (dictionary) {
        NSNumber *fileSystemSizeInBytes = [dictionary objectForKey: NSFileSystemSize];
        NSNumber *freeFileSystemSizeInBytes = [dictionary objectForKey:NSFileSystemFreeSize];
        totalSpace = [fileSystemSizeInBytes unsignedLongLongValue];
        totalFreeSpace = [freeFileSystemSizeInBytes unsignedLongLongValue];
        //NSLog(@"Memory Capacity of %llu MiB with %llu MiB Free memory available.", ((totalSpace/1024ll)/1024ll), ((totalFreeSpace/1024ll)/1024ll));
    } else {
        //NSLog(@"Error Obtaining System Memory Info: Domain = %@, Code = %ld", [error domain], (long)[error code]);
    }
    
    return ((totalFreeSpace/1024ll)/1024ll);
}

- (void)openSurveyMonkey:(const char *)hash userId:(const char *)userId {
    NSDictionary *dict=[[NSMutableDictionary alloc]init];
    [dict setValue:[NSString stringWithUTF8String:userId] forKey:@"userId"];

    self.feedbackController = [[SMFeedbackViewController alloc] initWithSurvey:[NSString stringWithUTF8String: hash] andCustomVariables:dict];
    self.feedbackController.delegate = self;

    [self.feedbackController scheduleInterceptFromViewController:UnityGetGLViewController() withAppTitle:@"Club Vegas"];
    [self.feedbackController presentFromViewController:UnityGetGLViewController() animated:YES completion:nil];

}

- (void)respondentDidEndSurvey:(SMRespondent *)respondent error:(NSError *)error {
    UnitySendMessage([self.unityObject UTF8String], "OnSurveyEnd", "");
    self.feedbackController = nil;
}

- (void)shareSNSUrl:(const char *)url {
    NSString *message   = [NSString stringWithUTF8String:url];
    NSArray *postItems  = @[message];

    UIActivityViewController *activityVc = [[UIActivityViewController alloc] initWithActivityItems:postItems applicationActivities:nil];

    if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad &&  [activityVc respondsToSelector:@selector(popoverPresentationController)] ) {
        UIPopoverController *popup = [[UIPopoverController alloc] initWithContentViewController:activityVc];

        [popup presentPopoverFromRect:CGRectMake(self.view.frame.size.width/2, self.view.frame.size.height/4, 0, 0)
                               inView:[UIApplication sharedApplication].keyWindow.rootViewController.view permittedArrowDirections:UIPopoverArrowDirectionAny animated:YES];
    }
    else
        [[UIApplication sharedApplication].keyWindow.rootViewController presentViewController:activityVc animated:YES completion:nil];

}

@end

//
// Extern Part, C Level
//
//------------------------------------------------------------------------------------

extern "C"
{
    void _init()
    {
        NativeHelper* nativeHelper = getNativeHelper();
        [nativeHelper init];
    }
    
    void _initialize(const char* unityObject)
    {
        NativeHelper* nativeHelper = getNativeHelper();
        [nativeHelper.unityObject setString:[NSString stringWithUTF8String:unityObject]];
    }

    BOOL _isNetworkAvailable()
    {
        NativeHelper* nativeHelper = getNativeHelper();
        return [nativeHelper isNetworkAvailable];
    }

    static char *g_uuid = NULL;
    
    char* _getDeviceID()
    {
        NSString *uuid = [Keychain udidUsingCFUUID];
        // NSLog(@"uuid %@", uuid);

        int uuidLength = [uuid length];
        
        if (g_uuid == nil)
        {
            g_uuid = (char*)malloc(uuidLength + 1);
            memset(g_uuid, NULL, uuidLength + 1);
            strncpy(g_uuid, [uuid UTF8String], uuidLength);
        }
        
        return g_uuid;
    }

    static char *g_server_base_url = NULL;
  
    char* _getServerBaseUrl()
    {
        if (g_server_base_url == nil)   {
          NativeHelper* nativeHelper = getNativeHelper();
          NSString *serverUrl = [nativeHelper getServerBaseUrl];
          if (serverUrl == nil)
          {
            return nil;
          }
          int length = [serverUrl length];
          g_server_base_url = (char*)malloc(length + 1);
          memset(g_server_base_url, NULL, length + 1);
          strncpy(g_server_base_url, [serverUrl UTF8String], length);
        }
        return g_server_base_url;
    }

    static char *g_chatting_url = NULL;

    char* _getChattingUrl()
    {
        if (g_chatting_url == nil)   {
          NativeHelper* nativeHelper = getNativeHelper();
          NSString *serverUrl = [nativeHelper getChattingUrl];
          if (serverUrl == nil)
          {
            return nil;
          }
          int length = [serverUrl length];
          g_chatting_url = (char*)malloc(length + 1);
          memset(g_chatting_url, NULL, length + 1);
          strncpy(g_chatting_url, [serverUrl UTF8String], length);
        }
        return g_chatting_url;
    }

    static char *g_app_download_url = NULL;

    char* _getAppDownloadUrl()
    {
        if (g_app_download_url == nil)   {
          NativeHelper* nativeHelper = getNativeHelper();
          NSString *appDownloadUrl = [nativeHelper getAppDownloadUrl];
          if (appDownloadUrl == nil)
          {
            return nil;
          }
          int length = [appDownloadUrl length];
          g_app_download_url = (char*)malloc(length + 1);
          memset(g_app_download_url, NULL, length + 1);
          strncpy(g_app_download_url, [appDownloadUrl UTF8String], length);
        }
        return g_app_download_url;
    }

    void _pickThumbnailFromAlbum(int width, int height, BOOL isCropable,
                                 const char* unityOkayCallback,
                                 const char* unityCancelCallback)
    {
        NativeHelper* nativeHelper = getNativeHelper();
        [nativeHelper.unityCallbackByPickingThumbnail setString:[NSString stringWithUTF8String:unityOkayCallback]];
        [nativeHelper.unityCallbackByCancelThumbnail setString:[NSString stringWithUTF8String:unityCancelCallback]];
        
        nativeHelper.thumbnailSize = CGSizeMake(width, height);
        nativeHelper.isCropable = isCropable;
        [nativeHelper pickImage];
    }

    void _setLocalPush(const char* pushID, const char* sender, const char* text, const char* type, int seconds, long getPushTimestamp)
    {
        // use in unity
        NSDictionary *dict=[[NSMutableDictionary alloc]init];
        [dict setValue:[NSString stringWithUTF8String:pushID] forKeyPath:@"NotificationID"];
        [dict setValue:[NSNumber numberWithLong:getPushTimestamp] forKeyPath:@"getPushTimestamp"];
        [dict setValue:[NSString stringWithUTF8String:type] forKeyPath:@"type"];
        [dict setValue:[NSString stringWithUTF8String:sender] forKey:@"title"];
        
        UILocalNotification* localNotification = [[UILocalNotification alloc] init];
        localNotification.alertBody=[NSString stringWithUTF8String:text];
        localNotification.timeZone=[NSTimeZone localTimeZone];
        localNotification.soundName = @"push_sound.caf";

        localNotification.userInfo = dict;
        localNotification.applicationIconBadgeNumber = [[UIApplication sharedApplication] applicationIconBadgeNumber] + 1;

        NSTimeInterval secondsForTenDays = seconds;
        NSDate *fireDate = [[NSDate alloc] init];
        fireDate = [NSDate dateWithTimeIntervalSinceNow:secondsForTenDays];

        localNotification.fireDate=fireDate;
        [[UIApplication sharedApplication] scheduleLocalNotification:localNotification];
        //NSLog(@"SetLocalPush : %@", [NSString stringWithUTF8String:pushID]);
    }

    void _deleteLocalPush(const char* pushID)
    {
        // use in unity
        UIApplication *app = [UIApplication sharedApplication];
        NSArray *eventArray = [app scheduledLocalNotifications];

        for (int i = 0; i < [eventArray count]; i++)
        {
            UILocalNotification* oneEvent = [eventArray objectAtIndex:i];
            NSDictionary *userInfoCurrent = oneEvent.userInfo;
            NSString *uid=[NSString stringWithFormat:@"%@",[userInfoCurrent valueForKey:@"NotificationID"]];

            if ([uid isEqualToString:[NSString stringWithUTF8String:pushID]])
            {
                // Cancelling local notification
                // NSLog(@"deleteLocalPush : %@", [NSString stringWithUTF8String:pushID]);
                [app cancelLocalNotification:oneEvent];
            }
        }
    }

    void _initAdjust(const char* unityObject, const char* unityAttributionCallback)
    {
        AdjustHelper* adjustHelper = ((UnityOverrideAppController *)GetAppController()).adjustHelper;
        if (adjustHelper == nil) {
            // NSLog(@"adjustHelper is not initialized");
            return;
        }
        
        [adjustHelper.unityObject setString:[NSString stringWithUTF8String:unityObject]];
        [adjustHelper.unityCallbackByAdjustAttribution setString:[NSString stringWithUTF8String:unityAttributionCallback]];

        if (adjustHelper.isAdjustAttributionCallBackAlreadyCalled) {
            ADJAttribution *attribution = [Adjust attribution];
            [adjustHelper adjustAttributionChanged: attribution];
        }
    }

    char* _getAdjustID()
    {
        NSString *adid = [Adjust adid];
        if (adid == nil)
        {
            return nil;
        }

        int length = [adid length];
        char *adidChar = (char*)malloc(length + 1);
        memset(adidChar, NULL, length + 1);
        strncpy(adidChar, [adid UTF8String], length);
        
        return adidChar;
    }
    
    void _sendAdjustEvent(const char* eventToken)
    {
        ADJEvent *event = [ADJEvent eventWithEventToken:[NSString stringWithUTF8String: eventToken]];
        [Adjust trackEvent:event];
    }
    
    void _sendAdjustRevenueEvent(const char* eventToken, double revenue, const char* purchaseID)
    {
        ADJEvent *event = [ADJEvent eventWithEventToken:[NSString stringWithUTF8String: eventToken]];
        [event setRevenue:revenue currency:@"USD"];
        [event setTransactionId:[NSString stringWithUTF8String: purchaseID]];
        [Adjust trackEvent:event];
    }

    void _sendFIREvent(const char* eventName)
    {
        [FIRAnalytics logEventWithName:[NSString stringWithUTF8String: eventName]
                            parameters:@{
                                         // @"name": name,
                                         // @"full_text": text
                                         }];
    }

    void _clearBadge()
    {
        [UIApplication sharedApplication].applicationIconBadgeNumber = -1;

        UNUserNotificationCenter * center = [UNUserNotificationCenter currentNotificationCenter];
        [center removeAllDeliveredNotifications];
    }

    void _logToiOS(const char* debugMessage)
    {
        NSLog(@"Unity %@", [NSString stringWithUTF8String:debugMessage]);
    }

    void _copyClipboard(const char* str)
    {
        [UIPasteboard generalPasteboard].string = [NSString stringWithUTF8String: str];
    }

    long _getFreeDiskSpace()
    {
        NativeHelper* nativeHelper = getNativeHelper();
        uint64_t freespace = 0;
        freespace = [nativeHelper getFreeDiskspace];
        
        return (long)freespace;
    }

    void _setIdleTimerDisabled(BOOL value)
    {
        [UIApplication sharedApplication].idleTimerDisabled = value;
    }
    
    void _openSurveyMonkey(const char* hash, const char* userId)
    {
        NativeHelper* nativeHelper = getNativeHelper();
        [nativeHelper openSurveyMonkey:hash userId:userId];
    }

    BOOL _getPushNotificationSubscribed()
    {
        OSDeviceState *deviceState = [OneSignal getDeviceState];
        return deviceState.isSubscribed;
    }

    void _openPresentAppSettings()
    {
        // ref: https://developer.apple.com/documentation/uikit/uiapplicationopensettingsurlstring?language=objc
        // Create the URL that deep links to your app's custom settings.
        NSURL *url = [[NSURL alloc] initWithString:UIApplicationOpenSettingsURLString];
        // Ask the system to open that URL.
        [[UIApplication sharedApplication] openURL:url
                                        options:@{}
                                completionHandler:nil];
    }

    void _setExternalUserId(const char* userId)
    {
        NSString* externalUserId = [NSString stringWithUTF8String:userId];
        // Setting External User Id with Callback Available in SDK Version 2.13.0+
        [OneSignal setExternalUserId:externalUserId];
    }

    void _removeExternalUserId()
    {
        [OneSignal removeExternalUserId];
    }

    void _testCrash()
    {
        @[][1];
    }

    void _shareSNSUrl(const char * url){
        NativeHelper* nativeHelper = getNativeHelper();
        [nativeHelper shareSNSUrl: url];
    }
}
