//
//  IDFAController.h
//  Unity-iPhone
//
//  Created by Michael Mensah on 27/08/2020.
//

#import <Foundation/Foundation.h>
#import "UnityAppController.h"

NS_ASSUME_NONNULL_BEGIN

@interface IDFAController : NSObject
@property (retain, nonatomic) NSMutableString* unityObject;

@end

static IDFAController* _idfaController = nil;
static IDFAController* getIDFAController() {
    if (_idfaController == nil) {
        _idfaController = [[IDFAController alloc] init];
    }
    return _idfaController;
}

NS_ASSUME_NONNULL_END
