//
//  UnityOverrideAppController.h
//  Unity-iPhone
//
//  Created by Ruman on 1/6/17.
//
//

#ifndef UnityOverrideAppController_h
#define UnityOverrideAppController_h

#include "AppDelegateListener.h"

#import <Foundation/Foundation.h>
#import <Adjust/Adjust.h>
#import <AdSupport/ASIdentifierManager.h>
#import <FBSDKCoreKit/FBSDKCoreKit.h>
#import <OneSignal/OneSignal.h>
#import <Bolts/Bolts.h>

#import "UnityAppController.h"
#import "BagelcodeIAP.h"
#import "AdjustHelper.h"
#import "VideoAdsController.h"
#import "Firebase.h"

@interface UnityOverrideAppController : UnityAppController
@property(retain, nonatomic) AdjustHelper *adjustHelper;
@end

#endif
