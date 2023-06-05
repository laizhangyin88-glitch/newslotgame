//
//  UnitySignInWithApple.mm
//  Unity-iPhone
//
//  Created by Paul Croft on 26/03/2020.
//

#import <Foundation/Foundation.h>
#import "UnityAppController.h"
#import "UnityOverrideAppController.h"
#import <AuthenticationServices/AuthenticationServices.h>

struct UserInfo
{
    const char * userId;
    const char * email;

    const char * firstName;
    const char * surname;
    
    const char * authorisationCode;
    const char * idToken;
    
    const char * error;

    ASUserDetectionStatus userDetectionStatus;
};

typedef void (*SignInWithAppleCallback)(int result, struct UserInfo s1);
API_AVAILABLE(ios(13.0), tvos(13.0))
typedef void (*CredentialStateCallback)(ASAuthorizationAppleIDProviderCredentialState state);

API_AVAILABLE(ios(13.0), tvos(13.0))
@interface UnitySignInWithApple : NSObject<ASAuthorizationControllerDelegate, ASAuthorizationControllerPresentationContextProviding>

@property (nonatomic) SignInWithAppleCallback loginCallback;
@property (nonatomic) CredentialStateCallback credentialStateCallback;

@end

API_AVAILABLE(ios(13.0), tvos(13.0))
static UnitySignInWithApple* _unitySignInWithAppleInstance;

@implementation UnitySignInWithApple
{
    ASAuthorizationAppleIDRequest* request;
}

+(UnitySignInWithApple*)instance
{
    if (_unitySignInWithAppleInstance == nil)
        _unitySignInWithAppleInstance = [[UnitySignInWithApple alloc] init];
    return _unitySignInWithAppleInstance;
}

-(void)startRequest
{
    if (@available(iOS 13.0, tvOS 13.0, *)) {
        ASAuthorizationAppleIDProvider* provider = [[ASAuthorizationAppleIDProvider alloc] init];
        request = [provider createRequest];
        [request setRequestedScopes: @[ASAuthorizationScopeEmail, ASAuthorizationScopeFullName]];

        ASAuthorizationController* controller = [[ASAuthorizationController alloc] initWithAuthorizationRequests:@[request]];
        controller.delegate = self;
        controller.presentationContextProvider = self;
        [controller performRequests];
    } else {
        // Fallback on earlier versions
    }
}

- (void)getCredentialState:(NSString *)userID
{
    ASAuthorizationAppleIDProvider* provider = [[ASAuthorizationAppleIDProvider alloc] init];
    [provider getCredentialStateForUserID:userID
                               completion:^(ASAuthorizationAppleIDProviderCredentialState credentialState, NSError * _Nullable error) {
        self.credentialStateCallback(credentialState);
    }];
}

-(ASPresentationAnchor)presentationAnchorForAuthorizationController:(ASAuthorizationController *)controller
{
    return ((UnityOverrideAppController *)GetAppController()).window;
}

-(void)authorizationController:(ASAuthorizationController *)controller didCompleteWithAuthorization:(ASAuthorization *)authorization
{
    if (self.loginCallback)
    {
        struct UserInfo data;

        if (@available(iOS 13.0, tvOS 13.0, *)) {
            ASAuthorizationAppleIDCredential* credential = (ASAuthorizationAppleIDCredential*)authorization.credential;
            NSString* idToken = [[NSString alloc] initWithData:credential.identityToken encoding:NSUTF8StringEncoding];
            NSString* authorisationCode = [[NSString alloc] initWithData:credential.authorizationCode encoding:NSUTF8StringEncoding];
            NSPersonNameComponents* name = credential.fullName;
            
            data.email = [credential.email UTF8String];
            data.userId = [credential.user UTF8String];
            
            data.firstName = [name.givenName UTF8String];
            data.surname = [name.familyName UTF8String];
            
            data.userDetectionStatus = credential.realUserStatus;
            
            data.idToken = [idToken UTF8String];
            data.authorisationCode = [authorisationCode UTF8String];
            
            data.error = "";
            
            self.loginCallback(1, data);
        } else {
            // Fallback on earlier versions
        }
    }
}

-(void)authorizationController:(ASAuthorizationController *)controller didCompleteWithError:(NSError *)error
{
    if (self.loginCallback)
    {
        // All members need to be set to a non-null value.
        struct UserInfo data;
        data.idToken = "";
        data.authorisationCode = "";
        data.firstName ="";
        data.surname = "";
        data.email = "";
        data.userId = "";
        data.userDetectionStatus = ASUserDetectionStatus(1);
        data.error = [error.localizedDescription UTF8String];
        self.loginCallback(0, data);
    }
}

@end

extern "C"
{
    void UnitySignInWithApple_Login(SignInWithAppleCallback callback)
    {
        if (@available(iOS 13.0, tvOS 13.0, *)) {
            UnitySignInWithApple* login = [UnitySignInWithApple instance];
            login.loginCallback = callback;
            [login startRequest];
        } else {
            // Fallback on earlier versions
        }
    }

    API_AVAILABLE(ios(13.0), tvos(13.0))
    void UnitySignInWithApple_GetCredentialState(const char *userID, CredentialStateCallback callback)
    {
        if (@available(iOS 13.0, tvOS 13.0, *)) {
            UnitySignInWithApple* login = [UnitySignInWithApple instance];
            login.credentialStateCallback = callback;
            [login getCredentialState: [NSString stringWithUTF8String: userID]];
        } else {
            // Fallback on earlier versions
        }
    }
}
