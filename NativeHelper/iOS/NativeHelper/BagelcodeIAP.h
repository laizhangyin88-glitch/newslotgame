//
//  BagelcodeIAP.h
//  Unity-iPhone
//
//  Created by Sangmin Lee on 1/24/17.
//
//

#import "UnityAppController.h"
#import <Foundation/Foundation.h>
#import <StoreKit/StoreKit.h>

@interface BagelcodeIAP : NSObject<SKProductsRequestDelegate, SKPaymentTransactionObserver>
{
	// SKProductsRequestDelegate for Retrieving Product Information
	// SKPaymentTransactionObserver for Delivering Products
}

@property (retain, nonatomic) NSMutableString* unityObject;
@property (retain, nonatomic) NSMutableString* unityMethodName;
@property (retain, nonatomic) NSString * purchaseCurrencyCode;
@property (retain, nonatomic) NSString * purchaseLocalPrice;

+ (id) sharedManager;

- (void) paymentRequestWithProductIdentifiers:(NSArray *) productIdentifiers;
- (void) paymentRequest:(SKProduct *) product;

@end

