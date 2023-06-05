//
//  IDFAController.m
//  Unity-iPhone
//
//  Created by Michael Mensah on 27/08/2020.
//

#import "IDFAController.h"

#import <AdSupport/AdSupport.h>
#import <AppTrackingTransparency/ATTrackingManager.h>

NSString *const kHasSeenIDFAConsentPopupKey = @"HasSeenIDFAConsentPopupKey";

@implementation IDFAController

#pragma mark - IDFAController

-(id)init
{
    self = [super init];
    if (self)
    {
        self.unityObject = [[NSMutableString alloc] initWithString:@""];
    }
    return self;
}

//trigger the prompt to get the IDFA
-(void)requestIDFA
{
    if (@available(iOS 14.5, *)) {
        [ATTrackingManager requestTrackingAuthorizationWithCompletionHandler:^(ATTrackingManagerAuthorizationStatus status) {

            switch (status) {
                case ATTrackingManagerAuthorizationStatusAuthorized:
                    UnitySendMessage([self.unityObject UTF8String], "IDFARequestResult", [@"ATTrackingManagerAuthorizationStatusAuthorized" UTF8String]);
                    break;
                    
                case ATTrackingManagerAuthorizationStatusRestricted:
                    UnitySendMessage([self.unityObject UTF8String], "IDFARequestResult", [@"ATTrackingManagerAuthorizationStatusRestricted" UTF8String]);
                    
                    break;
                case ATTrackingManagerAuthorizationStatusDenied:
                    UnitySendMessage([self.unityObject UTF8String], "IDFARequestResult", [@"ATTrackingManagerAuthorizationStatusDenied" UTF8String]);
                    break;
                    
                default:
                    break;
            }
            
        }];
    } else {
        UnitySendMessage([self.unityObject UTF8String], "IDFARequestResult", [@"ATTrackingManagerAuthorizationStatusDenied" UTF8String]); //should never get here as the check should only be triggered on ios 14.5
    }
}

-(BOOL)isEligibleToSeeIDFAConsentPopup
{
    BOOL eligible = false;
    
    if (@available(iOS 14.5, *))
    {
        eligible = true;
    }
    
    return eligible;
}

-(BOOL)hasSeenIDFAConsentPopup
{
    BOOL hasSeen = false;
    
    NSUserDefaults* defaults = [NSUserDefaults standardUserDefaults];
        
    hasSeen = [defaults objectForKey:kHasSeenIDFAConsentPopupKey] && [defaults boolForKey:kHasSeenIDFAConsentPopupKey];
    
    return hasSeen;
}

-(void)setHasSeenIDFAConsentPopup
{
    [[NSUserDefaults standardUserDefaults] setBool:YES forKey:kHasSeenIDFAConsentPopupKey];
    [[NSUserDefaults standardUserDefaults] synchronize];
}

#pragma mark - User Defaults

- (NSMutableDictionary *)userDefaultsWithUserId:(NSString *)userID
{
    //just incase there's something already saving to the defaults with just the user id, we'll add "_Defaults" on the end so the defaults will contain a dict called "slots|apple|1234_Defaults"
    NSString* userIDDictString = [userID stringByAppendingString:@"_Defaults"];
    NSMutableDictionary *userDefaultsDict = [NSMutableDictionary dictionaryWithDictionary:[[NSUserDefaults standardUserDefaults] objectForKey:userIDDictString]];
    
    return userDefaultsDict;
}

- (void)setUserDefaults:(NSDictionary *)userDefaults withUserID:(NSString *)userID
{
    if (userDefaults && userID)
    {
        NSString* userIDDictString = [userID stringByAppendingString:@"_Defaults"];
        
        [[NSUserDefaults standardUserDefaults] setObject:userDefaults forKey:userIDDictString];
        
        [[NSUserDefaults standardUserDefaults] synchronize];
    }
}

#pragma mark - Unity bridge

extern "C"
{
    void _initializeIDFA(const char* unityObjectName)
    {
        IDFAController* idfaController = getIDFAController();
        [idfaController.unityObject setString:[NSString stringWithUTF8String:unityObjectName]];
    }

    void _requestIDFA()
    {
        IDFAController* idfaController = getIDFAController();
        [idfaController requestIDFA];
    }
    
    bool _isEligibleToSeeIDFAConsentPopup()
    {
        IDFAController* idfaController = getIDFAController();
        return [idfaController isEligibleToSeeIDFAConsentPopup];
    }
    
    bool _hasSeenIDFAConsentPopup()
    {
        IDFAController* idfaController = getIDFAController();
        return [idfaController hasSeenIDFAConsentPopup];
    }
    
    void _setHasSeenIDFAConsentPopup()
    {
        IDFAController* idfaController = getIDFAController();
        [idfaController setHasSeenIDFAConsentPopup];
    }
}

@end
