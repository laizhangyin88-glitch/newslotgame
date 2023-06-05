//
//  AdjustHelper.h
//  Unity-iPhone
//
//  Created by Sangmin Lee on 20/03/2017.
//
//

#import <Adjust/Adjust.h>
#ifndef AdjustHelper_h
#define AdjustHelper_h

@interface AdjustHelper : UIResponder <AdjustDelegate>
@property (retain, nonatomic) NSMutableString* unityObject;
@property (retain, nonatomic) NSMutableString* unityCallbackByAdjustAttribution;
@property (assign, nonatomic) BOOL isAdjustAttributionCallBackAlreadyCalled;
@end
#endif /* AdjustHelper_h */
