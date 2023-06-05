//
//  FacebookController.m
//  Unity-iPhone
//
//  Created by Ruman on 1/6/17.
//
//

#import "FacebookController.h"

@implementation FacebookController

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

- (void)initAdvertiserTracking
{
  // NSLog(@"initAdvertiserTracking");
  [FBSDKSettings setAdvertiserTrackingEnabled:YES];
  [FBAdSettings setAdvertiserTrackingEnabled:YES];
}


- (void)FBLogin:(UIViewController *)view
{
    // NSLog(@"FBLogin");
    FBSDKLoginManager *loginManager = [[FBSDKLoginManager alloc] init];
    // NSLog(@"FBLogin after init");
    // [loginManager logInWithPermissions:@[@"public_profile", @"user_friends", @"email"]
    [loginManager logInWithPermissions:@[@"public_profile", @"email"]
                        fromViewController:view
                                   handler:^(FBSDKLoginManagerLoginResult *result, NSError *error){
                                       if (error)
                                       {
                                           [loginManager logOut];
                                           //    NSLog(@"%@", error);
                                           UnitySendMessage([self.unityObject UTF8String], [self.unityMethodName UTF8String], [@"Error" UTF8String]);
                                       }
                                       else if (result.isCancelled)
                                       {
                                           [loginManager logOut];
                                           //    NSLog(@"CANCELED");
                                           UnitySendMessage([self.unityObject UTF8String], [self.unityMethodName UTF8String], [@"Error" UTF8String]);
                                       }
                                       else
                                       {
                                           if (result.token)
                                           {
                                               //    NSLog(@"Logged in");
                                               [self getFacebookProfileInfos];
                                           }
                                           else
                                           {
                                               //    NSLog(@"Logged in but no token");
                                               [loginManager logOut];
                                               UnitySendMessage([self.unityObject UTF8String], [self.unityMethodName UTF8String], [@"Error" UTF8String]);
                                           }
                                       }
                                   }];
    
}

- (void)getFacebookProfileInfos
{
    NSMutableDictionary* json = [NSMutableDictionary new];
    
    FBSDKGraphRequestConnection *connection = [[FBSDKGraphRequestConnection alloc] init];
    
    if ([[FBSDKAccessToken currentAccessToken] hasGranted:@"public_profile"])
    {
        FBSDKGraphRequest *requestMe = [[FBSDKGraphRequest alloc] initWithGraphPath:@"me"
                                                                  parameters:@{ @"fields" : @"id"}];
        
        [connection addRequest:requestMe completion:^(id<FBSDKGraphRequestConnecting> _Nullable connection, id _Nullable result, NSError *_Nullable error)
         {
             if (error)
             {
                 //  NSLog(@"Error");
                 UnitySendMessage([self.unityObject UTF8String], [self.unityMethodName UTF8String], [@"Error" UTF8String]);
             }
             else
             {
                 [json setValue:[result objectForKey:@"id"] forKey:@"id"];
                 
                 NSData* jsonData     = [NSJSONSerialization dataWithJSONObject:json options:NSJSONWritingPrettyPrinted error:nil];
                 NSString* jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
                 UnitySendMessage([self.unityObject UTF8String], [self.unityMethodName UTF8String], [jsonString UTF8String]);
             }
             
         }];
    }
    
    [connection start];
}

- (void)FBLogout
{
    FBSDKLoginManager *loginManager = [[FBSDKLoginManager alloc] init];
    [loginManager logOut];
}

- (void)FBShare:(UIViewController *)view
       imageUrl:(NSString *)url
     contentUrl:(NSString *)contentUrl
  encodedAction:(NSString *)encodedAction
   contentTitle:(NSString *)title
    contentDesc:(NSString *)desc
{
    FBSDKShareLinkContent *content = [[FBSDKShareLinkContent alloc] init];
    
    NSURLComponents *components = [NSURLComponents componentsWithString:contentUrl];
    NSURLQueryItem *imageQueryItem = [NSURLQueryItem queryItemWithName:@"image" value:url];
    NSURLQueryItem *descriptionQueryItem = [NSURLQueryItem queryItemWithName:@"description" value:desc];
    NSURLQueryItem *titleQueryItem = [NSURLQueryItem queryItemWithName:@"title" value:title];
    NSURLQueryItem *deepLinkQueryItem = [NSURLQueryItem queryItemWithName:@"encoded_action" value:encodedAction];
    components.queryItems = @[ imageQueryItem, titleQueryItem, descriptionQueryItem, deepLinkQueryItem ];
    
    content.contentURL          = components.URL;

    // FBSDKShareDialog *dialog = [[FBSDKShareDialog alloc] init];
    // dialog.fromViewController = view;
    // dialog.shareContent = content;
    // dialog.delegate = self;

    // On debug logs

    [FBSDKSettings setLoggingBehaviors:[[NSMutableSet alloc] initWithObjects:FBSDKLoggingBehaviorGraphAPIDebugInfo, nil]];

    FBSDKShareDialog *dialog = [FBSDKShareDialog dialogWithViewController:view 
                                                        withContent:content
                                                        delegate:self];
    NSLog(@"FacebookController: FBShare");
    dialog.mode = FBSDKShareDialogModeNative;

    if (![dialog canShow]) {
        dialog.mode = FBSDKShareDialogModeAutomatic;
        NSLog(@"FacebookController: FBSDKShareDialogModeAutomatic");
    }

    // NSLog(@"FacebookController: FBShare");
    if ([[UIApplication sharedApplication] canOpenURL:[NSURL URLWithString:@"fb://"]])
    {
        dialog.mode = FBSDKShareDialogModeNative;
        // NSLog(@"FacebookController: FBSDKShareDialogModeNative");
    }
    else
    {
        dialog.mode = FBSDKShareDialogModeAutomatic;
        // NSLog(@"FacebookController: FBSDKShareDialogModeAutomatic");
    }
    

    if (![dialog canShow]) {
        // exception. 
        dialog.mode = FBSDKShareDialogModeAutomatic;
        // NSLog(@"FacebookController: FBSDKShareDialogModeAutomatic. exception.");
    }

    [dialog show];
}

- (void)FBGameRequest:(UIViewController *)view message:(NSString *)message title:(NSString *)title
{
    // NSLog(@"%@", message);
    // NSLog(@"%@", title);
    
    FBSDKGameRequestContent *gameRequestContent = [[FBSDKGameRequestContent alloc] init];
    // Look at FBSDKGameRequestContent for futher optional properties
    gameRequestContent.message = message;
    gameRequestContent.title = title;
    
    // Assuming self implements <FBSDKGameRequestDialogDelegate>
    FBSDKGameRequestDialog *dialog = [[FBSDKGameRequestDialog alloc] init];
    
    if (dialog.canShow)
    {
        dialog.content = gameRequestContent;
        dialog.delegate = self;
        [dialog show];
    }
}

- (NSString*)FBGetAccessToken
{
    if ([FBSDKAccessToken currentAccessToken]) {
        FBSDKAccessToken* token = [FBSDKAccessToken currentAccessToken];
        return [token tokenString];
    }
    return nil;
}

- (BOOL)FBHasPermission:(NSString *)permission
{
    if ([[FBSDKAccessToken currentAccessToken] hasGranted:permission]) {
        return true;
    }
    return false;
}

- (void)FBGetPublishPermission:(UIViewController *)view
{
    FBSDKLoginManager *loginManager = [[FBSDKLoginManager alloc] init];
    [loginManager logInWithPermissions:@[@"publish_actions"]
                           fromViewController:view
                                      handler:^(FBSDKLoginManagerLoginResult *result, NSError *error) {
                                          if (error)
                                          {
                                              UnitySendMessage([self.unityObject UTF8String], [self.unityMethodName UTF8String], [@"Error" UTF8String]);
                                          }
                                          else if (result.isCancelled)
                                          {
                                              UnitySendMessage([self.unityObject UTF8String], [self.unityMethodName UTF8String], [@"Error" UTF8String]);
                                          }
                                          else
                                          {
                                              if (result.token)
                                              {
                                                  //    NSLog(@"Logged in");
                                                  [self getFacebookProfileInfos];
                                              }
                                              else
                                              {
                                                  //    NSLog(@"Logged in but no token");
                                                  UnitySendMessage([self.unityObject UTF8String], [self.unityMethodName UTF8String], [@"Error" UTF8String]);
                                              }
                                          }
                                      }];
}


#pragma mark - FBSDKSharingDelegate
- (void)sharer:(id<FBSDKSharing>)sharer didCompleteWithResults :(NSDictionary *)results
{
     // NSLog(@"FB: SHARE RESULTS=%@\n",results);
     NSLog(@"FacebookController: SHARE RESULTS=%@\n",results);
     NSLog(@"%s %s %s", [self.unityObject UTF8String], [self.unityMethodName UTF8String], [@"Success" UTF8String]);
    
    UnitySendMessage([self.unityObject UTF8String], [self.unityMethodName UTF8String], [@"Success" UTF8String]);
}

- (void)sharer:(id<FBSDKSharing>)sharer didFailWithError:(NSError *)error
{
     // NSLog(@"FB: ERROR=%@\n", [error localizedDescription]);
     NSLog(@"FacebookController: ERROR=%@\n", [error localizedDescription]);
     NSLog(@"%s %s %s", [self.unityObject UTF8String], [self.unityMethodName UTF8String], [@"Success" UTF8String]);
    UnitySendMessage([self.unityObject UTF8String], [self.unityMethodName UTF8String], [@"Error" UTF8String]);
}

- (void)sharerDidCancel:(id<FBSDKSharing>)sharer
{
     // NSLog(@"FB: CANCELED SHARER=%@\n",[sharer debugDescription]);
     NSLog(@"FacebookController: CANCELED SHARER=%@\n", [sharer debugDescription]);
     NSLog(@"%s %s %s", [self.unityObject UTF8String], [self.unityMethodName UTF8String], [@"Success" UTF8String]);
    UnitySendMessage([self.unityObject UTF8String], [self.unityMethodName UTF8String], [@"Error" UTF8String]);
}

- (void)gameRequestDialog:    (FBSDKGameRequestDialog *)gameRequestDialog
   didCompleteWithResults:    (NSDictionary *)results
{
    // NSLog(@"complete %@", results);
    NSLog(@"FacebookController: gameRequestDialog complete=%@\n", results);
    UnitySendMessage([self.unityObject UTF8String], [self.unityMethodName UTF8String], [@"Success" UTF8String]);
}

- (void) gameRequestDialog:    (FBSDKGameRequestDialog *)gameRequestDialog
          didFailWithError:    (NSError *)error
{
    // NSLog(@"fail %@", error);
    NSLog(@"FacebookController: gameRequestDialog fail=%@\n", error);
    UnitySendMessage([self.unityObject UTF8String], [self.unityMethodName UTF8String], [@"Error" UTF8String]);
}

- (void) gameRequestDialogDidCancel:(FBSDKGameRequestDialog *)gameRequestDialog
{
    // NSLog(@"cancel");
    NSLog(@"FacebookController: gameRequestDialog cancel");
    UnitySendMessage([self.unityObject UTF8String], [self.unityMethodName UTF8String], [@"Cancel" UTF8String]);
}

- (void) FBShareMessenger: (UIViewController *)view contentUrl:(NSString *)contentUrl
{
    FBSDKShareLinkContent *content = [[FBSDKShareLinkContent alloc] init];
    content.contentURL = [NSURL URLWithString:contentUrl];

    FBSDKMessageDialog *messageDialog = [[FBSDKMessageDialog alloc] init];
    messageDialog.shareContent = content;

    if ([messageDialog canShow]) {
        [messageDialog show];
    }
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
        FacebookController* facebookController = getFacebookController();
        [facebookController init];
    }
    
    void _initializeFacebookController(const char* unithObjectName)
    {
        // NSLog(@"InitializeFacebookController");
        FacebookController* facebookController = getFacebookController();
        [facebookController initAdvertiserTracking];
        [facebookController.unityObject setString:[NSString stringWithUTF8String:unithObjectName]];
    }
    
    void _FBLogin(const char* unityMethodName)
    {
        FacebookController* facebookController = getFacebookController();
        [facebookController.unityMethodName setString:[NSString stringWithUTF8String:unityMethodName]];
        [facebookController FBLogin:GetAppController().rootViewController];
    }
    
    void _FBLogout()
    {
        FacebookController* facebookController = getFacebookController();
        [facebookController FBLogout];
    }
    
    void _FBShare(const char* linkUrl, const char* title, const char* description, const char* imageUrl, const char* encodedAction, const char* unityMethodName)
    {
        FacebookController* facebookController = getFacebookController();
        [facebookController.unityMethodName setString:[NSString stringWithUTF8String:unityMethodName]];
        [facebookController FBShare:GetAppController().rootViewController
                           imageUrl:[NSString stringWithUTF8String:imageUrl]
                         contentUrl:[NSString stringWithUTF8String:linkUrl]
                      encodedAction:[NSString stringWithUTF8String:encodedAction]
                       contentTitle:[NSString stringWithUTF8String:title]
                        contentDesc:[NSString stringWithUTF8String:description]];
    }
       
    void _FBGameRequest(const char* message, const char* title, const char* unityMethodName)
    {
        // NSLog(@"get Request");
        FacebookController* facebookController = getFacebookController();
        [facebookController.unityMethodName setString:[NSString stringWithUTF8String:unityMethodName]];
        [facebookController FBGameRequest:GetAppController().rootViewController
                                  message:[NSString stringWithUTF8String:message]
                                    title:[NSString stringWithUTF8String:title]];
    }

    char* _FBGetAccessToken()
    {
        FacebookController* facebookController = getFacebookController();
        NSString *token = [facebookController FBGetAccessToken];

        char* tokenChar = NULL;
        
        if (token != nil)
        {
            int tokenLength = [token length];
            
            tokenChar = (char*)malloc(tokenLength + 1);
            memset(tokenChar, NULL, tokenLength + 1);
            strncpy(tokenChar, [token UTF8String], tokenLength);
            
            return tokenChar;
        }
        else
        {
            return nil;
        }
    }
    
    bool _FBHasPermission(const char* permission)
    {
        FacebookController* facebookController = getFacebookController();
        return [facebookController FBHasPermission:[NSString stringWithUTF8String:permission]];
    }
    
    void _FBGetPublishPermission(const char* callback)
    {
        FacebookController* facebookController = getFacebookController();
        [facebookController.unityMethodName setString:[NSString stringWithUTF8String:callback]];
        [facebookController FBGetPublishPermission:GetAppController().rootViewController];
        
    }

    void _FBShareMessenger(const char* linkUrl, const char* callback)
    {
        FacebookController* facebookController = getFacebookController();
        [facebookController.unityMethodName setString:[NSString stringWithUTF8String:callback]];
        [facebookController FBShareMessenger:GetAppController().rootViewController
                                  contentUrl:[NSString stringWithUTF8String:linkUrl]];
    }

    bool _FBGetAvailableFacebookMessenger(const char* linkUrl)
    {
        NSString *urlStr = [NSString stringWithCString: linkUrl 
                                           encoding:NSUTF8StringEncoding];
        // if ([[UIApplication sharedApplication] canOpenURL:[NSURL URLWithString:urlStr]]) {
        if ([[UIApplication sharedApplication] canOpenURL:[NSURL URLWithString:@"fb-messenger://"]]) {
            // Installed
            return true;
        }
        else {
            // NOT Installed
            return false;
        } 
    }    
}
