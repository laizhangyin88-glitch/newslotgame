//
//  AdjustHelper.mm
//  Unity-iPhone
//
//  Created by Sangmin Lee on 20/03/2017.
//
//

#import "AdjustHelper.h"
#import <Foundation/Foundation.h>

@implementation AdjustHelper

-(id)init
{
    self = [super init];
    if (self)
    {
        self.unityObject                      = [[NSMutableString alloc] initWithString:@""];
        self.unityCallbackByAdjustAttribution = [[NSMutableString alloc] initWithString:@""];
        self.isAdjustAttributionCallBackAlreadyCalled = NO;
    }
    
    return self;
}

- (BOOL)adjustDeeplinkResponse:(NSURL *)deeplink {
    // deeplink object contains information about deferred deep link content
    // Apply your logic to determine whether the adjust SDK should try to open the deep link
    
    // NSLog(@"adjustDeepLinkResponse : %@", deeplink);
    UnitySendMessage("NativeHelper", "OnOpenUrl", [[deeplink absoluteString] UTF8String]);
    
    return YES;
    // or
    // return NO;
}

- (void)adjustAttributionChanged:(ADJAttribution *)attribution {
    self.isAdjustAttributionCallBackAlreadyCalled = YES;
    if ([self.unityCallbackByAdjustAttribution length] == 0) return;

    // NSLog(@"ajdustAttributionChanged");
    NSError* error;
    NSData* jsonData = [NSJSONSerialization dataWithJSONObject:[attribution dictionary]
                                                       options:NSJSONReadingMutableLeaves error:&error];
    NSString* nsJson=  [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    
    UnitySendMessage([self.unityObject UTF8String], [self.unityCallbackByAdjustAttribution UTF8String], [nsJson UTF8String]);
}

@end
